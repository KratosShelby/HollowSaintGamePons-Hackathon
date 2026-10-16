using System.Collections.Generic;
using UnityEngine;
using HollowSaint.Core;
using HollowSaint.Interaction;
using HollowSaint.Investigation;

namespace HollowSaint.World
{
    /// <summary>
    /// Where Dutch confronts the case's creature. Once the creature is identified in the Book,
    /// its tell appears and interacting opens the tool select.
    /// </summary>
    public class ConfrontTarget : Interactable
    {
        [SerializeField] CaseData forCase;
        [SerializeField] string confrontPrompt = "Confront";
        [Tooltip("Hide this object entirely (visual + collider) until the creature is identified.")]
        [SerializeField] bool hiddenUntilIdentified;
        [Tooltip("Visual root toggled by hiddenUntilIdentified.")]
        [SerializeField] GameObject visualRoot;
        [Tooltip("Supernatural-accent tell, shown once identified.")]
        [SerializeField] GameObject tell;
        [Tooltip("Said when examined before the creature is identified.")]
        [SerializeField] List<DialogueLine> notReadyLines = new List<DialogueLine>();

        Collider2D col;

        bool Ready => Game.Instance != null && Game.Instance.ActiveCase == forCase && Game.Instance.Identified;

        public override string Prompt => Ready ? confrontPrompt : base.Prompt;

        void Awake() => col = GetComponent<Collider2D>();

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
            bool ready = Ready;
            if (tell != null) tell.SetActive(ready);
            if (hiddenUntilIdentified)
            {
                if (visualRoot != null) visualRoot.SetActive(ready);
                if (col != null) col.enabled = ready;
            }
        }

        public override void Interact(GameObject interactor)
        {
            var game = Game.Instance;
            if (Ready) game.OpenTools();
            else if (notReadyLines.Count > 0) game.Say(notReadyLines);
            else game.Say(Speakers.Dutch, "Not until I know what I'm dealing with. The Book first.");
        }
    }
}
