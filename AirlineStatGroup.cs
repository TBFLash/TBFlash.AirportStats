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
        internal AirlineStats<IntStat> nAcceptedOffers = new AirlineStats<IntStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats10"));
        internal AirlineStats<PercentageStat> baseRefuelPercentage = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats11"));
        internal AirlineStats<PercentageStat> firstClassPercentage = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats12"));
        internal AirlineStats<MoneyStat> newFlightBonus = new AirlineStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats13"));
        internal AirlineStats<IntStat> peakFlightCount = new AirlineStats<IntStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats14"));
        internal AirlineStats<IntStat> nReps = new AirlineStats<IntStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats15"));
        internal AirlineStats<BoolStat> hasDeal = new AirlineStats<BoolStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats16"));

        internal AirlineStats<MoneyStat> runwayFees = new AirlineStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats17"));
        internal AirlineStats<MoneyStat> terminalFees = new AirlineStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats18"));
        internal AirlineStats<MoneyStat> dailyFees = new AirlineStats<MoneyStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats19"));
        internal AirlineStats<PercentageStat> fuelSatisfactionNegotiated = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats20"));
        internal AirlineStats<PercentageStat> reliabilityNegotiated = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats21"));
        internal AirlineStats<IntStat> offices = new AirlineStats<IntStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats22"));
        internal AirlineStats<BoolStat> conferenceRoom = new AirlineStats<BoolStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats23"));
        internal AirlineStats<IntStat> stores = new AirlineStats<IntStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats24"));
        internal AirlineStats<PercentageStat> storeShare = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats25"));
        internal AirlineStats<IntStat> cafes = new AirlineStats<IntStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats26"));
        internal AirlineStats<PercentageStat> cafeShare = new AirlineStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats27"));
        internal AirlineStats<IntStat> firstClassLounges = new AirlineStats<IntStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats28"));
        internal AirlineStats<IntStat> flightCrewLounges = new AirlineStats<IntStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats29"));
        internal AirlineStats<IntStat> smallGates = new AirlineStats<IntStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats30"));
        internal AirlineStats<IntStat> largeGates = new AirlineStats<IntStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats31"));
        internal AirlineStats<IntStat> XLGates = new AirlineStats<IntStat>(i18n.Get("TBFlash.AirportStats.AllAirlineStats.stats32"));
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
            str += airlineName.ForTable(airlineList, false, true);
            str += "</thead><tbody>";
            str += includeInSatisfaction.ForTable(airlineList, true);
            str += interest.ForTable(airlineList);
            str += communication.ForTable(airlineList, true);
            str += fuelSatisfaction.ForTable(airlineList);
            str += paxSatisfaction.ForTable(airlineList, true);
            str += fees.ForTable(airlineList);
            str += reliability.ForTable(airlineList, true);
            str += trust.ForTable(airlineList);
            str += facilityQuality.ForTable(airlineList, true);
            str += nAcceptedOffers.ForTable(airlineList);
            str += baseRefuelPercentage.ForTable(airlineList, true);
            str += firstClassPercentage.ForTable(airlineList);
            str += newFlightBonus.ForTable(airlineList, true);
            str += peakFlightCount.ForTable(airlineList);
            str += nReps.ForTable(airlineList, true);
            str += hasDeal.ForTable(airlineList);

            str += runwayFees.ForTable(airlineList, true);
            str += terminalFees.ForTable(airlineList);
            str += dailyFees.ForTable(airlineList, true);
            str += fuelSatisfactionNegotiated.ForTable(airlineList);
            str += reliabilityNegotiated.ForTable(airlineList, true);
            str += offices.ForTable(airlineList);
            str += conferenceRoom.ForTable(airlineList, true);
            str += stores.ForTable(airlineList);
            str += storeShare.ForTable(airlineList, true);
            str += cafes.ForTable(airlineList);
            str += cafeShare.ForTable(airlineList, true);
            str += firstClassLounges.ForTable(airlineList);
            str += flightCrewLounges.ForTable(airlineList, true);
            str += smallGates.ForTable(airlineList);
            str += largeGates.ForTable(airlineList, true);
            str += XLGates.ForTable(airlineList);
            str += paxPercent.ForTable(airlineList, true);
            str += penalty.ForTable(airlineList);
            str += "</tbody>";
            return str;
        }
    }
}
