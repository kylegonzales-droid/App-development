using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Manor.Editor
{
    /// <summary>
    /// Checks an imported character against the budgets and rules in
    /// docs/graphics/12-character-pipeline.md and docs/graphics/13-character-sourcing.md.
    ///
    /// The point is that a bad asset cannot land silently. Select a model in the
    /// Project window and run Manor > Validate Selected Character.
    /// </summary>
    public static class CharacterImportValidator
    {
        private enum Tier { Hero, Named, Reactive, Ambient }

        private static readonly Dictionary<Tier, int> TriangleBudget = new()
        {
            { Tier.Hero, 55_000 }, { Tier.Named, 30_000 },
            { Tier.Reactive, 14_000 }, { Tier.Ambient, 5_000 }
        };

        [MenuItem("Manor/Validate Selected Character", priority = 20)]
        public static void ValidateSelection()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("Manor", "Select a character model in the Project window.", "OK");
                return;
            }

            string path = AssetDatabase.GetAssetPath(selected);
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            var report = new StringBuilder();
            int problems = 0;

            report.AppendLine($"Character validation: {selected.name}");
            report.AppendLine(new string('-', 52));

            // Triangles, against the most permissive tier.
            int triangles = 0;
            foreach (var filter in selected.GetComponentsInChildren<MeshFilter>(true))
            {
                if (filter.sharedMesh != null) triangles += filter.sharedMesh.triangles.Length / 3;
            }
            foreach (var skinned in selected.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (skinned.sharedMesh != null) triangles += skinned.sharedMesh.triangles.Length / 3;
            }

            report.AppendLine($"Triangles: {triangles:N0}");
            if (triangles > TriangleBudget[Tier.Hero])
            {
                report.AppendLine($"  FAIL  over the hero budget of {TriangleBudget[Tier.Hero]:N0}");
                problems++;
            }
            else
            {
                Tier tier = triangles <= TriangleBudget[Tier.Ambient] ? Tier.Ambient
                    : triangles <= TriangleBudget[Tier.Reactive] ? Tier.Reactive
                    : triangles <= TriangleBudget[Tier.Named] ? Tier.Named : Tier.Hero;
                report.AppendLine($"  OK    fits the {tier} tier");
            }

            // Rig.
            if (importer != null)
            {
                report.AppendLine($"Rig: {importer.animationType}");
                if (importer.animationType != ModelImporterAnimationType.Human)
                {
                    report.AppendLine("  FAIL  must be Humanoid, or Mecanim retargeting is unavailable");
                    problems++;
                }

                if (importer.isReadable)
                {
                    report.AppendLine("Read/Write: enabled");
                    report.AppendLine("  FAIL  doubles memory; disable unless the mesh is read at runtime");
                    problems++;
                }

                if (Mathf.Abs(importer.globalScale - 1f) > 0.001f)
                {
                    report.AppendLine($"Scale: {importer.globalScale}");
                    report.AppendLine("  WARN  expected 1.0 with the source authored in metres");
                }
            }
            else
            {
                report.AppendLine("  WARN  no ModelImporter; is this an imported model asset?");
            }

            // Bone influences.
            foreach (var skinned in selected.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                if (skinned.sharedMesh == null) continue;

                var bonesPerVertex = skinned.sharedMesh.GetBonesPerVertex();
                if (!bonesPerVertex.IsCreated || bonesPerVertex.Length == 0) continue;

                byte worst = 0;
                for (int i = 0; i < bonesPerVertex.Length; i++)
                {
                    if (bonesPerVertex[i] > worst) worst = bonesPerVertex[i];
                }
                report.AppendLine($"Max bone influences ({skinned.name}): {worst}");
                if (worst > 4)
                {
                    report.AppendLine("  FAIL  mobile limit is 4 influences per vertex");
                    problems++;
                }
            }

            // Height sanity.
            var renderers = selected.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length > 0)
            {
                Bounds bounds = renderers[0].bounds;
                foreach (var r in renderers) bounds.Encapsulate(r.bounds);
                report.AppendLine($"Height: {bounds.size.y:0.00} m");
                if (bounds.size.y is < 1.4f or > 2.2f)
                {
                    report.AppendLine("  WARN  an adult should be roughly 1.7–1.85 m");
                }
            }

            // LODs.
            bool hasLods = selected.GetComponentInChildren<LODGroup>(true) != null;
            report.AppendLine($"LOD group: {(hasLods ? "present" : "missing")}");
            if (!hasLods) report.AppendLine("  WARN  add an LOD chain before shipping");

            report.AppendLine(new string('-', 52));
            report.AppendLine(problems == 0
                ? "PASSED — no blocking problems."
                : $"{problems} BLOCKING PROBLEM(S).");

            Debug.Log(report.ToString(), selected);
            EditorUtility.DisplayDialog("Manor character validation", report.ToString(), "OK");
        }
    }
}
