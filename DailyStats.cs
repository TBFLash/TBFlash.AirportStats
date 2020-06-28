using System.Collections.Generic;
using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class DailyStats<TStat> where TStat : Stat
    {
        internal readonly string statName;
        private readonly Dictionary<int, Stat> stats;
        internal SeriesData SeriesData { get; }
        internal bool HasData => stats.Count > 0 && stats.Count(x=> x.Value.HasNonZeroValue()) > 0;

        internal DailyStats(string statName, SeriesData seriesData)
        {
            this.statName = statName;
            stats = new Dictionary<int, Stat>();
            SeriesData = seriesData;
        }

        internal bool AddStat(int day, Stat stat)
        {
            if (stat.GetType() == typeof(TStat))
            {
                stats[day] = stat;
                return true;
            }
            return false;
        }

        internal void AddToValue(int day, Stat stat)
        {
            if (stat != null && stat.GetType() == typeof(TStat))
            {
                if (stats.TryGetValue(day, out Stat keyValue))
                {
                    keyValue.AddToValue(stat);
                }
                else
                {
                    stats[day] = stat;
                }
            }
        }

        internal Stat GetStat(int day)
        {
            stats.TryGetValue(day, out Stat value);
            return value;
        }

        internal void RemoveStats(int firstDay, int lastDay)
        {
            for (int i=firstDay; i<=lastDay; i++)
            {
                stats.Remove(i);
            }
        }

        internal string ForChart(int day)
        {
            return stats.TryGetValue(day, out Stat value) ? $"\"{SeriesData.Key}\":\"{value.ForChart()}\"" : $"\"{SeriesData.Key}\":\"\"";
        }

        internal string ForTable(int day)
        {
            return stats.TryGetValue(day, out Stat value) ? $"\t<td class=\"{value.GetInfoLevel()}\">{value.ForTable()}</td>\n" : "\t<td></td>\n";
        }

        internal string ForTable(PrintOptions printOptions = null, bool oddRow = false)
        {
            string str = $"<tr{(oddRow ? " class=\"oddRow\"" : string.Empty)}><td>{statName}</td>\n";
            if (printOptions.IncludeLifetime)
            {
                str += ForTable(0);
            }
            for (int i = printOptions.LastDay; i >= printOptions.FirstDay; i--)
            {
                str += ForTable(i);
            }
            str += "</tr>";
            return str;
        }
    }
}
