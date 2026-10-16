using UnityEngine;
using HollowSaint.Core;
using HollowSaint.Interaction;

namespace HollowSaint.World
{
    /// <summary>Opens the newspaper hub.</summary>
    public class NewspaperInteractable : Interactable
    {
        public override void Interact(GameObject interactor) => Game.Instance.OpenNewspaper();
    }
}
