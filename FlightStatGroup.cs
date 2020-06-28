namespace TBFlash.AirportStats
{
    internal class FlightStatGroup : StatGroup
    {
        internal DailyStats<IntStat> nSchedFlights;
        internal DailyStats<IntStat> nDelayedArrival;
        internal DailyStats<IntStat> nRequiresCrew;
        internal DailyStats<IntStat> nOntimeDeparture;
        internal DailyStats<IntStat> nDelayedDeparture;
        internal DailyStats<IntStat> nCancelled;
        internal DailyStats<IntStat> nAirportInvalid;
        internal DailyStats<IntStat> nWeather;
        internal DailyStats<IntStat> nRunway;
        internal DailyStats<IntStat> nGate;
        internal DailyStats<IntStat> nExpired;
        internal DailyStats<IntStat> nReneged;
        internal DailyStats<IntStat> nSmallGates;
        internal DailyStats<IntStat> nLargeGates;
        internal DailyStats<IntStat> nXLGates;
        internal DailyStats<AverageStat> ontimeDeparturePer;

        internal FlightStatGroup(string name) : base(name, new ChartOptions(ChartOptions.ChartTypes.line, i18n.Get("TBFlash.AirportStats.json.flightStats"), "false", i18n.Get("TBFlash.AirportStats.json.numberFlights")))
        {
            nSchedFlights = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats0"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.scheduledDepartures"), "schedDepart", "white", "4"));
            nDelayedArrival = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats1"), null);
            nRequiresCrew = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats2"), null);
            nOntimeDeparture = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats3"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.ontimeDepartures"), "ontimeDepart", "green", "1"));
            nDelayedDeparture = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats4"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.delayedDepartures"), "delayDepart", "cyan", "2"));
            nCancelled = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats5"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.canceled"), "canx", "red", "3"));
            nAirportInvalid = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats6"), null);
            nWeather = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats7"), null);
            nRunway = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats8"), null);
            nGate = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats9"), null);
            nExpired = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats10"), null);
            nReneged = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.stats11"), null);
            nSmallGates = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.smallFlights"), null);
            nLargeGates = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.largeFlights"), null);
            nXLGates = new DailyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.XLFlights"), null);
            ontimeDeparturePer = new DailyStats<AverageStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.ontimeDepartPer"), null);
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
            nSmallGates.RemoveStats(firstDay, lastDay);
            nLargeGates.RemoveStats(firstDay, lastDay);
            nXLGates.RemoveStats(firstDay, lastDay);
            ontimeDeparturePer.RemoveStats(firstDay, lastDay);
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
            string str = $"<tr class=\"statGroup\"><th colspan=\"2\"><a class=\"loadChart\" href=\"/chartdata?dataset=flightstats{(!string.IsNullOrEmpty(printOptions.AirlineName) ? "&airline=" + printOptions.AirlineName : string.Empty)}\" rel=\"#dialog\">{name}</a></th></tr>\n";
            str += nSchedFlights.ForTable(printOptions, true);
            str += nSmallGates.ForTable(printOptions);
            str += nLargeGates.ForTable(printOptions, true);
            str += nXLGates.ForTable(printOptions);
            str += nDelayedArrival.ForTable(printOptions, true);
            str += nRequiresCrew.ForTable(printOptions);
            str += nOntimeDeparture.ForTable(printOptions, true);
            str += ontimeDeparturePer.ForTable(printOptions);
            str += nDelayedDeparture.ForTable(printOptions, true);
            str += nCancelled.ForTable(printOptions);
            str += CanceledFor(printOptions, true);
            //str += nAirportInvalid.ForTable(printOptions, true);
            //str += nWeather.ForTable(printOptions);
            //str += nRunway.ForTable(printOptions, true);
            //str += nGate.ForTable(printOptions);
            //str += nExpired.ForTable(printOptions, true);
            //str += nReneged.ForTable(printOptions);
            return str;
        }

        private string CanceledFor(PrintOptions printOptions, bool oddRow)
        {
            string str = $"<tr{(oddRow ? " class=\"oddRow\"" : string.Empty)}><td>{i18n.Get("TBFlash.AirportStats.AirlineCompanyStats.canceledFor")}</td>\n";
            if (printOptions.IncludeLifetime)
                str += CanceledForContent(0, 0);
            str += CanceledForContent(printOptions.FirstDay, printOptions.LastDay);
            str += "</tr>";
            return str;
        }

        private string CanceledForContent(int start, int end)
        {
            string str = string.Empty;
            for (int i = end; i >= start; i--)
            {
                int numCanx = ((IntStat)nCancelled.GetStat(i))?.GetValue() ?? 0;
                int x;
                str += "<td>";
                if (numCanx > 0)
                {
                    if ((x = ((IntStat)nAirportInvalid.GetStat(i))?.GetValue() ?? 0) > 0)
                        str += $"{nAirportInvalid.statName}: {x}<br>";
                    if ((x = ((IntStat)nWeather.GetStat(i))?.GetValue() ?? 0) > 0)
                        str += $"{nWeather.statName}: {x}<br>";
                    if ((x = ((IntStat)nRunway.GetStat(i))?.GetValue() ?? 0) > 0)
                        str += $"{nRunway.statName}: {x}<br>";
                    if ((x = ((IntStat)nGate.GetStat(i))?.GetValue() ?? 0) > 0)
                        str += $"{nGate.statName}: {x}<br>";
                    if ((x = ((IntStat)nExpired.GetStat(i))?.GetValue() ?? 0) > 0)
                        str += $"{nExpired.statName}: {x}<br>";
                    if ((x = ((IntStat)nReneged.GetStat(i))?.GetValue() ?? 0) > 0)
                        str += $"{nReneged.statName}: {x}<br>";
                }
                str += "</td>";
            }
            return str;
        }
    }
}
