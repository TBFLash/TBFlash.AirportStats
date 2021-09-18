using System.Collections.Generic;
using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class ExpenseStatGroup : StatGroup
    {
        internal Dictionary<string, DailyStats<MoneyStat>> StatGroups { get; }

        internal ExpenseStatGroup(string name) : base(name, null)
        {
            StatGroups = new Dictionary<string, DailyStats<MoneyStat>>()
            {
                [nameof(GamedayReportingData.MoneyCategory.Advertising)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats21"), null),
                [nameof(GamedayReportingData.MoneyCategory.Airline_Fees)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats22"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.airlineExp"), "expAirline", "burlywood", "0", null, "2")),
                [nameof(GamedayReportingData.MoneyCategory.Bank)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats23"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.bankExp"), "expBank", "goldenrod", "0", null, "2")),
                [nameof(GamedayReportingData.MoneyCategory.Cheat)] = new DailyStats<MoneyStat>(i18n.Get(""), null),
                [nameof(GamedayReportingData.MoneyCategory.Fuel)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats24"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.fuelExp"), "expFuel", "orange", "0", null, "2")),
                [nameof(GamedayReportingData.MoneyCategory.Grant)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats25"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.grantExp"), "expGrant", "lightsalmon", "0", null, "2")),
                [nameof(GamedayReportingData.MoneyCategory.Infrastructure)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats39"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.infraExp"), "expInfra", "darksalmon", "0", null, "2")),
                [nameof(GamedayReportingData.MoneyCategory.Land_Purchase)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats40"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.landExp"), "expLand", "darkorange", "0", null, "2")),
                [nameof(GamedayReportingData.MoneyCategory.Maintenance)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats41"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.maintExp"), "expMain", "sandybrown", "0", null, "2")),
                [nameof(GamedayReportingData.MoneyCategory.Materials)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats42"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.matExp"), "expMat", "indianred", "0", null, "2")),
                //[nameof(GamedayReportingData.MoneyCategory.Parking)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats27"), null),
                [nameof(GamedayReportingData.MoneyCategory.Research)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats28"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.resExp"), "expRes", "palevioletred", "0", null, "2")),
                [nameof(GamedayReportingData.MoneyCategory.Retail)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats29"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.retExp"), "expRet", "orangered", "0", null, "2")),
                [nameof(GamedayReportingData.MoneyCategory.Runway_Fees)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats30"), null),
                [nameof(GamedayReportingData.MoneyCategory.Staff)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats32"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.staffExp"), "expStaff", "coral", "0", null, "2")),
                [nameof(GamedayReportingData.MoneyCategory.Taxes)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats46"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.taxes"), "expTax", "crimson", "0", null, "2")),
                [nameof(GamedayReportingData.MoneyCategory.Terminal_Fees)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats31"), null),
                [nameof(GamedayReportingData.MoneyCategory.Transportation)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats49"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.transExp"), "expTrans", "tomato", "0", null, "2")),
                [nameof(GamedayReportingData.MoneyCategory.Undefined)] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats33"), new SeriesData(i18n.Get("TBFlash.AirportStats.json.undefExp"), "expUndef", "red", "0", null, "2")),
                ["total"] = new DailyStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.LifetimeStats.stats51"), null),
            };
        }

        internal void RemoveAirlineStats(int firstDay, int lastDay)
        {
            foreach (DailyStats<MoneyStat> statGroup in StatGroups.Values)
            {
                statGroup.RemoveStats(firstDay, lastDay);
            }
        }

        internal void AddStat(string statGroup, int day, MoneyStat stat)
        {
            if (StatGroups.ContainsKey(statGroup))
            {
                StatGroups[statGroup].AddStat(day, stat);
            }
        }

        internal override string ForChart(PrintOptions printOptions = null)
        {
            string str = string.Empty;
            foreach (DailyStats<MoneyStat> statGroup in StatGroups.Values)
            {
                if (statGroup.HasData && statGroup.SeriesData != null)
                {
                    if (str.Length > 0)
                    {
                        str += ",";
                    }
                    str += statGroup.ForChart(printOptions.Day);
                }
            }
            return str;
        }

        internal override SeriesData GetSeriesData()
        {
            string labels = string.Empty;
            string keys = string.Empty;
            string colors = string.Empty;
            string orders = string.Empty;
            string stacks = string.Empty;
            foreach (DailyStats<MoneyStat> statGroup in StatGroups.Values)
            {
                if (statGroup.HasData && statGroup.SeriesData != null)
                {
                    if (labels.Length > 0)
                    {
                        labels += ",";
                        keys += ",";
                        colors += ",";
                        orders += ",";
                        stacks += ",";
                    }
                    labels += $"\"{statGroup.SeriesData.Label}\"";
                    keys += $"\"{statGroup.SeriesData.Key}\"";
                    colors += $"\"{statGroup.SeriesData.Color}\"";
                    orders += $"\"{statGroup.SeriesData.Order}\"";
                    stacks += $"\"{statGroup.SeriesData.Stack}\"";
                }
            }
            return new SeriesData(labels, keys, colors, orders, null, stacks);
        }

        internal override string ForTable(PrintOptions printOptions = null)
        {
            string str = $"<tr class=\"statGroup\"><th colspan=\"2\"><a class=\"loadChart\" href=\"/chartdata?dataset=revexp{(!string.IsNullOrEmpty(printOptions.AirlineName) ? "&airline=" + printOptions.AirlineName : string.Empty)}\" rel=\"#dialog\">{name}</a></th></tr>\n";
            bool oddRow = true;
            foreach (DailyStats<MoneyStat> statGroup in StatGroups.Values.Where(x => x.HasData))
            {
                str += statGroup.ForTable(printOptions, oddRow);
                oddRow = !oddRow;
            }
            return str;
        }
    }
}
