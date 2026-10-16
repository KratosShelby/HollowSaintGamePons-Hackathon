using System.Collections.Generic;
using UnityEngine;

namespace HollowSaint.Clues
{
    public class CaseFile : MonoBehaviour
    {
        public static CaseFile Instance { get; private set; }

        private readonly List<ClueData> collectedClues = new List<ClueData>();
        public IReadOnlyList<ClueData> CollectedClues => collectedClues;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void AddSymptom(ClueData clue)
        {
            if (collectedClues.Contains(clue)) return;
            collectedClues.Add(clue);
        }

        public HashSet<string> GetSymptomTags()
        {
            var tags = new HashSet<string>();
            foreach (var clue in collectedClues) tags.Add(clue.symptomTag);
            return tags;
        }

        public void ResetForNewCase()
        {
            collectedClues.Clear();
        }
    }
}
