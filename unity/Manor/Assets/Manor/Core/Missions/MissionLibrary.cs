using System.Collections.Generic;

namespace Manor.Core.Missions
{
    /// <summary>
    /// Authored missions. Data, not code: adding a mission means adding a method
    /// like the one below, never touching <see cref="MissionRunner"/>.
    /// </summary>
    public static class MissionLibrary
    {
        /// <summary>
        /// The prototype's opening mission. Walks the player through the slice so
        /// every system gets exercised: traversal, the river, an NPC conversation,
        /// the high street, and a return leg.
        /// </summary>
        public static Mission TheFirstDay() => new(
            id: "the_first_day",
            title: "The First Day",
            beats: new List<Beat>
            {
                new("explore_centre",
                    new Objective(ObjectiveKind.GoTo, "town_centre",
                        "Have a look round the town centre.", radius: 12f),
                    next: "find_river"),

                new("find_river",
                    new Objective(ObjectiveKind.GoTo, "riverside",
                        "Find the river.", radius: 10f),
                    next: "speak_local"),

                new("speak_local",
                    new Objective(ObjectiveKind.Talk, "local_resident",
                        "Speak to someone local."),
                    next: "visit_high_street",
                    onComplete: new[] { StateChange.Flag("met.first_local") }),

                new("visit_high_street",
                    new Objective(ObjectiveKind.GoTo, "high_street",
                        "Walk the high street.", radius: 12f),
                    next: "return_meeting_point"),

                new("return_meeting_point",
                    new Objective(ObjectiveKind.GoTo, "meeting_point",
                        "Head back to the meeting point.", radius: 8f),
                    onComplete: new[]
                    {
                        StateChange.Flag("mission.the_first_day.complete"),
                        StateChange.Add("standing.quayside", 5)
                    })
            });
    }
}
