using UnityEngine;
using HollowSaint.Core;

namespace HollowSaint.Player
{
    /// <summary>
    /// Top-down, 4-directional movement. No diagonals: when both axes are held,
    /// the axis pressed most recently wins. Frozen while any UI has focus.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 4f;
        [Tooltip("Art faces right; flipped when walking left.")]
        [SerializeField] SpriteRenderer visual;

        Rigidbody2D body;
        Vector2 moveDirection;
        bool horizontalHasPriority;
        Vector2 previousRaw;

        /// <summary>Last non-zero movement direction.</summary>
        public Vector2 Facing { get; private set; } = Vector2.down;
        public Vector2 MoveDirection => moveDirection;

        void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        void Update()
        {
            bool canMove = Game.Instance == null || Game.Instance.GameplayInputAllowed;
            Vector2 raw = canMove ? InputReader.ReadMove() : Vector2.zero;

            // Whichever axis was newly pressed this frame takes priority.
            if (raw.x != 0f && previousRaw.x == 0f) horizontalHasPriority = true;
            if (raw.y != 0f && previousRaw.y == 0f) horizontalHasPriority = false;
            previousRaw = raw;

            if (raw.x != 0f && raw.y != 0f)
                moveDirection = horizontalHasPriority ? new Vector2(raw.x, 0f) : new Vector2(0f, raw.y);
            else
                moveDirection = raw;

            if (moveDirection != Vector2.zero)
            {
                Facing = moveDirection;
                if (visual != null && moveDirection.x != 0f) visual.flipX = moveDirection.x < 0f;
            }
        }

        void FixedUpdate()
        {
            body.linearVelocity = moveDirection * moveSpeed;
        }
    }
}
