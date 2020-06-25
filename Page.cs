using SimAirport.Logging;
using System.Web.UI.WebControls;

namespace TBFlash.AirportStats
{
    static internal class Page
    {
        static internal string GetAircraftStats(AircraftConfig aircraftConfig)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            string str = string.Empty;
            StatLoader.UpdateAircraftStats(aircraftConfig);
            str += $"<div class=\"modal\"><h1>{aircraftConfig.DisplayName}</h1><table>\n";
            str += StatLoader.aircraftData.ForTable();
            str += "</table></div>";
            return str;
        }

        static internal string GetAirlineStats(bool activeOnly = false)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            StatLoader.UpdateAirlineStats();
            string str = activeOnly ? AirportStatUtils.PageHead(AirportStatUtils.PageTitles.ActiveAirlines, true) : AirportStatUtils.PageHead(AirportStatUtils.PageTitles.AllAirlines, true);
            str += "<table>";
            str += StatLoader.airportData.airlineStats.ForTable(new PrintOptions { ActiveOnly = activeOnly });
            str += "</table>" + AirportStatUtils.PageFooter();
            return str;
        }

        static internal string GetAirlineData(Airline airline = null)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool($"airlinename: {airline?.name ?? "null"}").WithCodepoint());
            string day = i18n.Get("TBFlash.AirportStats.utils.day");
            StatLoader.UpdateAirlineData();
            string str = airline == null ? AirportStatUtils.PageHead(AirportStatUtils.PageTitles.AirportStats, true) : AirportStatUtils.PageHead(airline, -1);
            str += "<table>\n<tr>\n<th></th>";
            str += airline == null ? "<th>" + i18n.Get("TBFlash.AirportStats.utils.lifetime") + "</th>" : string.Empty;
            for(int i= GameTimer.Day; i>=StatLoader.FirstDay; i--)
            {
                str += airline == null ? $"<th><a href=\"Daily Stats?Day={i}\">{day} {i}</a></th>" : $"<th><a href=\"/{airline.name}?Day={i}\">{day} {i}</a></th>";
            }
            str += "</tr>\n";
            str += (airline == null
                ? StatLoader.airportData.ForTable(new PrintOptions { FirstDay = StatLoader.FirstDay, LastDay = GameTimer.Day, IncludeLifetime = true })
                : StatLoader.airlineData.ForTable(new PrintOptions { FirstDay = StatLoader.FirstDay, LastDay = GameTimer.Day, AirlineName = airline.name, IncludeLifetime = false }));
            str += "</table>" + AirportStatUtils.PageFooter();
            return str;
        }

        static internal string GetChartData(string dataset, string airlineName)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            StatLoader.UpdateAirlineData();
            StatGroup statGroup = GetStatGroup(dataset, airlineName);
            PrintOptions printOptions = new PrintOptions() { FirstDay = StatLoader.FirstDay, LastDay = StatLoader.LastDay };
            string str = $"{{\n\t{statGroup.GetChartData().GetChartOptions()},\n\t{GetSeriesData(statGroup)},\n\t\"chartData\":[{{{statGroup.ForChart(printOptions)}}}]}}";
            AirportStatUtils.AirportStatsLogger(Log.FromPool(str).WithCodepoint());
            return str;
        }

        static internal string GetFlightData(int day, Airline airline = null)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            StatLoader.UpdateFlights();
            string str = AirportStatUtils.PageHead(airline, day);
            //str += "<table class=\"scrollable\">";
            str += "<table>";
            str += StatLoader.flightData.ForTable(new PrintOptions { Day = day, AirlineName = (airline?.name) });
            str += "</table>" + AirportStatUtils.PageFooter();
            return str;
        }

        static internal string GetFuelFutures()
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            string str = AirportStatUtils.PageHead(AirportStatUtils.PageTitles.FuelFutures, true);
            str += "<table>";
            str += StatLoader.airportData.fuelFutures.ForTable();
            str += "</table>" + AirportStatUtils.PageFooter();
            return str;
        }

        static private string GetSeriesData(StatGroup statGroup)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            SeriesData seriesData = statGroup.GetSeriesData();
            string label = "\"seriesConfig\":[{\"seriesLabels\":[" + seriesData.Label + "]";
            string key = ",\"seriesKeys\":[" + seriesData.Key + "]";
            string color = ",\"seriesColors\":[" + seriesData.Color + "]";
            string order = ",\"seriesOrders\":[" + seriesData.Order + "]";
            string stack = seriesData.Stack != null ? ",\"seriesStacks\":[" + seriesData.Stack + "]" : string.Empty;
            string yAxis = seriesData.YAxis != null ? ",\"seriesYAxis\":[" + seriesData.YAxis + "]" : string.Empty;
            return label + key + color + order + stack + yAxis + "}]";
        }

        static private StatGroup GetStatGroup (string dataset, string airlineName)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            if (string.IsNullOrEmpty(dataset))
            {
                return null;
            }
            switch (dataset.ToUpperInvariant())
            {
                case "FLIGHTSTATS":
                    if (!string.IsNullOrWhiteSpace(airlineName))
                        return StatLoader.airlineData.GetAirlineDailyData(airlineName).flightStats;
                    return StatLoader.airportData.flightStats;
                case "FUELFUTURES":
                    return StatLoader.airportData.fuelFutures;
                case "FUELSTATS":
                    if (!string.IsNullOrWhiteSpace(airlineName))
                        return StatLoader.airlineData.GetAirlineDailyData(airlineName).fuelStats;
                    return StatLoader.airportData.fuelStats;
                case "LUGGAGESTATS":
                    if (!string.IsNullOrWhiteSpace(airlineName))
                        return StatLoader.airlineData.GetAirlineDailyData(airlineName).luggageStats;
                    return StatLoader.airportData.luggageStats;
                case "PAXSTATS":
                    if (!string.IsNullOrWhiteSpace(airlineName))
                        return StatLoader.airlineData.GetAirlineDailyData(airlineName).passengerStats;
                    return StatLoader.airportData.passengerStats;
                case "PROFITS":
                    return StatLoader.airportData.revAndExpStats;
                default:
                    return null;
            }
        }
    }
}
