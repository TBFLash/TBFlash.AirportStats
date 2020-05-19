using System;
using System.Collections.Generic;
using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal static class TBFlash_Utils
    {
        private const bool isTBFlashDebug = false;
        private const string version = "beta .8";

        internal enum PageTitles
        {
            AirportStats = 0,
            AllAirlines = 1,
            ActiveAirlines = 2
        }

        internal static List<string> Urls = new List<string>
        {
            i18n.Get("TBFlash.AirportStats.utils.url0"),
            i18n.Get("TBFlash.AirportStats.utils.url1"),
            i18n.Get("TBFlash.AirportStats.utils.url2")
        };

        internal static List<string> Values = new List<string>
        {
            i18n.Get("TBFlash.AirportStats.utils.value0"),
            i18n.Get("TBFlash.AirportStats.utils.value1"),
            i18n.Get("TBFlash.AirportStats.utils.value2")
        };

        internal static void TBFlashLogger(Log log)
        {
            if (isTBFlashDebug)
            {
                Game.Logger.Write(log);
            }
        }

        internal static string FormatTime(double seconds, bool showDays = true, bool showSeconds = true, bool timeonly = false)
        {
            if (seconds == 0.0)
            {
                return string.Empty;
            }
            double days = Math.Floor(seconds / 86400.0);
            double hrs = Math.Floor((seconds - (days * 86400.0)) / 3600.0);
            double mins = Math.Floor((seconds - (days * 86400) - (hrs * 3600.0)) / 60);
            double secs = Math.Floor(seconds - (days * 86400) - (hrs * 3600.0) - (mins * 60));
            string str;
            if (timeonly)
            {
                str = $"{hrs:0}:{mins:00}";
                str = showDays ? $"{days:0}:{str}" : str;
                return showSeconds ? $"{str}:{secs:00}" : str;
            }
            else
            {
                str = $"{hrs:0}h:{mins:00}m";
                str = showDays ? $"{days:0}d:{str}" : str;
                return showSeconds ? $"{str}:{secs:00}s" : str;
            }
        }

        internal static string PageHead(TBFlash_Utils.PageTitles PageTitle)
        {
            string str = Styles();

            str += $"<body><form><h1>{Values[(int)PageTitle]}&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            foreach (int i in Enum.GetValues(typeof(PageTitles)).Cast<PageTitles>().Where(x => x != PageTitle).ToList())
            {
                str += $"<input type=\"button\" class=\"button\" onclick=\"location.href=\'{Urls[i]}\';\" value=\"{Values[i]}\"> ";
            }
            str += "</h1></form>\n";

            return str;
        }

        internal static string PageHead(Airline airline, int day = -1)
        {
            const string selected = "selected";
            string goText = i18n.Get("TBFlash.AirportStats.utils.go");
            string airlineName = airline != null ? airline.name : i18n.Get("TBFlash.AirportStats.utils.dailyStats");
            string st = day != -1 ? "&nbsp;-&nbsp;" + i18n.Get("TBFlash.AirportStats.utils.day") + " " + day : string.Empty;
            string str = Styles() + $"<body><form><h1><a href=\"/{airlineName}\">{airlineName}</a>{st}&nbsp;&nbsp;&nbsp;&nbsp;";
            if (day > -1)
            {
                str += "\n<script type=\"text/javascript\">function goToNewPage() { var url =\'/" + airlineName + "\' + \'?Day=\' + document.getElementById(\'daySelector\').value; if (url != \'none\') {window.location = url;}}</script>";
                str += "<select id=\"daySelector\" class=\"select\">";
                for (int i = (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i <= GameTimer.Day; i++)
                {
                    str += $"<option {(i == day ? selected : string.Empty)} value=\'{i}\'>Day {i}</option>";
                }
                str += $"</select><input type=\"button\" class=\"button\" value=\"{goText}\" onclick=\"goToNewPage();\">&nbsp;&nbsp;";
            }
            foreach (int i in Enum.GetValues(typeof(PageTitles)))
            {
                str += $"<input type=\"button\" class=\"button\" onclick=\"location.href=\'{Urls[i]}\';\" value=\"{Values[i]}\"> ";
            }
            str += "</h1></form>\n";

            return str;
        }

        internal static string PageFooter()
        {
           return $"<p class=\"footer\">Version: {version}</br>Brought to you by TBFlash</p></body></html>";
        }

        private static string Styles()
        {
            string str = "<html>\n<head>\n\t<style>";
            str += "\n\t\ta { color: white; text-decoration: underline; } ";
            str += "\n\t\tbody { background-color: #384953; } ";
            str += "\n\t\t.button { background-image: linear-gradient(#22495F, #223747); color: white; border-radius: 2px; vertical-align: middle; } ";
            str += "\n\t\t.select { background-color: #223747; color: white; border-radius: 2px; vertical-align: middle; padding: 1px;} ";
            str += "\n\t\t.footer{ font-size: .5625em; color: white; }";
            str += "\n\t\th1 { font-size: 1.75em; text-align: center; color: white; margin-block-end: 0em; margin-block-start: 0em;}";
            str += "\n\t\ttd { text-align: right; font-size: .6875em; } ";
            str += "\n\t\tth { font-size: .8125em; }";
            str += "\n\t\ttable, td, th { border: 1px solid #6D7778; color: white;} ";
            str += "\n\t</style>";
            str += "\n</head>\n";
            return str;
        }
    }
}
