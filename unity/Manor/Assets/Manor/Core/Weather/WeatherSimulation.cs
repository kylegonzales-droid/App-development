using System;

namespace Manor.Core.Weather
{
    /// <summary>
    /// Weather as a seeded Markov chain with dwell times and smooth transitions.
    /// Deterministic for a given seed, which is what makes the climate testable.
    ///
    /// Weather never snaps: every change blends over <see cref="TransitionSeconds"/>.
    /// </summary>
    public sealed class WeatherSimulation
    {
        public const float TransitionSeconds = 90f;

        /// <summary>A condition always holds at least this long, whatever the chain says.</summary>
        public const float MinDwellSeconds = 60f;

        private readonly Random _rng;

        private WeatherCondition _from;
        private WeatherCondition _to;
        private float _transitionElapsed;
        private float _dwellElapsed;
        private float _dwellTarget;

        public Season Season { get; set; }

        public WeatherSimulation(
            int seed, WeatherCondition initial = WeatherCondition.Overcast, Season season = Season.Autumn)
        {
            _rng = new Random(seed);
            _from = initial;
            _to = initial;
            _transitionElapsed = TransitionSeconds; // start settled
            Season = season;
            _dwellTarget = RollDwell(initial);
        }

        /// <summary>Set by a mission to force weather; cleared by <see cref="ReleaseScripted"/>.</summary>
        public bool IsScripted { get; private set; }

        public WeatherState Current => Blend();

        public void Advance(float deltaSeconds)
        {
            if (deltaSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(deltaSeconds), deltaSeconds, "Time does not run backwards.");
            }

            if (_transitionElapsed < TransitionSeconds)
            {
                _transitionElapsed += deltaSeconds;
                if (_transitionElapsed >= TransitionSeconds)
                {
                    _transitionElapsed = TransitionSeconds;
                    _from = _to;
                }
                return;
            }

            if (IsScripted) return;

            _dwellElapsed += deltaSeconds;
            if (_dwellElapsed < _dwellTarget) return;

            _dwellElapsed = 0f;
            WeatherCondition next = PickNext(_to);
            if (next == _to)
            {
                _dwellTarget = RollDwell(_to);
                return;
            }

            BeginTransition(next);
        }

        /// <summary>Force a condition for a story beat. Still transitions rather than snapping.</summary>
        public void ForceCondition(WeatherCondition condition)
        {
            IsScripted = true;
            if (condition != _to) BeginTransition(condition);
        }

        /// <summary>Hand control back to the simulation.</summary>
        public void ReleaseScripted()
        {
            IsScripted = false;
            _dwellElapsed = 0f;
            _dwellTarget = RollDwell(_to);
        }

        private void BeginTransition(WeatherCondition next)
        {
            _from = _to;
            _to = next;
            _transitionElapsed = 0f;
            _dwellElapsed = 0f;
            _dwellTarget = RollDwell(next);
        }

        private float RollDwell(WeatherCondition condition)
        {
            float mean = WeatherProfiles.For(condition).MeanDwellSeconds;
            // Uniform +/-40% around the mean, floored at the minimum dwell.
            float jitter = 0.6f + (float)_rng.NextDouble() * 0.8f;
            float dwell = mean * jitter;
            return dwell < MinDwellSeconds ? MinDwellSeconds : dwell;
        }

        private WeatherCondition PickNext(WeatherCondition current)
        {
            var candidates = ClimateTable.CandidatesFor(current);

            float total = 0f;
            for (int i = 0; i < candidates.Length; i++)
            {
                total += candidates[i].Weight * ClimateTable.SeasonalWeight(candidates[i].Next, Season);
            }

            if (total <= 0f) return current;

            float roll = (float)_rng.NextDouble() * total;
            float running = 0f;
            for (int i = 0; i < candidates.Length; i++)
            {
                running += candidates[i].Weight * ClimateTable.SeasonalWeight(candidates[i].Next, Season);
                if (roll <= running) return candidates[i].Next;
            }

            return candidates[candidates.Length - 1].Next;
        }

        private WeatherState Blend()
        {
            WeatherProfile a = WeatherProfiles.For(_from);
            WeatherProfile b = WeatherProfiles.For(_to);
            float t = TransitionSeconds <= 0f ? 1f : _transitionElapsed / TransitionSeconds;
            if (t > 1f) t = 1f;

            return new WeatherState(
                _from, _to, t,
                Lerp(a.Precipitation, b.Precipitation, t),
                Lerp(a.Snowfall, b.Snowfall, t),
                Lerp(a.CloudCover, b.CloudCover, t),
                Lerp(a.Visibility, b.Visibility, t),
                ClimateTable.BaseTemperature(Season),
                Lerp(a.WindSpeed, b.WindSpeed, t),
                Lerp(a.RelativeHumidity, b.RelativeHumidity, t));
        }

        private static float Lerp(float a, float b, float t) => a + (b - a) * t;
    }
}
