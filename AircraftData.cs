using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class AircraftData
    {
        internal LifetimeOnlyStats<StringStat> aircraftName = new LifetimeOnlyStats<StringStat>(i18n.Get("TBFlash.AirportStats.AircraftStats.stats0"));
        internal LifetimeOnlyStats<IntStat> nFlights = new LifetimeOnlyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AircraftStats.stats1"));
        internal LifetimeOnlyStats<BoolStat> isMod = new LifetimeOnlyStats<BoolStat>(i18n.Get("TBFlash.AirportStats.AircraftStats.stats2"));
        internal LifetimeOnlyStats<IntStat> paxCapacity = new LifetimeOnlyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AircraftStats.stats3"));
        internal LifetimeOnlyStats<IntStat> nFlightAttendants = new LifetimeOnlyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AircraftStats.stats4"));
        internal LifetimeOnlyStats<IntStat> nPilots = new LifetimeOnlyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AircraftStats.stats5"));
        internal LifetimeOnlyStats<PercentageStat> crewChangeModifier = new LifetimeOnlyStats<PercentageStat>(i18n.Get("TBFlash.AirportStats.AircraftStats.stats6"));
        internal LifetimeOnlyStats<IntStat> fuelCapacity = new LifetimeOnlyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AircraftStats.stats7"));
        internal LifetimeOnlyStats<IntStat> minRunwaySize = new LifetimeOnlyStats<IntStat>(i18n.Get("TBFlash.AirportStats.AircraftStats.stats8"));
        internal LifetimeOnlyStats<StringStat> minGateSize = new LifetimeOnlyStats<StringStat>(i18n.Get("TBFlash.AirportStats.AircraftStats.stats9"));
        internal LifetimeOnlyStats<StringStat> airlinesUsing = new LifetimeOnlyStats<StringStat>(i18n.Get("TBFlash.AirportStats.AircraftStats.stats10"));
        internal LifetimeOnlyStats<StringStat> shortName = new LifetimeOnlyStats<StringStat>(i18n.Get("TBFlash.AirportStats.AircraftStats.stats11"));

        internal void LoadAircraft(AircraftConfig ac)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            aircraftName.AddStat(new StringStat(ac.DisplayName));
            nFlights.AddStat(new IntStat(Game.current.flightScheduler.Today.Count(x => ac.DisplayName.Equals(x.flightSchema.aircraftConfig.DisplayName))));
            isMod.AddStat(new BoolStat(ac.isDefault));
            paxCapacity.AddStat(new IntStat(ac.Capacity));
            nFlightAttendants.AddStat(new IntStat(ac.nFlightAttendants));
            nPilots.AddStat(new IntStat(ac.nPilots));
            crewChangeModifier.AddStat(new PercentageStat(ac.CrewChangeChanceModifier));
            fuelCapacity.AddStat(new IntStat(ac.FuelCapacity / 1000));
            minRunwaySize.AddStat(new IntStat(ac.MinRunwaySize));
            minGateSize.AddStat(new StringStat(ac.MinGateSize.ToString()));
            string airlineString = string.Empty;
            foreach (Airline airline in Game.current.airlines)
            {
                if (airline.AircraftInFleet.Contains(ac.isDefault ? ac.DisplayName : ac.ReferenceID))
                {
                    airlineString += airline.name + "<br/>";
                }
            }
            airlinesUsing.AddStat(new StringStat(airlineString));
            shortName.AddStat(new StringStat(ac.DisplayShortName));
        }

        internal string ForTable()
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            string str = string.Empty;
            str += aircraftName.ForTable();
            str += nFlights.ForTable();
            str += isMod.ForTable();
            str += paxCapacity.ForTable();
            str += nFlightAttendants.ForTable();
            str += nPilots.ForTable();
            str += crewChangeModifier.ForTable();
            str += fuelCapacity.ForTable();
            str += minRunwaySize.ForTable();
            str += minGateSize.ForTable();
            str += airlinesUsing.ForTable();
            str += shortName.ForTable();
            return str;
        }
    }
}
