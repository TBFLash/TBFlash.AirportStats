using System;
using System.Collections.Generic;
using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class FlightData
    {
        private readonly IDictionary<int, IEnumerable<FlightRecord>> flights = new Dictionary<int, IEnumerable<FlightRecord>>();

        internal void AddFlights(int day)
        {
            flights.Remove(day);
            flights.Add(day, Game.current.flightRecords.GetForDay(day-1));
        }

        internal string ForTable(PrintOptions printOptions = null)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            return ForTableHeader(printOptions) + ForTableFlightRecords(printOptions);
        }

        private string ForTableFlightRecords(PrintOptions printOptions = null)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            string str = string.Empty;

            IOrderedEnumerable<FlightRecord> flightRecords = string.IsNullOrWhiteSpace(printOptions.AirlineName)
               ? flights[printOptions.Day].OrderBy(x => x.arrivalTime).ThenBy(x => x.airline)
               : flights[printOptions.Day].Where(x => x.airline == printOptions.AirlineName).OrderBy(x => x.arrivalTime).ThenBy(x => x.airline);

            if (!flightRecords.Any())
            {
                return str;
            }
            str += "<tbody>";
            bool oddRow = false;
            foreach (FlightRecord fr in flightRecords)
            {
                str += $"<tr{(oddRow ? " class=\"oddRow\"" : string.Empty)}>\n";
                if (string.IsNullOrWhiteSpace(printOptions.AirlineName))
                {
                    str += $"<td><a href=\"/{fr.airline}?Day={printOptions.Day}\">{fr.airline}</a></td>\n";
                }
                str += $"<td class=\"None\"><a class=\"ajax-dialog\" href=\"/aircraftstats?aircraft={fr.aircraft}\" rel=\"#dialog\">{fr.aircraft}</a></td>\n";
                str += $"<td class=\"None\">{DateTime.MinValue.AddSeconds(fr.arrivalTime * 60):t}</td>\n";
                str += $"<td class=\"{(fr.actual_arrivalTime <= 0 ? AirportStatUtils.InfoLevels.None : fr.actual_arrivalTime > fr.arrivalTime ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None)}\">{(fr.actual_arrivalTime > 0 ? DateTime.MinValue.AddSeconds(fr.actual_arrivalTime * 60).ToString("t") : string.Empty)}</td>\n";
                str += $"<td class=\"None\">{DateTime.MinValue.AddSeconds(fr.departureTime * 60):t}</td>\n";
                str += $"<td class=\"{(fr.actual_departureTime <= 0 ? AirportStatUtils.InfoLevels.None : fr.actual_departureTime - fr.departureTime > 15 ? AirportStatUtils.InfoLevels.Warning : fr.actual_departureTime - fr.departureTime > 0 ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None)}\">{(fr.actual_departureTime > 0 ? DateTime.MinValue.AddSeconds(fr.actual_departureTime * 60).ToString("t") : string.Empty)}</td>\n";
                str += $"<td class=\"None\">{fr.nArriving:#}</td>\n";
                str += $"<td class=\"None\">{(fr.actual_arrivalTime > 0 ? new TimeSpan(0, 0, (int)fr.time_deplaning * 60).ToString("g") : string.Empty)}</td>\n";
                str += $"<td class=\"None\">{fr.nDeparting:#}</td>\n";
                str += $"<td class=\"None\">{fr.nCheckedIn:#}</td>\n";
                str += $"<td class=\"{(fr.actual_departureTime <=0 ? AirportStatUtils.InfoLevels.None : fr.nBoarded < fr.nDeparting ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None)}\">{fr.nBoarded:#}</td>\n";
                str += $"<td class=\"None\">{(fr.nBoarded > 0 ? new TimeSpan(0, 0, (int)fr.time_boarding * 60).ToString("g") : string.Empty)}</td>\n";
                str += $"<td class=\"None\">{fr.nArrivalBags:#}</td>\n";
                str += $"<td class=\"None\">{fr.nBagsUnloaded:#}</td>\n";
                str += $"<td class=\"None\">{(fr.nBagsUnloaded > 0 ? DateTime.MinValue.AddSeconds(fr.time_bag_unload * 60).ToString("t") : string.Empty)}</td>\n";
                str += $"<td class=\"None\">{fr.nDepartingBags:#}</td>\n";
                str += $"<td class=\"{(fr.actual_departureTime <= 0 ? AirportStatUtils.InfoLevels.None : fr.nBagsLoaded < fr.nDepartingBags ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None)}\">{fr.nBagsLoaded:#}</td>\n";
                str += $"<td class=\"None\">{(fr.nBagsLoaded > 0 ? new TimeSpan(0, 0, (int)fr.time_bag_load * 60).ToString("g") : string.Empty)}</td>\n";
                str += $"<td class=\"None\">{fr.nFuelRequested / 1000:#,#}</td>\n";
                str += $"<td class=\"{(fr.actual_departureTime <=0 ? AirportStatUtils.InfoLevels.None : fr.nFuelRefueled < fr.nFuelRequested ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None)}\">{fr.nFuelRefueled / 1000:#,#}</td>\n";
                string st = string.Empty;
                foreach (Flight.Status stat in Enum.GetValues(typeof(Flight.Status)))
                {
                    if (AirportStatUtils.HasStatus(fr.status, stat))
                    {
                        st += i18n.Get("TBFlash.AirportStats.flightstatus." + Enum.GetName(typeof(Flight.Status), stat)) + "<br/>";
                    }
                }
                str += $"<td class=\"None\">{st}</td>\n";
                str += $"<td class=\"{(fr.reason.ToString().Length > 5 ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None)}\">{i18n.Get("UI.strings.flightstatusreason.") + fr.reason.ToString()}</td>\n";
                str += "</tr>\n";
                oddRow = !oddRow;
            }
            str += "</tbody>";
            return str;
        }

        private string ForTableHeader(PrintOptions printOptions = null)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            string str = string.Empty;
            str += "<thead><tr>\n";
            if (string.IsNullOrWhiteSpace(printOptions.AirlineName))
            {
                str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats0")}</th>\n";
            }
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats1")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats2")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats3")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats4")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats5")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats6")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats7")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats8")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats9")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats10")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats11")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats12")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats13")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats14")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats15")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats16")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats17")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats18")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats19")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats20")}</th>\n";
            str += $"<th>{i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats21")}</th>\n";
            str += "</tr></thead>\n";
            return str;
        }
    }
}
