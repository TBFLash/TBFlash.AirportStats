using System;

namespace TBFlash.AirportStats
{
    internal class StaffStatGroup : StatGroup
    {
        internal LifetimeOnlyStats<IntStat> nHires = new LifetimeOnlyStats<IntStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats52"));
        internal LifetimeOnlyStats<IntStat> nFires = new LifetimeOnlyStats<IntStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats53"));

        internal StaffStatGroup(string name) : base(name, null)
        {
        }

        internal override string ForChart(PrintOptions printOptions = null)
        {
            return string.Empty;
        }

        internal override string ForTable(PrintOptions printOptions = null)
        {
            string str = $"<tr class=\"statGroup\"><th colspan=\"2\">{name}</th></tr>\n";
            str += nHires.ForTable(true);
            str += nFires.ForTable();
            return str;
        }

        internal override SeriesData GetSeriesData()
        {
            return null;
        }
    }
}
