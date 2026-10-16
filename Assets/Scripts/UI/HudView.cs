using System.Text;
using UnityEngine;
using UnityEngine.UI;
using HollowSaint.Core;
using HollowSaint.Interaction;

namespace HollowSaint.UI
{
    /// <summary>Interact prompt, live case file, case counter and control hints.</summary>
    public class HudView : MonoBehaviour
    {
        Text prompt;
        GameObject casePanel;
        Text caseText;
        Text counter;
        Text hint;

        public string PromptText => prompt.enabled ? prompt.text : null;
        public string CaseFileText => casePanel.activeSelf ? caseText.text : null;
        public string CounterText => counter.text;

        public static HudView Create(Transform parent)
        {
            var root = UIFactory.Fill(parent, "HUD");
            var hud = root.gameObject.AddComponent<HudView>();

            hud.prompt = UIFactory.Label(root, "InteractPrompt", 28, Palette.Amber, TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-300, 24), new Vector2(300, 64));

            var panel = UIFactory.Panel(root, "CaseFilePanel", Palette.NearBlack,
                new Vector2(0, 1), new Vector2(0, 1), new Vector2(16, -236), new Vector2(476, -16));
            hud.casePanel = panel.gameObject;
            hud.caseText = UIFactory.Label(panel.transform, "CaseFileText", 22, Palette.Cream, TextAnchor.UpperLeft,
                Vector2.zero, Vector2.one, new Vector2(16, 10), new Vector2(-16, -10));

            hud.counter = UIFactory.Label(root, "CaseCounter", 22, Palette.CoatLit, TextAnchor.UpperRight,
                new Vector2(1, 1), new Vector2(1, 1), new Vector2(-376, -52), new Vector2(-16, -16));
            hud.hint = UIFactory.Label(root, "Controls", 18, Palette.NavyLight, TextAnchor.LowerRight,
                new Vector2(1, 0), new Vector2(1, 0), new Vector2(-476, 8), new Vector2(-16, 34));
            hud.hint.text = "WASD move   E examine   B The Book";
            return hud;
        }

        void Update()
        {
            var game = Game.Instance;
            if (game == null) return;
            bool exploring = game.Mode == GameMode.Explore;

            var current = exploring && PlayerInteractor.Active != null ? PlayerInteractor.Active.Current : null;
            prompt.enabled = current != null;
            if (current != null) prompt.text = $"[E] {current.Prompt}";

            hint.enabled = exploring;
            counter.enabled = game.Mode != GameMode.Ended;
            counter.text = $"Cases closed  {game.CasesClosed} / {game.TotalCases}";

            var c = game.ActiveCase;
            casePanel.SetActive(c != null && game.Mode != GameMode.Ended);
            if (c != null) caseText.text = BuildCaseText(game);
        }

        static string BuildCaseText(Game game)
        {
            var sb = new StringBuilder();
            sb.Append("<b>CASE FILE</b>  ").Append(game.ActiveCase.title).AppendLine();
            var symptoms = game.CaseFile.Symptoms;
            if (symptoms.Count == 0) sb.AppendLine("  (nothing yet)");
            foreach (var s in symptoms) sb.Append("  • ").AppendLine(s);
            sb.Append(game.Identified
                ? $"<color={Palette.Hex(Palette.Amber)}>Named: {game.ActiveCase.entity.displayName}</color>"
                : $"<color={Palette.Hex(Palette.NavyLight)}>Not yet named. Check The Book (B).</color>");
            return sb.ToString();
        }
    }
}
