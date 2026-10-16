using UnityEngine;
using UnityEngine.UI;
using HollowSaint.Core;

namespace HollowSaint.UI
{
    /// <summary>Final full-screen card after the last case closes.</summary>
    public class EndCardView : MonoBehaviour
    {
        GameObject root;
        Text title;
        Text body;

        public bool IsShown => root.activeSelf;
        public string Content => title.text + "\n" + body.text;

        public static EndCardView Create(Transform parent)
        {
            var bg = UIFactory.Panel(parent, "EndCard", Palette.NearBlack, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var view = bg.gameObject.AddComponent<EndCardView>();
            view.root = bg.gameObject;
            view.title = UIFactory.Label(bg.transform, "Title", 72, Palette.Amber, TextAnchor.MiddleCenter,
                new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(40, 0), new Vector2(-40, 140));
            view.title.fontStyle = FontStyle.Bold;
            view.body = UIFactory.Label(bg.transform, "Body", 28, Palette.Cream, TextAnchor.UpperCenter,
                new Vector2(0, 0), new Vector2(1, 0.5f), new Vector2(40, 40), new Vector2(-40, -10));
            view.root.SetActive(false);
            return view;
        }

        public void Show(GameConfig config, int closed, int total)
        {
            title.text = config.endTitle;
            body.text = $"CASES CLOSED  {closed} / {total}\n\n{config.endSubtitle}";
            root.SetActive(true);
        }
    }
}
