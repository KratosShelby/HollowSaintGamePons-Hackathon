using UnityEngine;

namespace HollowSaint.World
{
    /// <summary>Where Dutch appears when he enters this scene through a door with the matching id.</summary>
    public class SpawnPoint : MonoBehaviour
    {
        [SerializeField] string id;

        public string Id => id;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}
