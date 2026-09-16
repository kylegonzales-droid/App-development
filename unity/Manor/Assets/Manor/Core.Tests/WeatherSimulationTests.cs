using System.Collections.Generic;
using Manor.Core.Weather;
using NUnit.Framework;

namespace Manor.Core.Tests
{
    [TestFixture]
    public class WeatherSimulationTests
    {
        private static Dictionary<WeatherCondition, float> TimeByCondition(
            Season season, int seed, float totalSeconds, float step = 1f)
        {
            var sim = new WeatherSimulation(seed, WeatherCondition.Overcast, season);
            var totals = new Dictionary<WeatherCondition, float>();
            foreach (WeatherCondition c in System.Enum.GetValues(typeof(WeatherCondition)))
            {
                totals[c] = 0f;
            }

            for (float t = 0f; t < totalSeconds; t += step)
            {
                sim.Advance(step);
                totals[sim.Current.Dominant] += step;
            }
            return totals;
        }

        [Test]
        public void SameSeed_ProducesSameWeather()
        {
            var a = TimeByCondition(Season.Autumn, seed: 4242, totalSeconds: 20000f);
            var b = TimeByCondition(Season.Autumn, seed: 4242, totalSeconds: 20000f);
            CollectionAssert.AreEquivalent(a, b, "Weather must be deterministic for a given seed.");
        }

        [Test]
        public void DifferentSeeds_ProduceDifferentWeather()
        {
            var a = TimeByCondition(Season.Autumn, seed: 1, totalSeconds: 20000f);
            var b = TimeByCondition(Season.Autumn, seed: 2, totalSeconds: 20000f);
            Assert.That(a, Is.Not.EqualTo(b));
        }

        [Test]
        public void Weather_NeverSnaps()
        {
            var sim = new WeatherSimulation(7, WeatherCondition.Clear, Season.Autumn);
            float previous = sim.Current.Precipitation;

            for (int i = 0; i < 40000; i++)
            {
                sim.Advance(0.5f);
                float now = sim.Current.Precipitation;
                Assert.That(System.Math.Abs(now - previous), Is.LessThan(0.05f),
                    "Precipitation must blend, never jump.");
                previous = now;
            }
        }

        /// <summary>Kingston is grey. A world that is constantly dramatic has no weather at all.</summary>
        [Test]
        public void Climate_IsDominatedByGreyWeather()
        {
            var totals = TimeByCondition(Season.Autumn, seed: 99, totalSeconds: 400000f);
            float total = 0f;
            foreach (var kv in totals) total += kv.Value;

            float grey = totals[WeatherCondition.Overcast]
                         + totals[WeatherCondition.Cloudy]
                         + totals[WeatherCondition.LightRain];

            Assert.That(grey / total, Is.GreaterThan(0.6f),
                "Overcast, cloudy and light rain should hold most of an autumn.");
            Assert.That(totals[WeatherCondition.Storm] / total, Is.LessThan(0.10f),
                "Storms must stay rare enough to feel like events.");
        }

        [Test]
        public void Snow_NeverFallsOutsideWinter()
        {
            foreach (Season season in new[] { Season.Spring, Season.Summer, Season.Autumn })
            {
                var totals = TimeByCondition(season, seed: 5, totalSeconds: 300000f);
                Assert.That(totals[WeatherCondition.LightSnow], Is.Zero, $"Snow in {season}.");
                Assert.That(totals[WeatherCondition.HeavySnow], Is.Zero, $"Heavy snow in {season}.");
            }
        }

        [Test]
        public void Summer_IsClearerThanWinter()
        {
            var summer = TimeByCondition(Season.Summer, seed: 11, totalSeconds: 300000f);
            var winter = TimeByCondition(Season.Winter, seed: 11, totalSeconds: 300000f);
            Assert.That(summer[WeatherCondition.Clear], Is.GreaterThan(winter[WeatherCondition.Clear]));
        }

        [Test]
        public void Winter_IsFoggierThanSummer()
        {
            var summer = TimeByCondition(Season.Summer, seed: 21, totalSeconds: 300000f);
            var winter = TimeByCondition(Season.Winter, seed: 21, totalSeconds: 300000f);
            Assert.That(winter[WeatherCondition.Fog], Is.GreaterThan(summer[WeatherCondition.Fog]));
        }

        [Test]
        public void ForcedCondition_HoldsUntilReleased()
        {
            var sim = new WeatherSimulation(3, WeatherCondition.Clear, Season.Autumn);
            sim.ForceCondition(WeatherCondition.Fog);

            for (int i = 0; i < 20000; i++) sim.Advance(1f);

            Assert.That(sim.Current.Dominant, Is.EqualTo(WeatherCondition.Fog),
                "Scripted story weather must not drift.");

            sim.ReleaseScripted();
            Assert.That(sim.IsScripted, Is.False);
        }

        [Test]
        public void DerivedDryingConditions_TrackTheWeather()
        {
            var sim = new WeatherSimulation(8, WeatherCondition.Storm, Season.Winter);
            var drying = sim.Current.ToDryingConditions();
            Assert.That(drying.RelativeHumidity, Is.GreaterThan(0.9f));
            Assert.That(drying.WindSpeed, Is.GreaterThan(8f));
        }

        [Test]
        public void NegativeTime_IsRejected()
        {
            var sim = new WeatherSimulation(1);
            Assert.Throws<System.ArgumentOutOfRangeException>(() => sim.Advance(-1f));
        }
    }
}
