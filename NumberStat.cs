namespace TBFlash.AirportStats
{
    internal class NumberStat : Stat
    {
        private int value;

        internal NumberStat(int value, AirportStatUtils.InfoLevels infoLevel = AirportStatUtils.InfoLevels.None)
        {
            this.value = value;
            this.infoLevel = infoLevel;
        }

        internal override void AddToValue(Stat stat)
        {
            value += ((NumberStat)stat).value;
            if((int)stat.GetInfoLevel() > (int)infoLevel)
            {
                infoLevel = stat.GetInfoLevel();
            }
        }

        internal override string ForChart()
        {
            return value.ToString("F0");
        }

        internal override string ForTable()
        {
            return value.ToString("#,#");
        }

        internal override bool HasNonZeroValue()
        {
            return value != 0;
        }
    }
}
