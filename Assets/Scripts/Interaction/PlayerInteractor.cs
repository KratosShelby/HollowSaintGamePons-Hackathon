using UnityEngine;
using HollowSaint.Core;
using HollowSaint.Player;

namespace HollowSaint.Interaction
{
    /// <summary>
    /// Finds the closest Interactable within reach and calls it when E is pressed.
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] float reach = 1.2f;

        readonly Collider2D[] hits = new Collider2D[16];
        ContactFilter2D filter;

        /// <summary>The interactor in the current scene (for the HUD prompt).</summary>
        public static PlayerInteractor Active { get; private set; }

        public Interactable Current { get; private set; }

        void Awake()
        {
            filter = new ContactFilter2D();
            filter.NoFilter(); // includes triggers
        }

        void OnEnable() => Active = this;

        void OnDisable()
        {
            if (Active == this) Active = null;
        }

        void Update()
        {
            bool allowed = Game.Instance == null || Game.Instance.GameplayInputAllowed;
            Current = allowed ? FindClosest() : null;
            if (Current != null && InputReader.InteractPressedThisFrame())
                Current.Interact(gameObject);
        }

        Interactable FindClosest()
        {
            int count = Physics2D.OverlapCircle(transform.position, reach, filter, hits);
            Interactable best = null;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                if (!hits[i].enabled || !hits[i].TryGetComponent(out Interactable candidate)) continue;
                if (!candidate.isActiveAndEnabled || !candidate.CanInteract) continue;
                float d = ((Vector2)hits[i].ClosestPoint(transform.position) - (Vector2)transform.position).sqrMagnitude;
                if (d < bestDistance) { bestDistance = d; best = candidate; }
            }
            return best;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, reach);
        }
    }
}
