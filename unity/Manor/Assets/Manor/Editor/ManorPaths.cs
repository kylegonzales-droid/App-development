using System.IO;
using UnityEditor;
using UnityEngine;

namespace Manor.Editor
{
    /// <summary>Shared asset locations and material creation for the generators.</summary>
    public static class ManorPaths
    {
        public const string Root = "Assets/Manor";
        public const string Generated = Root + "/Generated";
        public const string Materials = Generated + "/Materials";
        public const string Prefabs = Generated + "/Prefabs";
        public const string Scenes = Root + "/Scenes";

        public static void EnsureFolders()
        {
            EnsureFolder(Generated);
            EnsureFolder(Materials);
            EnsureFolder(Prefabs);
            EnsureFolder(Scenes);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path)!.Replace('\\', '/');
            string leaf = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, leaf);
        }

        /// <summary>
        /// URP Lit material, created once and reused. Falls back to the built-in
        /// standard shader so generation still works if URP is not yet active.
        /// </summary>
        public static Material Material(string name, Color colour, float smoothness, float metallic = 0f)
        {
            string path = $"{Materials}/{name}.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader) { name = name };

            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", colour);
            if (material.HasProperty("_Color")) material.SetColor("_Color", colour);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", smoothness);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);

            AssetDatabase.CreateAsset(material, path);
            return material;
        }
    }
}
