using UnityEngine;

namespace HollowSaint.Investigation
{
    /// <summary>
    /// One piece of evidence. Case-agnostic: any case can reference any clue asset.
    /// </summary>
    [CreateAssetMenu(menuName = "Hollow Saint/Clue", fileName = "Clue_")]
    public class ClueData : ScriptableObject
    {
        [Tooltip("Stable unique id, e.g. \"famished.rot_patch\". Used to de-duplicate in the case file.")]
        public string id;
        public string title;
        [Tooltip("What Dutch says when he examines it.")]
        [TextArea] public string description;
        [Tooltip("Symptom this clue reveals, e.g. \"cold rot smell\". Must match a bestiary entry's symptom text exactly.")]
        public string symptom;
    }
}
