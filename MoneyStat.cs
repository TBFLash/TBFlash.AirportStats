using System;

namespace TBFlash.AirportStats
{
    internal class MoneyStat : Stat
    {
        private float value;
        private readonly int numDigits;

        internal MoneyStat(float value, int numDigits = 0, AirportStatUtils.InfoLevels infoLevel = AirportStatUtils.InfoLevels.None)
        {
            this.value = value;
            this.numDigits = numDigits;
            this.infoLevel = infoLevel;
        }

        internal override void AddToValue(Stat stat)
        {
            value += ((MoneyStat)stat).value;
            if ((int)stat.GetInfoLevel() > (int)infoLevel)
            {
                infoLevel = stat.GetInfoLevel();
            }
        }

        internal override string ForChart()
        {
            return value !=0.0f ? Math.Abs(value).ToString($"F{numDigits}") : string.Empty;
        }

        internal override string ForTable()
        {
            return value != 0.0f ? value.ToString($"C{numDigits}") : string.Empty;
        }

        internal override bool HasNonZeroValue()
        {
            return value != 0;
        }
    }
}
