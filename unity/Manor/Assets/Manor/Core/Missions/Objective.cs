namespace Manor.Core.Missions
{
    /// <summary>
    /// The composable objective vocabulary. Deliberately small: new missions come
    /// from recombination, not from new code.
    /// </summary>
    public enum ObjectiveKind
    {
        GoTo,
        Talk,
        Collect,
        Deliver,
        Follow,
        Evade,
        Protect,
        Survive,
        Race,
        Avoid,
        Wait
    }

    /// <summary>One thing the player is asked to do, plus the line shown on the HUD.</summary>
    public readonly struct Objective
    {
        public ObjectiveKind Kind { get; }

        /// <summary>Location id, character id, or item id depending on <see cref="Kind"/>.</summary>
        public string Target { get; }

        /// <summary>Plain-language line for the HUD. Never a checklist.</summary>
        public string Description { get; }

        /// <summary>Metres, for proximity objectives. Ignored otherwise.</summary>
        public float Radius { get; }

        public Objective(ObjectiveKind kind, string target, string description, float radius = 6f)
        {
            Kind = kind;
            Target = target;
            Description = description;
            Radius = radius;
        }

        public override string ToString() => $"{Kind}({Target})";
    }
}
