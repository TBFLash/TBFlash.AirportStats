using SimAirport.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace TBFlash.AirportStats
{
    internal class TBFlash_LifetimeStats
    {
        private readonly int arrayRows = 61;

        internal string GetLifetimeStats()
        {
            int counter = 1 + (GameTimer.Day <= 30 ? GameTimer.Day : 30);
            string[,] arr = LoadLifetimeArray();
            TBFlash_Utils.TBFlashLogger(Log.FromPool($"counter:{counter}").WithCodepoint());
            string lifetimeString = "<th>" + i18n.Get("TBFlash.AirportStats.utils.lifetime") + "</th>";
            string headerstring = lifetimeString;
            string day = i18n.Get("TBFlash.AirportStats.utils.day");
            for (int i = GameTimer.Day; i >= (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i--)
            {
                headerstring += $"<th><a href=\"Daily Stats?Day={i}\">{day} {i}</a></th>";
            }
            headerstring += "</tr>";

            string htmlCode = TBFlash_Utils.PageHead(TBFlash_Utils.PageTitles.AirportStats, true);
            htmlCode += $"<table><tr><th><a class=\"loadChart\" href=\"/chartdata?dataset=flightstats\" rel=\"#dialog\">{i18n.Get("TBFlash.AirportStats.LifetimeStats.header0")}</a></th>{headerstring}";
            for (int i = 0; i <= 6; i++)
            {
                htmlCode += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    htmlCode += $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }
           htmlCode += $"<tr><th><a class=\"loadChart\" href=\"/chartdata?dataset=paxstats\" rel=\"#dialog\">{i18n.Get("TBFlash.AirportStats.LifetimeStats.header1")}</a></th>{headerstring}";
            for (int i = 7; i <= 10; i++)
            {
                htmlCode += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    htmlCode += $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }
            htmlCode += $"<tr><th><a class=\"loadChart\" href=\"/chartdata?dataset=fuelstats\" rel=\"#dialog\">{i18n.Get("TBFlash.AirportStats.LifetimeStats.header2")}</a></th>{headerstring}";
            for (int i = 11; i <= 14; i++)
            {
                htmlCode += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    htmlCode += $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }
            htmlCode += $"<tr><th><a class=\"loadChart\" href=\"/chartdata?dataset=luggagestats\" rel=\"#dialog\">{i18n.Get("TBFlash.AirportStats.LifetimeStats.header3")}</a></th>{headerstring}";
            for (int i = 15; i <= 19; i++)
            {
                if (i != 17 && i != 19) {
                    htmlCode += "<tr>";
                    for (int j = 0; j <= counter; j++)
                    {
                        htmlCode += $"<td>{arr[i, j]}</td>";
                    }
                    htmlCode += "</tr>";
                }
            }
            htmlCode += $"<tr><th><a class=\"loadChart\" href=\"/chartdata?dataset=profits\" rel=\"#dialog\">{i18n.Get("TBFlash.AirportStats.LifetimeStats.header4")}</a></th>{headerstring}";
            for (int i = 20; i <= 33; i++)
            {
                htmlCode += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    htmlCode += $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }
            htmlCode += $"<tr><th><a class=\"loadChart\" href=\"/chartdata?dataset=profits\" rel=\"#dialog\">{i18n.Get("TBFlash.AirportStats.LifetimeStats.header5")}</a></th>{headerstring}";
            for (int i = 34; i <= 50; i++)
            {
                htmlCode += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    htmlCode += $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }
            htmlCode += $"<tr><th>{i18n.Get("TBFlash.AirportStats.LifetimeStats.header6")}</th>{lifetimeString}";
            for (int i = 51; i <= 52; i++)
            {
                htmlCode += "<tr>";
                for (int j = 0; j <= 1; j++)
                {
                    htmlCode += $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }
            htmlCode += $"<tr><th>{i18n.Get("TBFlash.AirportStats.LifetimeStats.header7")}</th>{lifetimeString}";
            for (int i = 53; i <= 57; i++)
            {
                htmlCode += "<tr>";
                for (int j = 0; j <= 1; j++)
                {
                    htmlCode += $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }
            htmlCode += $"<tr><th>{i18n.Get("TBFlash.AirportStats.LifetimeStats.header8")}</th>{lifetimeString}";
            for (int i = 58; i <= 60; i++)
            {
                htmlCode += "<tr>";
                for (int j = 0; j <= 1; j++)
                {
                    htmlCode += $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }
            htmlCode += "</table>";
            htmlCode += TBFlash_Utils.PageFooter();
            return htmlCode;
        }

        private string[,] LoadLifetimeArray()
        {
            int numdays = GameTimer.Day <= 30 ? GameTimer.Day : 30;
            GameLifetimeStats GLS = Game.current.lifetimeStats;
            string[,] arr = new string[arrayRows, numdays + 2];
            for(int i = 0; i<arrayRows; i++)
            {
                arr[i, 0] = i18n.Get($"TBFlash.AirportStats.LifetimeStats.stats{i}");
            }
            arr[1, 1] = GLS.flOnTime.ToString("#,###");
            arr[2, 1] = GLS.flDelays.ToString("#,###");
            arr[3, 1] = GLS.flCancels.ToString("#,###");
            arr[4, 1] = GLS.flReneges.ToString("#,###");
            arr[5, 1] = GLS.Landings.ToString("#,###");
            arr[6, 1] = GLS.Takeoffs.ToString("#,###");
            arr[9, 1] = GLS.pBoarded.ToString("#,###");
            arr[10, 1] = GLS.pMissed.ToString("#,###");
            arr[11, 1] = (GLS.fuelRequested / 1000).ToString("#,###");
            arr[12, 1] = (GLS.fuelfRefueled / 1000).ToString("#,###");
            arr[13, 1] = GLS.planesServedFuel.ToString("#,###");
            arr[15, 1] = GLS.pBagsLoaded.ToString("#,###");
            arr[16, 1] = GLS.pBagsUnloaded.ToString("#,###");
            arr[17, 1] = GLS.pBagSuccess.ToString("#,###");
            arr[18, 1] = GLS.pBagFail.ToString("#,###");
            arr[19, 1] = GLS.outdoorBaggageLoads.ToString("#,###");
            arr[20, 1] = GLS.mAdvertising.ToString("C0");
            arr[22, 1] = GLS.mLoans.ToString("C0");
            arr[23, 1] = GLS.mFuelRev.ToString("C0");
            arr[28, 1] = GLS.mRetailRev.ToString("C0");
            arr[29, 1] = GLS.mRwyUsageRev.ToString("C0");
            arr[30, 1] = GLS.mTerminalUsageRev.ToString("C0");
            arr[33, 1] = GLS.mRev.ToString("C0");
            arr[35, 1] = GLS.mInterest.ToString("C0");
            arr[43, 1] = GLS.mRetailExpense.ToString("C0");
            arr[44, 1] = GLS.mStaffWages.ToString("C0");
            arr[46, 1] = GLS.mIncomeTax.ToString("C0");
            arr[47, 1] = GLS.mPropertyTax.ToString("C0");
            arr[50, 1] = GLS.mExpense.ToString("C0");
            arr[51, 1] = GLS.sHires.ToString("#,###");
            arr[52, 1] = GLS.sFires.ToString("#,###");
            arr[53, 1] = TBFlash_Utils.FormatTime(GLS.tPaused);
            arr[54, 1] = TBFlash_Utils.FormatTime(GLS.tSpeed1);
            arr[55, 1] = TBFlash_Utils.FormatTime(GLS.tSpeed2);
            arr[56, 1] = TBFlash_Utils.FormatTime(GLS.tSpeed3);
            arr[57, 1] = TBFlash_Utils.FormatTime(GLS.tInactive);
            arr[58, 1] = GLS.tInteractions.ToString("#,###");
            arr[59, 1] = GLS.tClicks.ToString("#,###");
            arr[60, 1] = GLS.tClicksAlt.ToString("#,###");

            int j = 1;
            for (int i = GameTimer.Day; i >= (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i--)
            {
                j++;
                if (!Game.current.GameReports.TryGetValue(i, out GamedayReportingData GRD))
                {
                    break;
                }
                IEnumerable<FlightRecord> FlightRecords = Game.current.flightRecords.GetForDay(i - 1);

                arr[0, j] = GRD.FlightsCount.ToString("#,###");
                arr[1, j] = FlightRecords.Count(x => TBFlash_Utils.HasStatus(x.status, Flight.Status.Departed) && !TBFlash_Utils.HasStatus(x.status, Flight.Status.DelayedDeparture)).ToString("#,###");
                arr[2, j] = FlightRecords.Count(x => TBFlash_Utils.HasStatus(x.status, Flight.Status.DelayedDeparture)).ToString("#,###");
                arr[3, j] = GRD.FlightsCanceled.ToString("#,###");
                arr[7, j] = GRD.NumArrivalPax.ToString("#,###");
                arr[8, j] = GRD.NumConnectPax.ToString("#,###");
                arr[9, j] = GRD.BoardedFlight.ToString("#,###");
                arr[10, j] = GRD.MissedFlight.ToString("#,###");
                arr[11, j] = (FlightRecords.Sum(x => x.nFuelRequested) / 1000).ToString("#,###");
                arr[12, j] = (FlightRecords.Sum(x => x.nFuelRefueled) / 1000).ToString("#,###");
                arr[13, j] = (FlightRecords.Count(x => x.nFuelRefueled > 0)).ToString("#,###");
                arr[14, j] = GRD.FuelFailures.ToString("#,###");
                arr[15, j] = FlightRecords.Sum(x=>x.nBagsLoaded).ToString("#,###");
                arr[16, j] = FlightRecords.Sum(x => x.nBagsUnloaded).ToString("#,###");
                arr[18, j] = FlightRecords.Sum(x => TBFlash_Utils.HasStatus(x.status, Flight.Status.Departed) ? x.nDepartingBags - x.nBagsLoaded : 0).ToString("#,###");
                arr[20, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Advertising, true);
                arr[21, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Airline_Fees, true);
                arr[22, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Bank, true);
                arr[23, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Fuel, true);
                arr[24, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Grant, true);
                arr[25, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Materials, true);
                arr[26, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Parking, true);
                arr[27, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Research, true);
                arr[28, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Retail, true);
                arr[29, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Runway_Fees, true);
                arr[30, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Terminal_Fees, true);
                arr[31, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Staff, true);
                arr[32, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Undefined, true);
                arr[34, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Airline_Fees, false);
                arr[35, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Bank, false);
                arr[36, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Fuel, false);
                arr[37, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Grant, false);
                arr[38, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Infrastructure, false);
                arr[39, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Land_Purchase, false);
                arr[40, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Maintenance, false);
                arr[41, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Materials, false);
                arr[42, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Research, false);
                arr[43, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Retail, false);
                arr[44, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Staff, false);
                arr[45, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Taxes, false);
                arr[48, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Transportation, false);
                arr[49, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Undefined, false);
            }
            return arr;
        }

        private string GetDailyMoneyTotal(GamedayReportingData GRD, GamedayReportingData.MoneyCategory category, bool positive)
        {
            double num = 0.0;
            if (!GRD.CategorizedCashChanges.ContainsKey(category))
            {
                return string.Empty;
            }
            foreach (double num2 in GRD.CategorizedCashChanges[category].Values)
            {
                if ((positive && num2 >= 0.0) || (!positive && num2 <= 0.0))
                {
                    num += num2;
                }
            }
            return num != 0.0 ? num.ToString("C0") : string.Empty;
        }
    }
}
