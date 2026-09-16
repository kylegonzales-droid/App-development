using Manor.Core.Missions;
using Manor.Game.NPC;
using Manor.Game.Systems;
using UnityEngine;

namespace Manor.Game.Missions
{
    /// <summary>
    /// Reports NPC conversations to the mission runner.
    ///
    /// Keeps NPCController free of mission knowledge: the NPC just talks, this decides
    /// whether talking to it mattered.
    /// </summary>
    [RequireComponent(typeof(NPCController))]
    public sealed class MissionNPCBridge : MonoBehaviour
    {
        private NPCController _npc;

        private void Awake() => _npc = GetComponent<NPCController>();

        private void OnEnable() => _npc.Spoke += OnSpoke;
        private void OnDisable() => _npc.Spoke -= OnSpoke;

        private void OnSpoke(NPCController npc, string line)
        {
            if (string.IsNullOrEmpty(npc.MissionId)) return;
            GameDirector.Instance?.Missions.TryComplete(ObjectiveKind.Talk, npc.MissionId);
        }
    }
}
