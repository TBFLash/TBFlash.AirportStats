namespace TBFlash.AirportStats
{
    internal class IntStat : Stat
    {
        private int value;

        internal IntStat(int value, AirportStatUtils.InfoLevels infoLevel = AirportStatUtils.InfoLevels.None)
        {
            this.value = value;
            this.infoLevel = infoLevel;
        }

        internal override void AddToValue(Stat stat)
        {
            value += ((IntStat)stat).value;
            if((int)stat.GetInfoLevel() > (int)infoLevel)
            {
                infoLevel = stat.GetInfoLevel();
            }
        }

        internal int GetValue()
        {
            return value;
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
