namespace TBFlash.AirportStats
{
    internal class AirlineDailyData
    {
        internal FlightStatGroup flightStats = new FlightStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header0"));
        internal FuelStatGroup fuelStats = new FuelStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header2"));
        internal LuggageStatGroup luggageStats = new LuggageStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header3"));
        internal PassengerStatGroup passengerStats = new PassengerStatGroup(i18n.Get("TBFlash.AirportStats.LifetimeStats.header1"));

        internal void RemoveStats(int firstDay, int lastDay)
        {
            flightStats.RemoveAirlineStats(firstDay, lastDay);
            fuelStats.RemoveAirlineStats(firstDay, lastDay);
            luggageStats.RemoveAirlineStats(firstDay, lastDay);
            passengerStats.RemoveAirlineStats(firstDay, lastDay);
        }

        internal string ForTable(PrintOptions printOptions)
        {
            string str = string.Empty;
            str += flightStats.ForTable(printOptions);
            str += passengerStats.ForTable(printOptions);
            str += fuelStats.ForTable(printOptions);
            str += luggageStats.ForTable(printOptions);
            return str;
        }
    }
}
