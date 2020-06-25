using System.Collections.Generic;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class AirlineData
    {
        private readonly Dictionary<string, AirlineDailyData> airlineDailyData = new Dictionary<string, AirlineDailyData>();

        internal AirlineDailyData GetAirlineDailyData(string airlineName)
        {
            if (!airlineDailyData.TryGetValue(airlineName, out AirlineDailyData thisAirline))
            {
                thisAirline = new AirlineDailyData();
                airlineDailyData.Add(airlineName, thisAirline);
            }
            return thisAirline;
        }

        internal string ForTable(PrintOptions printOptions = null)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool($"printOptions: AirlineName: {printOptions.AirlineName}; lifetime: {printOptions.IncludeLifetime}; first: {printOptions.FirstDay}; last: {printOptions.LastDay}").WithCodepoint());

            return airlineDailyData.TryGetValue(printOptions.AirlineName, out AirlineDailyData airlineData) ? airlineData.ForTable(printOptions) : string.Empty;
        }
    }
}
