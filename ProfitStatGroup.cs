using System.Collections.Generic;
using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class ProfitStatGroup : StatGroup
    {
        internal DailyStats<MoneyStat> GrossProfit { get; }
        internal DailyStats<MoneyStat> OperatingProfit { get; }
        internal DailyStats<MoneyStat> NetProfit { get; }
        internal DailyStats<PercentageStat> GrossMargin { get; }
        internal DailyStats<PercentageStat> OperatingMargin { get; }
        internal DailyStats<PercentageStat> NetMargin { get; }



        internal ProfitStatGroup(string name) : base(name, new ChartOptions(ChartOptions.ChartTypes.multiAxisLine2, i18n.Get("TBFlash.AirportStats.json.profitStats"), "\"" + i18n.Get("UI.currency") + "\"", i18n.Get("TBFlash.AirportStats.json.profits"), i18n.Get("TBFlash.AirportStats.json.marginLabel")))
        {
            GrossProfit = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.json.grossProfit"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.grossProfit"), "grossProfit", "lightgreen", "1", nameof(ChartOptions.YAxisTypes.yAxisLeft)));
            OperatingProfit = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.json.operatingProfit"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.operatingProfit"), "operatingProfit", "lawngreen", "2", nameof(ChartOptions.YAxisTypes.yAxisLeft)));
            NetProfit = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.json.netProfit"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.netProfit"), "netProfit", "green", "3", nameof(ChartOptions.YAxisTypes.yAxisLeft)));
            GrossMargin = new DailyStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.json.grossMargin"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.grossMargin"), "grossMargin", "crimson", "4", nameof(ChartOptions.YAxisTypes.yAxisRight)));
            OperatingMargin = new DailyStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.json.operatingMargin"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.operatingMargin"), "operatingMargin", "sandybrown", "5", nameof(ChartOptions.YAxisTypes.yAxisRight)));
            NetMargin = new DailyStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.json.netMargin"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.netMargin"), "netMargin", "orange", "6", nameof(ChartOptions.YAxisTypes.yAxisRight)));
        }

        internal void RemoveAirlineStats(int firstDay, int lastDay)
        {
            GrossProfit.RemoveStats(firstDay, lastDay);
            OperatingProfit.RemoveStats(firstDay, lastDay);
            NetProfit.RemoveStats(firstDay, lastDay);
            GrossMargin.RemoveStats(firstDay, lastDay);
            OperatingMargin.RemoveStats(firstDay, lastDay);
            NetMargin.RemoveStats(firstDay, lastDay);
        }

        internal override string ForChart(PrintOptions printOptions = null)
        {
            string str = string.Empty;
            for (int day = printOptions.FirstDay; day <= printOptions.LastDay; day++)
            {
                str += day > printOptions.FirstDay ? "," : string.Empty;
               // str += $"\"{day}\":{{{GrossProfit.ForChart(day)},{OperatingProfit.ForChart(day)},{NetProfit.ForChart(day)},{GrossMargin.ForChart(day)},{OperatingMargin.ForChart(day)},{NetMargin.ForChart(day)}}}";
                str += $"\"{day}\":{{{NetProfit.ForChart(day)},{NetMargin.ForChart(day)}}}";
            }
            return str;
        }

        internal override string ForTable(PrintOptions printOptions = null)
        {
            string str = $"<tr class=\"statGroup\"><th colspan=\"2\"><a class=\"loadChart\" href=\"/chartdata?dataset=profits{(!string.IsNullOrEmpty(printOptions.AirlineName) ? "&airline=" + printOptions.AirlineName : string.Empty)}\" rel=\"#dialog\">{name}</a></th></tr>\n";
            //str += GrossProfit.ForTable(printOptions, true);
            //str += GrossMargin.ForTable(printOptions);
            //str += OperatingProfit.ForTable(printOptions, true);
            //str += OperatingMargin.ForTable(printOptions);
            str += NetProfit.ForTable(printOptions, true);
            str += NetMargin.ForTable(printOptions);
            return str;
        }

        internal override SeriesData GetSeriesData()
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            //string labels = $"\"{GrossProfit.SeriesData.Label}\",\"{OperatingProfit.SeriesData.Label}\",\"{NetProfit.SeriesData.Label}\",\"{GrossMargin.SeriesData.Label}\",\"{OperatingMargin.SeriesData.Label}\",\"{NetMargin.SeriesData.Label}\"";
            //string keys = $"\"{GrossProfit.SeriesData.Key}\",\"{OperatingProfit.SeriesData.Key}\",\"{NetProfit.SeriesData.Key}\",\"{GrossMargin.SeriesData.Key}\",\"{OperatingMargin.SeriesData.Key}\",\"{NetMargin.SeriesData.Key}\"";
            //string colors = $"\"{GrossProfit.SeriesData.Color}\",\"{OperatingProfit.SeriesData.Color}\",\"{NetProfit.SeriesData.Color}\",\"{GrossMargin.SeriesData.Color}\",\"{OperatingMargin.SeriesData.Color}\",\"{NetMargin.SeriesData.Color}\"";
            //string orders = $"\"{GrossProfit.SeriesData.Order}\",\"{OperatingProfit.SeriesData.Order}\",\"{NetProfit.SeriesData.Order}\",\"{GrossMargin.SeriesData.Order}\",\"{OperatingMargin.SeriesData.Order}\",\"{NetMargin.SeriesData.Order}\"";
            //string yAxis = $"\"{GrossProfit.SeriesData.YAxis}\",\"{OperatingProfit.SeriesData.YAxis}\",\"{NetProfit.SeriesData.YAxis}\",\"{GrossMargin.SeriesData.YAxis}\",\"{OperatingMargin.SeriesData.YAxis}\",\"{NetMargin.SeriesData.YAxis}\"";
            string labels = $"\"{NetProfit.SeriesData.Label}\",\"{NetMargin.SeriesData.Label}\"";
            string keys = $"\"{NetProfit.SeriesData.Key}\",\"{NetMargin.SeriesData.Key}\"";
            string colors = $"\"{NetProfit.SeriesData.Color}\",\"{NetMargin.SeriesData.Color}\"";
            string orders = $"\"{NetProfit.SeriesData.Order}\",\"{NetMargin.SeriesData.Order}\"";
            string yAxis = $"\"{NetProfit.SeriesData.YAxis}\",\"{NetMargin.SeriesData.YAxis}\"";
            return new SeriesData(labels, keys, colors, orders, yAxis);
        }
    }
}
