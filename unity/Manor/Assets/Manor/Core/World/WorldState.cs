using System.Collections.Generic;

namespace Manor.Core.World
{
    /// <summary>
    /// Flat, saveable key/value store of everything the world remembers.
    /// Missions write to it; spawn tables, dialogue and shop state read from it.
    /// This is the mechanism by which "the world changes after a mission" without
    /// bespoke code per mission.
    /// </summary>
    public sealed class WorldState
    {
        private readonly Dictionary<string, int> _values = new();

        public int Get(string key) => _values.TryGetValue(key, out int v) ? v : 0;
        public bool Flag(string key) => Get(key) != 0;
        public void Set(string key, int value) => _values[key] = value;
        public void SetFlag(string key, bool value = true) => _values[key] = value ? 1 : 0;
        public void Add(string key, int delta) => _values[key] = Get(key) + delta;

        /// <summary>Faction standing runs -100..100 and the two are independent, not one slider.</summary>
        public int StandingQuayside
        {
            get => Get("standing.quayside");
            set => Set("standing.quayside", Clamp(value));
        }

        public int StandingFiveStar
        {
            get => Get("standing.fiveStar");
            set => Set("standing.fiveStar", Clamp(value));
        }

        public IReadOnlyDictionary<string, int> All => _values;

        public void Clear() => _values.Clear();

        private static int Clamp(int v) => v < -100 ? -100 : (v > 100 ? 100 : v);
    }
}
