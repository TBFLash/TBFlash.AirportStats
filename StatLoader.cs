using System;
using System.Collections.Generic;
using System.Linq;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal static class StatLoader
    {
        static internal AirlineData airlineData = new AirlineData();
        static internal AirportData airportData = new AirportData();
        static internal AircraftData aircraftData = new AircraftData();
        static internal FlightData flightData = new FlightData();

        static internal int FirstDay { get; private set; }
        static internal int LastDay { get; private set; }
        static internal int LastFlightLoad { get; private set; }
        internal static bool FirstRun { get; set; } = true;

        static internal void Init()
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            FirstDay = GameTimer.Day > 30 ? GameTimer.Day - 29 : 1;
            LastDay = FirstDay + 1;
            LastFlightLoad = FirstDay + 1;
        }

        static internal void UpdateAircraftStats(AircraftConfig ac)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            aircraftData.LoadAircraft(ac);
        }

        static internal void UpdateAirlineData()
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            LoadAirlineData(LastDay == 1 ? 1 : LastDay - 1, GameTimer.Day);
            LoadAirportLifetimeStats();
            LastDay = GameTimer.Day;
        }

        static internal void UpdateAirlineStats()
        {
            LoadAirlineStats(true);
        }

        static internal void UpdateFlights()
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());
            LoadFlights(LastFlightLoad == 1 ? 1 : LastFlightLoad - 1, GameTimer.Day);
            LastFlightLoad = GameTimer.Day;
        }

        static private void LoadAirlineData(int start, int end)
        {
            AirportStatUtils.AirportStatsLogger(Log.FromPool($"start: {start}; end: {end}").WithCodepoint());
            airportData.RemoveAirlineDataStats(start, end);
            Dictionary<AircraftGate.GateSize, int> gateSizes = new Dictionary<AircraftGate.GateSize, int>
            {
                { AircraftGate.GateSize.Small, 0 },
                { AircraftGate.GateSize.Large, 0 },
                { AircraftGate.GateSize.Extra_Large, 0 }
            };

            foreach (Airline airline in AirlineManager.AllAirlines())
            {
                AirlineDailyData thisAirline = airlineData.GetAirlineDailyData(airline.name);
                thisAirline.RemoveStats(start, end);
                for (int i = end; i >= start; i--)
                {
                    IEnumerable<FlightRecord> flightRecords = Game.current.flightRecords.GetForDay(i - 1).Where(x => x.airline == airline.name);
                    if (flightRecords?.Any() != true)
                    {
                        continue;
                    }
                    thisAirline.passengerStats.nArriving.AddStat(i, new NumberStat(flightRecords.Sum(x => x.nArriving)));
                    thisAirline.passengerStats.nBoarded.AddStat(i, new NumberStat(flightRecords.Sum(x => x.nBoarded)));
                    thisAirline.passengerStats.nCheckedIn.AddStat(i, new NumberStat(flightRecords.Sum(x => x.nCheckedIn)));
                    thisAirline.passengerStats.nSchedDep.AddStat(i, new NumberStat(flightRecords.Sum(x => x.nDeparting)));
                    int missed = flightRecords.Sum(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Departed) || AirportStatUtils.HasStatus(x.status, Flight.Status.Canceled) ? x.nDeparting - x.nBoarded : 0);
                    thisAirline.passengerStats.nMissed.AddStat(i, new NumberStat(missed > 0 ? missed : 0, missed > 0 ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None));
                    thisAirline.passengerStats.timeBoarding.AddStat(i, new TimeStat(flightRecords.Sum(x => x.nBoarded > 0 ? x.time_boarding : 0) * 60f));
                    thisAirline.passengerStats.timeDeplaning.AddStat(i, new TimeStat(flightRecords.Sum(x => x.time_deplaning) * 60f));

                    airportData.passengerStats.nArriving.AddToValue(i, new NumberStat(flightRecords.Sum(x => x.nArriving)));
                    airportData.passengerStats.nBoarded.AddToValue(i, new NumberStat(flightRecords.Sum(x => x.nBoarded)));
                    airportData.passengerStats.nCheckedIn.AddToValue(i, new NumberStat(flightRecords.Sum(x => x.nCheckedIn)));
                    airportData.passengerStats.nSchedDep.AddToValue(i, new NumberStat(flightRecords.Sum(x => x.nDeparting)));
                    airportData.passengerStats.nMissed.AddToValue(i, new NumberStat(missed > 0 ? missed : 0, missed > 0 ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None));
                    airportData.passengerStats.timeBoarding.AddToValue(i, new TimeStat(flightRecords.Sum(x => x.nBoarded > 0 ? x.time_boarding : 0) * 60f));
                    airportData.passengerStats.timeDeplaning.AddToValue(i, new TimeStat(flightRecords.Sum(x => x.time_deplaning) * 60f));

                    int fuelDel = Convert.ToInt32(flightRecords.Sum(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Departed) ? x.nFuelRefueled : 0) / 1000);
                    int fuelReq = Convert.ToInt32(flightRecords.Sum(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Departed) ? x.nFuelRequested : 0) / 1000);
                    thisAirline.fuelStats.fuelDelivered.AddStat(i, new NumberStat(fuelDel, fuelDel < fuelReq ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None));
                    thisAirline.fuelStats.fuelRequested.AddStat(i, new NumberStat(fuelReq));
                    int fuelFailures = flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Departed) && x.nFuelRequested > 0 && x.nFuelRefueled == 0);
                    thisAirline.fuelStats.fuelingFailures.AddStat(i, new NumberStat(fuelFailures, fuelFailures > 0 ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None));
                    thisAirline.fuelStats.planesRefueled.AddStat(i, new NumberStat(flightRecords.Count(x => x.nFuelRefueled > 0)));

                    airportData.fuelStats.fuelDelivered.AddToValue(i, new NumberStat(fuelDel, fuelDel < fuelReq ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None));
                    airportData.fuelStats.fuelRequested.AddToValue(i, new NumberStat(fuelReq));
                    airportData.fuelStats.fuelingFailures.AddToValue(i, new NumberStat(fuelFailures, fuelFailures > 0 ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None));
                    airportData.fuelStats.planesRefueled.AddToValue(i, new NumberStat(flightRecords.Count(x => x.nFuelRefueled > 0)));

                    thisAirline.luggageStats.arrivingBags.AddStat(i, new NumberStat(flightRecords.Sum(x => x.nArrivalBags)));
                    thisAirline.luggageStats.bagsLoaded.AddStat(i, new NumberStat(flightRecords.Sum(x => x.nBagsLoaded)));
                    thisAirline.luggageStats.bagsUnloaded.AddStat(i, new NumberStat(flightRecords.Sum(x => x.nBagsUnloaded)));
                    thisAirline.luggageStats.departingBags.AddStat(i, new NumberStat(flightRecords.Sum(x => x.nDepartingBags)));
                    int lostBaggage = flightRecords.Sum(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Departed) ? x.nDepartingBags - x.nBagsLoaded : 0);
                    thisAirline.luggageStats.lostBags.AddStat(i, new NumberStat(lostBaggage, lostBaggage > 0 ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None));
                    thisAirline.luggageStats.timeLoadingBags.AddStat(i, new TimeStat(flightRecords.Sum(x => x.nBagsLoaded > 0 ? x.time_bag_load : 0) * 60f));

                    airportData.luggageStats.arrivingBags.AddToValue(i, new NumberStat(flightRecords.Sum(x => x.nArrivalBags)));
                    airportData.luggageStats.bagsLoaded.AddToValue(i, new NumberStat(flightRecords.Sum(x => x.nBagsLoaded)));
                    airportData.luggageStats.bagsUnloaded.AddToValue(i, new NumberStat(flightRecords.Sum(x => x.nBagsUnloaded)));
                    airportData.luggageStats.departingBags.AddToValue(i, new NumberStat(flightRecords.Sum(x => x.nDepartingBags)));
                    airportData.luggageStats.lostBags.AddToValue(i, new NumberStat(lostBaggage, lostBaggage > 0 ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None));
                    airportData.luggageStats.timeLoadingBags.AddToValue(i, new TimeStat(flightRecords.Sum(x => x.nBagsLoaded > 0 ? x.time_bag_load : 0) * 60f));

                    thisAirline.flightStats.nSchedFlights.AddStat(i, new NumberStat(flightRecords.Count()));
                    thisAirline.flightStats.nDelayedArrival.AddStat(i, new NumberStat(flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.DelayedArrival))));
                    thisAirline.flightStats.nRequiresCrew.AddStat(i, new NumberStat(flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.RequiresCrew))));
                    thisAirline.flightStats.nOntimeDeparture.AddStat(i, new NumberStat(flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Departed) && !AirportStatUtils.HasStatus(x.status, Flight.Status.DelayedDeparture))));
                    int delDep = flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.DelayedDeparture));
                    thisAirline.flightStats.nDelayedDeparture.AddStat(i, new NumberStat(delDep, delDep > 0 ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None));
                    int canx = flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Canceled));
                    thisAirline.flightStats.nCancelled.AddStat(i, new NumberStat(canx, canx > 0 ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None));
                    thisAirline.flightStats.nAirportInvalid.AddStat(i, new NumberStat(flightRecords.Count(x => x.reason == Flight.StatusReason.AirportInvalid)));
                    thisAirline.flightStats.nWeather.AddStat(i, new NumberStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Weather)));
                    thisAirline.flightStats.nRunway.AddStat(i, new NumberStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Runway)));
                    thisAirline.flightStats.nGate.AddStat(i, new NumberStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Gate)));
                    thisAirline.flightStats.nExpired.AddStat(i, new NumberStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Expired)));
                    thisAirline.flightStats.nReneged.AddStat(i, new NumberStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Reneged)));

                    airportData.flightStats.nSchedFlights.AddToValue(i, new NumberStat(flightRecords.Count()));
                    airportData.flightStats.nDelayedArrival.AddToValue(i, new NumberStat(flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.DelayedArrival))));
                    airportData.flightStats.nRequiresCrew.AddToValue(i, new NumberStat(flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.RequiresCrew))));
                    airportData.flightStats.nOntimeDeparture.AddToValue(i, new NumberStat(flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Departed) && !AirportStatUtils.HasStatus(x.status, Flight.Status.DelayedDeparture))));
                    airportData.flightStats.nDelayedDeparture.AddToValue(i, new NumberStat(delDep, delDep > 0 ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None));
                    airportData.flightStats.nCancelled.AddToValue(i, new NumberStat(canx, canx > 0 ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None));
                    airportData.flightStats.nAirportInvalid.AddToValue(i, new NumberStat(flightRecords.Count(x => x.reason == Flight.StatusReason.AirportInvalid)));
                    airportData.flightStats.nWeather.AddToValue(i, new NumberStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Weather)));
                    airportData.flightStats.nRunway.AddToValue(i, new NumberStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Runway)));
                    airportData.flightStats.nGate.AddToValue(i, new NumberStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Gate)));
                    airportData.flightStats.nExpired.AddToValue(i, new NumberStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Expired)));
                    airportData.flightStats.nReneged.AddToValue(i, new NumberStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Reneged)));

                    gateSizes[AircraftGate.GateSize.Small] = 0;
                    gateSizes[AircraftGate.GateSize.Large] = 0;
                    gateSizes[AircraftGate.GateSize.Extra_Large] = 0;
                    foreach (FlightRecord fr in flightRecords)
                    {
                        AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());

                        AircraftConfig ac = AircraftConfigManager.FindByAnyName(fr.aircraft, false);
                        if (ac != null)
                        {
                            AirportStatUtils.AirportStatsLogger(Log.FromPool(ac.MinGateSize.ToString()+"|"+gateSizes[ac.MinGateSize]).WithCodepoint());

                            gateSizes[ac.MinGateSize]++;
                        }
                    }
                    foreach (KeyValuePair<AircraftGate.GateSize, int> kvp in gateSizes)
                    {
                        AirportStatUtils.AirportStatsLogger(Log.FromPool("").WithCodepoint());

                        switch (kvp.Key)
                        {
                            case AircraftGate.GateSize.Small:
                                thisAirline.flightStats.nSmallGates.AddStat(i, new NumberStat(kvp.Value));
                                airportData.flightStats.nSmallGates.AddToValue(i, new NumberStat(kvp.Value));
                                break;
                            case AircraftGate.GateSize.Large:
                                thisAirline.flightStats.nLargeGates.AddStat(i, new NumberStat(kvp.Value));
                                airportData.flightStats.nLargeGates.AddToValue(i, new NumberStat(kvp.Value));
                                break;
                            case AircraftGate.GateSize.Extra_Large:
                                thisAirline.flightStats.nXLGates.AddStat(i, new NumberStat(kvp.Value));
                                airportData.flightStats.nXLGates.AddToValue(i, new NumberStat(kvp.Value));
                                break;
                        }
                    }
                }
            }

            for (int i = end; i >= start; i--)
            {
                Game.current.GameReports.TryGetValue(i, out GamedayReportingData gamedayData);
                if (gamedayData == null)
                {
                    continue;
                }
                airportData.passengerStats.nConnecting.AddStat(i, new NumberStat(gamedayData.NumConnectPax));
                airportData.fuelStats.avgFuelPrice.AddStat(i, new MoneyStat(GetAverageFuelCost(i), 2));
                GamedayReportingData.MoneyCategory category;
                float val;
                foreach (string name in Enum.GetNames(typeof(GamedayReportingData.MoneyCategory)))
                {
                    category = (GamedayReportingData.MoneyCategory)Enum.Parse(typeof(GamedayReportingData.MoneyCategory), name);
                    if ((val = GetDailyMoneyTotal(gamedayData, category, true)) != 0)
                    {
                        airportData.revAndExpStats.revenueStats.AddStat(name, i, new MoneyStat(val));
                    }
                    if ((val = GetDailyMoneyTotal(gamedayData, category, false)) != 0)
                    {
                        airportData.revAndExpStats.expenseStats.AddStat(name, i, new MoneyStat(val));
                    }
                }
                if ((val = GetDailyMoneyTotal(gamedayData, true)) != 0)
                {
                    airportData.revAndExpStats.revenueStats.AddStat("total", i, new MoneyStat(val));
                }
                if ((val = GetDailyMoneyTotal(gamedayData, false)) != 0)
                {
                    airportData.revAndExpStats.expenseStats.AddStat("total", i, new MoneyStat(val));
                }
            }
        }

        static private void LoadAirlineStats(bool deletePrevious = true)
        {
            if (deletePrevious)
            {
                airportData.ResetAirlineStats();
            }
            foreach (Airline airline in AirlineManager.AllAirlines())
            {
                airportData.airlineStats.airlineName.AddStat(airline.name, new StringStat(airline.name));
                airportData.airlineStats.includeInSatisfaction.AddStat(airline.name, new BoolStat(airline.IncludeInSatisfication));
                airportData.airlineStats.interest.AddStat(airline.name, new PercentageStat(airline.interest));
                AirlineNeed need = null;
                airportData.airlineStats.communication.AddStat(airline.name, new PercentageStat((airline.Needs?.AllNeeds.TryGetValue("Communication", out need) == true) ? 1f - need.AttenuatedScore : 0f));
                airportData.airlineStats.fuelSatisfaction.AddStat(airline.name, new PercentageStat((airline.Needs?.AllNeeds.TryGetValue("FuelSatisfaction", out need) == true) ? 1f - need.AttenuatedScore : 0f));
                airportData.airlineStats.paxSatisfaction.AddStat(airline.name, new PercentageStat((airline.Needs?.AllNeeds.TryGetValue("PaxSatisfaction", out need) == true) ? 1f - need.AttenuatedScore : 0f));
                airportData.airlineStats.fees.AddStat(airline.name, new PercentageStat((airline.Needs?.AllNeeds.TryGetValue("Fees", out need) == true) ? 1f - need.AttenuatedScore : 0f));
                airportData.airlineStats.reliability.AddStat(airline.name, new PercentageStat((airline.Needs?.AllNeeds.TryGetValue("Reliability", out need) == true) ? 1f - need.AttenuatedScore : 0f));
                airportData.airlineStats.trust.AddStat(airline.name, new PercentageStat((airline.Needs?.AllNeeds.TryGetValue("Trust", out need) == true) ? 1f - need.AttenuatedScore : 0f));
                airportData.airlineStats.facilityQuality.AddStat(airline.name, new PercentageStat((airline.Needs?.AllNeeds.TryGetValue("FacilityQuality", out need) == true) ? 1f - need.AttenuatedScore : 0f));
                airportData.airlineStats.nAcceptedOffers.AddStat(airline.name, new NumberStat(airline.nAcceptedOffers));
                airportData.airlineStats.baseRefuelPercentage.AddStat(airline.name, new PercentageStat(airline.BaseRefuelPercentage));
                airportData.airlineStats.firstClassPercentage.AddStat(airline.name, new PercentageStat(airline.FirstClassPercentage));
                airportData.airlineStats.newFlightBonus.AddStat(airline.name, new MoneyStat((float)airline.Income_NewFlightBonus_PerFlight));
                airportData.airlineStats.peakFlightCount.AddStat(airline.name, new NumberStat(airline.PeakFlightsCount));
                airportData.airlineStats.nReps.AddStat(airline.name, new NumberStat(airline.Reps?.Count ?? 0));
                airportData.airlineStats.hasDeal.AddStat(airline.name, new BoolStat(airline.Needs?.HasDeal ?? false));

                if (airline.Needs?.HasDeal ?? false)
                {
                    airportData.airlineStats.runwayFees.AddStat(airline.name, new MoneyStat(airline.Needs.NegotiatedRunwayFees));
                    airportData.airlineStats.terminalFees.AddStat(airline.name, new MoneyStat(airline.Needs.NegotiatedTerminalFees));
                    airportData.airlineStats.dailyFees.AddStat(airline.name, new MoneyStat(airline.Needs.NegotiatedDailyFees));
                    airportData.airlineStats.fuelSatisfactionNegotiated.AddStat(airline.name, new PercentageStat(airline.Needs.AllNeeds.TryGetValue("NegotiatedFuelSatisfaction", out need) ? 1f - ((double)need.target / 100) : 0f));
                    airportData.airlineStats.reliabilityNegotiated.AddStat(airline.name, new PercentageStat(airline.Needs.AllNeeds.TryGetValue("NegotiatedReliabilty", out need) ? 1f - ((double)need.target / 100) : 0f));
                    airportData.airlineStats.offices.AddStat(airline.name, new NumberStat(airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.Office)));
                    airportData.airlineStats.conferenceRoom.AddStat(airline.name, new BoolStat(airline.Needs.Conference != null));
                    airportData.airlineStats.stores.AddStat(airline.name, new NumberStat(airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.Store)));
                    airportData.airlineStats.storeShare.AddStat(airline.name, new PercentageStat(airline.Needs.NegotiatedStoreShare / 100f));
                    airportData.airlineStats.cafes.AddStat(airline.name, new NumberStat(airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.Cafe)));
                    airportData.airlineStats.cafeShare.AddStat(airline.name, new PercentageStat(airline.Needs.NegotiatedCafeShare / 100f));
                    airportData.airlineStats.firstClassLounges.AddStat(airline.name, new NumberStat(airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.First_Class_Lounge)));
                    airportData.airlineStats.flightCrewLounges.AddStat(airline.name, new NumberStat(airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.Flight_Crew_Lounge)));
                    airportData.airlineStats.smallGates.AddStat(airline.name, new NumberStat(Game.current.objectCache.AircraftGate_All.All().Count(x => x.Owner == airline && x.Size == AircraftGate.GateSize.Small)));
                    airportData.airlineStats.largeGates.AddStat(airline.name, new NumberStat(Game.current.objectCache.AircraftGate_All.All().Count(x => x.Owner == airline && x.Size == AircraftGate.GateSize.Large)));
                    airportData.airlineStats.XLGates.AddStat(airline.name, new NumberStat(Game.current.objectCache.AircraftGate_All.All().Count(x => x.Owner == airline && x.Size == AircraftGate.GateSize.Extra_Large)));
                    airportData.airlineStats.paxPercent.AddStat(airline.name, new PercentageStat(airline.Needs.NegotiatedPaxPercent / 100));
                    airportData.airlineStats.penalty.AddStat(airline.name, new MoneyStat(airline.Needs.NegotiatedPenalty));
                }
            }
        }

        static private void LoadAirportLifetimeStats()
        {
            airportData.ResetLifetimeStats();
            GameLifetimeStats GLS = Game.current.lifetimeStats;

            airportData.flightStats.nSchedFlights.AddStat(0, new NumberStat((int)GLS.Takeoffs));
            //airportData.flightStats.nOntimeDeparture.AddStat(0, new NumberStat((int)GLS.flOnTime));
            //airportData.flightStats.nDelayedDeparture.AddStat(0, new NumberStat((int)GLS.flDelays));
            airportData.flightStats.nCancelled.AddStat(0, new NumberStat((int)GLS.flCancels));
            airportData.flightStats.nReneged.AddStat(0, new NumberStat((int)GLS.flReneges));
                airportData.flightStats.nCancelled.AddToValue(0, new NumberStat((int)GLS.flReneges));
            //airportData.flightStats..AddStat(0, new NumberStat((int)GLS.Landings));
            airportData.passengerStats.nBoarded.AddStat(0, new NumberStat((int)GLS.pBoarded));
            airportData.passengerStats.nMissed.AddStat(0, new NumberStat((int)GLS.pMissed));
            airportData.fuelStats.avgFuelPrice.AddStat(0, new MoneyStat(GetAverageFuelCost(0),2));
            airportData.fuelStats.fuelRequested.AddStat(0, new NumberStat((int)(GLS.fuelRequested / 1000)));
            airportData.fuelStats.fuelDelivered.AddStat(0, new NumberStat((int)(GLS.fuelfRefueled / 1000)));
            airportData.fuelStats.planesRefueled.AddStat(0, new NumberStat((int)GLS.planesServedFuel));
            airportData.luggageStats.bagsLoaded.AddStat(0, new NumberStat((int)GLS.pBagsLoaded));
            airportData.luggageStats.bagsUnloaded.AddStat(0, new NumberStat((int)GLS.pBagsUnloaded));
            airportData.luggageStats.lostBags.AddStat(0, new NumberStat((int)GLS.pBagFail));
            //airportData.luggageStats..AddStat(0, new NumberStat((int)GLS.pBagSuccess));
            airportData.staffStats.nHires.AddStat(new NumberStat((int)GLS.sHires));
            airportData.staffStats.nFires.AddStat(new NumberStat((int)GLS.sFires));
            airportData.timeStats.tPaused.AddStat(new TimeStat((int)GLS.tPaused));
            airportData.timeStats.tSpeed1.AddStat(new TimeStat((int)GLS.tSpeed1));
            airportData.timeStats.tSpeed2.AddStat(new TimeStat((int)GLS.tSpeed2));
            airportData.timeStats.tSpeed3.AddStat(new TimeStat((int)GLS.tSpeed3));
            airportData.timeStats.tInactive.AddStat(new TimeStat((int)GLS.tInactive));
            airportData.interactions.keyboardInteractions.AddStat(new NumberStat((int)GLS.tInteractions));
            airportData.interactions.mouseClicks.AddStat(new NumberStat((int)GLS.tClicks));
            airportData.interactions.altMouseClicks.AddStat(new NumberStat((int)GLS.tClicksAlt));

            airportData.revAndExpStats.revenueStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Advertising), 0, new MoneyStat((float)GLS.mAdvertising));
            airportData.revAndExpStats.revenueStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Bank), 0, new MoneyStat((float)GLS.mLoans));
            airportData.revAndExpStats.revenueStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Fuel), 0, new MoneyStat((float)GLS.mFuelRev));
            airportData.revAndExpStats.revenueStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Retail), 0, new MoneyStat((float)GLS.mRetailRev));
            airportData.revAndExpStats.revenueStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Runway_Fees), 0, new MoneyStat((float)GLS.mRwyUsageRev));
            airportData.revAndExpStats.revenueStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Terminal_Fees), 0, new MoneyStat((float)GLS.mTerminalUsageRev));
            airportData.revAndExpStats.revenueStats.AddStat("total", 0, new MoneyStat((float)GLS.mRev));

            airportData.revAndExpStats.expenseStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Bank), 0, new MoneyStat((float)GLS.mInterest));
            airportData.revAndExpStats.expenseStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Retail), 0, new MoneyStat((float)GLS.mRetailExpense));
            airportData.revAndExpStats.expenseStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Staff), 0, new MoneyStat((float)GLS.mStaffWages));
            airportData.revAndExpStats.expenseStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Taxes), 0, new MoneyStat((float)(GLS.mIncomeTax + GLS.mPropertyTax)));
            airportData.revAndExpStats.expenseStats.AddStat("total", 0, new MoneyStat((float)GLS.mExpense));
        }

        static private void LoadFlights(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                flightData.AddFlights(i);
            }
        }

        static private float GetAverageFuelCost(int day)
        {
            FuelController fuelController = Game.current.fuelController;
            int endOfDay = (1440 * (day == 0 ? GameTimer.Day : day)) - 480;
            int startOfDay = day == 0 ? 0 : endOfDay - (day == 1 ? 960 : 1440);
            float totalCost = 0;
            for (int i = startOfDay; i < endOfDay; i += 60)
            {
                totalCost += fuelController.GetMarketPriceAtTime(i);
            }
            return totalCost / ((endOfDay - startOfDay) / 60);
        }

        static private float GetDailyMoneyTotal(GamedayReportingData GRD, GamedayReportingData.MoneyCategory category, bool positive)
        {
            float num = 0.0f;
            if (!GRD.CategorizedCashChanges.ContainsKey(category))
            {
                return num;
            }
            foreach (float num2 in GRD.CategorizedCashChanges[category].Values)
            {
                if ((positive && num2 >= 0.0) || (!positive && num2 <= 0.0))
                {
                    num += num2;
                }
            }
            return num;
        }

        static private float GetDailyMoneyTotal(GamedayReportingData GRD, bool positive)
        {
            float num = 0.0f;
            foreach(KeyValuePair<GamedayReportingData.MoneyCategory, Dictionary<string, double>> kvp in GRD.CategorizedCashChanges)
            {
                num += GetDailyMoneyTotal(GRD, kvp.Key, positive);
            }
            return num;
        }
    }
}
