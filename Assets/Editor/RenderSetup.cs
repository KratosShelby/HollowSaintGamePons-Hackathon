using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HollowSaint.EditorTools
{
    /// <summary>
    /// Puts the project on URP with the 2D Renderer so rooms can use 2D lights with real cast
    /// shadows. Idempotent: reuses the assets if they already exist.
    /// </summary>
    public static class RenderSetup
    {
        const string SettingsDir = "Assets/Settings";
        const string RendererPath = SettingsDir + "/Renderer2D.asset";
        const string PipelinePath = SettingsDir + "/URP_2D.asset";
        public const string LitMaterialPath = "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Lit-Default.mat";
        public const string UnlitMaterialPath = "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Unlit-Default.mat";

        [MenuItem("Hollow Saint/Setup 2D Lighting (URP)")]
        static void Menu() => Debug.Log(Ensure());

        public static string Ensure()
        {
            Directory.CreateDirectory(SettingsDir);

            var data = AssetDatabase.LoadAssetAtPath<Renderer2DData>(RendererPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<Renderer2DData>();
                AssetDatabase.CreateAsset(data, RendererPath);
            }
            // Top-down sorting: lower on screen draws in front.
            var so = new SerializedObject(data);
            var mode = so.FindProperty("m_TransparencySortMode");
            if (mode != null) mode.intValue = (int)TransparencySortMode.CustomAxis;
            var axis = so.FindProperty("m_TransparencySortAxis");
            if (axis != null) axis.vector3Value = new Vector3(0f, 1f, 0f);
            so.ApplyModifiedPropertiesWithoutUndo();

            var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (pipeline == null)
            {
                pipeline = UniversalRenderPipelineAsset.Create(data);
                AssetDatabase.CreateAsset(pipeline, PipelinePath);
            }
            pipeline.supportsHDR = false;
            pipeline.msaaSampleCount = 1;
            EditorUtility.SetDirty(pipeline);
            EditorUtility.SetDirty(data);

            GraphicsSettings.defaultRenderPipeline = pipeline;
            int current = QualitySettings.GetQualityLevel();
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = null; // use the default above at every level
            }
            QualitySettings.SetQualityLevel(current, false);

            AssetDatabase.SaveAssets();
            var lit = AssetDatabase.LoadAssetAtPath<Material>(LitMaterialPath);
            var unlit = AssetDatabase.LoadAssetAtPath<Material>(UnlitMaterialPath);
            return $"URP 2D active: {GraphicsSettings.defaultRenderPipeline?.name}; lit={(lit != null)} unlit={(unlit != null)}";
        }
    }
}
