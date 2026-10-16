using System;
using UnityEngine;

namespace HollowSaint.Core
{
    [Serializable]
    public struct DialogueLine
    {
        public string speaker;
        [TextArea] public string text;

        public DialogueLine(string speaker, string text)
        {
            this.speaker = speaker;
            this.text = text;
        }
    }

    public static class Speakers
    {
        public const string Dutch = "Dutch";
        public const string Radio = "Radio";
        public const string Reflection = "Reflection";
        public const string Narration = "";
    }
}
