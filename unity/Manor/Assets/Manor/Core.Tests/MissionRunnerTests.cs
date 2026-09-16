using System.Collections.Generic;
using Manor.Core.Missions;
using Manor.Core.World;
using NUnit.Framework;

namespace Manor.Core.Tests
{
    [TestFixture]
    public class MissionRunnerTests
    {
        private WorldState _world;
        private MissionRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _world = new WorldState();
            _runner = new MissionRunner(_world);
        }

        [Test]
        public void Begin_StartsAtTheFirstBeat()
        {
            _runner.Begin(MissionLibrary.TheFirstDay());
            Assert.That(_runner.Status, Is.EqualTo(MissionStatus.Running));
            Assert.That(_runner.CurrentBeat.Id, Is.EqualTo("explore_centre"));
        }

        [Test]
        public void TheFirstDay_RunsEndToEnd()
        {
            _runner.Begin(MissionLibrary.TheFirstDay());

            Assert.That(_runner.TryComplete(ObjectiveKind.GoTo, "town_centre"), Is.True);
            Assert.That(_runner.TryComplete(ObjectiveKind.GoTo, "riverside"), Is.True);
            Assert.That(_runner.TryComplete(ObjectiveKind.Talk, "local_resident"), Is.True);
            Assert.That(_runner.TryComplete(ObjectiveKind.GoTo, "high_street"), Is.True);
            Assert.That(_runner.TryComplete(ObjectiveKind.GoTo, "meeting_point"), Is.True);

            Assert.That(_runner.Status, Is.EqualTo(MissionStatus.Completed));
            Assert.That(_world.Flag("mission.the_first_day.complete"), Is.True);
            Assert.That(_world.StandingQuayside, Is.EqualTo(5));
        }

        [Test]
        public void WrongTarget_DoesNotAdvance()
        {
            _runner.Begin(MissionLibrary.TheFirstDay());
            Assert.That(_runner.TryComplete(ObjectiveKind.GoTo, "riverside"), Is.False,
                "Reaching a later zone early must not skip the mission forward.");
            Assert.That(_runner.CurrentBeat.Id, Is.EqualTo("explore_centre"));
        }

        [Test]
        public void WrongKind_DoesNotAdvance()
        {
            _runner.Begin(MissionLibrary.TheFirstDay());
            Assert.That(_runner.TryComplete(ObjectiveKind.Talk, "town_centre"), Is.False);
        }

        [Test]
        public void RepeatedTrigger_DoesNotDoubleAdvance()
        {
            _runner.Begin(MissionLibrary.TheFirstDay());
            _runner.TryComplete(ObjectiveKind.GoTo, "town_centre");
            _runner.TryComplete(ObjectiveKind.GoTo, "town_centre");
            Assert.That(_runner.CurrentBeat.Id, Is.EqualTo("find_river"),
                "A trigger firing twice must not skip a beat.");
        }

        [Test]
        public void StateChangesApplyOnlyOnCompletion()
        {
            _runner.Begin(MissionLibrary.TheFirstDay());
            _runner.TryComplete(ObjectiveKind.GoTo, "town_centre");
            _runner.TryComplete(ObjectiveKind.GoTo, "riverside");
            Assert.That(_world.Flag("met.first_local"), Is.False);

            _runner.TryComplete(ObjectiveKind.Talk, "local_resident");
            Assert.That(_world.Flag("met.first_local"), Is.True);
        }

        [Test]
        public void ObjectiveText_IsEmptyWhenNotRunning()
        {
            Assert.That(_runner.ObjectiveText, Is.Empty);
            _runner.Begin(MissionLibrary.TheFirstDay());
            Assert.That(_runner.ObjectiveText, Is.Not.Empty);
            _runner.Abandon();
            Assert.That(_runner.ObjectiveText, Is.Empty);
        }

        [Test]
        public void MissionCompleted_FiresExactlyOnce()
        {
            int fired = 0;
            _runner.MissionCompleted += _ => fired++;
            _runner.Begin(MissionLibrary.TheFirstDay());

            foreach (var (kind, target) in new[]
                     {
                         (ObjectiveKind.GoTo, "town_centre"), (ObjectiveKind.GoTo, "riverside"),
                         (ObjectiveKind.Talk, "local_resident"), (ObjectiveKind.GoTo, "high_street"),
                         (ObjectiveKind.GoTo, "meeting_point")
                     })
            {
                _runner.TryComplete(kind, target);
            }

            _runner.CompleteCurrentBeat(); // extra call after the end
            Assert.That(fired, Is.EqualTo(1));
        }

        [Test]
        public void DanglingTransition_IsRejectedAtConstruction()
        {
            Assert.Throws<System.ArgumentException>(() => new Mission("broken", "Broken",
                new List<Beat>
                {
                    new("a", new Objective(ObjectiveKind.GoTo, "x", "Go"), next: "does_not_exist")
                }));
        }

        [Test]
        public void DuplicateBeatId_IsRejected()
        {
            Assert.Throws<System.ArgumentException>(() => new Mission("dupes", "Dupes",
                new List<Beat>
                {
                    new("a", new Objective(ObjectiveKind.GoTo, "x", "Go")),
                    new("a", new Objective(ObjectiveKind.GoTo, "y", "Go"))
                }));
        }

        [Test]
        public void EveryBeatInTheFirstDay_IsReachableFromTheStart()
        {
            Mission mission = MissionLibrary.TheFirstDay();
            var reached = new HashSet<string>();
            string id = mission.FirstBeatId;

            while (!string.IsNullOrEmpty(id) && reached.Add(id))
            {
                id = mission.BeatById(id)?.Next;
            }

            Assert.That(reached.Count, Is.EqualTo(mission.BeatCount),
                "An unreachable beat is authored content the player can never see.");
        }

        [Test]
        public void FactionStanding_ClampsToRange()
        {
            _world.StandingQuayside = 500;
            Assert.That(_world.StandingQuayside, Is.EqualTo(100));
            _world.StandingFiveStar = -500;
            Assert.That(_world.StandingFiveStar, Is.EqualTo(-100));
        }

        [Test]
        public void FactionStandings_AreIndependent()
        {
            _world.StandingQuayside = 40;
            _world.StandingFiveStar = -20;
            Assert.That(_world.StandingQuayside, Is.EqualTo(40));
            Assert.That(_world.StandingFiveStar, Is.EqualTo(-20));
        }
    }
}
