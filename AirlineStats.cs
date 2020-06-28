using System.Collections.Generic;
using System.Linq;

namespace TBFlash.AirportStats
{
    internal class AirlineStats<TStat> where TStat : Stat
    {
        private readonly string statName;
        internal readonly IDictionary<string, Stat> stats;

        internal AirlineStats(string statName)
        {
            this.statName = statName;
            stats = new Dictionary<string, Stat>();
        }

        internal bool AddStat(string airline, Stat stat)
        {
            if(stat.GetType() == typeof(TStat))
            {
                stats[airline] = stat;
                return true;
            }
            return false;
        }

        internal Stat GetStat(string airline)
        {
            stats.TryGetValue(airline, out Stat value);
            return value;
        }

        internal void RemoveStats(string airline)
        {
                stats.Remove(airline);
        }

        internal string ForChart(PrintOptions printOptions = null)
        {
            return stats.TryGetValue(printOptions.AirlineName, out Stat value) ? value.ForChart() : string.Empty;
        }

        internal string ForTable(PrintOptions printOptions, bool isAirlineName)
        {
            Stat value;
            if (isAirlineName)
            {
                return stats.TryGetValue(printOptions.AirlineName, out value) ? $"\t<th><a href=\"{value.ForTable()}\">{value.ForTable()}</a></th>\n" : "\t<td></td>\n";
            }
            return stats.TryGetValue(printOptions.AirlineName, out value) ? $"\t<td class=\"{value.GetInfoLevel()}\">{value.ForTable()}</td>\n" : "\t<td></td>\n";
        }

        internal string ForTable(IOrderedEnumerable<string> airlineList, bool oddRow = false, bool isAirlineName = false)
        {
            string str = $"<tr{(oddRow ? " class=\"oddRow\"" : string.Empty)}>\n";
            str +=$"\t<td>{statName}</td>\n";
            foreach (string airline in airlineList)
            {
                PrintOptions printOptions = new PrintOptions
                {
                    AirlineName = airline
                };
                str += ForTable(printOptions, isAirlineName);
            }
            str += "</tr>\n";
            return str;
        }
    }
}
