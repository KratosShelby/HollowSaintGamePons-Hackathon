using UnityEngine;
using HollowSaint.Core;
using HollowSaint.Interaction;

namespace HollowSaint.World
{
    /// <summary>
    /// The hotel mirror. A throwaway line for most of the game; the confrontation point of the
    /// final case, where "the weakness" is to look.
    /// </summary>
    public class MirrorInteractable : Interactable
    {
        [Tooltip("Dutch's reflection; hidden until he finally looks, then shown in the supernatural accent.")]
        [SerializeField] SpriteRenderer reflection;
        [Tooltip("Supernatural-accent tell on the frame, shown once the Mirror-Born is identified.")]
        [SerializeField] GameObject tell;

        static bool FinalCase => Game.Instance != null && Game.Instance.ActiveCase != null && Game.Instance.ActiveCase.isFinal;
        static bool Ready => FinalCase && Game.Instance.Identified;

        public override string Prompt => Ready ? "Look in the mirror" : base.Prompt;

        void OnEnable()
        {
            if (Game.Instance != null) Game.Instance.Changed += Refresh;
            Refresh();
        }

        void OnDisable()
        {
            if (Game.Instance != null) Game.Instance.Changed -= Refresh;
        }

        void Refresh()
        {
            var game = Game.Instance;
            bool revealed = game != null && game.ReflectionRevealed;
            if (tell != null) tell.SetActive(Ready && !revealed);
            if (reflection != null)
            {
                reflection.gameObject.SetActive(revealed);
                reflection.color = Palette.Supernatural;
            }
        }

        public override void Interact(GameObject interactor)
        {
            var game = Game.Instance;
            if (Ready)
            {
                game.OpenTools();
                return;
            }
            if (FinalCase)
            {
                game.Say(Speakers.Dutch, "Not yet. Something's wrong in this room, and I want to know what first.");
                return;
            }
            var lines = game.Config.mirrorLines;
            if (lines.Count == 0) return;
            game.Say(Speakers.Dutch, lines[Mathf.Min(game.CasesClosed, lines.Count - 1)]);
        }
    }
}
