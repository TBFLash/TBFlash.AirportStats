using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class ChartOptions
    {
        internal enum ChartTypes
        {
            line,
            bar,
            radar,
            doughnut,
            polar,
            bubble,
            scatter,
            stackedBar,
            multiAxisLine,
            none
        }
        internal enum YAxisTypes
        {
            yAxisLeft,
            yAxisRight,
            yAxisRight2
        }

        private readonly ChartTypes chartType;
        private readonly string title;
        private readonly string yAxisLabel;
        private readonly string yAxisLabel2;
        private readonly string yAxisLabel3;
        private readonly string moneySetting;
        private readonly bool reverse;

        internal ChartOptions (ChartTypes chartType, string title, string moneySetting, string yAxisLabel, string yAxisLabel2 = null, string yAxisLabel3 = null, bool reverse = true)
        {
            this.chartType = chartType;
            this.title = title;
            this.moneySetting = moneySetting;
            this.yAxisLabel = yAxisLabel;
            this.yAxisLabel2 = yAxisLabel2;
            this.yAxisLabel3 = yAxisLabel3;
            this.reverse = reverse;
        }

        internal string GetChartOptions()
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            return $"\"chartOptions\":[{{\"title\":\"{title}\",\"type\":\"{chartType}\",\"money\":{moneySetting},\"options\":{GetOptions(chartType, reverse, yAxisLabel, yAxisLabel2, yAxisLabel3)}, \"day\": \"{i18n.Get("TBFlash.AirportStats.utils.day")}\"}}]";
        }

        internal ChartTypes GetChartType()
        {
            return chartType;
        }

        private static string GetOptions(ChartTypes chartType, bool reverse, string yAxisLabel, string yAxisLabel2 = null, string yAxisLabel3 = null)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            string Options = "{" +
                "\"scales\": {" +
                    "\"xAxes\": [{" +
                        "\"ticks\": {" +
                            $"\"reverse\": {(reverse ? "true" : "false")} " +
                        "}, " +
                        "\"gridLines\": {" +
                        "}, " +
                        "\"scaleLabel\":{" +
                            "\"display\": true, " +
                            "\"labelString\":\"Days\", " +
                            "\"fontSize\": 12 " +
                        "} ";
            Options += chartType == ChartTypes.stackedBar ? ", \"stacked\": true" : string.Empty;
            Options += "}], " + //end xAxes
                    "\"yAxes\": [{" +
                        "\"ticks\": {" +
                            "\"beginAtZero\": true";
            Options += "}, " +
                        "\"gridLines\":{" +
                        "}, " +
                        "\"scaleLabel\":{" +
                            "\"display\": true, " +
                            $"\"labelString\":\"{yAxisLabel}\", " +
                            "\"fontSize\": 12 " +
                        "}";
            Options += chartType == ChartTypes.stackedBar ? ", \"stacked\": true" : string.Empty;
            if (chartType == ChartTypes.multiAxisLine)
            {
                Options += $", \"id\": \"{nameof(YAxisTypes.yAxisLeft)}\", \"type\": \"linear\", \"position\": \"left\"}}"; //end of the first yAxis
                Options += ", {" +
                        "\"ticks\": {" +
                            "\"beginAtZero\": true" +
                        "}, " +
                        "\"gridLines\":{" +
                        "}, " +
                        "\"scaleLabel\":{" +
                            "\"display\": true, " +
                            $"\"labelString\":\"{yAxisLabel2}\", " +
                            "\"fontSize\": 12 " +
                        "}";
                Options += $", \"id\": \"{nameof(YAxisTypes.yAxisRight)}\", \"type\": \"linear\", \"position\": \"right\"}}"; //end of the second yAxis
                Options += ", {" +
                        "\"ticks\": {" +
                            "\"beginAtZero\": true" +
                        "}, " +
                        "\"gridLines\":{" +
                        "}, " +
                        "\"scaleLabel\":{" +
                            "\"display\": true, " +
                            $"\"labelString\":\"{yAxisLabel3}\", " +
                            "\"fontSize\": 12 " +
                        "}";
                Options += $", \"id\": \"{nameof(YAxisTypes.yAxisRight2)}\", \"type\": \"linear\", \"position\": \"right\""; //end of the third yAxis
            }
            Options += "}]" + //end yAxes
                "}, " + //end scales
                "\"tooltips\": {";
            Options += chartType == ChartTypes.stackedBar ? "\"mode\": \"index\", \"position\": \"nearest\", \"reverse\": true" : "\"mode\": \"point\"";
            Options += "}, " + //end tooltips
                "\"legend\": {";
            Options += chartType == ChartTypes.stackedBar ? "\"reverse\": true" : string.Empty;
            Options += "}" + //end legend
            "}";
            return Options;
        }
    }
}
