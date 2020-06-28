using System;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class AverageStat : Stat
    {
        private double totalValue;
        private int count;
        private readonly Type statType;

        internal AverageStat(double totalValue, int count, Type statType, AirportStatUtils.InfoLevels infoLevel = AirportStatUtils.InfoLevels.None)
        {
            this.totalValue = totalValue;
            this.count = count;
            this.statType = statType;
            this.infoLevel = infoLevel;
        }

        internal override void AddToValue(Stat stat)
        {
            totalValue += ((AverageStat)stat).totalValue;
            count += ((AverageStat)stat).count;
            if ((int)stat.GetInfoLevel() > (int)infoLevel)
            {
                infoLevel = stat.GetInfoLevel();
            }
        }

        internal override string ForChart()
        {
            throw new NotImplementedException();
        }

        internal override string ForTable()
        {
            double value = 0;
            if (count != 0)
                value = totalValue / count;
            if (statType == typeof(MoneyStat))
                return value.ToString("C2");
            if(statType==typeof(TimeStat))
                return TimeSpan.FromSeconds((int)value).ToString("g");
            if(statType==typeof(PercentageStat))
                return value.ToString("P1");
            return value.ToString("#,#.##");
        }

        internal override bool HasNonZeroValue()
        {
            return count > 0;
        }
    }
}
