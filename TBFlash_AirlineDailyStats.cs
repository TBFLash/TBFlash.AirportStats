using System;
using System.Collections.Generic;
using System.Linq;

namespace TBFlash.AirportStats
{
    internal class TBFlash_AirlineDailyStats
    {
        private readonly int arrayCols = 21;

        internal string GetDailyStats(Airline airline, int day)
        {
            string str = "<table>";
            string[,] arr = LoadArray(airline, day);
            for (int i = 0; i < (arr.Length / arrayCols); i++)
            {
                str += "<tr>";
                for (int j = 0; j < arrayCols; j++)
                {
                    str += $"<td>{arr[i, j]}</td>";
                }
                str += "</tr>";
            }
            str += "</table>";
            return str;
        }

        private string[,] LoadArray(Airline airline, int day)
        {
            IEnumerable<FlightRecord> flightRecords = Game.current.flightRecords.GetForDay(day-1).Where(x => x.airline == airline.name).OrderBy(y=>y.arrivalTime);
            int numFlights = flightRecords.Count();
            string[,] arr = new string[numFlights+1, arrayCols];
            for (int i = 0; i < arrayCols; i++)
            {
                arr[0, i] = i18n.Get($"TBFlash.AirportStats.AirlineDailyStats.stats{i}");
            }
            for (int i = 1; i <= numFlights; i++)
            {
                FlightRecord fr = flightRecords.ElementAt(i - 1);
                arr[i, 0] = fr.aircraft;
                arr[i, 1] = TBFlash_Utils.FormatTime(fr.arrivalTime * 60, false, false, true);
                arr[i, 2] = fr.actual_arrivalTime > 0 ? TBFlash_Utils.FormatTime(fr.actual_arrivalTime * 60, false, false, true) : string.Empty;
                arr[i, 3] = TBFlash_Utils.FormatTime(fr.departureTime * 60, false, false, true);
                arr[i, 4] = fr.actual_departureTime > 0 ? TBFlash_Utils.FormatTime(fr.actual_departureTime * 60, false, false, true) : string.Empty;
                arr[i, 5] = fr.nArriving.ToString("#");
                arr[i, 6] = fr.actual_arrivalTime > 0 ? TBFlash_Utils.FormatTime(fr.time_deplaning * 60, false, true, false) : string.Empty;
                arr[i, 7] = fr.nDeparting.ToString("#");
                arr[i, 8] = fr.nCheckedIn.ToString("#");
                arr[i, 9] = fr.nBoarded.ToString("#");
                arr[i, 10] = fr.nBoarded > 0 ? TBFlash_Utils.FormatTime(fr.time_boarding * 60, false, true, false) : string.Empty;
                arr[i, 11] = fr.nArrivalBags.ToString("#");
                arr[i, 12] = fr.nBagsUnloaded.ToString("#");
                arr[i, 13] = fr.nBagsUnloaded > 0 ? TBFlash_Utils.FormatTime(fr.time_bag_unload * 60, false, false, true) : string.Empty;
                arr[i, 14] = fr.nDepartingBags.ToString("#");
                arr[i, 15] = fr.nBagsLoaded.ToString("#");
                arr[i, 16] = fr.nBagsLoaded > 0 ? TBFlash_Utils.FormatTime(fr.time_bag_load * 60, false, true, false) : string.Empty;
                arr[i, 17] = (fr.nFuelRequested / 1000).ToString("#");
                arr[i, 18] = (fr.nFuelRefueled / 1000).ToString("#");
                string st = string.Empty;
                foreach (int stat in Enum.GetValues(typeof (Flight.Status)))
                {
                    if(HasStatus(fr.status, stat))
                    {
                        st += Enum.GetName(typeof(Flight.Status), stat) + "<br/>";
                    }
                }
                arr[i, 19] = st;
                arr[i, 20] = fr.reason.ToString();
            }

            return arr;
        }

        private bool HasStatus(int totalStatus, int status)
        {
            return (totalStatus & (int)status) == (int)status;
        }
    }
}
