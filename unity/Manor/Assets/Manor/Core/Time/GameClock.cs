using System;

namespace Manor.Core.Time
{
    /// <summary>
    /// Game time. One real minute is one game hour, so a full day is 24 real minutes.
    /// Pure: no engine types, no frame coupling. Callers supply elapsed real seconds.
    /// </summary>
    public sealed class GameClock
    {
        /// <summary>Real seconds per game hour.</summary>
        public const float RealSecondsPerGameHour = 60f;

        /// <summary>Real seconds in one full game day.</summary>
        public const float RealSecondsPerGameDay = RealSecondsPerGameHour * 24f;

        private float _hour;

        public GameClock(float startHour = 23.5f)
        {
            _hour = Wrap(startHour);
        }

        /// <summary>Hour of day in [0, 24).</summary>
        public float Hour => _hour;

        /// <summary>Whole days elapsed since the clock started.</summary>
        public int Day { get; private set; }

        public DayPart Part => PartForHour(_hour);

        /// <summary>Advance by elapsed real seconds. Negative input is rejected.</summary>
        public void Advance(float realSeconds)
        {
            if (realSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(realSeconds), realSeconds, "Time does not run backwards.");
            }

            float advanced = _hour + realSeconds / RealSecondsPerGameHour;
            Day += (int)Math.Floor(advanced / 24f);
            _hour = Wrap(advanced);
        }

        public static DayPart PartForHour(float hour)
        {
            float h = Wrap(hour);
            if (h >= 5f && h < 8f) return DayPart.Early;
            if (h >= 8f && h < 10f) return DayPart.Commute;
            if (h >= 10f && h < 16f) return DayPart.Trade;
            if (h >= 16f && h < 19f) return DayPart.Turnover;
            if (h >= 19f) return DayPart.Night;
            return DayPart.Dead; // 00:00–05:00
        }

        /// <summary>Fraction of the way through the current day, 0 at midnight.</summary>
        public float NormalisedDay => _hour / 24f;

        private static float Wrap(float hour)
        {
            float h = hour % 24f;
            return h < 0f ? h + 24f : h;
        }
    }
}
