using System.Collections.Generic;
using UnityEngine;
using HollowSaint.Core;
using HollowSaint.Interaction;

namespace HollowSaint.World
{
    /// <summary>A door to another scene. Dutch arrives at the SpawnPoint with the given id.</summary>
    public class DoorInteractable : Interactable
    {
        [SerializeField] string targetScene;
        [SerializeField] string targetSpawn;
        [Tooltip("The door won't open while the final case is active (Dutch can't leave his room).")]
        [SerializeField] bool lockedDuringFinalCase;
        [SerializeField] List<DialogueLine> lockedLines = new List<DialogueLine>();

        static bool FinalCase => Game.Instance != null && Game.Instance.ActiveCase != null && Game.Instance.ActiveCase.isFinal;

        public override void Interact(GameObject interactor)
        {
            var game = Game.Instance;
            if (lockedDuringFinalCase && FinalCase)
            {
                game.Say(lockedLines);
                return;
            }
            game.GoToScene(targetScene, targetSpawn);
        }
    }
}
