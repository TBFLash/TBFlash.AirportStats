using System;

namespace TBFlash.AirportStats
{
    internal class TimeStatGroup : StatGroup
    {
        internal LifetimeOnlyStats<TimeStat> tPaused = new LifetimeOnlyStats<TimeStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats54"));
        internal LifetimeOnlyStats<TimeStat> tSpeed1 = new LifetimeOnlyStats<TimeStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats55"));
        internal LifetimeOnlyStats<TimeStat> tSpeed2 = new LifetimeOnlyStats<TimeStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats56"));
        internal LifetimeOnlyStats<TimeStat> tSpeed3 = new LifetimeOnlyStats<TimeStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats57"));
        internal LifetimeOnlyStats<TimeStat> tInactive = new LifetimeOnlyStats<TimeStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats58"));

        internal TimeStatGroup(string name) : base(name, null)
        {
        }

        internal override string ForChart(PrintOptions printOptions = null)
        {
            return string.Empty;
        }

        internal override string ForTable(PrintOptions printOptions = null)
        {
            string str = $"<tr><th colspan=\"2\">{name}</th></tr>\n";
            str += tPaused.ForTable();
            str += tSpeed1.ForTable();
            str += tSpeed2.ForTable();
            str += tSpeed3.ForTable();
            str += tInactive.ForTable();
            return str;
        }

        internal override SeriesData GetSeriesData()
        {
            return null;
        }
    }
}
