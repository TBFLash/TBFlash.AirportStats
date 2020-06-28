namespace TBFlash.AirportStats
{
    internal class DoubleStat : Stat
    {
        private double value;

        internal DoubleStat(double value, AirportStatUtils.InfoLevels infoLevel = AirportStatUtils.InfoLevels.None)
        {
            this.value = value;
            this.infoLevel = infoLevel;
        }

        internal override void AddToValue(Stat stat)
        {
            value += ((DoubleStat)stat).value;
            if((int)stat.GetInfoLevel() > (int)infoLevel)
            {
                infoLevel = stat.GetInfoLevel();
            }
        }

        internal override string ForChart()
        {
            return value.ToString("F2");
        }

        internal override string ForTable()
        {
            return value.ToString("#,#.##");
        }

        internal override bool HasNonZeroValue()
        {
            return value != 0;
        }
    }
}
