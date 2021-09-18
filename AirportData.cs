using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class AirportData
    {
        internal FlightStatGroup flightStats = new FlightStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header0"));
        internal FuelStatGroup fuelStats = new FuelStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header2"));
        internal LuggageStatGroup luggageStats = new LuggageStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header3"));
        internal PassengerStatGroup passengerStats = new PassengerStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header1"));
        internal RevAndExpStatGroup revAndExpStats = new RevAndExpStatGroup("Revenue and Expenses");
        internal StaffStatGroup staffStats = new StaffStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header6"));
        internal TimeStatGroup timeStats = new TimeStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header7"));
        internal InteractionsStatGroup interactions = new InteractionsStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header8"));
        internal ProfitStatGroup profitStats = new ProfitStatGroup(i18n.Get("TBFlash.AirportStats.json.profitStats"));

        internal AirlineStatGroup airlineStats = new AirlineStatGroup();
        internal FuelFutures fuelFutures = new FuelFutures("FuelFutures");

        internal void RemoveAirlineDataStats(int firstDay, int lastDay)
        {
            flightStats.RemoveAirlineStats(firstDay, lastDay);
            fuelStats.RemoveAirlineStats(firstDay, lastDay);
            luggageStats.RemoveAirlineStats(firstDay, lastDay);
            passengerStats.RemoveAirlineStats(firstDay, lastDay);
            revAndExpStats.RemoveAirlineStats(firstDay, lastDay);
            profitStats.RemoveAirlineStats(firstDay, lastDay);
        }

        internal void ResetLifetimeStats()
        {
            staffStats = new StaffStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header6"));
            timeStats = new TimeStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header7"));
            interactions = new InteractionsStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header8"));
            RemoveAirlineDataStats(0, 0);
        }

        internal void ResetAirlineStats()
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            //if (airlineStats.airlineName.stats.Count > 0)
            //{
            airlineStats = new AirlineStatGroup();
            //}
        }

        internal string ForTable(PrintOptions printOptions = null)
        {
            string str = string.Empty;
            str += flightStats.ForTable(printOptions);
            str += passengerStats.ForTable(printOptions);
            str += fuelStats.ForTable(printOptions);
            str += luggageStats.ForTable(printOptions);
            str += revAndExpStats.ForTable(printOptions);
            str += profitStats.ForTable(printOptions);
            str += staffStats.ForTable(printOptions);
            str += timeStats.ForTable(printOptions);
            str += interactions.ForTable(printOptions);
            return str;
        }
    }
}
