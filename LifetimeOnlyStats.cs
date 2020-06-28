namespace TBFlash.AirportStats
{
    internal class LifetimeOnlyStats<TStat> where TStat : Stat
    {
        private readonly string statName;
        private Stat stat;

        internal LifetimeOnlyStats(string name)
        {
            statName = name;
        }

        internal void AddStat(Stat stat)
        {
            if(stat.GetType() == typeof(TStat))
            {
                this.stat = stat;
            }
        }

        internal string ForTable(bool oddRow = false)
        {
            string str = $"<tr{(oddRow ? " class=\"oddRow\"" : string.Empty)}>\n";
            str += $"\t<td>{statName}</td>\n";
            str += $"\t<td>{stat.ForTable()}</td>\n";
            str += "</tr>\n";
            return str;
        }
    }
}
