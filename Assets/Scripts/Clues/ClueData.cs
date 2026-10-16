using UnityEngine;

namespace HollowSaint.Clues
{
    [CreateAssetMenu(fileName = "NewClue", menuName = "Hollow Saint/Clue")]
    public class ClueData : ScriptableObject
    {
        public string clueId;
        [TextArea] public string description;
        public string symptomTag;
    }
}
