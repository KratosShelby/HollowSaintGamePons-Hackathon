using UnityEngine;
using UnityEngine.Events;

namespace HollowSaint.Interaction
{
    public class Interactable : MonoBehaviour
    {
        public UnityEvent OnInteract;
        [SerializeField] private bool oneShot = true;
        private bool used = false;

        public void TriggerInteract()
        {
            if (oneShot && used) return;
            used = true;
            OnInteract?.Invoke();
        }
    }
}
