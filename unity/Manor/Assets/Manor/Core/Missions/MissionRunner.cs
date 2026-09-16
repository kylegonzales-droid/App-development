using System;
using Manor.Core.World;

namespace Manor.Core.Missions
{
    public enum MissionStatus { NotStarted, Running, Completed, Abandoned }

    /// <summary>
    /// Runs a mission graph. Pure: no engine types, no coroutines, no scene.
    /// The presentation layer reports objective completion in; this decides what
    /// happens next and what the world remembers.
    /// </summary>
    public sealed class MissionRunner
    {
        private readonly WorldState _world;

        public MissionRunner(WorldState world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
        }

        public Mission Current { get; private set; }
        public Beat CurrentBeat { get; private set; }
        public MissionStatus Status { get; private set; } = MissionStatus.NotStarted;

        /// <summary>Raised when the active beat changes, including on start and finish.</summary>
        public event Action<Beat> BeatChanged;

        /// <summary>Raised once when the mission completes.</summary>
        public event Action<Mission> MissionCompleted;

        /// <summary>Line the HUD should show, or empty when nothing is active.</summary>
        public string ObjectiveText =>
            Status == MissionStatus.Running && CurrentBeat != null
                ? CurrentBeat.Objective.Description
                : string.Empty;

        public void Begin(Mission mission)
        {
            Current = mission ?? throw new ArgumentNullException(nameof(mission));
            CurrentBeat = mission.BeatById(mission.FirstBeatId);
            Status = MissionStatus.Running;
            BeatChanged?.Invoke(CurrentBeat);
        }

        /// <summary>
        /// Report that the current beat's objective was satisfied. Ignored unless the
        /// mission is running, so a duplicate trigger cannot skip a beat.
        /// </summary>
        public void CompleteCurrentBeat()
        {
            if (Status != MissionStatus.Running || CurrentBeat == null) return;

            Apply(CurrentBeat);

            string next = CurrentBeat.Next;
            if (string.IsNullOrEmpty(next))
            {
                CurrentBeat = null;
                Status = MissionStatus.Completed;
                BeatChanged?.Invoke(null);
                MissionCompleted?.Invoke(Current);
                return;
            }

            CurrentBeat = Current.BeatById(next);
            BeatChanged?.Invoke(CurrentBeat);
        }

        /// <summary>
        /// Convenience for proximity and interaction triggers: only completes if the
        /// reported objective actually matches the active one.
        /// </summary>
        public bool TryComplete(ObjectiveKind kind, string target)
        {
            if (Status != MissionStatus.Running || CurrentBeat == null) return false;

            Objective o = CurrentBeat.Objective;
            if (o.Kind != kind) return false;
            if (!string.Equals(o.Target, target, StringComparison.OrdinalIgnoreCase)) return false;

            CompleteCurrentBeat();
            return true;
        }

        public void Abandon()
        {
            if (Status != MissionStatus.Running) return;
            CurrentBeat = null;
            Status = MissionStatus.Abandoned;
            BeatChanged?.Invoke(null);
        }

        private void Apply(Beat beat)
        {
            foreach (StateChange change in beat.OnComplete)
            {
                if (change.IsAbsolute) _world.Set(change.Key, change.Delta);
                else _world.Add(change.Key, change.Delta);
            }
        }
    }
}
