using System.Collections.Generic;

namespace HollowSaint.Investigation
{
    /// <summary>
    /// Clues and symptoms collected during one case. De-duplicates by clue id, and lists
    /// each symptom once even if several clues reveal it.
    /// </summary>
    public class CaseFile
    {
        readonly List<ClueData> clues = new List<ClueData>();
        readonly HashSet<string> clueIds = new HashSet<string>();
        readonly List<string> symptoms = new List<string>();

        public IReadOnlyList<ClueData> Clues => clues;
        /// <summary>Distinct symptoms, in the order they were discovered.</summary>
        public IReadOnlyList<string> Symptoms => symptoms;

        /// <summary>Records a clue. Returns false if it was already in the file.</summary>
        public bool TryAdd(ClueData clue)
        {
            if (clue == null || string.IsNullOrEmpty(clue.id) || !clueIds.Add(clue.id))
                return false;

            clues.Add(clue);
            if (!string.IsNullOrEmpty(clue.symptom) && !symptoms.Contains(clue.symptom))
                symptoms.Add(clue.symptom);
            return true;
        }

        public bool Contains(ClueData clue) => clue != null && clueIds.Contains(clue.id);
    }
}
