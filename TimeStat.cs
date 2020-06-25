using System;

namespace TBFlash.AirportStats
{
    internal class TimeStat : Stat
    {
        private int value;
        private readonly bool timeOfDay;

        internal TimeStat(double value, bool timeOfDay = false, AirportStatUtils.InfoLevels infoLevel = AirportStatUtils.InfoLevels.None)
        {
            this.value = (int)value;
            this.timeOfDay = timeOfDay;
            this.infoLevel = infoLevel;
        }

        internal override void AddToValue(Stat stat)
        {
            value += ((TimeStat)stat).value;
            if ((int)stat.GetInfoLevel() > (int)infoLevel)
            {
                infoLevel = stat.GetInfoLevel();
            }
        }

        internal override string ForChart()
        {
            return string.Empty;
        }

        internal override string ForTable()
        {
            if (value == 0.0)
            {
                return string.Empty;
            }
            if (timeOfDay)
            {
                return DateTime.MinValue.AddSeconds(value).ToString("t");
            }
            else
            {
                //return new TimeSpan(0, 0, (int)seconds).ToString("g");
                return TimeSpan.FromSeconds(value).ToString("g");
            }
        }

        internal override bool HasNonZeroValue()
        {
            return value != 0;
        }
    }
}
