using UnityEngine;
using UnityEngine.UI;

namespace HollowSaint.UI
{
    /// <summary>Small helpers for building uGUI hierarchies from code.</summary>
    public static class UIFactory
    {
        public static Font Font;

        public static RectTransform Rect(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            return rt;
        }

        public static RectTransform Fill(Transform parent, string name, float inset = 0f) =>
            Rect(parent, name, Vector2.zero, Vector2.one, new Vector2(inset, inset), new Vector2(-inset, -inset));

        public static Image Panel(Transform parent, string name, Color color,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var rt = Rect(parent, name, anchorMin, anchorMax, offsetMin, offsetMax);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = color;
            return img;
        }

        public static Text Label(Transform parent, string name, int size, Color color, TextAnchor anchor,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var rt = Rect(parent, name, anchorMin, anchorMax, offsetMin, offsetMax);
            var text = rt.gameObject.AddComponent<Text>();
            text.font = Font;
            text.fontSize = size;
            text.color = color;
            text.alignment = anchor;
            text.supportRichText = true;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        /// <summary>A clickable row: background image + label. Colours are driven by the caller.</summary>
        public static Button Button(Transform parent, string name, Color background, out Text label, int fontSize, Color textColor,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var img = Panel(parent, name, background, anchorMin, anchorMax, offsetMin, offsetMax);
            var button = img.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            label = Label(img.transform, "Label", fontSize, textColor, TextAnchor.MiddleLeft,
                Vector2.zero, Vector2.one, new Vector2(12, 2), new Vector2(-12, -2));
            return button;
        }
    }
}
