using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class LuggageStatGroup : StatGroup
    {
        internal DailyStats<IntStat> arrivingBags;
        internal DailyStats<IntStat> bagsUnloaded;
        internal DailyStats<IntStat> departingBags;
        internal DailyStats<IntStat> bagsLoaded;
        internal DailyStats<TimeStat> timeLoadingBags;
        internal DailyStats<TimeStat> timeUnloadingBags;
        internal DailyStats<IntStat> lostBags;

        internal LuggageStatGroup(string name) : base(name, new ChartOptions(ChartOptions.ChartTypes.line, i18n.Get("TBFlash.AirportStats.json.luggageStats"), "false", i18n.Get("TBFlash.AirportStats.json.numberOfBags")))
        {
            arrivingBags = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats12"), null);
            bagsUnloaded = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats13"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.bagsUnloaded"), "unload", "ivory", "3"));
            departingBags = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats15"), null);
            bagsLoaded = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats16"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.bagsLoaded"), "load", "green", "1"));
            timeLoadingBags = new DailyStats<TimeStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats17"), null);
            timeUnloadingBags = new DailyStats<TimeStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats14"), null);
            lostBags = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats23"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.lostBags"), "lost", "red", "2"));
        }

        internal void RemoveAirlineStats(int firstDay, int lastDay)
        {
            arrivingBags.RemoveStats(firstDay, lastDay);
            bagsUnloaded.RemoveStats(firstDay, lastDay);
            departingBags.RemoveStats(firstDay, lastDay);
            bagsLoaded.RemoveStats(firstDay, lastDay);
            timeLoadingBags.RemoveStats(firstDay, lastDay);
            timeUnloadingBags.RemoveStats(firstDay, lastDay);
            lostBags.RemoveStats(firstDay, lastDay);
        }

        internal override string ForChart(PrintOptions printOptions = null)
        {
            string str = string.Empty;
            for (int day = printOptions.FirstDay; day <= printOptions.LastDay; day++)
            {
                str += day > printOptions.FirstDay ? "," : string.Empty;
                str += $"\"{day}\":{{{bagsUnloaded.ForChart(day)},{bagsLoaded.ForChart(day)},{lostBags.ForChart(day)}}}";
            }
            return str;
        }
        internal override SeriesData GetSeriesData()
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            string labels = $"\"{bagsUnloaded.SeriesData.Label}\",\"{bagsLoaded.SeriesData.Label}\",\"{lostBags.SeriesData.Label}\"";
            string keys = $"\"{bagsUnloaded.SeriesData.Key}\",\"{bagsLoaded.SeriesData.Key}\",\"{lostBags.SeriesData.Key}\"";
            string colors = $"\"{bagsUnloaded.SeriesData.Color}\",\"{bagsLoaded.SeriesData.Color}\",\"{lostBags.SeriesData.Color}\"";
            string orders = $"\"{bagsUnloaded.SeriesData.Order}\",\"{bagsLoaded.SeriesData.Order}\",\"{lostBags.SeriesData.Order}\"";
            return new SeriesData(labels, keys, colors, orders);
        }
        internal override string ForTable(PrintOptions printOptions = null)
        {
            string str = $"<tr class=\"statGroup\"><th colspan=\"2\"><a class=\"loadChart\" href=\"/chartdata?dataset=luggagestats{(!string.IsNullOrEmpty(printOptions.AirlineName) ? "&airline=" + printOptions.AirlineName : string.Empty)}\" rel=\"#dialog\">{name}</a></th></tr>\n";
            str += arrivingBags.ForTable(printOptions, true);
            str += bagsUnloaded.ForTable(printOptions);
            str += departingBags.ForTable(printOptions, true);
            str += bagsLoaded.ForTable(printOptions);
            str += lostBags.ForTable(printOptions, true);
            str += timeLoadingBags.ForTable(printOptions);
            //str += timeUnloadingBags.ForTable(printOptions);
            return str;
        }
    }
}
