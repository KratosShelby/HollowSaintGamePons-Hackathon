using System.Collections.Generic;
using UnityEngine;
using HollowSaint.Core;
using HollowSaint.Interaction;
using HollowSaint.Investigation;

namespace HollowSaint.World
{
    /// <summary>A relic found in a scene; adds a tool to Dutch's kit and disappears.</summary>
    public class ToolPickup : Interactable
    {
        [SerializeField] ToolData tool;
        [SerializeField] List<DialogueLine> lines = new List<DialogueLine>();

        public ToolData Tool => tool;

        void Start()
        {
            if (Game.Instance != null && Game.Instance.HasTool(tool))
                gameObject.SetActive(false);
        }

        public override void Interact(GameObject interactor)
        {
            var game = Game.Instance;
            game.AddTool(tool);
            var all = new List<DialogueLine>(lines)
            {
                new DialogueLine(Speakers.Narration, $"Added to the kit: {tool.displayName}")
            };
            gameObject.SetActive(false);
            game.Say(all);
        }
    }
}
