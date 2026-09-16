using Manor.Game.Missions;
using Manor.Game.NPC;
using UnityEditor;
using UnityEngine;

namespace Manor.Editor
{
    /// <summary>Creates the placeholder NPC prefab and archetype assets.</summary>
    public static class NPCPrefabBuilder
    {
        private const string PrefabPath = ManorPaths.Prefabs + "/NPC_Placeholder.prefab";

        public static GameObject EnsurePrefab()
        {
            ManorPaths.EnsureFolders();

            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (existing != null) return existing;

            var root = new GameObject("NPC_Placeholder");

            var controller = root.AddComponent<CharacterController>();
            controller.height = 1.75f;
            controller.radius = 0.3f;
            controller.center = new Vector3(0f, 0.875f, 0f);

            root.AddComponent<NPCController>();
            root.AddComponent<MissionNPCBridge>();

            Material body = ManorPaths.Material("NPCBody", new Color(0.26f, 0.27f, 0.30f), 0.18f);
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "Body";
            visual.transform.SetParent(root.transform);
            visual.transform.localPosition = new Vector3(0f, 0.875f, 0f);
            visual.transform.localScale = new Vector3(0.58f, 0.875f, 0.58f);
            visual.GetComponent<Renderer>().sharedMaterial = body;
            UnityEngine.Object.DestroyImmediate(visual.GetComponent<Collider>());

            // Interaction trigger, so the Interactor's overlap scan can find them.
            var trigger = root.AddComponent<CapsuleCollider>();
            trigger.isTrigger = true;
            trigger.height = 2f;
            trigger.radius = 0.6f;
            trigger.center = new Vector3(0f, 1f, 0f);

            var anchor = new GameObject("PromptAnchor");
            anchor.transform.SetParent(root.transform);
            anchor.transform.localPosition = new Vector3(0f, 1.85f, 0f);

            SerializedObject so = new(root.GetComponent<NPCController>());
            so.FindProperty("anchor").objectReferenceValue = anchor.transform;
            so.ApplyModifiedPropertiesWithoutUndo();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        public static NPCDefinition EnsureDefinition(
            string assetName, string displayName, NPCRole role, string missionId)
        {
            string path = $"{ManorPaths.Prefabs}/{assetName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<NPCDefinition>(path);
            if (existing != null) return existing;

            var def = ScriptableObject.CreateInstance<NPCDefinition>();
            def.displayName = displayName;
            def.role = role;
            def.missionId = missionId;
            def.lines = LinesFor(role);

            AssetDatabase.CreateAsset(def, path);
            return def;
        }

        private static string[] LinesFor(NPCRole role) => role switch
        {
            NPCRole.Resident => new[]
            {
                "Alright? You're not from round here, are you.",
                "River's that way, past the bridge. Can't miss it.",
                "High street's quiet this time of day. Give it an hour."
            },
            NPCRole.ShopWorker or NPCRole.CafeWorker => new[]
            {
                "We're open till six, if you're after anything.",
                "Card only, sorry. Machine's been playing up.",
                "Mind the step on your way out."
            },
            NPCRole.Student => new[]
            {
                "You know if the library's still open?",
                "Bus hasn't turned up in twenty minutes.",
                "Bit grim out, innit."
            },
            _ => new[]
            {
                "Alright?",
                "Bit grim out, innit.",
                "Excuse me, mate."
            }
        };
    }
}
