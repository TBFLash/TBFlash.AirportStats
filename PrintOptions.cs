namespace TBFlash.AirportStats
{
    internal class PrintOptions
    {
        internal int FirstDay { get; set; }
        internal int LastDay { get; set; }
        internal int Day { get; set; }
        internal bool IncludeLifetime { get; set; }
        internal bool ActiveOnly { get; set; }
        internal string AirlineName { get; set; }
    }
}
