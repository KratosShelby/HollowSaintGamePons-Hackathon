using UnityEngine;
using HollowSaint.Core;
using HollowSaint.Interaction;

namespace HollowSaint.Investigation
{
    /// <summary>
    /// A scene object that logs its clue's symptom to the case file when examined.
    /// </summary>
    public class ClueInteractable : Interactable
    {
        [SerializeField] ClueData clue;
        [Tooltip("Supernatural-accent glow shown until the clue is logged.")]
        [SerializeField] GameObject glow;

        public ClueData Clue => clue;

        void OnEnable()
        {
            if (Game.Instance != null) Game.Instance.Changed += RefreshGlow;
            RefreshGlow();
        }

        void OnDisable()
        {
            if (Game.Instance != null) Game.Instance.Changed -= RefreshGlow;
        }

        void RefreshGlow()
        {
            if (glow == null) return;
            bool logged = Game.Instance != null && Game.Instance.CaseFile.Contains(clue);
            glow.SetActive(!logged);
        }

        public override void Interact(GameObject interactor)
        {
            if (clue == null)
            {
                Debug.LogWarning($"[Clue] {name} has no ClueData assigned.", this);
                return;
            }

            var game = Game.Instance;
            bool isNew = game.LogClue(clue);
            var lines = new[]
            {
                new DialogueLine(Speakers.Dutch, clue.description),
                new DialogueLine(Speakers.Narration, isNew
                    ? $"Case file + {clue.symptom}"
                    : $"Already in the case file: {clue.symptom}")
            };
            game.Say(lines);
        }
    }
}
