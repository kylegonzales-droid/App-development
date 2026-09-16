using Manor.Core.Time;
using NUnit.Framework;

namespace Manor.Core.Tests
{
    [TestFixture]
    public class GameClockTests
    {
        [Test]
        public void OneRealMinute_IsOneGameHour()
        {
            var clock = new GameClock(startHour: 0f);
            clock.Advance(60f);
            Assert.That(clock.Hour, Is.EqualTo(1f).Within(1e-4f));
        }

        [Test]
        public void TwentyFourRealMinutes_IsOneDay()
        {
            var clock = new GameClock(startHour: 0f);
            clock.Advance(GameClock.RealSecondsPerGameDay);
            Assert.That(clock.Day, Is.EqualTo(1));
            Assert.That(clock.Hour, Is.EqualTo(0f).Within(1e-3f));
        }

        [Test]
        public void HourWrapsAcrossMidnight()
        {
            var clock = new GameClock(startHour: 23f);
            clock.Advance(120f); // two game hours
            Assert.That(clock.Hour, Is.EqualTo(1f).Within(1e-4f));
            Assert.That(clock.Day, Is.EqualTo(1));
        }

        [TestCase(6f, DayPart.Early)]
        [TestCase(9f, DayPart.Commute)]
        [TestCase(13f, DayPart.Trade)]
        [TestCase(17f, DayPart.Turnover)]
        [TestCase(22f, DayPart.Night)]
        [TestCase(3f, DayPart.Dead)]
        [TestCase(0f, DayPart.Dead)]
        public void DayPartBoundaries(float hour, DayPart expected)
        {
            Assert.That(GameClock.PartForHour(hour), Is.EqualTo(expected));
        }

        [Test]
        public void EveryHourMapsToADayPart()
        {
            for (float h = 0f; h < 24f; h += 0.25f)
            {
                Assert.DoesNotThrow(() => GameClock.PartForHour(h), $"hour {h}");
            }
        }

        [Test]
        public void NegativeTime_IsRejected()
        {
            var clock = new GameClock();
            Assert.Throws<System.ArgumentOutOfRangeException>(() => clock.Advance(-1f));
        }
    }
}
