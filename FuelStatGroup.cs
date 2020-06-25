using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class FuelStatGroup : StatGroup
    {
        internal DailyStats<NumberStat> fuelRequested;
        internal DailyStats<NumberStat> fuelDelivered;
        internal DailyStats<NumberStat> fuelingFailures;
        internal DailyStats<NumberStat> planesRefueled;
        internal DailyStats<MoneyStat> avgFuelPrice;

        internal FuelStatGroup(string name) : base(name, new ChartOptions(ChartOptions.ChartTypes.multiAxisLine, i18n.Get("TBFlash.AirportStats.json.fuelStats"), "false", i18n.Get("TBFlash.AirportStats.json.litersOfFuel"), i18n.Get("TBFlash.AirportStats.json.planesServed"), i18n.Get("TBFlash.AirportStats.json.fuelPrice")))
        {
            fuelRequested = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats18"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.fuelRequested"), "fuelReq", "ivory", "2", nameof(ChartOptions.YAxisTypes.yAxisLeft)));
            fuelDelivered = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AirlineDailyStats.stats19"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.fuelProvided"), "fuelProv", "green", "1", nameof(ChartOptions.YAxisTypes.yAxisLeft)));
            planesRefueled = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats14"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.servedFuel"), "served", "cyan", "4", nameof(ChartOptions.YAxisTypes.yAxisRight)));
            fuelingFailures = new DailyStats<NumberStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats15"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.fuelingFailures"), "failed", "red", "5", nameof(ChartOptions.YAxisTypes.yAxisRight)));
            avgFuelPrice = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats11"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.fuelPrice"), "price", "purple", "3", nameof(ChartOptions.YAxisTypes.yAxisRight2)));
        }

        internal void RemoveAirlineStats(int firstDay, int lastDay)
        {
            fuelRequested.RemoveStats(firstDay, lastDay);
            fuelDelivered.RemoveStats(firstDay, lastDay);
            fuelingFailures.RemoveStats(firstDay, lastDay);
            planesRefueled.RemoveStats(firstDay, lastDay);
            avgFuelPrice.RemoveStats(firstDay, lastDay);
        }

        internal override string ForChart(PrintOptions printOptions = null)
        {
            string str = string.Empty;
            for (int day = printOptions.FirstDay; day <= printOptions.LastDay; day++)
            {
                str += day > printOptions.FirstDay ? "," : string.Empty;
                str += $"\"{day}\":{{{fuelRequested.ForChart(day)},{fuelDelivered.ForChart(day)},{planesRefueled.ForChart(day)},{fuelingFailures.ForChart(day)},{StatLoader.airportData.fuelStats.avgFuelPrice.ForChart(day)}}}";
            }
            return str;
        }

        internal override SeriesData GetSeriesData()
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            string labels = $"\"{fuelRequested.SeriesData.Label}\",\"{fuelDelivered.SeriesData.Label}\",\"{planesRefueled.SeriesData.Label}\",\"{fuelingFailures.SeriesData.Label}\",\"{avgFuelPrice.SeriesData.Label}\"";
            string keys = $"\"{fuelRequested.SeriesData.Key}\",\"{fuelDelivered.SeriesData.Key}\",\"{planesRefueled.SeriesData.Key}\",\"{fuelingFailures.SeriesData.Key}\",\"{avgFuelPrice.SeriesData.Key}\"";
            string colors = $"\"{fuelRequested.SeriesData.Color}\",\"{fuelDelivered.SeriesData.Color}\",\"{planesRefueled.SeriesData.Color}\",\"{fuelingFailures.SeriesData.Color}\",\"{avgFuelPrice.SeriesData.Color}\"";
            string orders = $"\"{fuelRequested.SeriesData.Order}\",\"{fuelDelivered.SeriesData.Order}\",\"{planesRefueled.SeriesData.Order}\",\"{fuelingFailures.SeriesData.Order}\",\"{avgFuelPrice.SeriesData.Order}\"";
            string yAxis = $"\"{fuelRequested.SeriesData.YAxis}\",\"{fuelDelivered.SeriesData.YAxis}\",\"{planesRefueled.SeriesData.YAxis}\",\"{fuelingFailures.SeriesData.YAxis}\",\"{avgFuelPrice.SeriesData.YAxis}\"";
            return new SeriesData(labels, keys, colors, orders, yAxis);
        }

        internal override string ForTable(PrintOptions printOptions = null)
        {
            string str = $"<tr><th colspan=\"2\"><a class=\"loadChart\" href=\"/chartdata?dataset=fuelstats{(!string.IsNullOrEmpty(printOptions.AirlineName) ? "&airline=" + printOptions.AirlineName : string.Empty)}\" rel=\"#dialog\">{name}</a></th></tr>\n";
            str += fuelRequested.ForTable(printOptions);
            str += fuelDelivered.ForTable(printOptions);
            str += fuelingFailures.ForTable(printOptions);
            str += planesRefueled.ForTable(printOptions);
            str += StatLoader.airportData.fuelStats.avgFuelPrice.ForTable(printOptions);
            return str;
        }
    }
}
