using UnityEditor;
using UnityEngine;

namespace HollowSaint.EditorTools
{
    /// <summary>
    /// Locks import settings for everything under Assets/Art: Sprite, 16 pixels per unit,
    /// Point filtering, no compression, no mipmaps. Real art dropped into these folders
    /// imports correctly with no manual steps. Pivot / slicing are left to the artist.
    /// </summary>
    public class ArtImportPostprocessor : AssetPostprocessor
    {
        public const string ArtRoot = "Assets/Art/";
        public const int PixelsPerUnit = 16;

        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith(ArtRoot)) return;
            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            if (importer.spriteImportMode == SpriteImportMode.None)
                importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = PixelsPerUnit;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.alphaIsTransparency = true;
        }
    }
}
