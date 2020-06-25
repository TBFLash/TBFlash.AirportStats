using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class PassengerStatGroup : StatGroup
    {
        internal DailyStats<NumberStat> nArriving;
        internal DailyStats<NumberStat> nSchedDep;
        internal DailyStats<NumberStat> nCheckedIn;
        internal DailyStats<NumberStat> nBoarded;
        internal DailyStats<NumberStat> nMissed;
        internal DailyStats<NumberStat> nConnecting;
        internal DailyStats<TimeStat> timeDeplaning;
        internal DailyStats<TimeStat> timeBoarding;

        internal PassengerStatGroup(string name) : base(name, new ChartOptions(ChartOptions.ChartTypes.line, i18n.Get("TBFlash.AirportStats.json.paxStats"), "false", i18n.Get("TBFlash.AirportStats.json.numPax")))
        {
            nArriving = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats6"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.arriving"), "arrive", "ivory", "4"));
            nConnecting = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats8"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.connecting"), "connect", "cyan", "3"));
            nSchedDep = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats8"), null);
            nCheckedIn = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats9"), null);
            nBoarded = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats10"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.boarded"), "board", "green", "1"));
            nMissed = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats16"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.missedFlight"), "missed", "red", "2"));
            timeDeplaning = new DailyStats<TimeStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats7"), null);
            timeBoarding = new DailyStats<TimeStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats11"), null);
        }

        internal void RemoveAirlineStats(int firstDay, int lastDay)
        {
            nArriving.RemoveStats(firstDay, lastDay);
            nSchedDep.RemoveStats(firstDay, lastDay);
            nCheckedIn.RemoveStats(firstDay, lastDay);
            nBoarded.RemoveStats(firstDay, lastDay);
            nMissed.RemoveStats(firstDay, lastDay);
            nConnecting.RemoveStats(firstDay, lastDay);
            timeDeplaning.RemoveStats(firstDay, lastDay);
            timeBoarding.RemoveStats(firstDay, lastDay);
        }

        internal override string ForChart(PrintOptions printOptions = null)
        {
            string str = string.Empty;
            for (int day = printOptions.FirstDay; day <= printOptions.LastDay; day++)
            {
                str += day > printOptions.FirstDay ? "," : string.Empty;
                str += $"\"{day}\":{{{nArriving.ForChart(day)},{nConnecting.ForChart(day)},{nBoarded.ForChart(day)},{nMissed.ForChart(day)}}}";
            }
            return str;
        }

        internal override SeriesData GetSeriesData()
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            string labels = $"\"{nArriving.SeriesData.Label}\",\"{nConnecting.SeriesData.Label}\",\"{nBoarded.SeriesData.Label}\",\"{nMissed.SeriesData.Label}\"";
            string keys = $"\"{nArriving.SeriesData.Key}\",\"{nConnecting.SeriesData.Key}\",\"{nBoarded.SeriesData.Key}\",\"{nMissed.SeriesData.Key}\"";
            string colors = $"\"{nArriving.SeriesData.Color}\",\"{nConnecting.SeriesData.Color}\",\"{nBoarded.SeriesData.Color}\",\"{nMissed.SeriesData.Color}\"";
            string orders = $"\"{nArriving.SeriesData.Order}\",\"{nConnecting.SeriesData.Order}\",\"{nBoarded.SeriesData.Order}\",\"{nMissed.SeriesData.Order}\"";
            return new SeriesData(labels, keys, colors, orders);
        }

        internal override string ForTable(PrintOptions printOptions = null)
        {
            string str = $"<tr><th colspan=\"2\"><a class=\"loadChart\" href=\"/chartdata?dataset=paxstats{(!string.IsNullOrEmpty(printOptions.AirlineName) ? "&airline=" + printOptions.AirlineName : string.Empty)}\" rel=\"#dialog\">{name}</a></th></tr>\n";
            str += nArriving.ForTable(printOptions);
            str += nSchedDep.ForTable(printOptions);
            str += nCheckedIn.ForTable(printOptions);
            str += nBoarded.ForTable(printOptions);
            str += nMissed.ForTable(printOptions);
            str += nConnecting.ForTable(printOptions);
            str += timeDeplaning.ForTable(printOptions);
            str += timeBoarding.ForTable(printOptions);
            return str;
        }
    }
}
