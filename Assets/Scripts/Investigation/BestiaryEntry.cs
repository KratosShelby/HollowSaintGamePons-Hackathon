using System.Collections.Generic;
using UnityEngine;

namespace HollowSaint.Investigation
{
    /// <summary>
    /// A page of The Book: a creature, its symptoms and the specific method that ends it.
    /// </summary>
    [CreateAssetMenu(menuName = "Hollow Saint/Bestiary Entry", fileName = "Entry_")]
    public class BestiaryEntry : ScriptableObject
    {
        public string id;
        public string displayName;
        public string classification;
        [TextArea(3, 8)] public string lore;
        [Tooltip("Symptom strings. A case's clues must use exactly the same text.")]
        public List<string> symptoms = new List<string>();
        [TextArea] public string weaknessText;
        [Tooltip("The tool that resolves it. Leave empty for entries Dutch can't act on.")]
        public ToolData weaknessTool;
        [Tooltip("Rendered as Dutch's own handwriting.")]
        public bool handwritten;
        [Tooltip("Not in the Book until a case with this entity has started.")]
        public bool hiddenUntilItsCase;
    }
}
