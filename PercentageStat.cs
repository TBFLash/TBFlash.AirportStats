namespace TBFlash.AirportStats
{
    internal class PercentageStat : Stat
    {
        private double value;

        internal PercentageStat(double value, AirportStatUtils.InfoLevels infoLevel = AirportStatUtils.InfoLevels.None)
        {
            this.value = value;
            this.infoLevel = infoLevel;
        }

        internal override void AddToValue(Stat stat)
        {
            value += ((PercentageStat)stat).value;
            if ((int)stat.GetInfoLevel() > (int)infoLevel)
            {
                infoLevel = stat.GetInfoLevel();
            }
        }

        internal override string ForChart()
        {
            return (value*100).ToString("F1");
        }

        internal override string ForTable()
        {
            return value.ToString("P1");
        }

        internal override bool HasNonZeroValue()
        {
            return value != 0;
        }
        internal override float GetFloatValue()
        {
            return (float)value;
        }
    }
}
