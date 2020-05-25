using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    internal class TBFlash_ChartData
    {
        private enum Types
        {
            line,
            bar,
            radar,
            doughnut,
            polar,
            bubble,
            scatter,
            stackedBar,
            multiAxisLine,
            none
        }
        private enum YAxisTypes
        {
            yAxisLeft,
            yAxisRight
        }
        private string Title;
        private Types Type;
        private int NumSeries;
        private string Options;
        private string[,] dataArray;
        private readonly int additionalColumns = 7;
        private string YAxisLabel = "Total Number";
        private string YAxisLabel2 = "Total";
        private string MoneySetting = "false";

        internal string GetChartData(string dataSet)
        {
            string jsonCode;
            if (LoadArray(dataSet))
            {
                TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());
                SetOptions();
                jsonCode = $"{{\n\t{ChartOptions()},\n\t{SeriesConfig()},\n\t";
                jsonCode += "\"chartData\":[{";
                for (int j = additionalColumns; j < (GameTimer.Day < 30 ? GameTimer.Day : 30) + additionalColumns; j++)
                {
                    jsonCode += j > additionalColumns ? "," : string.Empty;
                    jsonCode += $"\"{dataArray[0, j]}\":{{";
                    bool hasEntry = false;
                    for (int i = 1; i <= NumSeries; i++)
                    {
                        if (dataArray[i, 5] == "visible")
                        {
                            jsonCode += hasEntry ? "," : string.Empty;
                            jsonCode += $"\"{dataArray[i, 1]}\":{dataArray[i, j]}";
                            hasEntry = true;
                        }
                    }
                    jsonCode += "}";
                }
                jsonCode += "}]}";
                TBFlash_Utils.TBFlashLogger(Log.FromPool(jsonCode).WithCodepoint());
            }
            else
            {
                jsonCode = "Configuration problem. No data to present.";
            }
            return jsonCode;
        }

        private bool LoadArray(string dataSet)
        {
            TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());
            switch (dataSet.ToUpperInvariant())
            {
                case "FLIGHTSTATS":
                    return LoadFlightStats();
                case "FUELSTATS":
                    return LoadFuelStats();
                case "LUGGAGESTATS":
                    return LoadLuggageStats();
                case "PAXSTATS":
                    return LoadPaxStats();
                case "PROFITS":
                    return LoadProfits();
                default:
                    return false;
            }
        }

        private bool LoadFlightStats()
        {
            TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());
            Title = "Flight Stats";
            Type = Types.line;
            NumSeries = 4;
            YAxisLabel = "Number Flights";
            dataArray = new string[NumSeries + 1, (GameTimer.Day <= 30 ? GameTimer.Day : 30) + additionalColumns];
            int j = additionalColumns - 1;
            for (int i = GameTimer.Day; i >= (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i--)
            {
                j++;
                if (!Game.current.GameReports.TryGetValue(i, out GamedayReportingData GRD))
                {
                    break;
                }
                dataArray[0, j] = i.ToString();
                dataArray[1, j] = GRD.FlightsCount.ToString("F0");
                dataArray[2, j] = (i != GameTimer.Day ? GRD.FlightsCount - GRD.FlightsDelayed - GRD.FlightsCanceled : 0).ToString("F0");
                dataArray[3, j] = GRD.FlightsDelayed.ToString("F0");
                dataArray[4, j] = GRD.FlightsCanceled.ToString("F0");
            }
            dataArray[1, 0] = "Scheduled Departures";
            dataArray[2, 0] = "OnTime Departures";
            dataArray[3, 0] = "Delayed Departures";
            dataArray[4, 0] = "Cancelled";
            dataArray[1, 1] = "schedDepart";
            dataArray[2, 1] = "ontimeDepart";
            dataArray[3, 1] = "delayDepart";
            dataArray[4, 1] = "canx";
            dataArray[1, 2] = "white";
            dataArray[2, 2] = "green";
            dataArray[3, 2] = "cyan";
            dataArray[4, 2] = "red";
            dataArray[1, 3] = "2";
            dataArray[2, 3] = "1";
            dataArray[3, 3] = "3";
            dataArray[4, 3] = "4";
            for(int i=1; i<=NumSeries; i++)
            {
                dataArray[i, 5] = "visible";
            }
            return true;
        }

        private bool LoadLuggageStats()
        {
            TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());
            Title = "Luggage Stats";
            Type = Types.line;
            NumSeries = 3;
            YAxisLabel = "Number of Bags";
            dataArray = new string[NumSeries + 1, (GameTimer.Day <= 30 ? GameTimer.Day : 30) + additionalColumns];
            int j = additionalColumns - 1;
            for (int i = GameTimer.Day; i >= (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i--)
            {
                j++;
                IEnumerable<FlightRecord> FlightRecords = Game.current.flightRecords.GetForDay(i - 1);
                dataArray[0, j] = i.ToString();
                dataArray[1, j] = FlightRecords.Sum(x => x.nBagsUnloaded).ToString("F0");
                dataArray[2, j] = FlightRecords.Sum(x => x.nBagsLoaded).ToString("F0");
                dataArray[3, j] = (i != GameTimer.Day ? (FlightRecords.Sum(x => x.nDepartingBags) - FlightRecords.Sum(x => x.nBagsLoaded)) : 0).ToString("F0");
            }
            dataArray[1, 0] = "Bags Unloaded";
            dataArray[2, 0] = "Bags Loaded";
            dataArray[3, 0] = "Lost Luggage";
            dataArray[1, 1] = "unload";
            dataArray[2, 1] = "load";
            dataArray[3, 1] = "lost";
            dataArray[1, 2] = "ivory";
            dataArray[2, 2] = "green";
            dataArray[3, 2] = "red";
            dataArray[1, 3] = "2";
            dataArray[2, 3] = "1";
            dataArray[3, 3] = "3";
            for (int i = 1; i <= NumSeries; i++)
            {
                dataArray[i, 5] = "visible";
            }
            return true;
        }
        private bool LoadFuelStats()
        {
            TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());
            Title = "Fuel Stats";
            Type = Types.multiAxisLine;
            NumSeries = 4;
            YAxisLabel = "Liters of Fuel";
            YAxisLabel2 = "Number of Planes Served";
            dataArray = new string[NumSeries + 1, (GameTimer.Day <= 30 ? GameTimer.Day : 30) + additionalColumns];
            int j = additionalColumns - 1;
            for (int i = GameTimer.Day; i >= (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i--)
            {
                j++;
                if (!Game.current.GameReports.TryGetValue(i, out GamedayReportingData GRD))
                {
                    break;
                }
                IEnumerable<FlightRecord> FlightRecords = Game.current.flightRecords.GetForDay(i - 1);

                dataArray[0, j] = i.ToString();
                dataArray[1, j] = (FlightRecords.Sum(x => x.nFuelRequested) / 1000).ToString("F0");
                dataArray[2, j] = (FlightRecords.Sum(x => x.nFuelRefueled) / 1000).ToString("F0");
                dataArray[3, j] = (FlightRecords.Count(x => x.nFuelRefueled > 0)).ToString("F0");
                dataArray[4, j] = GRD.FuelFailures.ToString("F0");
            }
            dataArray[1, 0] = "Fuel Requested";
            dataArray[2, 0] = "Fuel Provided";
            dataArray[3, 0] = "Planes Served Fuel";
            dataArray[4, 0] = "Fueling Failures";
            dataArray[1, 1] = "fuelReq";
            dataArray[2, 1] = "fuelProv";
            dataArray[3, 1] = "served";
            dataArray[4, 1] = "failed";
            dataArray[1, 2] = "ivory";
            dataArray[2, 2] = "green";
            dataArray[3, 2] = "cyan";
            dataArray[4, 2] = "red";
            dataArray[1, 3] = "2";
            dataArray[2, 3] = "1";
            dataArray[3, 3] = "3";
            dataArray[4, 3] = "4";
            dataArray[1, 6] = nameof(YAxisTypes.yAxisLeft);
            dataArray[2, 6] = nameof(YAxisTypes.yAxisLeft);
            dataArray[3, 6] = nameof(YAxisTypes.yAxisRight);
            dataArray[4, 6] = nameof(YAxisTypes.yAxisRight);
            for (int i = 1; i <= NumSeries; i++)
            {
                dataArray[i, 5] = "visible";
            }
            return true;
        }

        private bool LoadPaxStats()
        {
            TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());
            Title = "Passenger Stats";
            Type = Types.line;
            NumSeries = 4;
            YAxisLabel = "Number of Passengers";
            dataArray = new string[NumSeries + 1, (GameTimer.Day <= 30 ? GameTimer.Day : 30) + additionalColumns];
            int j = additionalColumns - 1;
            for (int i = GameTimer.Day; i >= (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i--)
            {
                j++;
                if (!Game.current.GameReports.TryGetValue(i, out GamedayReportingData GRD))
                {
                    break;
                }
                dataArray[0, j] = i.ToString();
                dataArray[1, j] = GRD.NumArrivalPax.ToString("F0");
                dataArray[2, j] = GRD.NumConnectPax.ToString("F0");
                dataArray[3, j] = GRD.BoardedFlight.ToString("F0");
                dataArray[4, j] = GRD.MissedFlight.ToString("F0");
            }
            dataArray[1, 0] = "Arriving";
            dataArray[2, 0] = "Connecting";
            dataArray[3, 0] = "Boarded";
            dataArray[4, 0] = "Missed Flight";
            dataArray[1, 1] = "arrive";
            dataArray[2, 1] = "connect";
            dataArray[3, 1] = "board";
            dataArray[4, 1] = "missed";
            dataArray[1, 2] = "ivory";
            dataArray[2, 2] = "gray";
            dataArray[3, 2] = "green";
            dataArray[4, 2] = "red";
            dataArray[1, 3] = "2";
            dataArray[2, 3] = "1";
            dataArray[3, 3] = "3";
            dataArray[4, 3] = "4";
            for (int i = 1; i <= NumSeries; i++)
            {
                dataArray[i, 5] = "visible";
            }
            return true;
        }

        private bool LoadProfits()
        {
            TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());
            Title = "Revenue / Expenses";
            Type = Types.stackedBar;
            NumSeries = 27;
            YAxisLabel = "Total Revenue and Expenses";
            MoneySetting = "\"" + i18n.Get("UI.currency") + "\"";
            dataArray = new string[NumSeries + 1, (GameTimer.Day <= 30 ? GameTimer.Day : 30) + additionalColumns];
            int j = additionalColumns -1;
            for (int i = GameTimer.Day; i >= (GameTimer.Day > 30 ? GameTimer.Day - 29 : 1); i--)
            {
                j++;
                if (!Game.current.GameReports.TryGetValue(i, out GamedayReportingData GRD))
                {
                    break;
                }
                dataArray[0, j] = i.ToString();
                dataArray[1, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Advertising, true);
                dataArray[2, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Airline_Fees, true);
                dataArray[3, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Bank, true);
                dataArray[4, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Fuel, true);
                dataArray[5, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Grant, true);
                dataArray[6, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Materials, true);
                dataArray[7, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Parking, true);
                dataArray[8, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Research, true);
                dataArray[9, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Retail, true);
                dataArray[10, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Runway_Fees, true);
                dataArray[11, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Terminal_Fees, true);
                dataArray[12, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Staff, true);
                dataArray[13, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Undefined, true);
                dataArray[14, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Airline_Fees, false);
                dataArray[15, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Bank, false);
                dataArray[16, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Fuel, false);
                dataArray[17, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Grant, false);
                dataArray[18, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Infrastructure, false);
                dataArray[19, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Land_Purchase, false);
                dataArray[20, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Maintenance, false);
                dataArray[21, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Materials, false);
                dataArray[22, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Research, false);
                dataArray[23, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Retail, false);
                dataArray[24, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Staff, false);
                dataArray[25, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Taxes, false);
                dataArray[26, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Transportation, false);
                dataArray[27, j] = GetDailyMoneyTotal(GRD, GamedayReportingData.MoneyCategory.Undefined, false);
            }
            dataArray[1, 0] ="Advertising Revenue";
            dataArray[2, 0] ="Airline Fee Revenue";
            dataArray[3, 0] ="Bank Revenue";
            dataArray[4, 0] ="Fuel Revenue";
            dataArray[5, 0] ="Grant Revenue";
            dataArray[6, 0] ="Materials Revenue";
            dataArray[7, 0] ="Parking Revenue";
            dataArray[8, 0] ="Research Revenue";
            dataArray[9, 0] ="Retail Revenue";
            dataArray[10, 0] ="Runway Fee Revenue";
            dataArray[11, 0] ="Terminal Fee Revenue";
            dataArray[12, 0] ="Staff Revenue";
            dataArray[13, 0] ="Undefined Revenue";
            dataArray[14, 0] ="Airline Fee Expenses";
            dataArray[15, 0] ="Bank Expenses";
            dataArray[16, 0] ="Fuel Expenses";
            dataArray[17, 0] ="Grant Expenses";
            dataArray[18, 0] ="Infrastructure Expenses";
            dataArray[19, 0] ="Land Purchase Expenses";
            dataArray[20, 0] ="Maintenance Expenses";
            dataArray[21, 0] ="Materials Expenses";
            dataArray[22, 0] ="Research Expenses";
            dataArray[23, 0] ="Retail Expenses";
            dataArray[24, 0] ="Staff Expenses";
            dataArray[25, 0] ="Taxes";
            dataArray[26, 0] ="Transportation Expenses";
            dataArray[27, 0] ="Undefined Expenses";
            dataArray[1, 1] ="revAds";
            dataArray[2, 1] ="revAirline";
            dataArray[3, 1] ="revBank";
            dataArray[4, 1] ="revFuel";
            dataArray[5, 1] ="revGrant";
            dataArray[6, 1] ="revMat";
            dataArray[7, 1] ="revPark";
            dataArray[8, 1] ="revRes";
            dataArray[9, 1] ="revRet";
            dataArray[10, 1] ="revRun";
            dataArray[11, 1] ="revTerm";
            dataArray[12, 1] ="revStaff";
            dataArray[13, 1] ="refUndef";
            dataArray[14, 1] ="expAirline";
            dataArray[15, 1] ="expBank";
            dataArray[16, 1] ="expFuel";
            dataArray[17, 1] ="expGrant";
            dataArray[18, 1] ="expInfra";
            dataArray[19, 1] ="expLand";
            dataArray[20, 1] ="expMain";
            dataArray[21, 1] ="expMat";
            dataArray[22, 1] ="expRes";
            dataArray[23, 1] ="expRet";
            dataArray[24, 1] ="expStaff";
            dataArray[25, 1] ="expTax";
            dataArray[26, 1] ="expTrans";
            dataArray[27, 1] ="expUndef";
            dataArray[1, 2] ="lightseagreen";
            dataArray[2, 2] ="lightgreen";
            dataArray[3, 2] ="lawngreen";
            dataArray[4, 2] ="greenyellow";
            dataArray[5, 2] ="mediumseagreen";
            dataArray[6, 2] ="darkseagreen";
            dataArray[7, 2] ="limegreen";
            dataArray[8, 2] ="yellowgreen";
            dataArray[9, 2] ="seagreen";
            dataArray[10, 2] ="mediumspringgreen";
            dataArray[11, 2] ="darkolivegreen";
            dataArray[12, 2] ="springgreen";
            dataArray[13, 2] ="green";
            dataArray[14, 2] ="burlywood";
            dataArray[15, 2] ="goldenrod";
            dataArray[16, 2] ="orange";
            dataArray[17, 2] ="lightsalmon";
            dataArray[18, 2] ="darksalmon";
            dataArray[19, 2] ="darkorange";
            dataArray[20, 2] ="sandybrown";
            dataArray[21, 2] ="indianred";
            dataArray[22, 2] ="palevioletred";
            dataArray[23, 2] ="orangered";
            dataArray[24, 2] ="coral";
            dataArray[25, 2] ="crimson";
            dataArray[26, 2] ="tomato";
            dataArray[27, 2] ="red";
            for (int i = 1; i <= NumSeries; i++)
            {
                dataArray[i,3] = "0";
            }
            for (int i = 1; i <= 13; i++)
            {
                dataArray[i,4] = "1";
            }
            for (int i = 14; i <= 27; i++)
            {
                dataArray[i, 4] = "2";
            }
            for (int i = 1; i <= NumSeries; i++)
            {
                dataArray[i, 5] = "notVisible";
                for (int k = additionalColumns; k < (GameTimer.Day < 30 ? GameTimer.Day : 30) + additionalColumns; k++)
                {
                    if (double.Parse(dataArray[i, k]) > 0.0)
                    {
                        dataArray[i, 5] = "visible";
                        break;
                    }
                }
            }
            return true;
        }

        private string GetDailyMoneyTotal(GamedayReportingData GRD, GamedayReportingData.MoneyCategory category, bool positive)
        {
            double num = 0.0;
            if (!GRD.CategorizedCashChanges.ContainsKey(category))
            {
                return num.ToString("F0");
            }
            foreach (double num2 in GRD.CategorizedCashChanges[category].Values)
            {
                if ((positive && num2 >= 0.0) || (!positive && num2 <= 0.0))
                {
                    num += num2;
                }
            }
            return Math.Abs(num).ToString("F0");
        }

        private void SetOptions()
        {
            TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());
            Options = "{" +
                "\"scales\": {" +
                    "\"xAxes\": [{" +
                        "\"ticks\": {" +
                            "\"reverse\":true " +
                        "}, " +
                        "\"gridLines\": {" +
                        "}, " +
                        "\"scaleLabel\":{" +
                            "\"display\": true, " +
                            "\"labelString\":\"Days\", " +
                            "\"fontSize\": 12 " +
                        "} ";
            Options += Type == Types.stackedBar ? ", \"stacked\": true" : string.Empty;
            Options += "}], " + //end xAxes
                    "\"yAxes\": [{" +
                        "\"ticks\": {" +
                            $"\"beginAtZero\": true";
            Options += "}, " +
                        "\"gridLines\":{" +
                        "}, " +
                        "\"scaleLabel\":{" +
                            "\"display\": true, " +
                            $"\"labelString\":\"{YAxisLabel}\", " +
                            "\"fontsize\": 12 " +
                        "}";
            Options += Type == Types.stackedBar ? ", \"stacked\": true" : string.Empty;
            if(Type == Types.multiAxisLine)
            {
                Options += $", \"id\": \"{nameof(YAxisTypes.yAxisLeft)}\", \"type\": \"linear\", \"position\": \"left\"}}"; //end of the first yAxis
                Options += ", {" +
                        "\"ticks\": {" +
                            $"\"beginAtZero\": true" +
                        "}, " +
                        "\"gridLines\":{" +
                        "}, " +
                        "\"scaleLabel\":{" +
                            "\"display\": true, " +
                            $"\"labelString\":\"{YAxisLabel2}\", " +
                            "\"fontSize\": 12 " +
                        "}";
                Options += $", \"id\": \"{nameof(YAxisTypes.yAxisRight)}\", \"type\": \"linear\", \"position\": \"right\""; //end of the second yAxis
            }
            Options += "}]" + //end yAxes
                "}, " + //end scales
                "\"tooltips\": {";
            Options += Type == Types.stackedBar ? "\"mode\": \"index\", \"position\": \"nearest\", \"reverse\": true" : "\"mode\": \"point\"";
            Options += "}, " + //end tooltips
                "\"legend\": {";
            Options += Type == Types.stackedBar ? "\"reverse\": true" : string.Empty;
            Options += "}"+ //end legend
            "}"; //end options
        }

        internal string ChartOptions()
        {
            TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());
            return $"\"chartOptions\":[{{\"title\":\"{Title}\",\"type\":\"{Type}\",\"money\":{MoneySetting},\"options\":{Options}}}]";
        }

        internal string SeriesConfig()
        {
            TBFlash_Utils.TBFlashLogger(Log.FromPool("").WithCodepoint());
            string seriesLabels = string.Empty;
            string seriesKeys = string.Empty;
            string seriesColors = string.Empty;
            string seriesOrders = string.Empty;
            string seriesStacks = string.Empty;
            string seriesYAxis = string.Empty;

            for (int i = 1; i < NumSeries + 1; i++)
            {
                if (dataArray[i, 5] == "visible")
                {
                    seriesLabels += (!string.IsNullOrEmpty(seriesLabels) ? "," : "\"seriesConfig\":[{\"seriesLabels\":[") + $"\"{dataArray[i, 0]}\"";
                    seriesKeys += (!string.IsNullOrEmpty(seriesKeys) ? "," : "],\"seriesKeys\":[") + $"\"{dataArray[i, 1]}\"";
                    seriesColors += (!string.IsNullOrEmpty(seriesColors) ? "," : "],\"seriesColors\":[") + $"\"{dataArray[i, 2]}\"";
                    seriesOrders += (!string.IsNullOrEmpty(seriesOrders) ? "," : "],\"seriesOrders\":[") + $"\"{dataArray[i, 3]}\"";
                    seriesStacks += Type == Types.stackedBar ? ((!string.IsNullOrEmpty(seriesStacks) ? "," : "],\"seriesStacks\":[") + $"\"{dataArray[i, 4]}\"") : string.Empty;
                    seriesYAxis += Type == Types.multiAxisLine ? ((!string.IsNullOrEmpty(seriesYAxis) ? "," : "],\"seriesYAxis\":[") + $"\"{dataArray[i, 6]}\"") : string.Empty;
                }
            }
 //           return $"{seriesLabels}{seriesKeys}{seriesColors}{seriesOrders}{(Type == Types.stackedBar ? seriesStacks : string.Empty)}{seriesYAxis}]}}]";
            return $"{seriesLabels}{seriesKeys}{seriesColors}{seriesOrders}{seriesStacks}{seriesYAxis}]}}]";
        }
    }
}
