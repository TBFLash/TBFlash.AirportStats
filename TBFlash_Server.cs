using System;
using System.Net;
using SimAirport.Logging;
using System.IO;
using UnityEngine;
using System.Reflection;
using System.Resources;

namespace TBFlash.AirportStats
{
    public delegate void delReceiveWebRequest(HttpListenerContext Context);

    public class TBFlash_Server
    {
        protected HttpListener Listener;
        protected bool isStarted = false;
        private readonly string URLBase = "http://localhost:2198/";
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
            TBFlash_Utils.TBFlashLogger(Log.FromPool("Listener Started").WithCodepoint());
            isStarted = true;
            IAsyncResult result = Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), Listener);
            Application.OpenURL(URLBase);
        }

        public void Stop()
        {
            Listener?.Close();
            Listener = null;
            isStarted = false;
            TBFlash_Utils.TBFlashLogger(Log.FromPool("Listener Stopped").WithCodepoint());
        }

        protected void WebRequestCallback(IAsyncResult result)
        {
            if (Listener == null)
                return;
            HttpListenerContext context = Listener.EndGetContext(result);
            Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), Listener);
            ReceiveWebRequest?.Invoke(context);
            ProcessRequest(context);
        }

        protected virtual void ProcessRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            response.ContentType = "text/html";

            string requestedPage = request.RawUrl.TrimStart(new char[] { '/', '\\' });
            TBFlash_Utils.TBFlashLogger(Log.FromPool(requestedPage).WithCodepoint());

            int index = requestedPage.IndexOf("?");
            if (index > 0)
            {
                requestedPage = requestedPage.Substring(0, index);
            }
            requestedPage = requestedPage.Replace("%20", " ");
            int day = int.TryParse(request.QueryString["day"], out int value) ? value : -1;
            string aircraft = request.QueryString["aircraft"];
            string dataset = request.QueryString["dataset"];
            string responseString = string.Empty;
            switch (requestedPage.ToUpperInvariant())
            {
                case "AIRLINES":
                    responseString += new TBFlash_AllAirlineStats().GetAllAirlineStats(true);
                    break;
                case "AIRCRAFTSTATS":
                    if (!string.IsNullOrEmpty(aircraft) && AircraftConfigManager.FindByAnyName(aircraft, false) != null)
                    {
                        responseString += new TBFlash_AircraftStats().GetAircraftStats(AircraftConfigManager.FindByAnyName(aircraft, false));
                    }
                    break;
                case "ALLAIRLINES":
                    responseString += new TBFlash_AllAirlineStats().GetAllAirlineStats();
                    break;
                case "DAILY STATS":
                    responseString += day == -1
                        ? new TBFlash_LifetimeStats().GetLifetimeStats()
                        : new TBFlash_AirlineDailyStats().GetDailyStats(null, day);
                    break;
                case "FAVICON.ICO":
                    break;
                case "AIRPORTSTATS.JS":
                    responseString += new ResourceManager("TBFlash.AirportStats.Resource1", Assembly.GetExecutingAssembly()).GetString("AirportStats");
                    response.ContentType = "text/javascript";
                    break;
                case "JQUERY.MIN.JS":
                    responseString += new ResourceManager("TBFlash.AirportStats.Resource1", Assembly.GetExecutingAssembly()).GetString("jquery_min");
                    response.ContentType = "text/javascript";
                    break;
                case "JQUERY-UI.MIN.JS":
                    responseString += new ResourceManager("TBFlash.AirportStats.Resource1", Assembly.GetExecutingAssembly()).GetString("jquery_ui_min");
                    response.ContentType = "text/javascript";
                    break;
                case "JQUERY-UI.CSS":
                    responseString += new ResourceManager("TBFlash.AirportStats.Resource1", Assembly.GetExecutingAssembly()).GetString("jquery_ui");
                    response.ContentType = "text/css";
                    break;
                case "CHART.MIN.JS":
                    responseString += new ResourceManager("TBFlash.AirportStats.Resource1", Assembly.GetExecutingAssembly()).GetString("Chart_min1");
                    response.ContentType = "text/javascript";
                    break;
                case "CHART.MIN.CSS":
                    responseString += new ResourceManager("TBFlash.AirportStats.Resource1", Assembly.GetExecutingAssembly()).GetString("Chart_min");
                    response.ContentType = "text/css";
                    break;
                case "CHARTDATA":
                    responseString += new TBFlash_ChartData().GetChartData(dataset);
                    response.ContentType = "application/json";
                    break;
                default:
                    Airline airline = AirlineManager.FindByName(requestedPage);
                    if (airline != null)
                    {
                        responseString += day == -1
                            ? new TBFlash_AirlineStats().GetAirlineStats(airline)
                            : new TBFlash_AirlineDailyStats().GetDailyStats(airline, day);
                    }
                    else
                    {
                        responseString += new TBFlash_LifetimeStats().GetLifetimeStats();
                    }
                    break;
            }

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream outputStream = response.OutputStream;
            outputStream.Write(buffer, 0, buffer.Length);
            outputStream.Close();
        }
    }
}
