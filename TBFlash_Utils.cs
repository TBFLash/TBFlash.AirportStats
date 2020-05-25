using System;
using System.Collections.Generic;
using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal static class TBFlash_Utils
    {
        private const bool isTBFlashDebug = false;
        private const string version = "beta .9";

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

        internal static string FormatTime(double seconds, bool timeOfDay = false)
        {
            if (seconds == 0.0)
            {
                return string.Empty;
            }
            if (timeOfDay)
            {
                return DateTime.MinValue.AddSeconds(seconds).ToString("t");
            }
            else
            {
                return new TimeSpan(0, 0, (int)seconds).ToString("g");
            }
        }

        internal static string PageHead(TBFlash_Utils.PageTitles PageTitle, bool includeJQuery = false)
        {
            string str = includeJQuery ? HeadTag(true, true) : HeadTag();

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
            string str = day != -1 ? HeadTag(true, true) : HeadTag();
            str += $"<body><form><h1><a href=\"/{airlineName}\">{airlineName}</a>{st}&nbsp;&nbsp;&nbsp;&nbsp;";
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
            string ver = i18n.Get("TBFlash.AirportStats.utils.version");
            string broughtBy = i18n.Get("TBFlash.AirportStats.utils.broughtBy");
            return $"<p class=\"footer\">{ver}: {version}</br>{broughtBy}</p></body></html>";
        }

        internal static string HeadTag(bool includeStyles = true, bool includeJQuery = false)
        {
            string str = "<html>\n<head><meta charset=\"UTF-8\"><link rel=\"icon\" href=\"data:;base64,=\">\n";
            str += includeStyles ? Styles() : string.Empty;
            str += includeJQuery ? JQuery() : string.Empty;
            str += "\n</head>\n";
            return str;
        }

        private static string JQuery()
        {
            string str = "<script type=\"text/javascript\" src=\"/jquery.min.js\"></script>\n";
            str += "<script type=\"text/javascript\" src=\"/jquery-ui.min.js\"></script>\n";
            str += "<script type=\"text/javascript\" src=\"/Chart.min.js\"></script>\n";
            str += "<link rel=\"stylesheet\" type=\"text/css\" href=\"/jquery-ui.css\">\n";
            str += "<link rel=\"stylesheet\" type=\"text/css\" href=\"/Chart.min.css\">\n";
            str += "<script type=\"text/javascript\" src=\"/AirportStats.js\"></script>\n";
            return str;
        }

        private static string Styles()
        {
            string str = "\n\t<style>";

            str += "\n\t\t.button { background-image: linear-gradient(#22495F, #223747); color: white; border-radius: 2px; vertical-align: middle; } ";
            str += "\n\t\t.select { background-color: #223747; color: lightgray; border-radius: 2px; vertical-align: middle; padding: 1px;} ";
            str += "\n\t\t.footer{ font-size: .5625em; color: lightgray; }";
            str += "\n\t\t.ui-dialog { border-radius: 8px !important; }";
            str += "\n\t\t.ui-widget-content { background: #223747 !important; }";
            str += "\n\t\t.ui-widget-header { background: #b8b8b8 !important; border: #888888 !important; }";
            str += "\n\t\t.ui-button .ui-icon { background-image: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAsAAAALCAYAAACprHcmAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAACOSURBVChTjY3RDYMwEENDJ4B92lH6AUtkm1bqKnSfdgPwu8Qh4gtLlnO552TIOaerutUc5U1+xXSImXv2Df7VnGUXSGYUe8PvmsgFgyj2hhf5XLC4Z99gdC6gBqIe/sj9i4h5LccDBnyWY6j/4S5HwXAPfmW+Ji0KDZ5qAjzKMdKF2Bv+y4Ns0GLmXvuUdkw7G3tVAShfAAAAAElFTkSuQmCC') !important; } ";
            str += "\n\t\t.ui-icon-closethick { background-position: 0px 0px !important;}";
            str += "\n\t\t.ui-icon { width: 11px !important; height: 11px !important; margin-top: -6px !important; margin-left: -6px !important; } ";
            str += "\n\t\ta { color: ivory; text-decoration: underline; } ";
            str += "\n\t\tbody { background-color: #384953; } ";
            str += "\n\t\th1 { font-size: 1.75em; text-align: center; color: lightgray; margin-block-end: 0em; margin-block-start: 0em;}";
            str += "\n\t\th2 { font-size: 1.25em; text-align: center; color: lightgray; margin-block-end: 0em; margin-block-start: 0em;}";
            str += "\n\t\ttd { text-align: right; font-size: .6875em; white-space: nowrap; } ";
            str += "\n\t\tth { font-size: .8125em; }";
            str += "\n\t\ttable, td, th { border: 1px solid #6D7778; color: lightgray;} ";
            str += "\n\t</style>";
            return str;
        }
    }
}
