using System.Collections.Generic;
using UnityEngine;
using HollowSaint.Core;

namespace HollowSaint.Investigation
{
    /// <summary>
    /// One case: where it happens, which creature is behind it, how the newspaper reports it,
    /// and the lines that frame it. All story text for a case lives here.
    /// </summary>
    [CreateAssetMenu(menuName = "Hollow Saint/Case", fileName = "Case_")]
    public class CaseData : ScriptableObject
    {
        public string id;
        public string title;
        [Tooltip("Scene loaded when the player follows this case's newspaper lead.")]
        public string sceneName;
        public BestiaryEntry entity;
        public List<ClueData> clues = new List<ClueData>();
        [Tooltip("The twist case: tools never work and the resolution is to look in the mirror.")]
        public bool isFinal;

        [Header("Newspaper")]
        public string edition;
        public List<string> flavorHeadlines = new List<string>();
        [Tooltip("The bolded real lead.")]
        public string leadHeadline;
        [Tooltip("Location/time detail baked into the lead.")]
        public string leadDetail;
        public List<DialogueLine> leadReaction = new List<DialogueLine>();

        [Header("Scene beats")]
        public List<DialogueLine> arrivalLines = new List<DialogueLine>();
        [Tooltip("Played the first time the Book is opened during this case.")]
        public List<DialogueLine> bookRevealLines = new List<DialogueLine>();
        public List<DialogueLine> identifyLines = new List<DialogueLine>();
        [Tooltip("{0} is replaced with the tool name.")]
        public string wrongToolLine = "I try the {0}. Nothing.";
        public List<DialogueLine> resolveLines = new List<DialogueLine>();
    }
}
