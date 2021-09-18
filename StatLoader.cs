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
                    IEnumerable<FlightRecord> flightRecords = Game.current.flightRecords.GetForDay(i).Where(x => x.airline == airline.name);

                    if (flightRecords?.Any() != true)
                    {
                        continue;
                    }

                    // Load Flight Stats
                    int nSchedFlights = flightRecords.Count();
                    int ontimeDepart = flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Departed) && !AirportStatUtils.HasStatus(x.status, Flight.Status.DelayedDeparture));
                    int delDep = flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.DelayedDeparture));
                    int canx = flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Canceled));
                    thisAirline.flightStats.nSchedFlights.AddStat(i, new IntStat(nSchedFlights));
                    thisAirline.flightStats.nDelayedArrival.AddStat(i, new IntStat(flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.DelayedArrival))));
                    thisAirline.flightStats.nRequiresCrew.AddStat(i, new IntStat(flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.RequiresCrew))));
                    thisAirline.flightStats.nOntimeDeparture.AddStat(i, new IntStat(ontimeDepart));
                    thisAirline.flightStats.nDelayedDeparture.AddStat(i, new IntStat(delDep, delDep > 0 ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None));
                    thisAirline.flightStats.nCancelled.AddStat(i, new IntStat(canx, canx > 0 ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None));
                    thisAirline.flightStats.nAirportInvalid.AddStat(i, new IntStat(flightRecords.Count(x => x.reason == Flight.StatusReason.AirportInvalid)));
                    thisAirline.flightStats.nWeather.AddStat(i, new IntStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Weather)));
                    thisAirline.flightStats.nRunway.AddStat(i, new IntStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Runway)));
                    thisAirline.flightStats.nGate.AddStat(i, new IntStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Gate)));
                    thisAirline.flightStats.nExpired.AddStat(i, new IntStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Expired)));
                    thisAirline.flightStats.nReneged.AddStat(i, new IntStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Reneged)));
                    thisAirline.flightStats.ontimeDeparturePer.AddStat(i, new AverageStat(ontimeDepart, ontimeDepart + delDep + canx, typeof(PercentageStat)));

                    airportData.flightStats.nSchedFlights.AddToValue(i, new IntStat(nSchedFlights));
                    airportData.flightStats.nDelayedArrival.AddToValue(i, new IntStat(flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.DelayedArrival))));
                    airportData.flightStats.nRequiresCrew.AddToValue(i, new IntStat(flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.RequiresCrew))));
                    airportData.flightStats.nOntimeDeparture.AddToValue(i, new IntStat(ontimeDepart));
                    airportData.flightStats.nDelayedDeparture.AddToValue(i, new IntStat(delDep, delDep > 0 ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None));
                    airportData.flightStats.nCancelled.AddToValue(i, new IntStat(canx, canx > 0 ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None));
                    airportData.flightStats.nAirportInvalid.AddToValue(i, new IntStat(flightRecords.Count(x => x.reason == Flight.StatusReason.AirportInvalid)));
                    airportData.flightStats.nWeather.AddToValue(i, new IntStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Weather)));
                    airportData.flightStats.nRunway.AddToValue(i, new IntStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Runway)));
                    airportData.flightStats.nGate.AddToValue(i, new IntStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Gate)));
                    airportData.flightStats.nExpired.AddToValue(i, new IntStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Expired)));
                    airportData.flightStats.nReneged.AddToValue(i, new IntStat(flightRecords.Count(x => x.reason == Flight.StatusReason.Reneged)));
                    airportData.flightStats.ontimeDeparturePer.AddToValue(i, new AverageStat(ontimeDepart, ontimeDepart + delDep + canx, typeof(PercentageStat)));

                    //Load Passenger Stats
                    int arrivingPax = flightRecords.Sum(x => x.nArriving);
                    int boardedPax = flightRecords.Sum(x => x.nBoarded);
                    int schedDepartingPax = flightRecords.Sum(x => x.nDeparting);
                    int missed = flightRecords.Sum(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Departed) || AirportStatUtils.HasStatus(x.status, Flight.Status.Canceled) ? x.nDeparting - x.nBoarded : 0);
                    double timeBoarding = flightRecords.Sum(x => x.nBoarded > 0 ? x.time_boarding : 0) * 60f;
                    thisAirline.passengerStats.nArriving.AddStat(i, new IntStat(arrivingPax));
                    thisAirline.passengerStats.nBoarded.AddStat(i, new IntStat(boardedPax));
                    thisAirline.passengerStats.nCheckedIn.AddStat(i, new IntStat(flightRecords.Sum(x => x.nCheckedIn)));
                    thisAirline.passengerStats.nSchedDep.AddStat(i, new IntStat(schedDepartingPax));                    
                    thisAirline.passengerStats.nMissed.AddStat(i, new IntStat(missed > 0 ? missed : 0, missed > 0 ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None));
                    thisAirline.passengerStats.timeBoarding.AddStat(i, new TimeStat(timeBoarding));
                    thisAirline.passengerStats.timeDeplaning.AddStat(i, new TimeStat(flightRecords.Sum(x => x.time_deplaning) * 60f));
                    thisAirline.passengerStats.arrPaxPerFlt.AddStat(i, new AverageStat(arrivingPax, nSchedFlights, typeof(IntStat)));
                    thisAirline.passengerStats.departPaxPerFlt.AddStat(i, new AverageStat(schedDepartingPax, nSchedFlights, typeof(IntStat)));
                    thisAirline.passengerStats.avgBoardTime.AddStat(i, new AverageStat(timeBoarding, ontimeDepart + delDep + canx, typeof(TimeStat)));
                    thisAirline.passengerStats.boardedPerFlt.AddStat(i, new AverageStat(boardedPax, boardedPax + missed, typeof(PercentageStat)));

                    airportData.passengerStats.nArriving.AddToValue(i, new IntStat(arrivingPax));
                    airportData.passengerStats.nBoarded.AddToValue(i, new IntStat(boardedPax));
                    airportData.passengerStats.nCheckedIn.AddToValue(i, new IntStat(flightRecords.Sum(x => x.nCheckedIn)));
                    airportData.passengerStats.nSchedDep.AddToValue(i, new IntStat(schedDepartingPax));
                    airportData.passengerStats.nMissed.AddToValue(i, new IntStat(missed > 0 ? missed : 0, missed > 0 ? AirportStatUtils.InfoLevels.Warning : AirportStatUtils.InfoLevels.None));
                    airportData.passengerStats.timeBoarding.AddToValue(i, new TimeStat(timeBoarding));
                    airportData.passengerStats.timeDeplaning.AddToValue(i, new TimeStat(flightRecords.Sum(x => x.time_deplaning) * 60f));
                    airportData.passengerStats.arrPaxPerFlt.AddToValue(i, new AverageStat(arrivingPax, nSchedFlights, typeof(IntStat)));
                    airportData.passengerStats.departPaxPerFlt.AddToValue(i, new AverageStat(schedDepartingPax, nSchedFlights, typeof(IntStat)));
                    airportData.passengerStats.avgBoardTime.AddToValue(i, new AverageStat(timeBoarding, ontimeDepart + delDep + canx, typeof(TimeStat)));
                    airportData.passengerStats.boardedPerFlt.AddToValue(i, new AverageStat(boardedPax, boardedPax + missed, typeof(PercentageStat)));

                    //Load Fuel Stats
                    int fuelDel = Convert.ToInt32(flightRecords.Sum(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Departed) ? x.nFuelRefueled : 0) / 1000);
                    int fuelReq = Convert.ToInt32(flightRecords.Sum(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Departed) ? x.nFuelRequested : 0) / 1000);
                    int fuelFailures = flightRecords.Count(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Departed) && x.nFuelRequested > 0 && x.nFuelRefueled == 0);
                    thisAirline.fuelStats.fuelDelivered.AddStat(i, new IntStat(fuelDel, fuelDel < fuelReq ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None));
                    thisAirline.fuelStats.fuelRequested.AddStat(i, new IntStat(fuelReq));
                    thisAirline.fuelStats.fuelingFailures.AddStat(i, new IntStat(fuelFailures, fuelFailures > 0 ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None));
                    thisAirline.fuelStats.planesRefueled.AddStat(i, new IntStat(flightRecords.Count(x => x.nFuelRefueled > 0)));

                    airportData.fuelStats.fuelDelivered.AddToValue(i, new IntStat(fuelDel, fuelDel < fuelReq ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None));
                    airportData.fuelStats.fuelRequested.AddToValue(i, new IntStat(fuelReq));
                    airportData.fuelStats.fuelingFailures.AddToValue(i, new IntStat(fuelFailures, fuelFailures > 0 ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None));
                    airportData.fuelStats.planesRefueled.AddToValue(i, new IntStat(flightRecords.Count(x => x.nFuelRefueled > 0)));

                    // Load Luggage Stats
                    int lostBaggage = flightRecords.Sum(x => AirportStatUtils.HasStatus(x.status, Flight.Status.Departed) ? x.nDepartingBags - x.nBagsLoaded : 0);
                    thisAirline.luggageStats.arrivingBags.AddStat(i, new IntStat(flightRecords.Sum(x => x.nArrivalBags)));
                    thisAirline.luggageStats.bagsLoaded.AddStat(i, new IntStat(flightRecords.Sum(x => x.nBagsLoaded)));
                    thisAirline.luggageStats.bagsUnloaded.AddStat(i, new IntStat(flightRecords.Sum(x => x.nBagsUnloaded)));
                    thisAirline.luggageStats.departingBags.AddStat(i, new IntStat(flightRecords.Sum(x => x.nDepartingBags)));
                    thisAirline.luggageStats.lostBags.AddStat(i, new IntStat(lostBaggage, lostBaggage > 0 ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None));
                    thisAirline.luggageStats.timeLoadingBags.AddStat(i, new TimeStat(flightRecords.Sum(x => x.nBagsLoaded > 0 ? x.time_bag_load : 0) * 60f));

                    airportData.luggageStats.arrivingBags.AddToValue(i, new IntStat(flightRecords.Sum(x => x.nArrivalBags)));
                    airportData.luggageStats.bagsLoaded.AddToValue(i, new IntStat(flightRecords.Sum(x => x.nBagsLoaded)));
                    airportData.luggageStats.bagsUnloaded.AddToValue(i, new IntStat(flightRecords.Sum(x => x.nBagsUnloaded)));
                    airportData.luggageStats.departingBags.AddToValue(i, new IntStat(flightRecords.Sum(x => x.nDepartingBags)));
                    airportData.luggageStats.lostBags.AddToValue(i, new IntStat(lostBaggage, lostBaggage > 0 ? AirportStatUtils.InfoLevels.Info : AirportStatUtils.InfoLevels.None));
                    airportData.luggageStats.timeLoadingBags.AddToValue(i, new TimeStat(flightRecords.Sum(x => x.nBagsLoaded > 0 ? x.time_bag_load : 0) * 60f));

                    //Load Gate Stats (into FlightStats)
                    gateSizes[AircraftGate.GateSize.Small] = 0;
                    gateSizes[AircraftGate.GateSize.Large] = 0;
                    gateSizes[AircraftGate.GateSize.Extra_Large] = 0;
                    foreach (FlightRecord fr in flightRecords)
                    {
                        AircraftConfig ac = AircraftConfigManager.FindByAnyName(fr.aircraft, false);
                        if (ac != null)
                        {
                            gateSizes[ac.MinGateSize]++;
                        }
                    }
                    foreach (KeyValuePair<AircraftGate.GateSize, int> kvp in gateSizes)
                    {
                        switch (kvp.Key)
                        {
                            case AircraftGate.GateSize.Small:
                                thisAirline.flightStats.nSmallGates.AddStat(i, new IntStat(kvp.Value));
                                airportData.flightStats.nSmallGates.AddToValue(i, new IntStat(kvp.Value));
                                break;
                            case AircraftGate.GateSize.Large:
                                thisAirline.flightStats.nLargeGates.AddStat(i, new IntStat(kvp.Value));
                                airportData.flightStats.nLargeGates.AddToValue(i, new IntStat(kvp.Value));
                                break;
                            case AircraftGate.GateSize.Extra_Large:
                                thisAirline.flightStats.nXLGates.AddStat(i, new IntStat(kvp.Value));
                                airportData.flightStats.nXLGates.AddToValue(i, new IntStat(kvp.Value));
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
                airportData.passengerStats.nConnecting.AddStat(i, new IntStat(gamedayData.NumConnectPax));
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
                    int boarded = ((IntStat)airportData.passengerStats.nBoarded.GetStat(i))?.GetValue() ?? 0;
                    int missed = ((IntStat)airportData.passengerStats.nMissed.GetStat(i))?.GetValue() ?? 0;
                    airportData.revAndExpStats.revenueStats.RevPerPax.AddStat(i, new AverageStat(val, boarded + missed, typeof(MoneyStat)));
                }
                if ((val = GetDailyMoneyTotal(gamedayData, false)) != 0)
                {
                    airportData.revAndExpStats.expenseStats.AddStat("total", i, new MoneyStat(val));
                }
                //set profits
                AirportStatUtils.AirportStatsLogger(Log.FromPool("Loading Profits").WithCodepoint());

                val = ((MoneyStat)airportData.revAndExpStats.revenueStats.StatGroups["total"].GetStat(i))?.GetFloatValue() ?? 0;
                float netCost = -((MoneyStat)airportData.revAndExpStats.expenseStats.StatGroups["total"].GetStat(i))?.GetFloatValue() ?? 0;

                float operatingCost = netCost + ((MoneyStat)airportData.revAndExpStats.expenseStats.StatGroups[nameof(GamedayReportingData.MoneyCategory.Taxes)].GetStat(i))?.GetFloatValue() ?? 0 - 
                    ((MoneyStat)airportData.revAndExpStats.expenseStats.StatGroups[nameof(GamedayReportingData.MoneyCategory.Bank)].GetStat(i))?.GetFloatValue() ?? 0;
                float grossCost = - (((MoneyStat)airportData.revAndExpStats.expenseStats.StatGroups[nameof(GamedayReportingData.MoneyCategory.Fuel)].GetStat(i))?.GetFloatValue() ?? 0 +
                    ((MoneyStat)airportData.revAndExpStats.expenseStats.StatGroups[nameof(GamedayReportingData.MoneyCategory.Infrastructure)].GetStat(i))?.GetFloatValue() ?? 0 +
                    ((MoneyStat)airportData.revAndExpStats.expenseStats.StatGroups[nameof(GamedayReportingData.MoneyCategory.Land_Purchase)].GetStat(i))?.GetFloatValue() ?? 0 +
                    ((MoneyStat)airportData.revAndExpStats.expenseStats.StatGroups[nameof(GamedayReportingData.MoneyCategory.Materials)].GetStat(i))?.GetFloatValue() ?? 0 +
                    ((MoneyStat)airportData.revAndExpStats.expenseStats.StatGroups[nameof(GamedayReportingData.MoneyCategory.Retail)].GetStat(i))?.GetFloatValue() ?? 0 +
                    ((MoneyStat)airportData.revAndExpStats.expenseStats.StatGroups[nameof(GamedayReportingData.MoneyCategory.Staff)].GetStat(i))?.GetFloatValue() ?? 0 +
                    ((MoneyStat)airportData.revAndExpStats.expenseStats.StatGroups[nameof(GamedayReportingData.MoneyCategory.Maintenance)].GetStat(i))?.GetFloatValue() ?? 0);
                AirportStatUtils.AirportStatsLogger(Log.FromPool($"Revenue: {val}; NetCost: {netCost}; Operating: {operatingCost}; Gross: {grossCost}").WithCodepoint());

                airportData.profitStats.GrossProfit.AddStat(i, new MoneyStat(val - grossCost));
                airportData.profitStats.OperatingProfit.AddStat(i, new MoneyStat(val - operatingCost));
                airportData.profitStats.NetProfit.AddStat(i, new MoneyStat(val - netCost));
                if (val != 0)
                {
                    if(val > grossCost)
                        airportData.profitStats.GrossMargin.AddStat(i, new PercentageStat((val - grossCost) / val));
                    if(val > operatingCost)
                        airportData.profitStats.OperatingMargin.AddStat(i, new PercentageStat((val - operatingCost) / val));
                    if (val > netCost)
                        airportData.profitStats.NetMargin.AddStat(i, new PercentageStat((val - netCost) / val));
                }
                AirportStatUtils.AirportStatsLogger(Log.FromPool("Completed Loading Profits").WithCodepoint());
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
                airportData.airlineStats.nAcceptedOffers.AddStat(airline.name, new IntStat(airline.nAcceptedOffers));
                airportData.airlineStats.baseRefuelPercentage.AddStat(airline.name, new PercentageStat(airline.BaseRefuelPercentage));
                airportData.airlineStats.firstClassPercentage.AddStat(airline.name, new PercentageStat(airline.FirstClassPercentage));
                airportData.airlineStats.newFlightBonus.AddStat(airline.name, new MoneyStat((float)airline.Income_NewFlightBonus_PerFlight));
                airportData.airlineStats.peakFlightCount.AddStat(airline.name, new IntStat(airline.PeakFlightsCount));
                airportData.airlineStats.nReps.AddStat(airline.name, new IntStat(airline.Reps?.Count ?? 0));
                airportData.airlineStats.hasDeal.AddStat(airline.name, new BoolStat(airline.Needs?.HasDeal ?? false));

                if (airline.Needs?.HasDeal ?? false)
                {
                    airportData.airlineStats.runwayFees.AddStat(airline.name, new MoneyStat(airline.Needs.NegotiatedRunwayFees));
                    airportData.airlineStats.terminalFees.AddStat(airline.name, new MoneyStat(airline.Needs.NegotiatedTerminalFees));
                    airportData.airlineStats.dailyFees.AddStat(airline.name, new MoneyStat(airline.Needs.NegotiatedDailyFees));
                    airportData.airlineStats.fuelSatisfactionNegotiated.AddStat(airline.name, new PercentageStat(airline.Needs.AllNeeds.TryGetValue("NegotiatedFuelSatisfaction", out need) ? 1f - ((double)need.target / 100f) : 0f));
                    airportData.airlineStats.reliabilityNegotiated.AddStat(airline.name, new PercentageStat(airline.Needs.AllNeeds.TryGetValue("NegotiatedReliability", out need) ? 1f - ((double)need.target / 100f) : 0f));
                    airportData.airlineStats.offices.AddStat(airline.name, new IntStat(airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.Office)));
                    airportData.airlineStats.conferenceRoom.AddStat(airline.name, new BoolStat(airline.Needs.Conference != null));
                    airportData.airlineStats.stores.AddStat(airline.name, new IntStat(airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.Store)));
                    airportData.airlineStats.storeShare.AddStat(airline.name, new PercentageStat(airline.Needs.NegotiatedStoreShare / 100f));
                    airportData.airlineStats.cafes.AddStat(airline.name, new IntStat(airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.Cafe)));
                    airportData.airlineStats.cafeShare.AddStat(airline.name, new PercentageStat(airline.Needs.NegotiatedCafeShare / 100f));
                    airportData.airlineStats.firstClassLounges.AddStat(airline.name, new IntStat(airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.First_Class_Lounge)));
                    airportData.airlineStats.flightCrewLounges.AddStat(airline.name, new IntStat(airline.Needs.AssignedZones.Count(x => x.type == Zone.ZoneType.Flight_Crew_Lounge)));
                    airportData.airlineStats.smallGates.AddStat(airline.name, new IntStat(Game.current.objectCache.AircraftGate_All.All().Count(x => x.Owner == airline && x.Size == AircraftGate.GateSize.Small)));
                    airportData.airlineStats.largeGates.AddStat(airline.name, new IntStat(Game.current.objectCache.AircraftGate_All.All().Count(x => x.Owner == airline && x.Size == AircraftGate.GateSize.Large)));
                    airportData.airlineStats.XLGates.AddStat(airline.name, new IntStat(Game.current.objectCache.AircraftGate_All.All().Count(x => x.Owner == airline && x.Size == AircraftGate.GateSize.Extra_Large)));
                    airportData.airlineStats.paxPercent.AddStat(airline.name, new PercentageStat(airline.Needs.NegotiatedPaxPercent / 100f));
                    airportData.airlineStats.penalty.AddStat(airline.name, new MoneyStat(airline.Needs.NegotiatedPenalty));
                }
            }
        }

        static private void LoadAirportLifetimeStats()
        {
            airportData.ResetLifetimeStats();
            GameLifetimeStats GLS = Game.current.lifetimeStats;

            airportData.flightStats.nSchedFlights.AddStat(0, new IntStat((int)GLS.Takeoffs));
            //airportData.flightStats.nOntimeDeparture.AddStat(0, new NumberStat((int)GLS.flOnTime));
            //airportData.flightStats.nDelayedDeparture.AddStat(0, new NumberStat((int)GLS.flDelays));
            airportData.flightStats.nCancelled.AddStat(0, new IntStat((int)GLS.flCancels));
            airportData.flightStats.nReneged.AddStat(0, new IntStat((int)GLS.flReneges));
                airportData.flightStats.nCancelled.AddToValue(0, new IntStat((int)GLS.flReneges));
            //airportData.flightStats..AddStat(0, new NumberStat((int)GLS.Landings));
            airportData.passengerStats.nSchedDep.AddStat(0, new IntStat((int)(GLS.pBoarded + GLS.pMissed)));
            airportData.passengerStats.nBoarded.AddStat(0, new IntStat((int)GLS.pBoarded));
            airportData.passengerStats.departPaxPerFlt.AddStat(0, new AverageStat((int)(GLS.pBoarded + GLS.pMissed), (int)GLS.Takeoffs, typeof(IntStat)));
            airportData.passengerStats.nMissed.AddStat(0, new IntStat((int)GLS.pMissed));
            airportData.passengerStats.boardedPerFlt.AddStat(0, new AverageStat((int)GLS.pBoarded, (int)(GLS.pBoarded + GLS.pMissed), typeof(PercentageStat)));

            airportData.fuelStats.avgFuelPrice.AddStat(0, new MoneyStat(GetAverageFuelCost(0),2));
            airportData.fuelStats.fuelRequested.AddStat(0, new IntStat((int)(GLS.fuelRequested / 1000)));
            airportData.fuelStats.fuelDelivered.AddStat(0, new IntStat((int)(GLS.fuelfRefueled / 1000)));
            airportData.fuelStats.planesRefueled.AddStat(0, new IntStat((int)GLS.planesServedFuel));
            airportData.luggageStats.bagsLoaded.AddStat(0, new IntStat((int)GLS.pBagsLoaded));
            airportData.luggageStats.bagsUnloaded.AddStat(0, new IntStat((int)GLS.pBagsUnloaded));
            airportData.luggageStats.lostBags.AddStat(0, new IntStat((int)GLS.pBagFail));
            //airportData.luggageStats..AddStat(0, new NumberStat((int)GLS.pBagSuccess));
            airportData.staffStats.nHires.AddStat(new IntStat((int)GLS.sHires));
            airportData.staffStats.nFires.AddStat(new IntStat((int)GLS.sFires));
            airportData.timeStats.tPaused.AddStat(new TimeStat((int)GLS.tPaused));
            airportData.timeStats.tSpeed1.AddStat(new TimeStat((int)GLS.tSpeed1));
            airportData.timeStats.tSpeed2.AddStat(new TimeStat((int)GLS.tSpeed2));
            airportData.timeStats.tSpeed3.AddStat(new TimeStat((int)GLS.tSpeed3));
            airportData.timeStats.tInactive.AddStat(new TimeStat((int)GLS.tInactive));
            airportData.interactions.keyboardInteractions.AddStat(new IntStat((int)GLS.tInteractions));
            airportData.interactions.mouseClicks.AddStat(new IntStat((int)GLS.tClicks));
            airportData.interactions.altMouseClicks.AddStat(new IntStat((int)GLS.tClicksAlt));

            airportData.revAndExpStats.revenueStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Advertising), 0, new MoneyStat((float)GLS.mAdvertising));
            airportData.revAndExpStats.revenueStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Bank), 0, new MoneyStat((float)GLS.mLoans));
            airportData.revAndExpStats.revenueStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Fuel), 0, new MoneyStat((float)GLS.mFuelRev));
            airportData.revAndExpStats.revenueStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Retail), 0, new MoneyStat((float)GLS.mRetailRev));
            airportData.revAndExpStats.revenueStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Runway_Fees), 0, new MoneyStat((float)GLS.mRwyUsageRev));
            airportData.revAndExpStats.revenueStats.AddStat(nameof(GamedayReportingData.MoneyCategory.Terminal_Fees), 0, new MoneyStat((float)GLS.mTerminalUsageRev));
            airportData.revAndExpStats.revenueStats.AddStat("total", 0, new MoneyStat((float)GLS.mRev));
            airportData.revAndExpStats.revenueStats.RevPerPax.AddStat(0, new AverageStat(GLS.mRev, (int)(GLS.pBoarded + GLS.pMissed), typeof(MoneyStat)));

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
