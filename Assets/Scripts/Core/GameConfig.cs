using System.Collections.Generic;
using UnityEngine;
using HollowSaint.Investigation;

namespace HollowSaint.Core
{
    /// <summary>
    /// Top-level content: case order, the Book, the starting kit and the hotel's recurring lines.
    /// Loaded from Resources/GameConfig at boot.
    /// </summary>
    [CreateAssetMenu(menuName = "Hollow Saint/Game Config", fileName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public const string ResourcePath = "GameConfig";

        [Header("Structure")]
        public string hotelScene = "Hotel";
        public List<CaseData> cases = new List<CaseData>();
        [Tooltip("Pages of The Book, in order.")]
        public List<BestiaryEntry> bestiary = new List<BestiaryEntry>();
        public List<ToolData> startingTools = new List<ToolData>();

        [Header("Hotel")]
        [Tooltip("Played word for word on every return to the hotel.")]
        public DialogueLine radioLine;
        [Tooltip("Dutch's reaction to the radio, indexed by cases closed.")]
        public List<string> radioReactions = new List<string>();
        [Tooltip("Mirror flavour lines, indexed by cases closed (last one repeats).")]
        public List<string> mirrorLines = new List<string>();

        [Header("Ending")]
        [Tooltip("Played after choosing to look in the mirror.")]
        public List<DialogueLine> endingLines = new List<DialogueLine>();
        public string endTitle = "HOLLOW SAINT";
        [TextArea] public string endSubtitle;

        [Header("Presentation")]
        public string newspaperName = "THE CASPIAN COURIER";
        [Tooltip("UI font asset (serif or typewriter). Overrides osFontNames when set.")]
        public Font uiFont;
        [Tooltip("Installed OS fonts to try, in order, when no uiFont asset is set.")]
        public string[] osFontNames = { "Georgia", "Palatino Linotype", "Times New Roman" };
    }
}
