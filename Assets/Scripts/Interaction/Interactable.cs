using UnityEngine;

namespace HollowSaint.Interaction
{
    /// <summary>
    /// Base class for anything the player can press E on. Needs a Collider2D so the
    /// PlayerInteractor can find it.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] string prompt = "Inspect";

        public virtual string Prompt => prompt;

        /// <summary>False hides the prompt and skips this object.</summary>
        public virtual bool CanInteract => true;

        public abstract void Interact(GameObject interactor);
    }
}
