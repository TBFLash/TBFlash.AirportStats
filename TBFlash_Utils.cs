using System;
using System.Collections.Generic;
using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal static class TBFlash_Utils
    {
        private const bool isTBFlashDebug = false;
        private const string version = "1.1";

        internal enum PageTitles
        {
            AirportStats = 0,
            AllAirlines = 1,
            ActiveAirlines = 2
        }

        internal static List<string> Urls = new List<string>
        {
            "/",
            "/AllAirlines",
            "/Airlines"
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

            str += $"<body><form><h1><span class=\"infoIcon\"><a class=\"info-dialog\" href=\"/Information\" rel=\"#infodialog\">{i18n.Get("TBFlash.AirportStats.infoPage.information")}</a></span>{Values[(int)PageTitle]}&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
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
            string str = HeadTag(true, true);
            str += $"<body><form><h1><span class=\"infoIcon\"><a class=\"info-dialog\" href=\"/Information\" rel=\"#infodialog\">{i18n.Get("TBFlash.AirportStats.infoPage.information")}</a></span><a href=\"/{airlineName}\">{airlineName}</a>{st}&nbsp;&nbsp;&nbsp;&nbsp;";
            if (day > -1)
            {
                str += "\n<script type=\"text/javascript\">function goToNewPage() { var url =\'/" + airlineName + "\' + \'?Day=\' + document.getElementById(\'daySelector\').value; if (url != \'none\') {window.location = url;}}</script>";
                str += "<select id=\"daySelector\" class=\"select\">";
                for (int i = (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i <= GameTimer.Day; i++)
                {
                    str += $"<option {(i == day ? selected : string.Empty)} value=\'{i}\'>{i18n.Get("TBFlash.AirportStats.utils.day")} {i}</option>";
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
            return $"<div id=\"dialog\"></div><div id=\"infodialog\"></div><p class=\"footer\">{i18n.Get("TBFlash.AirportStats.utils.version")}: {version}</br>{i18n.Get("TBFlash.AirportStats.utils.broughtBy")}</p></body></html>";
        }

        internal static string HeadTag(bool includeStyles = true, bool includeScripts = false)
        {
            string str = "<html>\n<head><meta charset=\"UTF-8\"><link rel=\"icon\" href=\"data:;base64,=\">\n";
            str += includeStyles ? Styles() : string.Empty;
            str += includeScripts ? Scripts() : string.Empty;
            str += "\n</head>\n";
            return str;
        }

        private static string Scripts()
        {
            string str = "<script type=\"text/javascript\" src=\"/jquery.min.js\"></script>\n";
            str += "<script type=\"text/javascript\" src=\"/jquery-ui.min.js\"></script>\n";
            str += "<script type=\"text/javascript\" src=\"/Chart.min.js\"></script>\n";
            str += "<script type=\"text/javascript\" src=\"/AirportStats.js\"></script>\n";
            return str;
        }

        private static string Styles()
        {
            string str = "<link rel=\"stylesheet\" type=\"text/css\" href=\"/jquery-ui.min.css\">\n";
            str += "<link rel=\"stylesheet\" type=\"text/css\" href=\"/Chart.min.css\">\n";
            str += "<link rel=\"stylesheet\" type=\"text/css\" href=\"/AirportStats.css\">\n";
            return str;
        }

        internal static string InvalidAircraftType()
        {
            return $"<div class=\"modal\"><h1>{i18n.Get("TBFlash.AirportStats.utils.invalidEntry")}</h1>{i18n.Get("TBFlash.AirportStats.utils.invalidAircraft")}</div>";
        }

        internal static string InformationDialog()
        {
            string htmlCode = $"<div class=\"modal\"><h1>{i18n.Get("TBFlash.AirportStats.infoPage.information")}</h1>";
            htmlCode += $"<p><strong>{i18n.Get("TBFlash.AirportStats.infoPage.modName")}</strong> {i18n.Get("TBFlash.AirportStats.infoPage.intro")}" +
                "<ul>" +
                    $"<li>{i18n.Get("TBFlash.AirportStats.infoPage.bullet1")}</li>" +
                    $"<li>{i18n.Get("TBFlash.AirportStats.infoPage.bullet2a")} <em>{i18n.Get("TBFlash.AirportStats.infoPage.bullet2b")}</em>{i18n.Get("TBFlash.AirportStats.infoPage.bullet2c")} <em> {i18n.Get("TBFlash.AirportStats.infoPage.bullet2d")}</em></li>" +
                    $"<li>{i18n.Get("TBFlash.AirportStats.infoPage.bullet3a")} <em>{i18n.Get("TBFlash.AirportStats.infoPage.bullet3b")}</em> {i18n.Get("TBFlash.AirportStats.infoPage.bullet3c")}</li>" +
                "</ul></p>" +
                $"<p>{i18n.Get("TBFlash.AirportStats.infoPage.techNotes")}" +
                $"<ul><li>{i18n.Get("TBFlash.AirportStats.infoPage.tnbullet0")}</li>" +
                    $"<li>{i18n.Get("TBFlash.AirportStats.infoPage.tnbullet1")}</li>" +
                    $"<li>{i18n.Get("TBFlash.AirportStats.infoPage.tnbullet2")}</li>" +
                    $"<li>{i18n.Get("TBFlash.AirportStats.infoPage.tnbullet3")}</li>" +
                    $"<li><strong>{i18n.Get("TBFlash.AirportStats.infoPage.tnbullet4a")}</strong> {i18n.Get("TBFlash.AirportStats.infoPage.tnbullet4b")}</li>" +
                    $"<li>{i18n.Get("TBFlash.AirportStats.infoPage.tnbullet5")}</li>" +
                "</ul></p>" +
                "</div>";
            return htmlCode;
        }

        internal static bool HasStatus(int totalStatus, Flight.Status status)
        {
            return (totalStatus & (int)status) == (int)status;
        }
    }
}
