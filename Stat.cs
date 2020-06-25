namespace TBFlash.AirportStats
{
    internal abstract class Stat
    {
        protected AirportStatUtils.InfoLevels infoLevel;

        internal AirportStatUtils.InfoLevels GetInfoLevel()
        {
            return infoLevel;
        }

        internal abstract bool HasNonZeroValue();

        internal abstract void AddToValue(Stat stat);

        internal abstract string ForChart();

        internal abstract string ForTable();
    }
}
