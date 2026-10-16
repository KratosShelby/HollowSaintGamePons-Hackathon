using UnityEngine;

namespace HollowSaint.Investigation
{
    /// <summary>
    /// Something in Dutch's kit: standard gear he starts with, or a relic picked up in a scene.
    /// </summary>
    [CreateAssetMenu(menuName = "Hollow Saint/Tool", fileName = "Tool_")]
    public class ToolData : ScriptableObject
    {
        public string id;
        public string displayName;
        [TextArea] public string description;
        [Tooltip("Optional icon for the tool-select UI.")]
        public Sprite icon;
        [Tooltip("What Dutch says when he tries this on the Mirror-Born (it never works).")]
        [TextArea] public string mirrorBornLine;
    }
}
