using Manor.Core.Missions;
using Manor.Game.Systems;
using UnityEngine;

namespace Manor.Game.Missions
{
    /// <summary>
    /// A named place. Completes a GoTo beat when the player is inside it.
    /// The mission graph knows only the id; the scene decides where that is.
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public sealed class MissionZone : MonoBehaviour
    {
        [Tooltip("Must match the Objective target, e.g. town_centre, riverside, high_street.")]
        [SerializeField] private string zoneId = "town_centre";

        [SerializeField] private string playerTag = "Player";

        public string ZoneId => zoneId;

        public void Configure(string id, float radius)
        {
            zoneId = id;
            var sphere = GetComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = radius;
        }

        private void Reset()
        {
            var sphere = GetComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = 8f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;
            GameDirector.Instance?.Missions.TryComplete(ObjectiveKind.GoTo, zoneId);
        }

        private void OnDrawGizmos()
        {
            var sphere = GetComponent<SphereCollider>();
            if (sphere == null) return;
            Gizmos.color = new Color(0.95f, 0.63f, 0.24f, 0.25f);
            Gizmos.DrawWireSphere(transform.position + sphere.center, sphere.radius);
        }
    }
}
