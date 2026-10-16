using System.Text;
using UnityEngine;
using UnityEngine.UI;
using HollowSaint.Core;
using HollowSaint.Investigation;
using HollowSaint.Player;

namespace HollowSaint.UI
{
    /// <summary>
    /// The hub: a front page of flavour headlines and one bold real lead. Clicking the lead
    /// (or E) sends Dutch to the case; Esc puts the paper down.
    /// </summary>
    public class NewspaperView : MonoBehaviour
    {
        GameObject root;
        Text masthead;
        Text edition;
        Text flavor;
        Text leadLabel;
        Text leadDetail;

        public bool IsOpen => root.activeSelf;
        public string LeadText => leadLabel.text;
        public string FlavorText => flavor.text;

        public static NewspaperView Create(Transform parent, GameConfig config)
        {
            var rt = UIFactory.Fill(parent, "Newspaper");
            var view = rt.gameObject.AddComponent<NewspaperView>();
            view.root = rt.gameObject;

            var paper = UIFactory.Panel(rt, "Paper", Palette.Cream,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-420, -330), new Vector2(420, 330));
            view.masthead = UIFactory.Label(paper.transform, "Masthead", 48, Palette.NearBlack, TextAnchor.UpperCenter,
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(20, -84), new Vector2(-20, -16));
            view.masthead.fontStyle = FontStyle.Bold;
            view.masthead.text = config.newspaperName;
            view.edition = UIFactory.Label(paper.transform, "Edition", 18, Palette.NavyLight, TextAnchor.UpperCenter,
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(20, -112), new Vector2(-20, -84));
            UIFactory.Panel(paper.transform, "Rule", Palette.NearBlack, new Vector2(0, 1), new Vector2(1, 1), new Vector2(30, -120), new Vector2(-30, -116));

            var lead = UIFactory.Button(paper.transform, "Lead", Palette.Cream, out view.leadLabel, 32, Palette.NearBlack,
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(30, -270), new Vector2(-30, -132));
            view.leadLabel.alignment = TextAnchor.UpperLeft;
            view.leadLabel.fontStyle = FontStyle.Bold;
            lead.onClick.AddListener(view.Follow);
            view.leadDetail = UIFactory.Label(paper.transform, "LeadDetail", 22, Palette.CoatShadow, TextAnchor.UpperLeft,
                new Vector2(0, 1), new Vector2(1, 1), new Vector2(42, -318), new Vector2(-42, -272));
            UIFactory.Panel(paper.transform, "Rule2", Palette.NearBlack, new Vector2(0, 1), new Vector2(1, 1), new Vector2(30, -332), new Vector2(-30, -330));

            view.flavor = UIFactory.Label(paper.transform, "Flavor", 22, Palette.Navy, TextAnchor.UpperLeft,
                new Vector2(0, 0), new Vector2(1, 1), new Vector2(42, 70), new Vector2(-42, -346));
            var footer = UIFactory.Label(paper.transform, "Footer", 18, Palette.NavyLight, TextAnchor.LowerCenter,
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(20, 14), new Vector2(-20, 50));
            footer.text = "click the bold story or press E to go     Esc to put it down";
            view.root.SetActive(false);
            return view;
        }

        public void Open(CaseData c)
        {
            edition.text = c.edition;
            leadLabel.text = c.leadHeadline;
            leadDetail.text = c.leadDetail;
            var sb = new StringBuilder();
            foreach (var h in c.flavorHeadlines) sb.Append(h).Append("\n\n");
            flavor.text = sb.ToString().TrimEnd();
            root.SetActive(true);
        }

        public void Close()
        {
            if (root != null) root.SetActive(false);
        }

        void Update()
        {
            if (!IsOpen) return;
            var game = Game.Instance;
            if (game == null || !game.InputAllowedFor(GameMode.Newspaper)) return;
            if (InputReader.CancelPressedThisFrame()) game.CloseNewspaper();
            else if (InputReader.ConfirmPressedThisFrame()) Follow();
        }

        void Follow()
        {
            var game = Game.Instance;
            if (game == null || !game.InputAllowedFor(GameMode.Newspaper)) return;
            game.FollowLead();
        }
    }
}
