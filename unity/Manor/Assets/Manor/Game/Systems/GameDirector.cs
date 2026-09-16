using Manor.Core.Missions;
using Manor.Core.Time;
using Manor.Core.Weather;
using Manor.Core.World;
using UnityEngine;

namespace Manor.Game.Systems
{
    /// <summary>
    /// Owns the pure simulation and ticks it. Everything else reads from here.
    ///
    /// The engine-facing layer deliberately holds no game state of its own — clock,
    /// weather, world flags and mission progress all live in Manor.Core, which has no
    /// engine references and is unit-testable without entering play mode.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public sealed class GameDirector : MonoBehaviour
    {
        [Header("Time")]
        [SerializeField] private float startHour = 17.5f;
        [Tooltip("Multiplier on the 1 real minute = 1 game hour baseline.")]
        [SerializeField] private float timeScale = 1f;

        [Header("Weather")]
        [SerializeField] private int weatherSeed = 1979;
        [SerializeField] private WeatherCondition startCondition = WeatherCondition.Overcast;
        [SerializeField] private Season season = Season.Autumn;

        public static GameDirector Instance { get; private set; }

        public GameClock Clock { get; private set; }
        public WeatherSimulation Weather { get; private set; }
        public WorldState World { get; private set; }
        public MissionRunner Missions { get; private set; }

        public WeatherState WeatherNow => Weather.Current;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Clock = new GameClock(startHour);
            Weather = new WeatherSimulation(weatherSeed, startCondition, season);
            World = new WorldState();
            Missions = new MissionRunner(World);
        }

        private void Start() => Missions.Begin(MissionLibrary.TheFirstDay());

        private void Update()
        {
            float dt = Time.deltaTime * Mathf.Max(timeScale, 0f);
            Clock.Advance(dt);
            Weather.Advance(dt);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>Force weather for testing or a story beat.</summary>
        public void ForceWeather(WeatherCondition condition) => Weather.ForceCondition(condition);

        public void ReleaseWeather() => Weather.ReleaseScripted();

        public void SetSeason(Season newSeason)
        {
            season = newSeason;
            Weather.Season = newSeason;
        }
    }
}
