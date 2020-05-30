using System.Collections.Generic;
using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class TBFlash_AirlineStats
    {
        private readonly int arrayRows = 25;

        internal string GetAirlineStats(Airline airline)
        {
            int counter = 1 + (GameTimer.Day <= 30 ? GameTimer.Day : 30);
            TBFlash_Utils.TBFlashLogger(Log.FromPool($"counter:{counter}").WithCodepoint());
            string[,] arr = LoadArray(airline);
            string day = i18n.Get("TBFlash.AirportStats.utils.day");
            string headerstring = string.Empty;
            for (int i = GameTimer.Day; i >= (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i--)
            {
                headerstring += $"<th><a href=\"/{airline.name}?Day={i}\">{day} {i}</a></th>";
            }
            headerstring += "</tr>";
            string htmlCode = TBFlash_Utils.PageHead(airline, -1);
            htmlCode += $"<table><tr><th><a class=\"loadChart\" href=\"/chartdata?dataset=flightstats&airline={airline.name}\" rel=\"#dialog\">{i18n.Get("TBFlash.AirportStats.LifetimeStats.header0")}</a></th>{headerstring}";
            for (int i = 0; i < 12; i++)
            {
                htmlCode += "<tr>";
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    htmlCode += $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }

            htmlCode += $"<tr><th><a class=\"loadChart\" href=\"/chartdata?dataset=paxstats&airline={airline.name}\" rel=\"#dialog\">{i18n.Get("TBFlash.AirportStats.LifetimeStats.header1")}</a></th>{headerstring}";
            for (int i = 12; i < 19; i++)
            {
                htmlCode += "<tr>";
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    htmlCode += $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }

            htmlCode += $"<tr><th><a class=\"loadChart\" href=\"/chartdata?dataset=fuelstats&airline={airline.name}\" rel=\"#dialog\">{i18n.Get("TBFlash.AirportStats.LifetimeStats.header2")}</a></th>{headerstring}";
            for (int i = 19; i < 21; i++)
            {
                htmlCode += "<tr>";
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    htmlCode += $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }

            htmlCode += $"<tr><th><a class=\"loadChart\" href=\"/chartdata?dataset=luggagestats&airline={airline.name}\" rel=\"#dialog\">{i18n.Get("TBFlash.AirportStats.LifetimeStats.header3")}</a></th>{headerstring}";
            for (int i = 21; i < 25; i++)
            {
                htmlCode += "<tr>";
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    htmlCode += $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }

            htmlCode += "</table>" + TBFlash_Utils.PageFooter();
            return htmlCode;
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
                IEnumerable<FlightRecord> flightRecords = Game.current.flightRecords.GetForDay(i-1).Where(x => x.airline == airline.name);
                arr[0, j] = flightRecords.Any() ? flightRecords.Count().ToString("#,###"):"None";
                arr[1, j] = flightRecords.Count(x => TBFlash_Utils.HasStatus(x.status, Flight.Status.DelayedArrival)).ToString("#,###");
                arr[2, j] = flightRecords.Count(x => TBFlash_Utils.HasStatus(x.status, Flight.Status.RequiresCrew)).ToString("#,###");
                arr[3, j] = flightRecords.Count(x => TBFlash_Utils.HasStatus(x.status, Flight.Status.Departed) && !TBFlash_Utils.HasStatus(x.status, Flight.Status.DelayedDeparture)).ToString("#,###");
                arr[4, j] = flightRecords.Count(x => TBFlash_Utils.HasStatus(x.status, Flight.Status.DelayedDeparture)).ToString("#,###");
                arr[5, j] = flightRecords.Count(x => TBFlash_Utils.HasStatus(x.status, Flight.Status.Canceled)).ToString("#,###");
                arr[6, j] = flightRecords.Count(x => x.reason == Flight.StatusReason.AirportInvalid).ToString("#,###");
                arr[7, j] = flightRecords.Count(x => x.reason == Flight.StatusReason.Weather).ToString("#,###");
                arr[8, j] = flightRecords.Count(x => x.reason == Flight.StatusReason.Runway).ToString("#,###");
                arr[9, j] = flightRecords.Count(x => x.reason == Flight.StatusReason.Gate).ToString("#,###");
                arr[10, j] = flightRecords.Count(x => x.reason == Flight.StatusReason.Expired).ToString("#,###");
                arr[11, j] = flightRecords.Count(x => x.reason == Flight.StatusReason.Reneged).ToString("#,###");
                arr[12, j] = flightRecords.Sum(x => x.nArriving).ToString("#,###");
                arr[13, j] = flightRecords.Sum(x => x.nDeparting).ToString("#,###");
                arr[14, j] = flightRecords.Sum(x => x.nCheckedIn).ToString("#,###");
                arr[15, j] = flightRecords.Sum(x => x.nBoarded).ToString("#,###");
                arr[16, j] = flightRecords.Sum(x => TBFlash_Utils.HasStatus(x.status, Flight.Status.Departed) ? x.nDeparting - x.nBoarded : 0).ToString("#,###");
                arr[17, j] = TBFlash_Utils.FormatTime(flightRecords.Sum(x => x.time_deplaning) * 60f);
                arr[18, j] = TBFlash_Utils.FormatTime(flightRecords.Sum(x => x.nBoarded > 0 ? x.time_boarding : 0) * 60f);
                arr[19, j] = (flightRecords.Sum(x => x.nFuelRequested) / 1000).ToString("#,###");
                arr[20, j] = (flightRecords.Sum(x => x.nFuelRefueled) / 1000).ToString("#,###");
                arr[21, j] = flightRecords.Sum(x => x.nBagsUnloaded).ToString("#,###");
                arr[22, j] = flightRecords.Sum(x => x.nBagsLoaded).ToString("#,###");
                arr[23, j] = flightRecords.Sum(x => TBFlash_Utils.HasStatus(x.status, Flight.Status.Departed) ? x.nDepartingBags - x.nBagsLoaded : 0).ToString("#,###");
                arr[24, j] = TBFlash_Utils.FormatTime(flightRecords.Sum(x => x.nBagsLoaded > 0 ? x.time_bag_load : 0) * 60f);
            }
            return arr;
        }
    }
}
