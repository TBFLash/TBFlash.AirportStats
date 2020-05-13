using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using SimAirport.Logging;
using System.IO;

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

        private string GetLifetimeStats()
        {
            string str = string.Empty;
            GameLifetimeStats GLS = Game.current.lifetimeStats;
            str += "<h1>Flight Stats</h1>";
            str += $"On Time Flights: {GLS.flOnTime:N0}<br/>";
            str += $"Delayed Flights: {GLS.flDelays:N0}<br/>";
            str += $"Cancelled Flights: {GLS.flCancels:N0}<br/>";
            str += $"Reneged Flights: {GLS.flReneges:N0}<br/>";
            str += $"Landings: {GLS.Landings:N0}<br/>";
            str += $"Takeoffs: {GLS.Takeoffs:N0}<br/>";

            str += "<h1>Passenger Stats</h1>";
            str += $"Boarded Flight: {GLS.pBoarded:N0}<br/>";
            str += $"Missed Flight: {GLS.pMissed:N0}<br/>";

            str += "<h1>Fuel Stats</h1>";
            str += $"Fuel Requested: {GLS.fuelRequested / 1000:N0}L<br/>";
            str += $"Fuel Provided: {GLS.fuelfRefueled / 1000:N0}L<br/>";
            str += $"Planes Served Fuel: {GLS.planesServedFuel:N0}<br/>";

            str += "<h1>Luggage Stats</h1>";
            str += $"Bags Loaded: {GLS.pBagsLoaded:N0}<br/>";
            str += $"Bags Unloaded: {GLS.pBagsUnloaded:N0}<br/>";
            str += $"Luggage Success: {GLS.pBagSuccess:N0}<br/>";
            str += $"Lost Luggage: {GLS.pBagFail:N0}<br/>";
            str += $"Outdoor Baggage Loads: {GLS.outdoorBaggageLoads:N0}<br/>";

            str += "<h1>Revenue</h1>";
            str += $"Advertising: {GLS.mAdvertising:C0}<br/>";
            str += $"Fuel: {GLS.mFuelRev:C0}<br/>";
            str += $"Loans: {GLS.mLoans:C0}<br/>";
            str += $"Retail: {GLS.mRetailRev:C0}<br/>";
            str += $"Runway Usage: {GLS.mRwyUsageRev:C0}<br/>";
            str += $"Terminal Usage: {GLS.mTerminalUsageRev:C0}<br/>";
            str += $"Total Revenue: {GLS.mRev:C0}<br/>";

            str += "<h1>Expenses</h1>";
            str += $"Interest: {GLS.mInterest:C0}<br/>";
            str += $"Retail: {GLS.mRetailExpense:C0}<br/>";
            str += $"Staff Wages: {GLS.mStaffWages:C0}<br/>";
            str += $"Income Tax: {GLS.mIncomeTax:C0}<br/>";
            str += $"Property Tax: {GLS.mPropertyTax:C0}<br/>";
            str += $"Total Expenses: {GLS.mExpense:C0}<br/>";

            str += "<h1>Staff Stats</h1>";
            str += $"Hired: {GLS.sHires:N0}<br/>";
            str += $"Fired: {GLS.sFires:N0}<br/>";
            str += $"Fired Staff - Total hours hired: {GLS.sHours_FiredStaff:N0}<br/>";

            str += "<h1>Time Stats</h1>";
            str += $"Time Paused: {FormatTime(GLS.tPaused)}<br/>";
            str += $"Speed 1: {FormatTime(GLS.tSpeed1)}<br/>";
            str += $"Speed 2: {FormatTime(GLS.tSpeed2)}<br/>";
            str += $"Speed 3: {FormatTime(GLS.tSpeed3)}<br/>";
            str += $"Inactive: {FormatTime(GLS.tInactive)}<br/>";

            str += "<h1>Keyboard / Mouse Stats</h1>";
            str += $"Keyboard Clicks: {GLS.tInteractions:N0}<br/>";
            str += $"Left Mouse Clicks: {GLS.tClicks:N0}<br/>";
            str += $"Right Mouse Clicks: {GLS.tClicksAlt:N0}<br/>";

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
