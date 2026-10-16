using UnityEditor;
using UnityEngine;

namespace HollowSaint.EditorTools
{
    /// <summary>
    /// Import rules for painted art under Assets/Art, so a file dropped into the right folder
    /// fits the room with no manual steps. The room is 20 x 11.25 world units.
    ///
    ///   Art/Backgrounds/  full-room paintings: scaled so the image width spans the room.
    ///   Art/Characters/   scaled so the image height is CharacterHeight units (Dutch's blockout).
    ///   Art/Props/        clue objects etc.: 100 px per unit; adjust the Visual's scale in the scene.
    ///   Art/Placeholder/, Art/Lighting/  generated flat shapes: 1 image = 1 unit, point filtered.
    ///
    /// Painted art gets bilinear filtering and high-quality compression. Pivot and slicing are
    /// left to the artist.
    /// </summary>
    public class ArtImportPostprocessor : AssetPostprocessor
    {
        public const string ArtRoot = "Assets/Art/";
        public const float RoomWidth = 20f;
        public const float CharacterHeight = 2f;

        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(ArtRoot)) return;
            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            if (importer.spriteImportMode == SpriteImportMode.None)
                importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.alphaIsTransparency = true;

            importer.GetSourceTextureWidthAndHeight(out int width, out int height);
            bool generated = assetPath.StartsWith(ArtRoot + "Placeholder/") || assetPath.StartsWith(ArtRoot + "Lighting/");

            if (generated)
            {
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.spritePixelsPerUnit = Mathf.Max(1, width);
                return;
            }

            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            if (assetPath.StartsWith(ArtRoot + "Backgrounds/"))
                importer.spritePixelsPerUnit = Mathf.Max(1f, width / RoomWidth);
            else if (assetPath.StartsWith(ArtRoot + "Characters/"))
                importer.spritePixelsPerUnit = Mathf.Max(1f, height / CharacterHeight);
            else
                importer.spritePixelsPerUnit = 100f;
        }
    }
}
