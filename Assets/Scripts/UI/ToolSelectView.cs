using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HollowSaint.Core;
using HollowSaint.Player;

namespace HollowSaint.UI
{
    /// <summary>Pick what to use on the creature. W/S choose, E confirm, Esc steps back.</summary>
    public class ToolSelectView : MonoBehaviour
    {
        const float RowHeight = 54f;

        GameObject root;
        RectTransform list;
        Text title;
        Text detail;

        readonly List<ToolOption> options = new List<ToolOption>();
        readonly List<Image> rowBackgrounds = new List<Image>();
        readonly List<Text> rowLabels = new List<Text>();
        int selected;

        public bool IsOpen => root.activeSelf;
        public IReadOnlyList<ToolOption> Options => options;
        public int Selected => selected;
        public string Title => title.text;

        public static ToolSelectView Create(Transform parent)
        {
            var rt = UIFactory.Fill(parent, "ToolSelect");
            var view = rt.gameObject.AddComponent<ToolSelectView>();
            view.root = rt.gameObject;

            var panel = UIFactory.Panel(rt, "Panel", Palette.NearBlack,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-330, -250), new Vector2(330, 290));
            UIFactory.Panel(panel.transform, "Edge", Palette.CoatShadow, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -6), Vector2.zero);
            view.title = UIFactory.Label(panel.transform, "Title", 30, Palette.Amber, TextAnchor.UpperLeft,
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(24, -60), new Vector2(-24, -16));
            view.list = UIFactory.Rect(panel.transform, "Options", new Vector2(0, 0), new Vector2(1, 1), new Vector2(20, 110), new Vector2(-20, -70));
            view.detail = UIFactory.Label(panel.transform, "Detail", 20, Palette.CoatLit, TextAnchor.UpperLeft,
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(24, 14), new Vector2(-24, 104));
            view.root.SetActive(false);
            return view;
        }

        public void Open(List<ToolOption> newOptions, string heading)
        {
            options.Clear();
            options.AddRange(newOptions);
            title.text = heading;
            selected = 0;
            BuildRows();
            root.SetActive(true);
            Render();
        }

        public void Close()
        {
            if (root != null) root.SetActive(false);
        }

        void BuildRows()
        {
            foreach (Transform child in list) Destroy(child.gameObject);
            rowBackgrounds.Clear();
            rowLabels.Clear();
            for (int i = 0; i < options.Count; i++)
            {
                int index = i;
                float top = -i * RowHeight;
                var button = UIFactory.Button(list, "Option" + i, Palette.NearBlack, out var label, 26, Palette.Cream,
                    new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, top - RowHeight + 4), new Vector2(0, top));
                button.onClick.AddListener(() => Choose(index));
                label.text = LabelFor(options[i]);
                rowBackgrounds.Add(button.GetComponent<Image>());
                rowLabels.Add(label);
            }
        }

        static string LabelFor(ToolOption o)
        {
            switch (o.kind)
            {
                case ToolOption.Kind.StepBack: return "Step back";
                case ToolOption.Kind.Look: return "Look.";
                default:
                    return o.spent
                        ? $"<color={Palette.Hex(Palette.NavyLight)}>{o.tool.displayName}  (didn't work)</color>"
                        : o.tool.displayName;
            }
        }

        void Update()
        {
            if (!IsOpen) return;
            var game = Game.Instance;
            if (game == null || !game.InputAllowedFor(GameMode.Tools)) return;

            if (InputReader.CancelPressedThisFrame())
            {
                game.ChooseTool(ToolOption.StepBack);
                return;
            }
            var nav = InputReader.NavPressedThisFrame();
            if (nav.y != 0)
            {
                selected = (selected - nav.y + options.Count) % options.Count;
                Render();
            }
            else if (InputReader.ConfirmPressedThisFrame())
            {
                Choose(selected);
            }
        }

        void Choose(int index)
        {
            var game = Game.Instance;
            if (game == null || !game.InputAllowedFor(GameMode.Tools) || index < 0 || index >= options.Count) return;
            game.ChooseTool(options[index]);
        }

        void Render()
        {
            for (int i = 0; i < rowBackgrounds.Count; i++)
            {
                bool on = i == selected;
                rowBackgrounds[i].color = on ? Palette.Navy : Palette.NearBlack;
                rowLabels[i].text = (on ? "▸ " : "   ") + LabelFor(options[i]);
            }
            var o = options[selected];
            detail.text = o.kind == ToolOption.Kind.Tool ? o.tool.description
                : o.kind == ToolOption.Kind.Look ? "" : "Not yet.";
        }
    }
}
