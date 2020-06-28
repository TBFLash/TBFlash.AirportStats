using System;

namespace TBFlash.AirportStats
{
    internal class InteractionsStatGroup : StatGroup
    {
        internal LifetimeOnlyStats<IntStat> keyboardInteractions = new LifetimeOnlyStats<IntStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats59"));
        internal LifetimeOnlyStats<IntStat> mouseClicks = new LifetimeOnlyStats<IntStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats60"));
        internal LifetimeOnlyStats<IntStat> altMouseClicks = new LifetimeOnlyStats<IntStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats61"));

        internal InteractionsStatGroup(string name) : base(name, null)
        {
        }

        internal override string ForChart(PrintOptions printOptions = null)
        {
            return string.Empty;
        }

        internal override string ForTable(PrintOptions printOptions = null)
        {
            string str = $"<tr class=\"statGroup\"><th colspan=\"2\">{name}</th></tr>\n";
            str += keyboardInteractions.ForTable(true);
            str += mouseClicks.ForTable();
            str += altMouseClicks.ForTable(true);
            return str;
        }

        internal override SeriesData GetSeriesData()
        {
            return null;
        }
    }
}
