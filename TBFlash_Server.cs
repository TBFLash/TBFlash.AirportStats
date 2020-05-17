using System;
using System.Net;
using SimAirport.Logging;
using System.IO;
using UnityEngine;

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
            // Get out the context object
            HttpListenerContext context = Listener.EndGetContext(result);
            // *** Immediately set up the next context
            Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), Listener);
            ReceiveWebRequest?.Invoke(context);
            ProcessRequest(context);
        }

        protected virtual void ProcessRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string str = request.RawUrl.TrimStart(new char[] { '/', '\\' });
            int index = str.IndexOf("?");
            if (index > 0)
            {
                str = str.Substring(0, index);
            }
            string responseString = string.Empty;
            if (str.Equals("Airlines", StringComparison.InvariantCultureIgnoreCase))
            {
                responseString += TBFlash_Utils.PageHead(TBFlash_Utils.PageTitles.ActiveAirlines);
                TBFlash_AllAirlineStats content = new TBFlash_AllAirlineStats();
                responseString += content.GetAllAirlineStats(true);
            }
            else if (str.Equals("AllAirlines", StringComparison.InvariantCultureIgnoreCase))
            {
                responseString += TBFlash_Utils.PageHead(TBFlash_Utils.PageTitles.AllAirlines);
                TBFlash_AllAirlineStats content = new TBFlash_AllAirlineStats();
                responseString += content.GetAllAirlineStats();
            }
            else if(str.Equals(string.Empty))
            {
                responseString += TBFlash_Utils.PageHead(TBFlash_Utils.PageTitles.AirportStats);
                TBFlash_LifetimeStats content = new TBFlash_LifetimeStats();
                responseString += content.GetLifetimeStats();
            }
            else
            {
                string newStr = str.Replace("%20", " ");
                Airline airline = AirlineManager.FindByName(newStr);
                if (airline != null)
                {
                    int day = int.TryParse(request.QueryString["day"], out int value) ? value : -1;
                    responseString += TBFlash_Utils.PageHead(airline, day);
                    if (day == -1)
                    {
                        TBFlash_AirlineStats content = new TBFlash_AirlineStats();
                        responseString += content.GetAirlineStats(airline);
                    }
                    else
                    {
                        TBFlash_AirlineDailyStats content = new TBFlash_AirlineDailyStats();
                        responseString += content.GetDailyStats(airline, day);
                    }
                }
                else
                {
                    responseString += TBFlash_Utils.PageHead(TBFlash_Utils.PageTitles.AirportStats);
                    TBFlash_LifetimeStats content = new TBFlash_LifetimeStats();
                    responseString += content.GetLifetimeStats();
                }
            }
            responseString += TBFlash_Utils.PageFooter();

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentType = "text/html";
            response.ContentLength64 = buffer.Length;
            Stream outputStream = response.OutputStream;
            outputStream.Write(buffer, 0, buffer.Length);
            outputStream.Close();
        }
    }
}
