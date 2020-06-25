namespace TBFlash.AirportStats
{
    internal class RevAndExpStatGroup : StatGroup
    {
        internal RevenueStatGroup revenueStats = new RevenueStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header4"));
        internal ExpenseStatGroup expenseStats = new ExpenseStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header5"));

        internal RevAndExpStatGroup(string name) : base(name, new ChartOptions(ChartOptions.ChartTypes.stackedBar, i18n.Get("TBFlash.AirportStats.json.revandexp"), "\"" + i18n.Get("UI.currency") + "\"", i18n.Get("TBFlash.AirportStats.json.totalrevandexp")))
        {
        }

        internal void RemoveAirlineStats(int firstDay, int lastDay)
        {
            revenueStats.RemoveAirlineStats(firstDay, lastDay);
            expenseStats.RemoveAirlineStats(firstDay, lastDay);
        }

        internal override string ForChart(PrintOptions printOptions = null)
        {
            string str = string.Empty;
            for (int day = printOptions.FirstDay; day <= printOptions.LastDay; day++)
            {
                str += day > printOptions.FirstDay ? "," : string.Empty;
                PrintOptions po = new PrintOptions() { Day = day };
                str += $"\"{day}\":{{{revenueStats.ForChart(po)},{expenseStats.ForChart(po)}}}";
            }
            return str;
        }
        internal override SeriesData GetSeriesData()
        {
            SeriesData revenue = revenueStats.GetSeriesData();
            SeriesData expenses = expenseStats.GetSeriesData();
            return new SeriesData($"{revenue.Label},{expenses.Label}", $"{revenue.Key},{expenses.Key}", $"{revenue.Color},{expenses.Color}", $"{revenue.Order},{expenses.Order}", null, $"{revenue.Stack},{expenses.Stack}");
        }

        internal override string ForTable(PrintOptions printOptions = null)
        {
            string str = revenueStats.ForTable(printOptions);
            str += expenseStats.ForTable(printOptions);
            return str;
        }
    }
}
