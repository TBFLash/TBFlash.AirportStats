using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class TBFlash_AircraftStats
    {
        private readonly int arrayRows = 12;

        internal string GetAircraftStats(AircraftConfig aircraftConfig)
        {
            TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());

            string[,] arr = LoadArray(aircraftConfig);
            string htmlCode = $"<div class=\"modal\"><h1>{arr[0, 1]}</h1><table>";
            for (int i=1; i<arrayRows; i++)
            {
                htmlCode += "<tr>";
                for(int j=0; j<2; j++)
                {
                    htmlCode += $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }
            htmlCode += "</table></div>";
            return htmlCode;
        }

        private string[,] LoadArray(AircraftConfig ac)
        {
            string[,] arr = new string[arrayRows, 2];
            for (int i = 0; i < arrayRows; i++)
            {
                arr[i, 0] = i18n.Get($"TBFlash.AirportStats.AircraftStats.stats{i}");
            }
            arr[0, 1] = ac.DisplayName;
            arr[1, 1] = Game.current.flightScheduler.Today.Count(x => ac.DisplayName.Equals(x.flightSchema.aircraftConfig.DisplayName)).ToString("#");
            arr[2, 1] = ac.isDefault ? "No" : "Yes";
            arr[3, 1] = ac.Capacity.ToString("#");
            arr[4, 1] = ac.nFlightAttendants.ToString("#");
            arr[5, 1] = ac.nPilots.ToString("#");
            arr[6, 1] = ac.CrewChangeChanceModifier.ToString("P0");
            arr[7, 1] = (ac.FuelCapacity/1000).ToString("#,###");
            arr[8, 1] = ac.MinRunwaySize.ToString("#,###");
            arr[9, 1] = ac.MinGateSize.ToString();
            string airlineString = string.Empty;
            foreach (Airline airline in Game.current.airlines)
            {
                if (airline.AircraftInFleet.Contains(ac.isDefault ? ac.DisplayName : ac.ReferenceID))
                {
                    TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());
                    airlineString += airline.name + "<br/>";
                }
            }
            arr[10, 1] = airlineString;
            arr[11, 1] = ac.DisplayShortName;
            return arr;
        }
    }
}
