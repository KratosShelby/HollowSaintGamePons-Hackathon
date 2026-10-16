using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using HollowSaint.Core;
using HollowSaint.Investigation;
using HollowSaint.Player;

namespace HollowSaint.UI
{
    /// <summary>
    /// The Book: one bestiary entry per spread. Symptoms already in Dutch's journal are ticked,
    /// so cross-referencing is a matter of finding the page where everything lines up.
    /// A/D turn pages, E names the creature, B/Esc closes.
    /// </summary>
    public class BookView : MonoBehaviour
    {
        GameObject root;
        Text leftPage;
        Text rightPage;
        Text footer;

        readonly List<BestiaryEntry> entries = new List<BestiaryEntry>();
        int page;

        public bool IsOpen => root.activeSelf;
        public BestiaryEntry CurrentEntry => IsOpen && entries.Count > 0 ? entries[page] : null;
        public string LeftText => leftPage.text;
        public string RightText => rightPage.text;

        public static BookView Create(Transform parent)
        {
            var rt = UIFactory.Fill(parent, "Book");
            var view = rt.gameObject.AddComponent<BookView>();
            view.root = rt.gameObject;

            var cover = UIFactory.Panel(rt, "Cover", Palette.CoatShadow,
                new Vector2(0.07f, 0.1f), new Vector2(0.93f, 0.95f), Vector2.zero, Vector2.zero);
            var left = UIFactory.Panel(cover.transform, "LeftPage", Palette.Cream,
                new Vector2(0, 0), new Vector2(0.5f, 1), new Vector2(18, 64), new Vector2(-6, -18));
            var right = UIFactory.Panel(cover.transform, "RightPage", Palette.Cream,
                new Vector2(0.5f, 0), new Vector2(1, 1), new Vector2(6, 64), new Vector2(-18, -18));
            view.leftPage = UIFactory.Label(left.transform, "Text", 22, Palette.Navy, TextAnchor.UpperLeft,
                Vector2.zero, Vector2.one, new Vector2(28, 20), new Vector2(-28, -24));
            view.rightPage = UIFactory.Label(right.transform, "Text", 19, Palette.Navy, TextAnchor.UpperLeft,
                Vector2.zero, Vector2.one, new Vector2(24, 60), new Vector2(-24, -20));
            view.footer = UIFactory.Label(cover.transform, "Footer", 20, Palette.Cream, TextAnchor.MiddleCenter,
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(200, 12), new Vector2(-200, 56));

            Button prev = UIFactory.Button(cover.transform, "Prev", Palette.Navy, out var prevLabel, 22, Palette.Cream,
                new Vector2(0, 0), new Vector2(0, 0), new Vector2(18, 12), new Vector2(170, 52));
            prevLabel.text = "◂ prev";
            prev.onClick.AddListener(() => view.Turn(-1));
            Button next = UIFactory.Button(cover.transform, "Next", Palette.Navy, out var nextLabel, 22, Palette.Cream,
                new Vector2(1, 0), new Vector2(1, 0), new Vector2(-170, 12), new Vector2(-18, 52));
            nextLabel.text = "next ▸";
            nextLabel.alignment = TextAnchor.MiddleRight;
            next.onClick.AddListener(() => view.Turn(1));
            Button name = UIFactory.Button(right.transform, "NameIt", Palette.Navy, out var nameLabel, 22, Palette.Amber,
                new Vector2(1, 0), new Vector2(1, 0), new Vector2(-220, 14), new Vector2(-14, 54));
            nameLabel.text = "[E] Name it";
            nameLabel.alignment = TextAnchor.MiddleCenter;
            name.onClick.AddListener(view.NameCurrent);

            view.root.SetActive(false);
            return view;
        }

        public void Open(List<BestiaryEntry> visible, int startPage)
        {
            entries.Clear();
            entries.AddRange(visible);
            page = Mathf.Clamp(startPage, 0, Mathf.Max(0, entries.Count - 1));
            root.SetActive(true);
            if (Game.Instance != null) Game.Instance.Changed += Render;
            Render();
        }

        public void Close()
        {
            if (Game.Instance != null) Game.Instance.Changed -= Render;
            if (root != null) root.SetActive(false);
        }

        void Update()
        {
            if (!IsOpen) return;
            var game = Game.Instance;
            if (game == null || !game.InputAllowedFor(GameMode.Book)) return;

            if (InputReader.BookPressedThisFrame() || InputReader.CancelPressedThisFrame())
            {
                game.CloseBook();
                return;
            }
            var nav = InputReader.NavPressedThisFrame();
            if (nav.x != 0) Turn(nav.x);
            else if (InputReader.ConfirmPressedThisFrame()) NameCurrent();
        }

        void Turn(int delta)
        {
            if (entries.Count == 0) return;
            var game = Game.Instance;
            if (game != null && !game.InputAllowedFor(GameMode.Book)) return;
            page = (page + delta + entries.Count) % entries.Count;
            Render();
        }

        void NameCurrent()
        {
            var game = Game.Instance;
            if (game == null || !game.InputAllowedFor(GameMode.Book) || CurrentEntry == null) return;
            game.TryIdentify(CurrentEntry);
        }

        void Render()
        {
            if (entries.Count == 0)
            {
                leftPage.text = "(empty)";
                rightPage.text = "";
                return;
            }
            var game = Game.Instance;
            var e = entries[page];
            string ink = Palette.Hex(e.handwritten ? Palette.CoatShadow : Palette.Navy);
            string dim = Palette.Hex(Palette.NavyLight);
            string hi = Palette.Hex(Palette.CoatShadow);

            var left = new StringBuilder();
            left.Append($"<size=36><b><color={ink}>{(e.handwritten ? $"<i>{e.displayName}</i>" : e.displayName)}</color></b></size>\n");
            left.Append($"<color={dim}>{e.classification}</color>\n\n");
            left.Append(e.handwritten ? $"<i><color={ink}>{e.lore}</color></i>" : e.lore);
            leftPage.text = left.ToString();

            int found = game != null ? game.SignsFound(e) : 0;
            var right = new StringBuilder();
            right.Append($"<b>SIGNS</b>  <color={dim}>{found} / {e.symptoms.Count} seen</color>\n");
            foreach (var s in e.symptoms)
            {
                bool seen = game != null && game.HasSign(s);
                right.Append(seen ? $"<b>[x] {s}</b>\n" : $"<color={dim}>[  ] {s}</color>\n");
            }
            right.Append("\n");
            right.Append(e.handwritten ? $"<i><color={ink}>{e.weaknessText}</color></i>" : e.weaknessText);
            if (game != null && game.Identified && game.ActiveCase != null && game.ActiveCase.entity == e)
                right.Append($"\n\n<color={hi}><b>— NAMED —</b></color>");
            rightPage.text = right.ToString();

            footer.text = $"page {page + 1} / {entries.Count}     A / D turn     B close";
        }
    }
}
