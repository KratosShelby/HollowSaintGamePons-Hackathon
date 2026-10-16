using UnityEngine;

namespace HollowSaint.Interaction
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float interactRadius = 1f;
        [SerializeField] private LayerMask interactableLayer;

        private Interactable nearest;

        private void Update()
        {
            nearest = FindNearestInteractable();

            if (nearest != null && Input.GetKeyDown(KeyCode.E))
            {
                nearest.TriggerInteract();
            }
        }

        private Interactable FindNearestInteractable()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactableLayer);
            Interactable closest = null;
            float closestDist = float.MaxValue;

            foreach (var hit in hits)
            {
                var interactable = hit.GetComponent<Interactable>();
                if (interactable == null) continue;

                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = interactable;
                }
            }
            return closest;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRadius);
        }
    }
}
