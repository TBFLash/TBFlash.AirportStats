namespace TBFlash.AirportStats
{
    internal class SeriesData
    {
        internal string Label { get; }
        internal string Key { get; }
        internal string Color { get; }
        internal string Order { get; }
        internal string Stack { get; }
        internal string YAxis { get; }

        internal SeriesData(string label, string key, string color, string order, string yAxis = null, string stack = null)
        {
            Label = label;
            Key = key;
            Color = color;
            Order = order;
            Stack = stack;
            YAxis = yAxis;
        }
    }
}
