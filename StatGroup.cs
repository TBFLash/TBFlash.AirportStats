namespace TBFlash.AirportStats
{
    internal abstract class StatGroup
    {
        protected readonly string name;
        private readonly ChartOptions chartData;

        internal StatGroup(string name, ChartOptions chartData)
        {
            this.name = name;
            this.chartData = chartData;
        }

        internal ChartOptions GetChartData()
        {
            return chartData;
        }

        internal abstract SeriesData GetSeriesData();

        internal abstract string ForChart(PrintOptions printOptions = null);

        internal abstract string ForTable(PrintOptions printOptions = null);
    }
}
