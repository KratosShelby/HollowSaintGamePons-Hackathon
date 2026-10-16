using System.Collections.Generic;
using UnityEngine;
using HollowSaint.Core;
using HollowSaint.Interaction;

namespace HollowSaint.World
{
    /// <summary>Plays a few lines of dialogue. Pure flavour.</summary>
    public class TalkInteractable : Interactable
    {
        [SerializeField] List<DialogueLine> lines = new List<DialogueLine>();

        public override void Interact(GameObject interactor) => Game.Instance.Say(lines);
    }
}
