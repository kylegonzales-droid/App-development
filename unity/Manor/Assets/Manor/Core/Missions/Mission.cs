using System;
using System.Collections.Generic;

namespace Manor.Core.Missions
{
    /// <summary>A node in a mission graph.</summary>
    public sealed class Beat
    {
        public string Id { get; }
        public Objective Objective { get; }

        /// <summary>Beat to run when this one completes. Null or empty ends the mission.</summary>
        public string Next { get; }

        /// <summary>Applied to WorldState when this beat completes.</summary>
        public IReadOnlyList<StateChange> OnComplete { get; }

        public Beat(string id, Objective objective, string next = null,
                    IReadOnlyList<StateChange> onComplete = null)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Beats need an id.", nameof(id));
            Id = id;
            Objective = objective;
            Next = next;
            OnComplete = onComplete ?? Array.Empty<StateChange>();
        }
    }

    /// <summary>A single write into WorldState, applied when a beat completes.</summary>
    public readonly struct StateChange
    {
        public string Key { get; }
        public int Delta { get; }
        public bool IsAbsolute { get; }

        private StateChange(string key, int value, bool absolute)
        {
            Key = key;
            Delta = value;
            IsAbsolute = absolute;
        }

        public static StateChange Add(string key, int delta) => new(key, delta, false);
        public static StateChange Set(string key, int value) => new(key, value, true);
        public static StateChange Flag(string key) => new(key, 1, true);
    }

    public sealed class Mission
    {
        public string Id { get; }
        public string Title { get; }
        public string FirstBeatId { get; }

        private readonly Dictionary<string, Beat> _beats = new();

        public Mission(string id, string title, IEnumerable<Beat> beats)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Missions need an id.", nameof(id));
            if (beats == null) throw new ArgumentNullException(nameof(beats));

            Id = id;
            Title = title;

            string first = null;
            foreach (Beat b in beats)
            {
                if (_beats.ContainsKey(b.Id))
                {
                    throw new ArgumentException($"Duplicate beat id '{b.Id}' in mission '{id}'.", nameof(beats));
                }
                _beats[b.Id] = b;
                first ??= b.Id;
            }

            if (first == null) throw new ArgumentException($"Mission '{id}' has no beats.", nameof(beats));
            FirstBeatId = first;

            // Fail loudly at construction rather than mid-play on a dangling transition.
            foreach (Beat b in _beats.Values)
            {
                if (!string.IsNullOrEmpty(b.Next) && !_beats.ContainsKey(b.Next))
                {
                    throw new ArgumentException(
                        $"Beat '{b.Id}' in mission '{id}' points at unknown beat '{b.Next}'.", nameof(beats));
                }
            }
        }

        public Beat BeatById(string beatId) => _beats.TryGetValue(beatId, out Beat b) ? b : null;
        public int BeatCount => _beats.Count;
        public IEnumerable<Beat> Beats => _beats.Values;
    }
}
