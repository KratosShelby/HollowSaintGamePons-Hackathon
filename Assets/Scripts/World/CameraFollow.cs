using UnityEngine;

namespace HollowSaint.World
{
    /// <summary>Horizontal follow for scenes wider than one screen, clamped to the scene's edges.</summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] float minX = -8f;
        [SerializeField] float maxX = 8f;
        [SerializeField] float smoothTime = 0.15f;

        float velocity;

        void Start()
        {
            if (target != null) SnapTo(target.position.x);
        }

        void LateUpdate()
        {
            if (target == null) return;
            float goal = Mathf.Clamp(target.position.x, minX, maxX);
            var p = transform.position;
            p.x = Mathf.SmoothDamp(p.x, goal, ref velocity, smoothTime);
            transform.position = p;
        }

        public void SnapTo(float x)
        {
            var p = transform.position;
            p.x = Mathf.Clamp(x, minX, maxX);
            transform.position = p;
            velocity = 0f;
        }
    }
}
