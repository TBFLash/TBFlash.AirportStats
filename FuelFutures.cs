using System.Collections.Generic;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class FuelFutures : StatGroup
    {
        private static readonly FuelController fuelController = Game.current.fuelController;
        private readonly Dictionary<int, KeyValuePair<int, float>> minValues = new Dictionary<int, KeyValuePair<int, float>>();
        private readonly Dictionary<int, KeyValuePair<int, float>> maxValues = new Dictionary<int, KeyValuePair<int, float>>();
        private readonly int numDays = 14;

        internal FuelFutures(string name) : base(name, new ChartOptions(ChartOptions.ChartTypes.line, i18n.Get("TBFlash.AirportStats.json.fuelFutures"), "\"" + i18n.Get("UI.currency") + "\"", i18n.Get("TBFlash.AirportStats.json.cost"), null, null, false))
        {
        }

        internal override string ForChart(PrintOptions printOptions = null)
        {
            string str = string.Empty;
            int startDay = GameTimer.Day + numDays -1;
            for (int i = startDay; i >= GameTimer.Day; i--)
            {
                LoadDictionary(i);
                str += i < startDay ? "," : string.Empty;
                str += $"\"{i}\":{{\"low\":\"{minValues[i].Value}\",\"high\":\"{maxValues[i].Value}\"}}";
            }
            return str;
        }

        internal override SeriesData GetSeriesData()
        {
            string labels = $"\"{i18n.Get("TBFlash.AirportStats.json.lowestCost")}\",\"{i18n.Get("TBFlash.AirportStats.json.highestCost")}\"";
            const string keys = "\"low\",\"high\"";
            const string colors = "\"green\",\"red\"";
            const string orders = "\"1\",\"2\"";
            return new SeriesData(labels, keys, colors, orders);
        }

        internal override string ForTable(PrintOptions printOptions = null)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            string str = string.Empty;
            int day = GameTimer.Day;
            for(int j = day; j < (day + numDays); j++)
            {
                LoadDictionary(j);
            }

            str += $"<tr><td></td><th colspan=\"2\">{i18n.Get("TBFlash.AirportStats.json.lowestCost")}</th><th colspan=\"2\">{i18n.Get("TBFlash.AirportStats.json.highestCost")}</th></tr>\n";
            str += $"<tr><td></td><th style=\"width: 70px;\">{i18n.Get("TBFlash.AirportStats.json.cost")}</th><th style=\"width: 70px;\">{i18n.Get("TBFlash.AirportStats.json.time")}</th><th style=\"width: 70px;\">{i18n.Get("TBFlash.AirportStats.json.cost")}</th><th style=\"width: 70px;\">{i18n.Get("TBFlash.AirportStats.json.time")}</th><th><a class=\"loadChart\" href=\"/chartdata?dataset=fuelFutures\" rel=\"#dialog\">{i18n.Get("TBFlash.AirportStats.json.chart")}</a></th></tr>\n";

            float lowvalue = CalcLowHigh(true);
            float highvalue = CalcLowHigh(false);
            for (int i = day; i < (day + numDays); i++)
            {
                str += GetFuelForTable(i, lowvalue, highvalue);
            }
            return str;
        }

        private string GetFuelForTable(int day, float lowvalue, float highvalue)
        {
            string str = $"<tr><td>{i18n.Get("TBFlash.AirportStats.utils.day")} {day}</td>";
            str += $"<td {(minValues[day].Value == lowvalue ? "class=\"goldStar\"" : string.Empty)}>{minValues[day].Value:C4}</td><td>{AirportStatUtils.FormatTime(minValues[day].Key*60, true)}</td>\n";
            str += $"<td {(maxValues[day].Value == highvalue ? "class=\"goldStar\"" : string.Empty)}>{maxValues[day].Value:C4}</td><td>{AirportStatUtils.FormatTime(maxValues[day].Key*60, true)}</td>\n";
            return str + "</tr>";
        }

        private float CalcLowHigh(bool low)
        {
            float value = low ? 100f : 0f;
            for (int i = GameTimer.Day; i < (GameTimer.Day + numDays); i++)
            {
                value = low ? (minValues[i].Value < value ? minValues[i].Value : value) : (maxValues[i].Value > value ? maxValues[i].Value : value);
            }
            return value;
        }

        private void LoadDictionary(int day)
        {
            if (!minValues.ContainsKey(day))
            {
                float minValue = 100;
                float maxValue = 0;
                int minKey = 0;
                int maxKey = 0;
                float value;
                int startTime = GameTimer.MidnightForDay(day);
                AirportStatUtils.AirportStatsLogger(Log.FromPool($"Day: {day}; start{startTime}; end{startTime + 1440}").WithCodepoint());

                for (int i = startTime; i < (startTime + 1440); i++)
                {
                    value = fuelController.GetMarketPriceAtTime(i);
                    if(value < minValue)
                    {
                        minValue = value;
                        minKey = i;
                    }
                    if(value > maxValue)
                    {
                        maxValue = value;
                        maxKey = i;
                    }
                }
                minValues[day] = new KeyValuePair<int, float>(minKey, minValue);
                maxValues[day] = new KeyValuePair<int, float>(maxKey, maxValue);
            }
        }
    }
}
