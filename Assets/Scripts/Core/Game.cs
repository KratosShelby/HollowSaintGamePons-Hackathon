using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HollowSaint.Investigation;
using HollowSaint.Player;
using HollowSaint.UI;

namespace HollowSaint.Core
{
    public enum GameMode { Explore, Dialogue, Book, Tools, Newspaper, Ended }

    /// <summary>
    /// Owns the run: which case is active, what's been found, what Dutch is carrying, and
    /// which screen has input. Created automatically before the first scene loads and kept
    /// for the whole session.
    /// </summary>
    public class Game : MonoBehaviour
    {
        public static Game Instance { get; private set; }

        public GameConfig Config { get; private set; }
        public UIRoot UI { get; private set; }

        public GameMode Mode { get; private set; } = GameMode.Explore;
        public int ModeChangedFrame { get; private set; } = -1;

        public int CasesClosed { get; private set; }
        public CaseData ActiveCase { get; private set; }
        public bool Identified { get; private set; }
        public CaseFile CaseFile { get; private set; } = new CaseFile();
        public bool ReflectionRevealed { get; private set; }

        readonly List<string> journal = new List<string>();
        readonly List<ToolData> tools = new List<ToolData>();
        readonly HashSet<ToolData> spentTools = new HashSet<ToolData>();
        readonly HashSet<CaseData> startedCases = new HashSet<CaseData>();
        readonly HashSet<CaseData> bookRevealPlayed = new HashSet<CaseData>();
        string pendingSpawn;
        bool hotelWakePending = true;

        /// <summary>Every symptom ever logged, across all cases. The Book checks against this.</summary>
        public IReadOnlyList<string> Journal => journal;
        public IReadOnlyList<ToolData> Tools => tools;
        public int TotalCases => Config.cases.Count;
        public CaseData NextCase => CasesClosed < Config.cases.Count ? Config.cases[CasesClosed] : null;

        /// <summary>Raised whenever case/kit/progress state changes.</summary>
        public event Action Changed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => Instance = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Boot()
        {
            if (Instance != null) return;
            var config = Resources.Load<GameConfig>(GameConfig.ResourcePath);
            if (config == null)
            {
                Debug.LogError($"[Game] No GameConfig at Resources/{GameConfig.ResourcePath}. Run Hollow Saint > Build Content.");
                return;
            }
            var go = new GameObject("Game");
            go.AddComponent<Game>().Init(config);
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void Init(GameConfig config)
        {
            Config = config;
            tools.AddRange(config.startingTools);
            UI = UIRoot.Create(transform, config);
        }

        void Update()
        {
            if (GameplayInputAllowed && InputReader.BookPressedThisFrame())
                OpenBook();
        }

        // ---------------------------------------------------------------- modes

        /// <summary>True when the player may move and interact this frame.</summary>
        public bool GameplayInputAllowed => Mode == GameMode.Explore && Time.frameCount != ModeChangedFrame;

        /// <summary>True when the screen for <paramref name="mode"/> may read input this frame.</summary>
        public bool InputAllowedFor(GameMode mode) => Mode == mode && Time.frameCount != ModeChangedFrame;

        void SetMode(GameMode mode)
        {
            Mode = mode;
            ModeChangedFrame = Time.frameCount;
        }

        void NotifyChanged() => Changed?.Invoke();

        // ---------------------------------------------------------------- dialogue

        public void Say(IList<DialogueLine> lines, Action onDone = null)
        {
            if (lines == null || lines.Count == 0)
            {
                onDone?.Invoke();
                return;
            }
            var resume = Mode == GameMode.Dialogue ? GameMode.Explore : Mode;
            SetMode(GameMode.Dialogue);
            UI.Dialogue.Play(lines, () =>
            {
                SetMode(resume);
                onDone?.Invoke();
            });
        }

        public void Say(string speaker, string text, Action onDone = null) =>
            Say(new[] { new DialogueLine(speaker, text) }, onDone);

        // ---------------------------------------------------------------- scenes

        /// <summary>Called by the scene's SceneIntro once it has loaded.</summary>
        public void BeginScene(CaseData sceneCase, bool isHotel)
        {
            SetMode(GameMode.Explore);
            UI.CloseAll();

            // Opened a case scene directly in the Editor: adopt its case so it is playable.
            if (!isHotel && sceneCase != null && ActiveCase != sceneCase)
            {
                Debug.Log($"[Game] Scene started directly; adopting case '{sceneCase.id}'.");
                CasesClosed = Mathf.Max(0, Config.cases.IndexOf(sceneCase));
                BeginCaseState(sceneCase);
            }

            if (isHotel)
            {
                if (ActiveCase != null && ActiveCase.isFinal)
                {
                    Say(ActiveCase.arrivalLines);
                }
                else if (hotelWakePending)
                {
                    // Waking up (game start, or back from a case): the radio, word for word.
                    hotelWakePending = false;
                    var lines = new List<DialogueLine> { Config.radioLine };
                    if (Config.radioReactions.Count > 0)
                        lines.Add(new DialogueLine(Speakers.Dutch, Config.radioReactions[Mathf.Min(CasesClosed, Config.radioReactions.Count - 1)]));
                    Say(lines);
                }
            }
            else if (ActiveCase != null)
            {
                Say(ActiveCase.arrivalLines);
            }
            NotifyChanged();
        }

        void LoadScene(string sceneName, string spawnId = null)
        {
            pendingSpawn = spawnId;
            SetMode(GameMode.Explore);
            UI.CloseAll();
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>Walk through a door: load the scene and arrive at the named spawn point.</summary>
        public void GoToScene(string sceneName, string spawnId)
        {
            if (Mode != GameMode.Explore || string.IsNullOrEmpty(sceneName)) return;
            Debug.Log($"[Game] Door -> {sceneName} ({spawnId})");
            LoadScene(sceneName, spawnId);
        }

        /// <summary>The spawn point id for the scene that just loaded (null = the scene's default).</summary>
        public string ConsumePendingSpawn()
        {
            var id = pendingSpawn;
            pendingSpawn = null;
            return id;
        }

        // ---------------------------------------------------------------- newspaper / case lifecycle

        public void OpenNewspaper()
        {
            if (Mode != GameMode.Explore) return;
            if (ActiveCase != null)
            {
                Say(Speakers.Dutch, ActiveCase.isFinal ? "I've read enough." : "Already on a case. Paper can wait.");
                return;
            }
            var next = NextCase;
            if (next == null)
            {
                Say(Speakers.Dutch, "Nothing left in here I want to read.");
                return;
            }
            SetMode(GameMode.Newspaper);
            UI.Newspaper.Open(next);
        }

        public void CloseNewspaper()
        {
            UI.Newspaper.Close();
            SetMode(GameMode.Explore);
        }

        public void FollowLead()
        {
            var next = NextCase;
            CloseNewspaper();
            if (next == null) return;
            Say(next.leadReaction, () => StartCase(next));
        }

        void StartCase(CaseData c)
        {
            BeginCaseState(c);
            NotifyChanged();
            LoadScene(c.sceneName);
        }

        void BeginCaseState(CaseData c)
        {
            ActiveCase = c;
            startedCases.Add(c);
            Identified = false;
            CaseFile = new CaseFile();
        }

        void CloseCase()
        {
            var closed = ActiveCase;
            CasesClosed++;
            ActiveCase = null;
            Identified = false;
            NotifyChanged();
            Debug.Log($"[Game] Case closed: '{closed?.id}'. Cases closed: {CasesClosed}/{TotalCases}");

            if ((closed != null && closed.isFinal) || CasesClosed >= Config.cases.Count)
            {
                SetMode(GameMode.Ended);
                UI.CloseAll();
                UI.EndCard.Show(Config, CasesClosed, TotalCases);
                return;
            }
            hotelWakePending = true;
            Say(Speakers.Dutch, $"Case closed. That's {CasesClosed}.", () => LoadScene(Config.hotelScene));
        }

        // ---------------------------------------------------------------- investigation

        /// <summary>Logs a clue to the case file and journal. Returns true if it was new.</summary>
        public bool LogClue(ClueData clue)
        {
            bool isNew = CaseFile.TryAdd(clue);
            if (!string.IsNullOrEmpty(clue.symptom) && !journal.Contains(clue.symptom))
                journal.Add(clue.symptom);
            if (isNew)
            {
                Debug.Log($"[Case] Logged '{clue.id}' -> {clue.symptom}");
                NotifyChanged();
            }
            return isNew;
        }

        public bool HasSign(string symptom) => journal.Contains(symptom);

        public bool HasTool(ToolData tool) => tools.Contains(tool);

        public bool AddTool(ToolData tool)
        {
            if (tool == null || tools.Contains(tool)) return false;
            tools.Add(tool);
            Debug.Log($"[Kit] Picked up '{tool.id}'");
            NotifyChanged();
            return true;
        }

        // ---------------------------------------------------------------- the Book

        public List<BestiaryEntry> VisibleEntries()
        {
            var list = new List<BestiaryEntry>();
            foreach (var e in Config.bestiary)
            {
                if (e == null) continue;
                if (e.hiddenUntilItsCase && !EntityCaseStarted(e)) continue;
                list.Add(e);
            }
            return list;
        }

        bool EntityCaseStarted(BestiaryEntry e)
        {
            foreach (var c in startedCases)
                if (c.entity == e) return true;
            return false;
        }

        public void OpenBook()
        {
            if (Mode != GameMode.Explore) return;
            var entries = VisibleEntries();
            int start = 0;
            if (ActiveCase != null && ActiveCase.isFinal)
                start = Mathf.Max(0, entries.IndexOf(ActiveCase.entity));

            SetMode(GameMode.Book);
            UI.Book.Open(entries, start);

            if (ActiveCase != null && ActiveCase.bookRevealLines.Count > 0 && bookRevealPlayed.Add(ActiveCase))
                Say(ActiveCase.bookRevealLines);
        }

        public void CloseBook()
        {
            UI.Book.Close();
            SetMode(GameMode.Explore);
        }

        public int SignsFound(BestiaryEntry e)
        {
            int n = 0;
            foreach (var s in e.symptoms) if (HasSign(s)) n++;
            return n;
        }

        public void TryIdentify(BestiaryEntry entry)
        {
            if (entry == null) return;
            if (ActiveCase == null)
            {
                Say(Speakers.Dutch, "Nothing to hunt yet. Just reading.");
                return;
            }
            if (Identified)
            {
                Say(Speakers.Dutch, entry == ActiveCase.entity
                    ? "Already know what it is. Now I deal with it."
                    : "I already know what this is. It isn't that.");
                return;
            }

            int have = SignsFound(entry);
            int need = entry.symptoms.Count;
            if (entry != ActiveCase.entity)
            {
                Say(Speakers.Dutch, have == 0
                    ? $"Nothing I've seen points to {entry.displayName}."
                    : $"Some of it fits {entry.displayName}. Not all of it. Wrong page.");
                return;
            }
            if (have < need)
            {
                Say(Speakers.Dutch, $"Could be {entry.displayName}. {have} of {need} signs. Not enough to bet on yet.");
                return;
            }

            Identified = true;
            Debug.Log($"[Case] Identified '{entry.id}'");
            NotifyChanged();
            Say(ActiveCase.identifyLines, () =>
            {
                if (Mode == GameMode.Book) CloseBook();
            });
        }

        // ---------------------------------------------------------------- confront

        public void OpenTools()
        {
            if (Mode != GameMode.Explore) return;
            SetMode(GameMode.Tools);
            UI.Tools.Open(BuildToolOptions(), ToolPromptTitle());
        }

        string ToolPromptTitle()
        {
            if (ActiveCase != null && ActiveCase.isFinal && AllToolsSpent()) return "Nothing in the kit works.";
            return "What do I use?";
        }

        bool AllToolsSpent()
        {
            foreach (var t in tools) if (!spentTools.Contains(t)) return false;
            return true;
        }

        List<ToolOption> BuildToolOptions()
        {
            var options = new List<ToolOption>();
            if (ActiveCase != null && ActiveCase.isFinal && AllToolsSpent())
            {
                options.Add(ToolOption.Look);
                return options;
            }
            foreach (var t in tools)
                options.Add(ToolOption.ForTool(t, spentTools.Contains(t)));
            options.Add(ToolOption.StepBack);
            return options;
        }

        public void ChooseTool(ToolOption option)
        {
            UI.Tools.Close();
            SetMode(GameMode.Explore);

            switch (option.kind)
            {
                case ToolOption.Kind.StepBack:
                    return;
                case ToolOption.Kind.Look:
                    FinalLook();
                    return;
            }

            var tool = option.tool;
            if (ActiveCase == null || !Identified || tool == null) return;

            if (ActiveCase.isFinal)
            {
                if (!spentTools.Add(tool))
                {
                    Say(Speakers.Dutch, "Didn't work the first time.", OpenTools);
                    return;
                }
                Debug.Log($"[Case] Mirror-Born: '{tool.id}' fails");
                NotifyChanged();
                Say(Speakers.Dutch, tool.mirrorBornLine, OpenTools);
                return;
            }

            if (tool == ActiveCase.entity.weaknessTool)
            {
                Debug.Log($"[Case] Correct method '{tool.id}' for '{ActiveCase.entity.id}'");
                Say(ActiveCase.resolveLines, CloseCase);
            }
            else
            {
                Debug.Log($"[Case] Wrong method '{tool.id}'");
                Say(Speakers.Dutch, string.Format(ActiveCase.wrongToolLine, tool.displayName));
            }
        }

        void FinalLook()
        {
            ReflectionRevealed = true;
            Debug.Log("[Case] Dutch looks in the mirror.");
            NotifyChanged();
            Say(Config.endingLines, CloseCase);
        }
    }
}
