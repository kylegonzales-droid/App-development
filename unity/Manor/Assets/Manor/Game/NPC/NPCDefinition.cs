using UnityEngine;

namespace Manor.Game.NPC
{
    public enum NPCRole
    {
        Pedestrian,
        Commuter,
        Student,
        Resident,
        ShopWorker,
        CafeWorker,
        Security
    }

    /// <summary>
    /// Authored NPC archetype. A ScriptableObject so designers add characters as
    /// assets rather than as code.
    /// </summary>
    [CreateAssetMenu(menuName = "Manor/NPC Definition", fileName = "NPC_")]
    public sealed class NPCDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string displayName = "Local";
        public NPCRole role = NPCRole.Pedestrian;

        [Tooltip("Mission target id, e.g. local_resident. Leave blank if not mission-relevant.")]
        public string missionId = "";

        [Header("Movement")]
        public float walkSpeed = 1.3f;
        public float wanderRadius = 14f;
        [Tooltip("Seconds to pause at each destination.")]
        public Vector2 idleRange = new(2f, 7f);

        [Header("Dialogue")]
        [TextArea(2, 4)] public string[] lines =
        {
            "Alright?",
            "Bit grim out, innit.",
            "You after the high street? Straight down, past the bridge."
        };
    }
}
