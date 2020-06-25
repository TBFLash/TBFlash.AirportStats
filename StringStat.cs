namespace TBFlash.AirportStats
{
    internal class StringStat : Stat
    {
        private readonly string value;

        internal StringStat(string value, AirportStatUtils.InfoLevels infoLevel = AirportStatUtils.InfoLevels.None)
        {
            this.value = value;
            this.infoLevel = infoLevel;
        }

        internal override void AddToValue(Stat stat)
        {
            if ((int)stat.GetInfoLevel() > (int)infoLevel)
            {
                infoLevel = stat.GetInfoLevel();
            }
        }

        internal override string ForChart()
        {
            return value;
        }

        internal override string ForTable()
        {
            return value;
        }

        internal override bool HasNonZeroValue()
        {
            return true;
        }
    }
}
