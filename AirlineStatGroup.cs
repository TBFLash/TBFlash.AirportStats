using System.Collections.Generic;
using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class AirlineStatGroup
    {
        internal AirlineStats<StringStat> airlineName = new AirlineStats<StringStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats0"));
        internal AirlineStats<BoolStat> includeInSatisfaction = new AirlineStats<BoolStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats1"));
        internal AirlineStats<PercentageStat> interest = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats2"));
        internal AirlineStats<PercentageStat> communication = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats3"));
        internal AirlineStats<PercentageStat> fuelSatisfaction = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats4"));
        internal AirlineStats<PercentageStat> paxSatisfaction = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats5"));
        internal AirlineStats<PercentageStat> fees = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats6"));
        internal AirlineStats<PercentageStat> reliability = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats7"));
        internal AirlineStats<PercentageStat> trust = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats8"));
        internal AirlineStats<PercentageStat> facilityQuality = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats9"));
        internal AirlineStats<NumberStat> nAcceptedOffers = new AirlineStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats10"));
        internal AirlineStats<PercentageStat> baseRefuelPercentage = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats11"));
        internal AirlineStats<PercentageStat> firstClassPercentage = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats12"));
        internal AirlineStats<MoneyStat> newFlightBonus = new AirlineStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats13"));
        internal AirlineStats<NumberStat> peakFlightCount = new AirlineStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats14"));
        internal AirlineStats<NumberStat> nReps = new AirlineStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats15"));
        internal AirlineStats<BoolStat> hasDeal = new AirlineStats<BoolStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats16"));

        internal AirlineStats<MoneyStat> runwayFees = new AirlineStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats17"));
        internal AirlineStats<MoneyStat> terminalFees = new AirlineStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats18"));
        internal AirlineStats<MoneyStat> dailyFees = new AirlineStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats19"));
        internal AirlineStats<PercentageStat> fuelSatisfactionNegotiated = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats20"));
        internal AirlineStats<PercentageStat> reliabilityNegotiated = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats21"));
        internal AirlineStats<NumberStat> offices = new AirlineStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats22"));
        internal AirlineStats<BoolStat> conferenceRoom = new AirlineStats<BoolStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats23"));
        internal AirlineStats<NumberStat> stores = new AirlineStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats24"));
        internal AirlineStats<PercentageStat> storeShare = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats25"));
        internal AirlineStats<NumberStat> cafes = new AirlineStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats26"));
        internal AirlineStats<PercentageStat> cafeShare = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats27"));
        internal AirlineStats<NumberStat> firstClassLounges = new AirlineStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats28"));
        internal AirlineStats<NumberStat> flightCrewLounges = new AirlineStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats29"));
        internal AirlineStats<NumberStat> smallGates = new AirlineStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats30"));
        internal AirlineStats<NumberStat> largeGates = new AirlineStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats31"));
        internal AirlineStats<NumberStat> XLGates = new AirlineStats<NumberStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats32"));
        internal AirlineStats<PercentageStat> paxPercent = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats33"));
        internal AirlineStats<MoneyStat> penalty = new AirlineStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats34"));

        internal AirlineStatGroup()
        {
        }

        private IEnumerable<string> FindActiveAirlines(IDictionary<string, Stat> dict)
        {
            foreach(string key in dict.Keys)
            {
                if(((BoolStat)dict[key]).GetValue())
                {
                    yield return key;
                }
            }
        }

        internal string ForTable(PrintOptions printOptions = null)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            IEnumerable<string> list = printOptions.ActiveOnly ? FindActiveAirlines(includeInSatisfaction.stats) : includeInSatisfaction.stats.Keys;
            IOrderedEnumerable<string> airlineList = list.OrderBy(x => x);
            string str = string.Empty;
            str += "<thead>";
            str += airlineName.ForTable(airlineList, true);
            str += "</thead><tbody>";
            str += includeInSatisfaction.ForTable(airlineList);
            str += interest.ForTable(airlineList);
            str += communication.ForTable(airlineList);
            str += fuelSatisfaction.ForTable(airlineList);
            str += paxSatisfaction.ForTable(airlineList);
            str += fees.ForTable(airlineList);
            str += reliability.ForTable(airlineList);
            str += trust.ForTable(airlineList);
            str += facilityQuality.ForTable(airlineList);
            str += nAcceptedOffers.ForTable(airlineList);
            str += baseRefuelPercentage.ForTable(airlineList);
            str += firstClassPercentage.ForTable(airlineList);
            str += newFlightBonus.ForTable(airlineList);
            str += peakFlightCount.ForTable(airlineList);
            str += nReps.ForTable(airlineList);
            str += hasDeal.ForTable(airlineList);

            str += runwayFees.ForTable(airlineList);
            str += terminalFees.ForTable(airlineList);
            str += dailyFees.ForTable(airlineList);
            str += fuelSatisfactionNegotiated.ForTable(airlineList);
            str += reliabilityNegotiated.ForTable(airlineList);
            str += offices.ForTable(airlineList);
            str += conferenceRoom.ForTable(airlineList);
            str += stores.ForTable(airlineList);
            str += storeShare.ForTable(airlineList);
            str += cafes.ForTable(airlineList);
            str += cafeShare.ForTable(airlineList);
            str += firstClassLounges.ForTable(airlineList);
            str += flightCrewLounges.ForTable(airlineList);
            str += smallGates.ForTable(airlineList);
            str += largeGates.ForTable(airlineList);
            str += XLGates.ForTable(airlineList);
            str += paxPercent.ForTable(airlineList);
            str += penalty.ForTable(airlineList);
            str += "</tbody>";
            return str;
        }
    }
}
