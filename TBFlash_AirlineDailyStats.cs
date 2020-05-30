using System;
using System.Linq;

namespace TBFlash.AirportStats
{
    internal class TBFlash_AirlineDailyStats
    {
        private readonly int arrayCols = 22;

        internal string GetDailyStats(Airline airline, int day)
        {
            string htmlCode = TBFlash_Utils.PageHead(airline, day);
            htmlCode += "<table>";
            string[,] arr = LoadArray(airline, day);
            for (int i = 0; i < (arr.Length / arrayCols); i++)
            {
                htmlCode += "<tr>";

                for (int j = airline == null ? 0 : 1; j < arrayCols; j++)
                {
                    htmlCode += i == 0 ? $"<th>{arr[i, j]}</th>"
                        : j == 0
                            ? $"<td><a href=\"/{arr[i, j]}?Day={day}\">{arr[i, j]}</a></td>"
                            : j == 1
                                ? $"<td><a class=\"ajax-dialog\" href=\"/aircraftstats?aircraft={arr[i, j]}\" rel=\"#dialog\">{arr[i, j]}</a></td>"
                                : $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }
            htmlCode += "</table>";
            htmlCode += TBFlash_Utils.PageFooter();
            return htmlCode;
        }

        private string[,] LoadArray(Airline airline, int day)
        {
            IOrderedEnumerable<FlightRecord> flightRecords = airline == null
                ? Game.current.flightRecords.GetForDay(day - 1).OrderBy(x => x.arrivalTime).ThenBy(x => x.airline)
                : Game.current.flightRecords.GetForDay(day - 1).Where(x => x.airline == airline.name).OrderBy(x => x.arrivalTime).ThenBy(x => x.airline);
            int numFlights = flightRecords.Count();
            string[,] arr = new string[numFlights+1, arrayCols];
            for (int i = 0; i < arrayCols; i++)
            {
                arr[0, i] = i18n.Get($"TBFlash.AirportStats.AirlineDailyStats.stats{i}");
            }
            for (int i = 1; i <= numFlights; i++)
            {
                FlightRecord fr = flightRecords.ElementAt(i - 1);
                arr[i, 0] = fr.airline;
                arr[i, 1] = fr.aircraft;
                arr[i, 2] = TBFlash_Utils.FormatTime(fr.arrivalTime * 60, true);
                arr[i, 3] = fr.actual_arrivalTime > 0 ? TBFlash_Utils.FormatTime(fr.actual_arrivalTime * 60, true) : string.Empty;
                arr[i, 4] = TBFlash_Utils.FormatTime(fr.departureTime * 60, true);
                arr[i, 5] = fr.actual_departureTime > 0 ? TBFlash_Utils.FormatTime(fr.actual_departureTime * 60, true) : string.Empty;
                arr[i, 6] = fr.nArriving.ToString("#");
                arr[i, 7] = fr.actual_arrivalTime > 0 ? TBFlash_Utils.FormatTime(fr.time_deplaning * 60) : string.Empty;
                arr[i, 8] = fr.nDeparting.ToString("#");
                arr[i, 9] = fr.nCheckedIn.ToString("#");
                arr[i, 10] = fr.nBoarded.ToString("#");
                arr[i, 11] = fr.nBoarded > 0 ? TBFlash_Utils.FormatTime(fr.time_boarding * 60) : string.Empty;
                arr[i, 12] = fr.nArrivalBags.ToString("#");
                arr[i, 13] = fr.nBagsUnloaded.ToString("#");
                arr[i, 14] = fr.nBagsUnloaded > 0 ? TBFlash_Utils.FormatTime(fr.time_bag_unload * 60, true) : string.Empty;
                arr[i, 15] = fr.nDepartingBags.ToString("#");
                arr[i, 16] = fr.nBagsLoaded.ToString("#");
                arr[i, 17] = fr.nBagsLoaded > 0 ? TBFlash_Utils.FormatTime(fr.time_bag_load * 60) : string.Empty;
                arr[i, 18] = (fr.nFuelRequested / 1000).ToString("#,###");
                arr[i, 19] = (fr.nFuelRefueled / 1000).ToString("#,###");
                string st = string.Empty;
                foreach (Flight.Status stat in Enum.GetValues(typeof(Flight.Status)))
                {
                    if (TBFlash_Utils.HasStatus(fr.status, stat))
                    {
                        st += i18n.Get("TBFlash.AirportStats.flightstatus." + Enum.GetName(typeof(Flight.Status), stat)) + "<br/>";
                    }
                }
                arr[i, 20] = st;
                arr[i, 21] = i18n.Get("UI.strings.flightstatusreason.") + fr.reason.ToString();
            }
            return arr;
        }
    }
}
