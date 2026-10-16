using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace HollowSaint.Player
{
    /// <summary>
    /// Single place that reads the keyboard, so gameplay and UI code don't care whether the
    /// project uses the Input System package or the legacy Input Manager.
    /// </summary>
    public static class InputReader
    {
        /// <summary>WASD / arrow keys as a raw axis vector (each component -1, 0 or 1).</summary>
        public static Vector2 ReadMove()
        {
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb == null) return Vector2.zero;
            float x = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1f : 0f)
                    - (kb.aKey.isPressed || kb.leftArrowKey.isPressed ? 1f : 0f);
            float y = (kb.wKey.isPressed || kb.upArrowKey.isPressed ? 1f : 0f)
                    - (kb.sKey.isPressed || kb.downArrowKey.isPressed ? 1f : 0f);
            return new Vector2(x, y);
#else
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#endif
        }

        /// <summary>E: interact in the world.</summary>
        public static bool InteractPressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            return kb != null && kb.eKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.E);
#endif
        }

        /// <summary>E / Enter / Space: advance dialogue, confirm a menu choice.</summary>
        public static bool ConfirmPressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            return kb != null && (kb.eKey.wasPressedThisFrame || kb.enterKey.wasPressedThisFrame
                                  || kb.numpadEnterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame);
#else
            return Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space);
#endif
        }

        /// <summary>Escape: back out of a screen.</summary>
        public static bool CancelPressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            return kb != null && kb.escapeKey.wasPressedThisFrame;
#else
            return Input.GetKeyDown(KeyCode.Escape);
#endif
        }

        /// <summary>B / Tab: open or close The Book.</summary>
        public static bool BookPressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            return kb != null && (kb.bKey.wasPressedThisFrame || kb.tabKey.wasPressedThisFrame);
#else
            return Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Tab);
#endif
        }

        /// <summary>Menu navigation: -1 / +1 on each axis for the frame a direction was pressed.</summary>
        public static Vector2Int NavPressedThisFrame()
        {
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb == null) return Vector2Int.zero;
            int x = (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame ? 1 : 0)
                  - (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame ? 1 : 0);
            int y = (kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame ? 1 : 0)
                  - (kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame ? 1 : 0);
            return new Vector2Int(x, y);
#else
            int x = (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0)
                  - (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ? 1 : 0);
            int y = (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0)
                  - (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) ? 1 : 0);
            return new Vector2Int(x, y);
#endif
        }
    }
}
