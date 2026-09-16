namespace Manor.Core.Time
{
    /// <summary>Coarse slices of the day that drive NPC density, lighting mood and job availability.</summary>
    public enum DayPart
    {
        Early,      // 05–08
        Commute,    // 08–10
        Trade,      // 10–16
        Turnover,   // 16–19
        Night,      // 19–00
        Dead        // 00–05
    }
}
