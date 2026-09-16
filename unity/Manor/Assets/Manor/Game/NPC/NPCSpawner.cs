using System.Collections.Generic;
using UnityEngine;

namespace Manor.Game.NPC
{
    /// <summary>
    /// Spawns NPCs around a point at scene start, pooled for reuse.
    /// Density will later come from the day-part tables in docs/design/04-npc-system.md;
    /// for the slice it is a fixed count per spawner.
    /// </summary>
    public sealed class NPCSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject npcPrefab;
        [SerializeField] private NPCDefinition[] definitions;
        [SerializeField] private int count = 6;
        [SerializeField] private float spawnRadius = 18f;

        private readonly List<NPCController> _spawned = new();

        public IReadOnlyList<NPCController> Spawned => _spawned;

        private void Start() => SpawnAll();

        public void SpawnAll()
        {
            if (npcPrefab == null)
            {
                Debug.LogWarning($"[{nameof(NPCSpawner)}] No NPC prefab assigned on '{name}'.", this);
                return;
            }

            for (int i = 0; i < count; i++)
            {
                Vector2 offset = Random.insideUnitCircle * spawnRadius;
                Vector3 position = transform.position + new Vector3(offset.x, 0f, offset.y);

                // Drop onto whatever ground is beneath, so spawners need no precise height.
                if (Physics.Raycast(position + Vector3.up * 30f, Vector3.down,
                        out RaycastHit hit, 60f, ~0, QueryTriggerInteraction.Ignore))
                {
                    position = hit.point;
                }

                GameObject instance = Instantiate(npcPrefab, position, Quaternion.identity, transform);
                instance.name = $"NPC_{i:00}";

                if (instance.TryGetComponent(out NPCController npc))
                {
                    if (definitions is { Length: > 0 })
                    {
                        npc.Configure(definitions[Random.Range(0, definitions.Length)]);
                    }
                    _spawned.Add(npc);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.31f, 0.58f, 0.47f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
