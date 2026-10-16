using UnityEngine;

namespace HollowSaint.Core
{
    /// <summary>
    /// The locked 8-colour duotone noir palette. Nothing in the game should use a colour
    /// that isn't one of these.
    /// </summary>
    public static class Palette
    {
        public static readonly Color NearBlack = new Color32(0x0d, 0x0d, 0x1a, 0xff);
        public static readonly Color Navy = new Color32(0x1b, 0x1b, 0x2f, 0xff);
        public static readonly Color NavyLight = new Color32(0x2e, 0x2e, 0x52, 0xff);
        public static readonly Color CoatShadow = new Color32(0x7a, 0x4a, 0x2b, 0xff);
        public static readonly Color CoatLit = new Color32(0xc9, 0x7d, 0x3f, 0xff);
        public static readonly Color Amber = new Color32(0xf2, 0xa6, 0x5a, 0xff);
        public static readonly Color Cream = new Color32(0xf4, 0xe9, 0xd8, 0xff);
        /// <summary>Supernatural accent. Clue glow, entity tells and the final reflection only.</summary>
        public static readonly Color Supernatural = new Color32(0x9d, 0x4e, 0xdd, 0xff);

        public static readonly Color[] All =
        {
            NearBlack, Navy, NavyLight, CoatShadow, CoatLit, Amber, Cream, Supernatural
        };

        public static string Hex(Color c) => "#" + ColorUtility.ToHtmlStringRGB(c);

        public static bool IsPaletteColor(Color c)
        {
            Color32 a = c;
            foreach (Color p in All)
            {
                Color32 b = p;
                if (a.r == b.r && a.g == b.g && a.b == b.b) return true;
            }
            return false;
        }
    }
}
