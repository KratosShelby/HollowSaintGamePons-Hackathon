using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using HollowSaint.Core;
using HollowSaint.Interaction;
using HollowSaint.Investigation;
using HollowSaint.Player;
using HollowSaint.World;

namespace HollowSaint.EditorTools
{
    /// <summary>
    /// Builds the four game scenes (hotel + three case rooms) from code and registers them in
    /// Build Settings. Every prop is a root (collider + behaviour, scale 1) with a "Visual" child
    /// holding the SpriteRenderer, so swapping in real art never disturbs gameplay colliders.
    ///
    /// Room space: the camera shows 20 x 11.25 units (320 x 180 px at 16 PPU). The back wall
    /// fills the top band; walkable floor is roughly x -9.6..9.6, y -5.2..3.6.
    /// </summary>
    public static class SceneBuilder
    {
        const string SceneDir = "Assets/Scenes";
        public const float CameraSize = 5.625f; // 180 px / 16 PPU / 2

        static Sprite square;
        static Sprite glow;
        static Sprite dutch;
        static Sprite dutchSilhouette;
        static Transform propsRoot;

        /// <summary>
        /// Art + content now, scenes on the next editor tick: saving content can reimport the
        /// case/clue assets, and references captured before that import go stale.
        /// </summary>
        [MenuItem("Hollow Saint/Build Everything")]
        static void BuildEverythingMenu()
        {
            PlaceholderArt.GenerateAll();
            ContentBuilder.BuildAll();
            EditorApplication.delayCall += () => Debug.Log(BuildScenes());
        }

        [MenuItem("Hollow Saint/Build Scenes")]
        static void BuildScenesMenu() => Debug.Log(BuildScenes());

        public static string BuildScenes()
        {
            square = PlaceholderArt.Load(PlaceholderArt.SquarePath);
            glow = PlaceholderArt.Load(PlaceholderArt.GlowPath);
            dutch = PlaceholderArt.Load(PlaceholderArt.DutchPath);
            dutchSilhouette = PlaceholderArt.Load(PlaceholderArt.DutchSilhouettePath);



            ConfigureProjectSettings();

            var built = new List<string>
            {
                BuildHotel(3),
                BuildFamished(0),
                BuildEcho(1),
                BuildLodger(2),
            };

            var scenes = new List<EditorBuildSettingsScene>();
            foreach (var path in built) scenes.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/SampleScene.unity") != null)
                AssetDatabase.DeleteAsset("Assets/Scenes/SampleScene.unity");

            EditorSceneManager.OpenScene(built[0]);
            return "Built: " + string.Join(", ", built);
        }

        static void ConfigureProjectSettings()
        {
            // Top-down: lower on screen draws in front.
            GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
            GraphicsSettings.transparencySortAxis = new Vector3(0f, 1f, 0f);
        }

        // ================================================================ scenes

        static string BuildHotel(int caseIndex)
        {
            BeginScene("Hotel  ·  Room 414, Hotel Caspian Rest", out var path, "Hotel");
            var finalCase = LoadCase(caseIndex);
            Room();

            // Back wall details.
            Decor("Window", new Vector2(-1f, 4.5f), new Vector2(3.2f, 1.6f), Palette.NearBlack, -14);
            Decor("CityLight1", new Vector2(-1.9f, 4.2f), new Vector2(0.2f, 0.2f), Palette.Amber, -13);
            Decor("CityLight2", new Vector2(-0.3f, 4.7f), new Vector2(0.2f, 0.2f), Palette.Amber, -13);
            Decor("CityLight3", new Vector2(0.4f, 4.1f), new Vector2(0.2f, 0.2f), Palette.Amber, -13);
            var lamp = Decor("StandingLamp", new Vector2(-8.8f, 3.2f), new Vector2(0.7f, 0.5f), Palette.Amber, 2);
            Decor("LampPole", new Vector2(-8.8f, 2.5f), new Vector2(0.15f, 1f), Palette.NearBlack, 1);
            Decor("Door", new Vector2(0f, -5.35f), new Vector2(1.8f, 0.5f), Palette.CoatShadow, -12);

            // Bed, nightstand + radio.
            var bed = Bed("Bed", new Vector2(-6.5f, 0.3f));
            var nightstand = Block("Nightstand", new Vector2(-4.3f, 2.4f), new Vector2(1f, 1f), Palette.CoatShadow);
            Decor("Radio", new Vector2(-4.3f, 2.55f), new Vector2(0.75f, 0.45f), Palette.Navy, 3, nightstand.transform);
            Decor("RadioDial", new Vector2(-4.1f, 2.55f), new Vector2(0.15f, 0.15f), Palette.Amber, 4, nightstand.transform);

            // Desk with newspaper and ashtray.
            Block("Desk", new Vector2(4f, 2.6f), new Vector2(3.6f, 1.2f), Palette.CoatShadow);
            var paper = Trigger("Newspaper", new Vector2(3.2f, 2.6f), new Vector2(1.0f, 0.7f), Palette.Cream, 3);
            Decor("PaperLines", new Vector2(3.2f, 2.7f), new Vector2(0.7f, 0.1f), Palette.NavyLight, 4, paper.transform);
            AddInteractable<NewspaperInteractable>(paper, "Read the newspaper");
            Decor("Ashtray", new Vector2(5.2f, 2.65f), new Vector2(0.45f, 0.3f), Palette.NavyLight, 3);
            Decor("Butt1", new Vector2(5.1f, 2.7f), new Vector2(0.15f, 0.08f), Palette.Cream, 4);
            Decor("Butt2", new Vector2(5.3f, 2.62f), new Vector2(0.15f, 0.08f), Palette.Cream, 4);

            // Mirror on the right wall.
            var mirror = Block("Mirror", new Vector2(9.35f, -0.5f), new Vector2(0.5f, 2f), Palette.CoatShadow, 1);
            Decor("Glass", new Vector2(9.35f, -0.5f), new Vector2(0.3f, 1.7f), Palette.NavyLight, 2, mirror.transform);
            var reflection = SpriteChild("Reflection", dutchSilhouette, new Vector2(9.35f, -1.35f), 0.85f, 3, mirror.transform);
            reflection.gameObject.SetActive(false);
            var mirrorTell = Glow("Tell", new Vector2(9.35f, 0.65f), mirror.transform);
            var mirrorBehaviour = AddInteractable<MirrorInteractable>(mirror, "Mirror");
            SerializedSet.Set(mirrorBehaviour, "reflection", reflection);
            SerializedSet.Set(mirrorBehaviour, "tell", mirrorTell);

            // Final-case clues: the room itself, only examinable during the Mirror-Born case.
            var finalClues = new GameObject("FinalCaseClues").transform;
            var radioClue = ClueSpot("RadioClue", finalCase.clues[0], "Listen to the radio", new Vector2(-4.3f, 2.4f), new Vector2(1.4f, 1.4f), finalClues);
            var bedClue = ClueSpot("BedClue", finalCase.clues[1], "Look at the bed", new Vector2(-6.5f, 0.3f), new Vector2(3.4f, 4.4f), finalClues);
            var ashClue = ClueSpot("AshtrayClue", finalCase.clues[2], "Look at the ashtray", new Vector2(5.2f, 2.65f), new Vector2(0.9f, 0.9f), finalClues);
            var conditional = new GameObject("FinalCaseSwitch").AddComponent<CaseConditional>();
            SerializedSet.Set(conditional, "whileCase", finalCase);
            SerializedSet.Set(conditional, "showDuring", new List<GameObject> { radioClue, bedClue, ashClue });
            foreach (var go in new[] { radioClue, bedClue, ashClue }) go.SetActive(false);

            Player(new Vector2(0f, -2f));
            Intro(null, isHotel: true);
            return Save(path);
        }

        static string BuildFamished(int caseIndex)
        {
            BeginScene("Case 1  ·  Harbor Street Diner kitchen", out var path, ContentBuilder.FamishedScene);
            var c = LoadCase(caseIndex);
            Room();

            Block("Counter", new Vector2(-4f, 3.0f), new Vector2(6f, 1f), Palette.CoatShadow);
            Decor("Stove", new Vector2(-6f, 3.05f), new Vector2(1.4f, 0.8f), Palette.NearBlack, 3);
            Decor("Pilot", new Vector2(-6f, 3.05f), new Vector2(0.2f, 0.2f), Palette.Amber, 4);
            Block("Island", new Vector2(2f, 0.4f), new Vector2(3f, 1.2f), Palette.CoatShadow);
            Decor("Plates", new Vector2(1.6f, 0.5f), new Vector2(0.8f, 0.3f), Palette.Cream, 3);

            var salt = Trigger("SaltShaker", new Vector2(0.2f, 2.9f), new Vector2(0.8f, 0.8f), Palette.Cream, 2, new Vector2(0.3f, 0.45f));
            var pickup = AddInteractable<ToolPickup>(salt, "Take the salt shaker");
            SerializedSet.Set(pickup, "tool", AssetDatabase.LoadAssetAtPath<ToolData>("Assets/Data/Tools/Tool_RockSalt.asset"));
            SerializedSet.Set(pickup, "lines", new List<DialogueLine>
            {
                new DialogueLine(Speakers.Dutch, "Salt shaker by the stove, still full. Coarse grey rock salt. This kitchen's own."),
            });

            var rot = Trigger("RotPatch", new Vector2(-4f, -2.2f), new Vector2(2.4f, 1.4f), Palette.NearBlack, -9);
            Clue(rot, c.clues[0], "Examine the stain");

            var lamp = Block("FlickeringLamp", new Vector2(4.5f, 3.2f), new Vector2(0.7f, 0.5f), Palette.Amber, 2);
            lamp.transform.Find("Visual").gameObject.AddComponent<ColorBlink>();
            Clue(lamp, c.clues[1], "Examine the lamp");

            var door = Block("PantryDoor", new Vector2(9.35f, -1.2f), new Vector2(0.5f, 2.2f), Palette.CoatShadow, 1);
            ClawMarks(door.transform, new Vector2(9.35f, -1.1f));
            Clue(door, c.clues[2], "Examine the pantry door");

            var threshold = Trigger("PantryThreshold", new Vector2(8.3f, -1.2f), new Vector2(0.9f, 1.4f), Palette.NearBlack, -8);
            var tell = Glow("Tell", new Vector2(8.3f, -1.2f), threshold.transform);
            var confront = AddInteractable<ConfrontTarget>(threshold, "Pantry");
            SerializedSet.Set(confront, "forCase", c);
            SerializedSet.Set(confront, "confrontPrompt", "Deal with what's in the pantry");
            SerializedSet.Set(confront, "hiddenUntilIdentified", true);
            SerializedSet.Set(confront, "visualRoot", threshold.transform.Find("Visual").gameObject);
            SerializedSet.Set(confront, "tell", tell);

            Player(new Vector2(-7f, -3.5f));
            Intro(c, isHotel: false);
            return Save(path);
        }

        static string BuildEcho(int caseIndex)
        {
            BeginScene("Case 2  ·  Lantern Lane No. 6, dining room", out var path, ContentBuilder.EchoScene);
            var c = LoadCase(caseIndex);
            Room();

            var table = Block("DiningTable", new Vector2(0f, 0f), new Vector2(4f, 1.6f), Palette.CoatShadow);
            Decor("Cloth", new Vector2(0f, 0.05f), new Vector2(3.4f, 1.2f), Palette.Cream, 2, table.transform);
            Decor("SilverCase", new Vector2(0f, 0.1f), new Vector2(1.2f, 0.45f), Palette.NavyLight, 3, table.transform);
            Decor("ChairLeft", new Vector2(-2.5f, 0f), new Vector2(0.7f, 0.7f), Palette.CoatLit, 1);
            Decor("ChairRight", new Vector2(2.5f, 0f), new Vector2(0.7f, 0.7f), Palette.CoatLit, 1);
            var tell = SpriteChild("Tell", glow, new Vector2(0f, 1.3f), 1.5f, 8, table.transform);
            tell.color = Palette.Supernatural;
            tell.gameObject.AddComponent<ColorBlink>();
            SetBlink(tell.gameObject, Palette.Supernatural, hide: true);
            var confront = AddInteractable<ConfrontTarget>(table, "Silver service");
            SerializedSet.Set(confront, "forCase", c);
            SerializedSet.Set(confront, "confrontPrompt", "Return what was lost");
            SerializedSet.Set(confront, "tell", tell.gameObject);
            SerializedSet.Set(confront, "notReadyLines", new List<DialogueLine>
            {
                new DialogueLine(Speakers.Dutch, "A silver service laid for one. Eleven spoons in a case cut for twelve."),
            });

            var vent = Block("WhisperingVent", new Vector2(-9.35f, 1f), new Vector2(0.5f, 0.8f), Palette.NearBlack, 1);
            Clue(vent, c.clues[0], "Listen at the vent");

            var sideboard = Block("Sideboard", new Vector2(5f, 3.0f), new Vector2(3f, 0.9f), Palette.CoatShadow);
            Decor("Candle1", new Vector2(4.3f, 3.15f), new Vector2(0.18f, 0.5f), Palette.Amber, 3, sideboard.transform);
            Decor("Candle2", new Vector2(5.8f, 3.15f), new Vector2(0.18f, 0.5f), Palette.Amber, 3, sideboard.transform);
            Clue(sideboard, c.clues[1], "Examine the sideboard");

            var hallMirror = Block("HallMirror", new Vector2(-4f, 3.25f), new Vector2(1f, 0.8f), Palette.CoatShadow, 1);
            Decor("Glass", new Vector2(-4f, 3.25f), new Vector2(0.75f, 0.6f), Palette.NavyLight, 2, hallMirror.transform);
            Clue(hallMirror, c.clues[2], "Look at the mirror");

            Block("Armchair", new Vector2(8.6f, -2.2f), new Vector2(1.2f, 1.2f), Palette.NavyLight);
            Decor("SideTable", new Vector2(7.2f, -3.3f), new Vector2(0.9f, 0.6f), Palette.CoatShadow, 1);
            var spoon = Trigger("SideTableDrawer", new Vector2(7.2f, -3.6f), new Vector2(0.9f, 0.6f), Palette.CoatLit, 2, new Vector2(0.6f, 0.12f));
            var pickup = AddInteractable<ToolPickup>(spoon, "Open the drawer");
            SerializedSet.Set(pickup, "tool", AssetDatabase.LoadAssetAtPath<ToolData>("Assets/Data/Tools/Tool_SilverSpoon.asset"));
            SerializedSet.Set(pickup, "lines", new List<DialogueLine>
            {
                new DialogueLine(Speakers.Dutch, "Side-table drawer, under the napkins: a silver spoon. Tarnished. Engraved \"L.A.\""),
            });

            Player(new Vector2(-7f, -3.5f));
            Intro(c, isHotel: false);
            return Save(path);
        }

        static string BuildLodger(int caseIndex)
        {
            BeginScene("Case 3  ·  Caravanserai Inn, Room 9", out var path, ContentBuilder.LodgerScene);
            var c = LoadCase(caseIndex);
            Room();

            var bed = Bed("Bed", new Vector2(-6.5f, 0.3f));
            Clue(bed, c.clues[2], "Look at the bed");
            var nightstand = Block("Nightstand", new Vector2(-4.3f, 2.4f), new Vector2(1f, 1f), Palette.CoatShadow);
            Decor("Lamp", new Vector2(-4.3f, 2.6f), new Vector2(0.4f, 0.4f), Palette.Amber, 3, nightstand.transform);

            var clock = Block("WallClock", new Vector2(0f, 3.25f), new Vector2(0.8f, 0.8f), Palette.Cream, 1);
            Decor("Hand", new Vector2(0.1f, 3.2f), new Vector2(0.3f, 0.08f), Palette.NearBlack, 2, clock.transform);
            Clue(clock, c.clues[0], "Look at the clock");

            Decor("Window", new Vector2(5f, 4.4f), new Vector2(2.6f, 1.4f), Palette.NearBlack, -14);
            var sill = Block("Windowsill", new Vector2(5f, 3.0f), new Vector2(2.4f, 0.5f), Palette.CoatShadow);
            Decor("Burn1", new Vector2(4.3f, 3.0f), new Vector2(0.2f, 0.12f), Palette.NearBlack, 3, sill.transform);
            Decor("Burn2", new Vector2(4.9f, 3.05f), new Vector2(0.2f, 0.12f), Palette.NearBlack, 3, sill.transform);
            Decor("Burn3", new Vector2(5.6f, 2.95f), new Vector2(0.2f, 0.12f), Palette.NearBlack, 3, sill.transform);
            Clue(sill, c.clues[1], "Examine the windowsill");

            var desk = Block("WritingDesk", new Vector2(3f, -2.6f), new Vector2(2.4f, 1f), Palette.CoatShadow);
            Decor("Register", new Vector2(3f, -2.55f), new Vector2(1f, 0.6f), Palette.Cream, 3, desk.transform);
            var tell = Glow("Tell", new Vector2(3f, -2.2f), desk.transform);
            var confront = AddInteractable<ConfrontTarget>(desk, "Guest register");
            SerializedSet.Set(confront, "forCase", c);
            SerializedSet.Set(confront, "confrontPrompt", "Strike the name");
            SerializedSet.Set(confront, "tell", tell);
            SerializedSet.Set(confront, "notReadyLines", new List<DialogueLine>
            {
                new DialogueLine(Speakers.Dutch, "The inn's register. Room 9: a name in fresh ink, signed every night. The ink won't hold still long enough to read."),
            });

            Block("Radiator", new Vector2(9.35f, -2.5f), new Vector2(0.5f, 1.2f), Palette.NavyLight, 1);
            var pen = Trigger("FountainPen", new Vector2(8.4f, -3.6f), new Vector2(0.6f, 0.6f), Palette.Navy, 2, new Vector2(0.45f, 0.12f));
            var pickup = AddInteractable<ToolPickup>(pen, "Pick up the pen");
            SerializedSet.Set(pickup, "tool", AssetDatabase.LoadAssetAtPath<ToolData>("Assets/Data/Tools/Tool_RegisterPen.asset"));
            SerializedSet.Set(pickup, "lines", new List<DialogueLine>
            {
                new DialogueLine(Speakers.Dutch, "Behind the radiator: a fountain pen. Nib's still wet."),
            });

            Player(new Vector2(-2f, -3.8f));
            Intro(c, isHotel: false);
            return Save(path);
        }

        // ================================================================ building blocks

        /// <summary>
        /// Load a case after the scene it goes into has been created: NewScene unloads assets
        /// nothing references yet, which would leave earlier-loaded references dead.
        /// </summary>
        static CaseData LoadCase(int index) => AssetDatabase.LoadAssetAtPath<CaseData>(ContentBuilder.CasePaths[index]);

        static void BeginScene(string label, out string path, string sceneName)
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            // NewScene unloads unreferenced assets; (re)load what this scene will reference.
            square = PlaceholderArt.Load(PlaceholderArt.SquarePath);
            glow = PlaceholderArt.Load(PlaceholderArt.GlowPath);
            dutch = PlaceholderArt.Load(PlaceholderArt.DutchPath);
            dutchSilhouette = PlaceholderArt.Load(PlaceholderArt.DutchSilhouettePath);
            path = $"{SceneDir}/{sceneName}.unity";
            var note = new GameObject("// " + label);
            note.tag = "EditorOnly";

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.transform.position = new Vector3(0f, 0f, -10f);
            var cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = CameraSize;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Palette.NearBlack;
            camGo.AddComponent<AudioListener>();

            propsRoot = new GameObject("Room").transform;
        }

        static string Save(string path)
        {
            System.IO.Directory.CreateDirectory(SceneDir);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
            return path;
        }

        static void Room()
        {
            Decor("Floor", new Vector2(0f, -0.8f), new Vector2(19.2f, 8.8f), Palette.Navy, -20);
            Block("BackWall", new Vector2(0f, 4.6f), new Vector2(20f, 2.05f), Palette.NavyLight, -15);
            Decor("Skirting", new Vector2(0f, 3.62f), new Vector2(19.2f, 0.12f), Palette.NearBlack, -14);
            Block("FrontWall", new Vector2(0f, -5.4f), new Vector2(20f, 0.45f), Palette.NearBlack, -15);
            Block("LeftWall", new Vector2(-9.8f, 0f), new Vector2(0.4f, 11.25f), Palette.NearBlack, -15);
            Block("RightWall", new Vector2(9.8f, 0f), new Vector2(0.4f, 11.25f), Palette.NearBlack, -15);
        }

        static GameObject Bed(string name, Vector2 pos)
        {
            var bed = Block(name, pos, new Vector2(3f, 4f), Palette.CoatShadow);
            Decor("Blanket", pos + new Vector2(0f, -0.4f), new Vector2(2.6f, 2.9f), Palette.NavyLight, 2, bed.transform);
            Decor("Pillow", pos + new Vector2(0f, 1.45f), new Vector2(2.2f, 0.6f), Palette.Cream, 2, bed.transform);
            return bed;
        }

        static void ClawMarks(Transform parent, Vector2 center)
        {
            for (int i = 0; i < 3; i++)
            {
                var m = Decor("ClawMark" + i, center + new Vector2(-0.12f + i * 0.12f, 0.1f), new Vector2(0.05f, 0.6f), Palette.NearBlack, 3, parent);
                m.transform.localRotation = Quaternion.Euler(0, 0, 10f);
            }
        }

        /// <summary>Solid prop: root with BoxCollider2D, "Visual" child sprite.</summary>
        static GameObject Block(string name, Vector2 pos, Vector2 size, Color color, int order = 0, Transform parent = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent != null ? parent : propsRoot, false);
            go.transform.position = pos;
            go.AddComponent<BoxCollider2D>().size = size;
            Visual(go.transform, size, color, order);
            return go;
        }

        /// <summary>Walk-through prop (trigger collider). visualSize defaults to the trigger size.</summary>
        static GameObject Trigger(string name, Vector2 pos, Vector2 size, Color color, int order = 0, Vector2? visualSize = null)
        {
            var go = Block(name, pos, size, color, order);
            go.GetComponent<BoxCollider2D>().isTrigger = true;
            if (visualSize.HasValue) go.transform.Find("Visual").localScale = new Vector3(visualSize.Value.x, visualSize.Value.y, 1f);
            return go;
        }

        /// <summary>Invisible trigger that carries a clue; used to make existing props examinable.</summary>
        static GameObject ClueSpot(string name, ClueData clue, string prompt, Vector2 pos, Vector2 size, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            var col = go.AddComponent<BoxCollider2D>();
            col.size = size;
            col.isTrigger = true;
            Clue(go, clue, prompt);
            return go;
        }

        static void Visual(Transform parent, Vector2 size, Color color, int order)
        {
            var v = new GameObject("Visual");
            v.transform.SetParent(parent, false);
            v.transform.localScale = new Vector3(size.x, size.y, 1f);
            var sr = v.AddComponent<SpriteRenderer>();
            sr.sprite = square;
            sr.color = color;
            sr.sortingOrder = order;
        }

        static GameObject Decor(string name, Vector2 pos, Vector2 size, Color color, int order, Transform parent = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent != null ? parent : propsRoot, false);
            go.transform.position = pos;
            go.transform.localScale = new Vector3(size.x, size.y, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = square;
            sr.color = color;
            sr.sortingOrder = order;
            return go;
        }

        static SpriteRenderer SpriteChild(string name, Sprite sprite, Vector2 pos, float scale, int order, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = pos;
            go.transform.localScale = new Vector3(scale, scale, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = order;
            return sr;
        }

        /// <summary>Blinking supernatural-accent marker (clue glow / entity tell).</summary>
        static GameObject Glow(string name, Vector2 pos, Transform parent)
        {
            var sr = SpriteChild(name, glow, pos, 0.6f, 9, parent);
            sr.color = Palette.Supernatural;
            sr.gameObject.AddComponent<ColorBlink>();
            SetBlink(sr.gameObject, Palette.Supernatural, hide: true);
            return sr.gameObject;
        }

        static void SetBlink(GameObject go, Color on, bool hide)
        {
            var blink = go.GetComponent<ColorBlink>();
            SerializedSet.Set(blink, "onColor", on);
            SerializedSet.Set(blink, "hideWhenOff", hide);
            SerializedSet.Set(blink, "onSeconds", new Vector2(0.5f, 0.9f));
            SerializedSet.Set(blink, "offSeconds", new Vector2(0.25f, 0.4f));
        }

        static T AddInteractable<T>(GameObject go, string prompt) where T : Interactable
        {
            var c = go.AddComponent<T>();
            SerializedSet.Set(c, "prompt", prompt);
            return c;
        }

        static void Clue(GameObject go, ClueData clue, string prompt)
        {
            var glowGo = Glow("ClueGlow", (Vector2)go.transform.position + new Vector2(0f, 0.2f), go.transform);
            var c = AddInteractable<ClueInteractable>(go, prompt);
            SerializedSet.Set(c, "clue", clue);
            SerializedSet.Set(c, "glow", glowGo);
        }

        static void Player(Vector2 spawn)
        {
            var go = new GameObject("Player");
            go.transform.position = spawn;
            go.AddComponent<Rigidbody2D>();
            var col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.8f, 0.5f);
            col.offset = new Vector2(0f, 0.25f);
            var visual = SpriteChild("Visual", dutch, spawn + new Vector2(0f, -0.05f), 1f, 5, go.transform);
            var controller = go.AddComponent<PlayerController>();
            SerializedSet.Set(controller, "visual", visual);
            go.AddComponent<PlayerInteractor>();
        }

        static void Intro(CaseData c, bool isHotel)
        {
            var intro = new GameObject("SceneIntro").AddComponent<SceneIntro>();
            SerializedSet.Set(intro, "sceneCase", c);
            SerializedSet.Set(intro, "isHotel", isHotel);
        }
    }
}
