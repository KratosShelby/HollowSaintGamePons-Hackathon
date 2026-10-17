using UnityEngine;
using HollowSaint.Core;
using HollowSaint.Investigation;

namespace HollowSaint.World
{
    /// <summary>
    /// Tells the Game which scene just loaded so it can play the right opening beat, and puts
    /// Dutch at the spawn point of the door he came through.
    /// </summary>
    public class SceneIntro : MonoBehaviour
    {
        [Tooltip("The case this scene belongs to. Empty for the hotel, street and cafe.")]
        [SerializeField] CaseData sceneCase;
        [SerializeField] bool isHotel;

        void Start()
        {
            var game = Game.Instance;
            if (game == null) return;
            PlaceAtSpawn(game.ConsumePendingSpawn());
            game.BeginScene(sceneCase, isHotel);
        }

        static void PlaceAtSpawn(string spawnId)
        {
            if (string.IsNullOrEmpty(spawnId)) return;
            var player = GameObject.FindWithTag("Player");
            if (player == null) return;
            foreach (var spawn in FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None))
            {
                if (spawn.Id != spawnId) continue;
                player.transform.position = spawn.transform.position;
                if (player.TryGetComponent(out Rigidbody2D body)) body.position = spawn.transform.position;
                var follow = FindFirstObjectByType<CameraFollow>();
                if (follow != null) follow.SnapTo(spawn.transform.position.x);
                return;
            }
            Debug.LogWarning($"[SceneIntro] No SpawnPoint '{spawnId}' in this scene.");
        }
    }
}
