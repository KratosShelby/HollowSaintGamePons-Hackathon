using HollowSaint.Investigation;

namespace HollowSaint.Core
{
    /// <summary>One row in the tool-select menu.</summary>
    public readonly struct ToolOption
    {
        public enum Kind { Tool, StepBack, Look }

        public readonly Kind kind;
        public readonly ToolData tool;
        /// <summary>Already tried and failed (Mirror-Born only).</summary>
        public readonly bool spent;

        public ToolOption(Kind kind, ToolData tool = null, bool spent = false)
        {
            this.kind = kind;
            this.tool = tool;
            this.spent = spent;
        }

        public static ToolOption ForTool(ToolData tool, bool spent = false) => new ToolOption(Kind.Tool, tool, spent);
        public static readonly ToolOption StepBack = new ToolOption(Kind.StepBack);
        public static readonly ToolOption Look = new ToolOption(Kind.Look);
    }
}
