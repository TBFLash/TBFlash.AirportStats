using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using SimAirport.Logging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;
using UnityEngine;

namespace TBFlash.AirportStats
{
    public delegate void delReceiveWebRequest(HttpListenerContext Context);

    public class TBFlash_Server
    {
        protected HttpListener Listener;
        protected bool isStarted = false;
        private readonly string URLBase = "http://localhost:2198/";
        private readonly bool isTBFlashDebug = true;
        public event delReceiveWebRequest ReceiveWebRequest;

        public void Start()
        {
            if (isStarted)
            {
                return;
            }
            if (Listener == null)
            {
                Listener = new HttpListener();
            }
            Listener.Prefixes.Add(URLBase);
            Listener.Start();
            TBFlashLogger(Log.FromPool("Listener Started").WithCodepoint());
            isStarted = true;
            IAsyncResult result = Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), Listener);
            Application.OpenURL(URLBase);
        }

        protected void WebRequestCallback(IAsyncResult result)

        {
            if (Listener == null)
                return;
            // Get out the context object
            HttpListenerContext context = Listener.EndGetContext(result);
            // *** Immediately set up the next context
            Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), Listener);
            ReceiveWebRequest?.Invoke(context);
            ProcessRequest(context);
        }

        /// <summary>
        /// Overridable method that can be used to implement a custom hnandler
        /// </summary>
        /// <param name="Context"></param>
        protected virtual void ProcessRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string str = request.RawUrl.TrimStart(new char[] { '/', '\\' });
            string responseString = "<HTML><BODY>";
            if (string.IsNullOrEmpty(str))
            {
                responseString += GetLifetimeStats();
            }
            else if(Int32.TryParse(str,out int x))
            {
                responseString += $"I have an int - {x}";
            }
            else
            {
                responseString += "Error";
            }
            responseString += "</BODY></HTML>";

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;
            Stream outputStream = response.OutputStream;
            outputStream.Write(buffer, 0, buffer.Length);
            outputStream.Close();
        }

        private string[,] LoadArray()
        {
            GameLifetimeStats GLS = Game.current.lifetimeStats;
            int numdays = GameTimer.Day <= 30 ? GameTimer.Day : 30;
            TBFlashLogger(Log.FromPool($"GameTimer:{GameTimer.Day}, numdays:{numdays}").WithCodepoint());

            string[,] arr = new string[40,numdays+2];

            arr[0,0] = i18n.Get("TBFlash.AirportStats.FS1");
            arr[1,0] = i18n.Get("TBFlash.AirportStats.FS2");
            arr[2,0] = i18n.Get("TBFlash.AirportStats.FS3");
            arr[3,0] = i18n.Get("TBFlash.AirportStats.FS4");
            arr[4,0] = i18n.Get("TBFlash.AirportStats.FS5");
            arr[5,0] = i18n.Get("TBFlash.AirportStats.FS6");
            arr[6,0] = i18n.Get("TBFlash.AirportStats.PS1");
            arr[7,0] = i18n.Get("TBFlash.AirportStats.PS2");
            arr[8,0] = i18n.Get("TBFlash.AirportStats.FuS1");
            arr[9,0] = i18n.Get("TBFlash.AirportStats.FuS2");
            arr[10,0] = i18n.Get("TBFlash.AirportStats.FuS3");
            arr[11,0] = i18n.Get("TBFlash.AirportStats.LS1");
            arr[12,0] = i18n.Get("TBFlash.AirportStats.LS2");
            arr[13,0] = i18n.Get("TBFlash.AirportStats.LS3");
            arr[14,0] = i18n.Get("TBFlash.AirportStats.LS4");
            arr[15,0] = i18n.Get("TBFlash.AirportStats.LS5");
            arr[16,0] = i18n.Get("TBFlash.AirportStats.Rev1");
            arr[17,0] = i18n.Get("TBFlash.AirportStats.Rev2");
            arr[18,0] = i18n.Get("TBFlash.AirportStats.Rev3");
            arr[19,0] = i18n.Get("TBFlash.AirportStats.Rev4");
            arr[20,0] = i18n.Get("TBFlash.AirportStats.Rev5");
            arr[21,0] = i18n.Get("TBFlash.AirportStats.Rev6");
            arr[22,0] = i18n.Get("TBFlash.AirportStats.Rev7");
            arr[23,0] = i18n.Get("TBFlash.AirportStats.Exp1");
            arr[24,0] = i18n.Get("TBFlash.AirportStats.Exp2");
            arr[25,0] = i18n.Get("TBFlash.AirportStats.Exp3");
            arr[26,0] = i18n.Get("TBFlash.AirportStats.Exp4");
            arr[27,0] = i18n.Get("TBFlash.AirportStats.Exp5");
            arr[28,0] = i18n.Get("TBFlash.AirportStats.Exp6");
            arr[29,0] = i18n.Get("TBFlash.AirportStats.SS1");
            arr[30,0] = i18n.Get("TBFlash.AirportStats.SS2");
            arr[31,0] = i18n.Get("TBFlash.AirportStats.SS3");
            arr[32,0] = i18n.Get("TBFlash.AirportStats.TS1");
            arr[33,0] = i18n.Get("TBFlash.AirportStats.TS2");
            arr[34,0] = i18n.Get("TBFlash.AirportStats.TS3");
            arr[35,0] = i18n.Get("TBFlash.AirportStats.TS4");
            arr[36,0] = i18n.Get("TBFlash.AirportStats.TS5");
            arr[37,0] = i18n.Get("TBFlash.AirportStats.KS1");
            arr[38,0] = i18n.Get("TBFlash.AirportStats.KS2");
            arr[39,0] = i18n.Get("TBFlash.AirportStats.KS3");

            arr[0, 1] = GLS.flOnTime.ToString("N0");
            arr[1, 1] = GLS.flDelays.ToString("N0");
            arr[2, 1] = GLS.flCancels.ToString("N0");
            arr[3, 1] = GLS.flReneges.ToString("N0");
            arr[4, 1] = GLS.Landings.ToString("N0");
            arr[5, 1] = GLS.Takeoffs.ToString("N0");
            arr[6, 1] = GLS.pBoarded.ToString("N0");
            arr[7, 1] = GLS.pMissed.ToString("N0");
            arr[8, 1] = (GLS.fuelRequested / 1000).ToString("N0");
            arr[9, 1] = (GLS.fuelfRefueled / 1000).ToString("N0");
            arr[10, 1] = GLS.planesServedFuel.ToString("N0");
            arr[11, 1] = GLS.pBagsLoaded.ToString("N0");
            arr[12, 1] = GLS.pBagsUnloaded.ToString("N0");
            arr[13, 1] = GLS.pBagSuccess.ToString("N0");
            arr[14, 1] = GLS.pBagFail.ToString("N0");
            arr[15, 1] = GLS.outdoorBaggageLoads.ToString("N0");
            arr[16, 1] = GLS.mAdvertising.ToString("C0");
            arr[17, 1] = GLS.mFuelRev.ToString("C0");
            arr[18, 1] = GLS.mLoans.ToString("C0");
            arr[19, 1] = GLS.mRetailRev.ToString("C0");
            arr[20, 1] = GLS.mRwyUsageRev.ToString("C0");
            arr[21, 1] = GLS.mTerminalUsageRev.ToString("C0");
            arr[22, 1] = GLS.mRev.ToString("C0");
            arr[23, 1] = GLS.mInterest.ToString("C0");
            arr[24, 1] = GLS.mRetailExpense.ToString("C0");
            arr[25, 1] = GLS.mStaffWages.ToString("C0");
            arr[26, 1] = GLS.mIncomeTax.ToString("C0");
            arr[27, 1] = GLS.mPropertyTax.ToString("C0");
            arr[28, 1] = GLS.mExpense.ToString("C0");
            arr[29, 1] = GLS.sHires.ToString("N0");
            arr[30, 1] = GLS.sFires.ToString("N0");
            arr[31, 1] = GLS.sHours_FiredStaff.ToString("N0");
            arr[32, 1] = FormatTime(GLS.tPaused);
            arr[33, 1] = FormatTime(GLS.tSpeed1);
            arr[34, 1] = FormatTime(GLS.tSpeed2);
            arr[35, 1] = FormatTime(GLS.tSpeed3);
            arr[36, 1] = FormatTime(GLS.tInactive);
            arr[37, 1] = GLS.tInteractions.ToString("N0");
            arr[38, 1] = GLS.tClicks.ToString("N0");
            arr[39, 1] = GLS.tClicksAlt.ToString("N0");

            int j = 1;
            int minimumday = GameTimer.Day > 30 ? GameTimer.Day - 29 : 1;
            for (int i = GameTimer.Day; i >= (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i--)
            {
                j++;
                TBFlashLogger(Log.FromPool($"i:{i},j:{j}").WithCodepoint());

                if (!Game.current.GameReports.TryGetValue(i, out GamedayReportingData GRD))
                {
                    break;
                }
                arr[0, j] = GRD.FlightsCount.ToString("N0"); //This is flights scheduled, not flights on time
                arr[1, j] = GRD.FlightsDelayed.ToString("N0");
                arr[2, j] = GRD.FlightsCanceled.ToString("N0");

                arr[6, j] = GRD.BoardedFlight.ToString("N0");
                arr[7, j] = GRD.MissedFlight.ToString("N0");

                arr[13, j] = GRD.LuggageSucceeded.ToString("N0");
                arr[14, j] = GRD.LuggageLost.ToString("N0");

                arr[16, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Advertising, true);
                arr[17, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Fuel, true);

                arr[19, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Retail, true);
                arr[20, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Runway_Fees, true);
                arr[21, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Terminal_Fees, true);

                arr[25, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Staff, false);

                //  arr[, j] = GRD.FuelFailures.ToString("N0");
                //  arr[, j] = GRD.NumArrivalPax.ToString("N0");
                //  arr[, j] = GRD.NumConnectPax.ToString("N0");
                //  arr[, j] = GRD.AirlineInterestLevels.ToString("N0");
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
            return num.ToString("C0");
        }

        private string GetLifetimeStats()
        {
            int counter = 1 + (GameTimer.Day <= 30 ? GameTimer.Day : 30);
            int minimumday = GameTimer.Day > 30 ? GameTimer.Day - 29 : 1;
            string[,] arr = LoadArray();
            TBFlashLogger(Log.FromPool($"counter:{counter}").WithCodepoint());

            string headerstring = "<td>Lifetime</td>";
            for (int i = GameTimer.Day; i >= (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i--)
            {
                headerstring += $"<td>Day{i}</td>";
            }
            headerstring += "</tr>";

            string str = $"<table border=\"1\"><tr><th>{i18n.Get("TBFlash.AirportStats.FS0")}</th>{headerstring}";
            for (int i = 0; i <= 5; i++)
            {
                str += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    str += $"<td>{arr[i,j]}</td>";
                }
                str += "</tr>";
            }
            str += $"<tr><th>{i18n.Get("TBFlash.AirportStats.PS0")}</th>{headerstring}";
            for (int i = 6; i <= 7; i++)
            {
                str += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    str += $"<td>{arr[i, j]}</td>";
                }
                str += "</tr>";
            }
            str += $"<tr><th>{i18n.Get("TBFlash.AirportStats.FuS0")}</th>{headerstring}";
            for (int i = 8; i <= 10; i++)
            {
                str += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    str += $"<td>{arr[i, j]}</td>";
                }
                str += "</tr>";
            }
            str += $"<tr><th>{i18n.Get("TBFlash.AirportStats.LS0")}</th>{headerstring}";
            for (int i = 11; i <= 15; i++)
            {
                str += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    str += $"<td>{arr[i, j]}</td>";
                }
                str += "</tr>";
            }
            str += $"<tr><th>{i18n.Get("TBFlash.AirportStats.Rev0")}</th>{headerstring}";
            for (int i = 16; i <= 22; i++)
            {
                str += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    str += $"<td>{arr[i, j]}</td>";
                }
                str += "</tr>";
            }
            str += $"<tr><th>{i18n.Get("TBFlash.AirportStats.Exp0")}</th>{headerstring}";
            for (int i = 23; i <= 28; i++)
            {
                str += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    str += $"<td>{arr[i, j]}</td>";
                }
                str += "</tr>";
            }
            str += $"<tr><th>{i18n.Get("TBFlash.AirportStats.SS0")}</th>{headerstring}";
            for (int i = 29; i <= 31; i++)
            {
                str += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    str += $"<td>{arr[i, j]}</td>";
                }
                str += "</tr>";
            }
            str += $"<tr><th>{i18n.Get("TBFlash.AirportStats.TS0")}</th>{headerstring}";
            for (int i = 32; i <= 36; i++)
            {
                str += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    str += $"<td>{arr[i, j]}</td>";
                }
                str += "</tr>";
            }
            str += $"<tr><th>{i18n.Get("TBFlash.AirportStats.KS0")}</th>{headerstring}";
            for (int i = 37; i <= 39; i++)
            {
                str += "<tr>";
                for (int j = 0; j <= counter; j++)
                {
                    str += $"<td>{arr[i, j]}</td>";
                }
                str += "</tr>";
            }

            str += "</table>";
            return str;
        }

        private string FormatTime(double seconds)
        {
            double days = Math.Floor(seconds / 86400.0);
            double hrs = Math.Floor((seconds - (days * 86400.0)) / 3600.0);
            double mins = Math.Floor((seconds - (days * 86400) - (hrs * 3600.0)) / 60);
            double secs = Math.Floor(seconds - (days * 86400) - (hrs * 3600.0) - (mins * 60));
            return $"{days}d:{hrs}h:{mins}m:{secs}s";
        }

        public void Stop()
        {
            Listener?.Close();
            Listener = null;
            isStarted = false;
            TBFlashLogger(Log.FromPool("Listener Stopped").WithCodepoint());
        }

        private void TBFlashLogger(Log log)
        {
            if (isTBFlashDebug)
            {
                Game.Logger.Write(log);
            }
        }
    }
}
