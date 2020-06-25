namespace TBFlash.AirportStats
{
    internal class FlightStatGroup : StatGroup
    {
        internal DailyStats<NumberStat> nSchedFlights;
        internal DailyStats<NumberStat> nDelayedArrival;
        internal DailyStats<NumberStat> nRequiresCrew;
        internal DailyStats<NumberStat> nOntimeDeparture;
        internal DailyStats<NumberStat> nDelayedDeparture;
        internal DailyStats<NumberStat> nCancelled;
        internal DailyStats<NumberStat> nAirportInvalid;
        internal DailyStats<NumberStat> nWeather;
        internal DailyStats<NumberStat> nRunway;
        internal DailyStats<NumberStat> nGate;
        internal DailyStats<NumberStat> nExpired;
        internal DailyStats<NumberStat> nReneged;

        internal FlightStatGroup(string name) : base(name, new ChartOptions(ChartOptions.ChartTypes.line, i18n.Get("TBFlash.AirportStats.json.flightStats"), "false", i18n.Get("TBFlash.AirportStats.json.numberFlights")))
        {
            nSchedFlights = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats0"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.scheduledDepartures"), "schedDepart", "white", "4"));
            nDelayedArrival = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats1"), null);
            nRequiresCrew = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats2"), null);
            nOntimeDeparture = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats3"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.ontimeDepartures"), "ontimeDepart", "green", "1"));
            nDelayedDeparture = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats4"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.delayedDepartures"), "delayDepart", "cyan", "2"));
            nCancelled = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats5"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.canceled"), "canx", "red", "3"));
            nAirportInvalid = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats6"), null);
            nWeather = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats7"), null);
            nRunway = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats8"), null);
            nGate = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats9"), null);
            nExpired = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats10"), null);
            nReneged = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats11"), null);
        }

        internal void RemoveAirlineStats(int firstDay, int lastDay)
        {
            nSchedFlights.RemoveStats(firstDay, lastDay);
            nDelayedArrival.RemoveStats(firstDay, lastDay);
            nRequiresCrew.RemoveStats(firstDay, lastDay);
            nOntimeDeparture.RemoveStats(firstDay, lastDay);
            nDelayedDeparture.RemoveStats(firstDay, lastDay);
            nCancelled.RemoveStats(firstDay, lastDay);
            nAirportInvalid.RemoveStats(firstDay, lastDay);
            nWeather.RemoveStats(firstDay, lastDay);
            nRunway.RemoveStats(firstDay, lastDay);
            nGate.RemoveStats(firstDay, lastDay);
            nExpired.RemoveStats(firstDay, lastDay);
            nReneged.RemoveStats(firstDay, lastDay);
        }

        internal override string ForChart(PrintOptions printOptions = null)
        {
            string str = string.Empty;
            for(int day = printOptions.FirstDay; day <= printOptions.LastDay; day++)
            {
                str += day > printOptions.FirstDay ? "," : string.Empty;
                str += $"\"{day}\":{{{nSchedFlights.ForChart(day)},{nOntimeDeparture.ForChart(day)},{nDelayedDeparture.ForChart(day)},{nCancelled.ForChart(day)}}}";
            }
            return str;
        }

        internal override SeriesData GetSeriesData()
        {
            string labels = $"\"{nSchedFlights.SeriesData.Label}\",\"{nOntimeDeparture.SeriesData.Label}\",\"{nDelayedDeparture.SeriesData.Label}\",\"{nCancelled.SeriesData.Label}\"";
            string keys = $"\"{nSchedFlights.SeriesData.Key}\",\"{nOntimeDeparture.SeriesData.Key}\",\"{nDelayedDeparture.SeriesData.Key}\",\"{nCancelled.SeriesData.Key}\"";
            string colors = $"\"{nSchedFlights.SeriesData.Color}\",\"{nOntimeDeparture.SeriesData.Color}\",\"{nDelayedDeparture.SeriesData.Color}\",\"{nCancelled.SeriesData.Color}\"";
            string orders = $"\"{nSchedFlights.SeriesData.Order}\",\"{nOntimeDeparture.SeriesData.Order}\",\"{nDelayedDeparture.SeriesData.Order}\",\"{nCancelled.SeriesData.Order}\"";
            return new SeriesData(labels, keys, colors, orders);
        }

        internal override string ForTable(PrintOptions printOptions = null)
        {
            string str = $"<tr><th colspan=\"2\"><a class=\"loadChart\" href=\"/chartdata?dataset=flightstats{(!string.IsNullOrEmpty(printOptions.AirlineName) ? "&airline=" + printOptions.AirlineName : string.Empty)}\" rel=\"#dialog\">{name}</a></th></tr>\n";
            str += nSchedFlights.ForTable(printOptions);
            str += nDelayedArrival.ForTable(printOptions);
            str += nRequiresCrew.ForTable(printOptions);
            str += nOntimeDeparture.ForTable(printOptions);
            str += nDelayedDeparture.ForTable(printOptions);
            str += nCancelled.ForTable(printOptions);
            str += nAirportInvalid.ForTable(printOptions);
            str += nWeather.ForTable(printOptions);
            str += nRunway.ForTable(printOptions);
            str += nGate.ForTable(printOptions);
            str += nExpired.ForTable(printOptions);
            str += nReneged.ForTable(printOptions);
            return str;
        }
    }
}
