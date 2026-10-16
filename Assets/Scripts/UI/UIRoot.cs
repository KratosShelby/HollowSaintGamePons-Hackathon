using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using HollowSaint.Core;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace HollowSaint.UI
{
    /// <summary>
    /// The persistent screen-space UI. Built from code at boot so every scene gets the same HUD,
    /// dialogue box, Book, tool select, newspaper and end card without per-scene wiring.
    /// </summary>
    public class UIRoot : MonoBehaviour
    {
        public const float ReferenceWidth = 1280f;
        public const float ReferenceHeight = 720f;

        public HudView Hud { get; private set; }
        public BookView Book { get; private set; }
        public ToolSelectView Tools { get; private set; }
        public NewspaperView Newspaper { get; private set; }
        public DialogueBox Dialogue { get; private set; }
        public EndCardView EndCard { get; private set; }

        public static UIRoot Create(Transform parent, GameConfig config)
        {
            UIFactory.Font = ResolveFont(config);

            var go = new GameObject("UI", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();

            var es = new GameObject("EventSystem", typeof(EventSystem));
            es.transform.SetParent(parent, false);
#if ENABLE_INPUT_SYSTEM
            es.AddComponent<InputSystemUIInputModule>();
#else
            es.AddComponent<StandaloneInputModule>();
#endif

            var root = go.AddComponent<UIRoot>();
            // Sibling order = draw order: dialogue sits above the full-screen views.
            root.Hud = HudView.Create(go.transform);
            root.Book = BookView.Create(go.transform);
            root.Tools = ToolSelectView.Create(go.transform);
            root.Newspaper = NewspaperView.Create(go.transform, config);
            root.Dialogue = DialogueBox.Create(go.transform);
            root.EndCard = EndCardView.Create(go.transform);
            root.CloseAll();
            return root;
        }

        static Font ResolveFont(GameConfig config)
        {
            if (config.uiFont != null) return config.uiFont;
            if (config.osFontNames != null && config.osFontNames.Length > 0)
            {
                var installed = new System.Collections.Generic.HashSet<string>(Font.GetOSInstalledFontNames());
                foreach (var name in config.osFontNames)
                    if (installed.Contains(name))
                        return Font.CreateDynamicFontFromOSFont(name, 32);
            }
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        public void CloseAll()
        {
            Book.Close();
            Tools.Close();
            Newspaper.Close();
            Dialogue.Cancel();
        }
    }
}
