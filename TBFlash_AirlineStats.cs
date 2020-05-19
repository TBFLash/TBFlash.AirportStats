using System.Collections.Generic;
using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class TBFlash_AirlineStats
    {
        private readonly int arrayRows = 26;

        internal string GetAirlineStats(Airline airline)
        {
            int counter = 1 + (GameTimer.Day <= 30 ? GameTimer.Day : 30);
            TBFlash_Utils.TBFlashLogger(Log.FromPool($"counter:{counter}").WithCodepoint());
            string[,] arr = LoadArray(airline);

            string str = "<table><tr><th></th>";
            string day = i18n.Get("TBFlash.AirportStats.utils.day");
            for (int i = GameTimer.Day; i >= (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i--)
            {
                str += $"<th><a href=\"/{airline.name}?Day={i}\">{day} {i}</a></th>";
            }

            for (int i = 0; i < arrayRows; i++)
            {
                str += "<tr>";
                for (int j = 0; j < (arr.Length / arrayRows); j++)
                {
                    str += $"<td>{arr[i, j]}</td>";
                }
                str += "</tr>";
            }
            str += "</table>";
            return str;
        }

        private string[,] LoadArray(Airline airline)
        {
            int numdays = GameTimer.Day <= 30 ? GameTimer.Day : 30;
            TBFlash_Utils.TBFlashLogger(Log.FromPool($"GameTimer:{GameTimer.Day}, numdays:{numdays}").WithCodepoint());
            string[,] arr = new string[arrayRows, numdays+1];

            for (int i = 0; i < arrayRows; i++)
            {
                arr[i, 0] = i18n.Get($"TBFlash.AirportStats.AirlineCompanyStats.stats{i}");
            }
            TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());

            int j = 0;
            for (int i = GameTimer.Day; i >= (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i--)
            {
                j++;
                TBFlash_Utils.TBFlashLogger(Log.FromPool($"i:{i}; j:{j}").WithCodepoint());

                IEnumerable<FlightRecord> frs = Game.current.flightRecords.GetForDay(i-1).Where(x => x.airline == airline.name);
                arr[0, j] = frs.Any() ? frs.Count().ToString("#,###"):"None";
                arr[1, j] = frs.Sum(x => x.nArriving).ToString("#,###");
                arr[2, j] = TBFlash_Utils.FormatTime(frs.Sum(x => x.time_deplaning)*60f);
                arr[3, j] = frs.Sum(x => x.nDeparting).ToString("#,###");
                arr[4, j] = frs.Sum(x => x.nCheckedIn).ToString("#,###");
                arr[5, j] = frs.Sum(x => x.nBoarded).ToString("#,###");
                arr[6, j] = TBFlash_Utils.FormatTime(frs.Sum(x => x.nBoarded > 0 ? x.time_boarding : 0) * 60f);
                arr[7, j] = frs.Sum(x => x.nArrivalBags).ToString("#,###");
                arr[8, j] = frs.Sum(x => x.nBagsUnloaded).ToString("#,###");
                arr[9, j] = frs.Sum(x => x.nDepartingBags).ToString("#,###");
                arr[10, j] = frs.Sum(x => x.nBagsLoaded).ToString("#,###");
                arr[11, j] = TBFlash_Utils.FormatTime(frs.Sum(x => x.nBagsLoaded > 0 ? x.time_bag_load : 0) * 60f);
                arr[12, j] = (frs.Sum(x => x.nFuelRequested)/1000).ToString("#,###");
                arr[13, j] = (frs.Sum(x => x.nFuelRefueled) / 1000).ToString("#,###");
                arr[14, j] = frs.Count(x => HasStatus(x.status, Flight.Status.Enroute)).ToString("#,###");
                arr[15, j] = frs.Count(x => HasStatus(x.status, Flight.Status.OnGround)).ToString("#,###");
                arr[16, j] = frs.Count(x => HasStatus(x.status, Flight.Status.DelayedArrival)).ToString("#,###");
                arr[17, j] = frs.Count(x => HasStatus(x.status, Flight.Status.Departed)).ToString("#,###");
                arr[18, j] = frs.Count(x => HasStatus(x.status, Flight.Status.DelayedDeparture)).ToString("#,###");
                arr[19, j] = frs.Count(x => HasStatus(x.status, Flight.Status.Canceled)).ToString("#,###");
                arr[20, j] = frs.Count(x => x.reason == Flight.StatusReason.AirportInvalid).ToString("#,###");
                arr[21, j] = frs.Count(x => x.reason == Flight.StatusReason.Weather).ToString("#,###");
                arr[22, j] = frs.Count(x => x.reason == Flight.StatusReason.Runway).ToString("#,###");
                arr[23, j] = frs.Count(x => x.reason == Flight.StatusReason.Gate).ToString("#,###");
                arr[24, j] = frs.Count(x => x.reason == Flight.StatusReason.Expired).ToString("#,###");
                arr[25, j] = frs.Count(x => x.reason == Flight.StatusReason.Reneged).ToString("#,###");
            }
            return arr;
        }

        private bool HasStatus(int totalStatus, Flight.Status status)
        {
            return(totalStatus & (int)status) == (int)status;
        }
    }
}
