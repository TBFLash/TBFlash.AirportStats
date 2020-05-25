using System.Collections.Generic;
using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class TBFlash_AllAirlineStats
    {
        private readonly int arrayRows = 35;

        internal string GetAllAirlineStats(bool activeOnly = false)
        {
            TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());
            string[,] arr = LoadArray(activeOnly);

            string htmlCode = activeOnly ? TBFlash_Utils.PageHead(TBFlash_Utils.PageTitles.ActiveAirlines) : TBFlash_Utils.PageHead(TBFlash_Utils.PageTitles.AllAirlines);
            htmlCode += "<table>";
            for (int i = 0; i < arrayRows; i++)
            {
                htmlCode += "<tr>";
                for (int j = 0; j < (arr.Length / arrayRows); j++)
                {
                    htmlCode += i == 0 ? $"<th><a href=\"/{arr[i, j]}\">{arr[i, j]}</a></th>" : $"<td>{arr[i, j]}</td>";
                }
                htmlCode += "</tr>";
            }
            htmlCode += "</table>" + TBFlash_Utils.PageFooter();
            return htmlCode;
        }

        private string[,] LoadArray(bool activeOnly = false)
        {
            TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());

            IEnumerable<Airline> airlines = activeOnly ? AirlineManager.AllAirlines().Where(x => x.IncludeInSatisfication) : AirlineManager.AllAirlines();

            string[,] arr = new string[arrayRows, airlines.Count() + 1];
            for (int i = 1; i < arrayRows; i++)
            {
                arr[i, 0] = i18n.Get($"TBFlash.AirportStats.AllAirlineStats.stats{i}");
            }

            int j = 0;
            foreach (Airline airline in airlines.OrderBy(y => y.name))
            {
                j++;
                TBFlash_Utils.TBFlashLogger(Log.FromPool($"j:{j}; Num Airlines:{airlines.Count()}; name: {airline.name}").WithCodepoint());
                if (airline == null)
                {
                    break;
                }
                arr[0, j] = airline.name;
                arr[1, j] = airline.IncludeInSatisfication ? i18n.Get("TBFlash.AirportStats.utils.yes") : i18n.Get("TBFlash.AirportStats.utils.no");
                arr[2, j] = airline.interest.ToString("P1");
                AirlineNeed need = null;
                arr[3, j] = ((airline.Needs?.AllNeeds.TryGetValue("Communication", out need) == true) ? 1f - need.AttenuatedScore : 0f).ToString("P1");
                arr[4, j] = ((airline.Needs?.AllNeeds.TryGetValue("FuelSatisfaction", out need) == true) ? 1f - need.AttenuatedScore : 0f).ToString("P1");
                arr[5, j] = ((airline.Needs?.AllNeeds.TryGetValue("PaxSatisfaction", out need) == true) ? 1f - need.AttenuatedScore : 0f).ToString("P1");
                arr[6, j] = ((airline.Needs?.AllNeeds.TryGetValue("Fees", out need) == true) ? 1f - need.AttenuatedScore : 0f).ToString("P1");
                arr[7, j] = ((airline.Needs?.AllNeeds.TryGetValue("Reliability", out need) == true) ? 1f - need.AttenuatedScore : 0f).ToString("P1");
                arr[8, j] = ((airline.Needs?.AllNeeds.TryGetValue("Trust", out need) == true) ? 1f - need.AttenuatedScore : 0f).ToString("P1");
                arr[9, j] = ((airline.Needs?.AllNeeds.TryGetValue("FacilityQuality", out need) == true) ? 1f - need.AttenuatedScore : 0f).ToString("P1");
                arr[10, j] = airline.nAcceptedOffers.ToString("#");
                arr[11, j] = airline.BaseRefuelPercentage.ToString("P1");
                arr[12, j] = airline.FirstClassPercentage.ToString("P1");
                arr[13, j] = airline.Income_NewFlightBonus_PerFlight.ToString("C0");
                arr[14, j] = airline.PeakFlightsCount.ToString("#");
                arr[15, j] = airline.Reps?.Count.ToString("#") ?? string.Empty;
                arr[16, j] = airline.Needs?.HasDeal == true ? "Yes" : "No";
                /* string airlineStr = string.Empty;
                foreach(string aircraft in airline.AircraftInFleet)
                {
                    airlineStr += aircraft + "<br/>";
                }
                arr[35, j] = airlineStr;*/
                if (airline.Needs?.HasDeal == true)
                {
                    arr[17, j] = airline.Needs.NegotiatedRunwayFees.ToString("C0");
                    arr[18, j] = airline.Needs.NegotiatedTerminalFees.ToString("C0");
                    arr[19, j] = airline.Needs.NegotiatedDailyFees.ToString("C0");
                    arr[20, j] = (airline.Needs.AllNeeds.TryGetValue("NegotiatedFuelSatisfaction", out need) ? 1f - ((double)need.target / 100) : 0f).ToString("P1");
                    arr[21, j] = (airline.Needs.AllNeeds.TryGetValue("NegotiatedReliabilty", out need) ? 1f - ((double)need.target / 100) : 0f).ToString("P1");
                    arr[22, j] = airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.Office).ToString("#");
                    arr[23, j] = airline.Needs.Conference != null ? i18n.Get("TBFlash.AirlineStats.utils.yes") : i18n.Get("TBFlash.AirlineStats.utils.no");
                    arr[24, j] = airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.Store).ToString("#");
                    arr[25, j] = (((double)airline.Needs.NegotiatedStoreShare)/100f).ToString("P1");
                    arr[26, j] = airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.Cafe).ToString("#");
                    arr[27, j] = (((double)airline.Needs.NegotiatedCafeShare)/100f).ToString("P1");
                    arr[28, j] = airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.First_Class_Lounge).ToString("#");
                    arr[29, j] = airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.Flight_Crew_Lounge).ToString("#");
                    arr[30, j] = Game.current.objectCache.AircraftGate_All.All().Count(x => x.Owner == airline && x.Size == AircraftGate.GateSize.Small).ToString("#");
                    arr[31, j] = Game.current.objectCache.AircraftGate_All.All().Count(x => x.Owner == airline && x.Size == AircraftGate.GateSize.Large).ToString("#");
                    arr[32, j] = Game.current.objectCache.AircraftGate_All.All().Count(x => x.Owner == airline && x.Size == AircraftGate.GateSize.Extra_Large).ToString("#");
                    arr[33, j] = ((double)airline.Needs.NegotiatedPaxPercent / 100).ToString("P0");
                    arr[34, j] = airline.Needs.NegotiatedPenalty.ToString("C0");
                }
            }
            return arr;
        }
    }
}
