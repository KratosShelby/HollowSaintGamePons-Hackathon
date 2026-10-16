using System.IO;
using UnityEditor;
using UnityEngine;
using HollowSaint.Core;

namespace HollowSaint.EditorTools
{
    /// <summary>
    /// Generates the placeholder sprites the blockout uses. Real art replaces these files
    /// (same path) or is assigned in the Inspector; nothing in code references them.
    /// Existing files are never overwritten, so hand-drawn art dropped in is safe.
    /// </summary>
    public static class PlaceholderArt
    {
        public const string SquarePath = "Assets/Art/Placeholder/Square.png";
        public const string GlowPath = "Assets/Art/Placeholder/Glow.png";
        public const string DutchPath = "Assets/Art/Characters/Dutch.png";
        public const string DutchSilhouettePath = "Assets/Art/Characters/Dutch_Silhouette.png";
        public const string BlindsPath = "Assets/Art/Lighting/Blinds.png";

        [MenuItem("Hollow Saint/Generate Placeholder Art")]
        public static void GenerateAll()
        {
            // Move the Step 1 square into the placeholder folder if it is still at the old path.
            if (File.Exists("Assets/Art/Square.png") && !File.Exists(SquarePath))
            {
                Directory.CreateDirectory("Assets/Art/Placeholder");
                AssetDatabase.Refresh();
                AssetDatabase.MoveAsset("Assets/Art/Square.png", SquarePath);
            }

            Write(SquarePath, 16, 16, (x, y) => Color.white, SpriteAlignment.Center);
            Write(GlowPath, 16, 16, (x, y) =>
            {
                // Small diamond, hard-edged.
                int d = Mathf.Abs(x - 7) + Mathf.Abs(y - 7);
                return d <= 5 ? Color.white : Color.clear;
            }, SpriteAlignment.Center);
            Write(DutchPath, 32, 32, DutchPixel, SpriteAlignment.BottomCenter);
            Write(DutchSilhouettePath, 32, 32, (x, y) => DutchPixel(x, y).a > 0f ? Color.white : Color.clear, SpriteAlignment.BottomCenter);
            // Light cookie: horizontal slats, 5 px lit / 3 px shadow, for venetian-blind stripes.
            Write(BlindsPath, 64, 64, (x, y) => (y % 8) < 5 ? Color.white : Color.clear, SpriteAlignment.Center);
            AssetDatabase.Refresh();
        }

        public static Sprite Load(string path) => AssetDatabase.LoadAssetAtPath<Sprite>(path);

        /// <summary>
        /// 32x32 blockout of Dutch: wide-brim hat, shadowed face, long coat lit on the right.
        /// Rough on purpose; proves silhouette, palette and proportions only.
        /// </summary>
        static Color DutchPixel(int x, int y)
        {
            Color clear = Color.clear;
            // Hat crown and brim.
            if (y >= 25 && y <= 29 && x >= 11 && x <= 20) return y == 25 ? Palette.NavyLight : Palette.Navy;
            if (y == 24 && x >= 7 && x <= 24) return Palette.Navy;
            if (y == 23 && x >= 9 && x <= 22) return Palette.NearBlack;
            // Face in shadow under the brim.
            if (y >= 20 && y <= 22 && x >= 13 && x <= 18) return Palette.NearBlack;
            // Collar.
            if (y == 19 && x >= 11 && x <= 20) return Palette.CoatShadow;
            // Coat body, flaring at the hem; lit on the right edge.
            if (y >= 6 && y <= 18)
            {
                int left = y <= 9 ? 9 : 10;
                int right = y <= 9 ? 22 : 21;
                if (x < left || x > right) return clear;
                if (x >= right - 1) return Palette.CoatLit;
                if (y >= 12 && y <= 13 && x >= left + 1 && x <= right - 3) return Palette.NearBlack; // belt
                return Palette.CoatShadow;
            }
            // Legs.
            if (y >= 1 && y <= 5 && ((x >= 12 && x <= 14) || (x >= 17 && x <= 19))) return Palette.Navy;
            if (y == 0 && ((x >= 11 && x <= 14) || (x >= 17 && x <= 20))) return Palette.NearBlack;
            return clear;
        }

        static void Write(string path, int w, int h, System.Func<int, int, Color> pixel, SpriteAlignment alignment)
        {
            if (File.Exists(path)) return;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    tex.SetPixel(x, y, pixel(x, y));
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);

            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)alignment;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }
    }
}
