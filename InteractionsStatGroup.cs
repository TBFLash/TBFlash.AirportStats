using System;

namespace TBFlash.AirportStats
{
    internal class InteractionsStatGroup : StatGroup
    {
        internal LifetimeOnlyStats<NumberStat> keyboardInteractions = new LifetimeOnlyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats59"));
        internal LifetimeOnlyStats<NumberStat> mouseClicks = new LifetimeOnlyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats60"));
        internal LifetimeOnlyStats<NumberStat> altMouseClicks = new LifetimeOnlyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats61"));

        internal InteractionsStatGroup(string name) : base(name, null)
        {
        }

        internal override string ForChart(PrintOptions printOptions = null)
        {
            return string.Empty;
        }

        internal override string ForTable(PrintOptions printOptions = null)
        {
            string str = $"<tr><th colspan=\"2\">{name}</th></tr>\n";
            str += keyboardInteractions.ForTable();
            str += mouseClicks.ForTable();
            str += altMouseClicks.ForTable();
            return str;
        }

        internal override SeriesData GetSeriesData()
        {
            return null;
        }
    }
}
