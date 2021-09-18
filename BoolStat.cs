namespace TBFlash.AirportStats
{
    internal class BoolStat : Stat
    {
        private bool value;

        internal BoolStat(bool value, AirportStatUtils.InfoLevels infoLevel = AirportStatUtils.InfoLevels.None)
        {
            this.value = value;
            this.infoLevel = infoLevel;
        }

        internal bool GetValue()
        {
            return value;
        }

        internal override void AddToValue(Stat stat)
        {
            value = ((BoolStat)stat).value;
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
            return value ? "Yes" : "No";
        }

        internal override bool HasNonZeroValue()
        {
            return true;
        }
        internal override float GetFloatValue()
        {
            return (float) (value ? 1 : 0);
        }
    }
}
