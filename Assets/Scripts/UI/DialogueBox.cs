using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HollowSaint.Core;
using HollowSaint.Player;

namespace HollowSaint.UI
{
    /// <summary>Bottom-of-screen box that shows lines one at a time. E / Enter / click advances.</summary>
    public class DialogueBox : MonoBehaviour
    {
        GameObject panel;
        Text speakerLabel;
        Text bodyLabel;
        Text hintLabel;

        readonly List<DialogueLine> lines = new List<DialogueLine>();
        int index;
        Action onDone;

        public bool IsOpen => panel.activeSelf;
        public string CurrentSpeaker => IsOpen ? lines[index].speaker : null;
        public string CurrentText => IsOpen ? lines[index].text : null;

        public static DialogueBox Create(Transform parent)
        {
            var root = UIFactory.Fill(parent, "DialogueBox");
            var box = root.gameObject.AddComponent<DialogueBox>();

            var bg = UIFactory.Panel(root, "Panel", Palette.NearBlack,
                new Vector2(0.08f, 0.03f), new Vector2(0.92f, 0.27f), Vector2.zero, Vector2.zero);
            var border = UIFactory.Panel(bg.transform, "Border", Palette.NavyLight, Vector2.zero, Vector2.one, new Vector2(4, 4), new Vector2(-4, -4));
            UIFactory.Panel(border.transform, "Inner", Palette.NearBlack, Vector2.zero, Vector2.one, new Vector2(3, 3), new Vector2(-3, -3));
            box.panel = bg.gameObject;

            var click = bg.gameObject.AddComponent<Button>();
            click.transition = Selectable.Transition.None;
            click.navigation = new Navigation { mode = Navigation.Mode.None };
            click.onClick.AddListener(box.Advance);

            box.speakerLabel = UIFactory.Label(bg.transform, "Speaker", 26, Palette.Amber, TextAnchor.UpperLeft,
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(28, -50), new Vector2(-28, -14));
            box.bodyLabel = UIFactory.Label(bg.transform, "Body", 28, Palette.Cream, TextAnchor.UpperLeft,
                Vector2.zero, Vector2.one, new Vector2(28, 20), new Vector2(-28, -54));
            box.hintLabel = UIFactory.Label(bg.transform, "Hint", 20, Palette.NavyLight, TextAnchor.LowerRight,
                new Vector2(1, 0), new Vector2(1, 0), new Vector2(-220, 10), new Vector2(-20, 40));
            box.hintLabel.text = "E  ▸";
            box.panel.SetActive(false);
            return box;
        }

        public void Play(IList<DialogueLine> newLines, Action done)
        {
            lines.Clear();
            lines.AddRange(newLines);
            index = 0;
            onDone = done;
            panel.SetActive(true);
            Render();
        }

        /// <summary>Hide without firing the completion callback (scene change).</summary>
        public void Cancel()
        {
            lines.Clear();
            onDone = null;
            if (panel != null) panel.SetActive(false);
        }

        void Update()
        {
            if (!IsOpen) return;
            var game = Game.Instance;
            if (game != null && game.InputAllowedFor(GameMode.Dialogue) && InputReader.ConfirmPressedThisFrame())
                Advance();
        }

        void Advance()
        {
            if (!IsOpen) return;
            var game = Game.Instance;
            if (game != null && !game.InputAllowedFor(GameMode.Dialogue)) return;

            index++;
            if (index < lines.Count)
            {
                Render();
                return;
            }
            panel.SetActive(false);
            var done = onDone;
            onDone = null;
            done?.Invoke();
        }

        void Render()
        {
            var line = lines[index];
            bool narration = string.IsNullOrEmpty(line.speaker);
            speakerLabel.text = narration ? "" : line.speaker.ToUpperInvariant();
            speakerLabel.color = line.speaker == Speakers.Reflection ? Palette.Supernatural : Palette.Amber;
            bodyLabel.text = narration ? $"<i>{line.text}</i>" : line.text;
            bodyLabel.color = narration ? Palette.CoatLit : Palette.Cream;
        }
    }
}
