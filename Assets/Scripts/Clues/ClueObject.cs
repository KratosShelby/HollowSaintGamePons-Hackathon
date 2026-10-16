using UnityEngine;
using HollowSaint.Interaction;

namespace HollowSaint.Clues
{
    [RequireComponent(typeof(Interactable))]
    public class ClueObject : MonoBehaviour
    {
        [SerializeField] private ClueData clue;

        private void Awake()
        {
            var interactable = GetComponent<Interactable>();
            interactable.OnInteract.AddListener(Collect);
        }

        private void Collect()
        {
            if (clue == null) return;
            CaseFile.Instance.AddSymptom(clue);
        }
    }
}
