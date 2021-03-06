﻿#region Copyright
//  Copyright 2016 OSIsoft, LLC
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
#endregion

using System.Collections.Generic;
using System.Linq;
using OSIsoft.AF;
using OSIsoft.AF.EventFrame;
using OSIsoft.AF.Time;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.PI;
using OSIsoft.AF.Search;
using LimitCalculatorSDK;

namespace EventFrameAnalysis
{
    class LimitCalculation
    {
        private PISystem pisystem;
        public AFDatabase afdatabase;
        static readonly AFEnumerationValue nodata = (new PIServers().DefaultPIServer).StateSets["SYSTEM"]["NO Data"];
        static AFTimeSpan interval = new AFTimeSpan(seconds: 1);

        private CalculationPreference preference;
        private double offset;
        private readonly AFEventFrameSearch eventFrameQuery;
        private AFEventFrameSearch currentFrameQuery;
        private readonly AFAttribute sensor;
        private Dictionary<AFAttributeTrait, AFValues> bounds = new Dictionary<AFAttributeTrait, AFValues> { };
        private Dictionary<AFAttributeTrait, AFAttribute> boundAttributes = new Dictionary<AFAttributeTrait, AFAttribute> { };

        private string calculationName;
        Dictionary<AFAttributeTrait, string> calculationsToPerform;

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LimitCalculation(CalculationPreference preference, string calculationName)
        {
            this.calculationName = calculationName;
            try {
                logger.Info($"Starting calculations for {calculationName}");
                string afattributepath = preference.sensorPath;
                string eventQuery = preference.eventFrameQuery;
                calculationsToPerform = preference.getTraitDictionary();
                offset = preference.offset;
                this.preference = preference;
                sensor = AFAttribute.FindAttribute(afattributepath, null);
                pisystem = sensor.PISystem;
                afdatabase = sensor.Database;
                foreach (KeyValuePair<AFAttributeTrait, string> pair in calculationsToPerform)
                {
                    bounds[pair.Key] = new AFValues();
                    AFAttribute possibleAttribute = sensor.GetAttributeByTrait(pair.Key);
                    boundAttributes[pair.Key] = possibleAttribute;
                    logger.Info($"Will perform calculation for limit: {pair.Key}");
                    if (possibleAttribute == null)
                    {
                        logger.Error($"{calculationName}: The limit {pair.Key} is not defined yet is used.");
                    }
                }
                eventFrameQuery = new AFEventFrameSearch(afdatabase, "eventFrameSearch", eventQuery);
            }
            catch (System.Exception e)
            {
                logger.Error($"{calculationName} the following error occured: {e.Message}");
            }
            logger.Info($"{calculationName}: Doing the initial run");
            InitialRun();
        }

        internal bool timelessMatch(AFEventFrameSearch query, AFEventFrame ef)
        {
            List<AFSearchToken> tokens = query.Tokens.ToList();
            tokens.RemoveAll(t => t.Filter == AFSearchFilter.InProgress || t.Filter == AFSearchFilter.Start || t.Filter == AFSearchFilter.End || t.Filter == AFSearchFilter.Duration);

            AFEventFrameSearch timeless = new AFEventFrameSearch(query.Database, "AllEventFrames", tokens);
            logger.Debug($"{calculationName} : Attemps to match {timeless.ToString()} on event {ef.Name}");
            try
            {
                return timeless.IsMatch(ef);
            }
            catch (System.FormatException e)
            {
                logger.Error($"There was an error with the query: {e.Message}");
                return false;
            }
        }

        internal static AFEventFrameSearch currentEventFrame(AFEventFrameSearch query)
        {
            List<AFSearchToken> tokens = query.Tokens.ToList();
            tokens.RemoveAll(t => t.Filter == AFSearchFilter.InProgress || t.Filter == AFSearchFilter.AllDescendants || t.Filter == AFSearchFilter.End || t.Filter == AFSearchFilter.Start || t.Filter == AFSearchFilter.Duration);
            AFSearchToken inprogress = new AFSearchToken(AFSearchFilter.InProgress, AFSearchOperator.Equal, "True");
            tokens.Add(inprogress);
            return new AFEventFrameSearch(query.Database, "CurrentEventFrame", tokens);
        }

        internal AFValue performCalculation(string equation, AFValues slice)
        {
            IDictionary<AFSummaryTypes, AFValue> statisticForSlice = GetStatistics(slice);
            AFTime time = statisticForSlice[AFSummaryTypes.Average].Timestamp;
            double mean = statisticForSlice[AFSummaryTypes.Average].ValueAsDouble();
            //double stddev = statisticForSlice[AFSummaryTypes.StdDev].ValueAsDouble();
            //double maximum = statisticForSlice[AFSummaryTypes.Maximum].ValueAsDouble();
            //double minimum = statisticForSlice[AFSummaryTypes.Minimum].ValueAsDouble();
            switch (equation)
            {
                /*
                case "μ + 3σ":
                    return new AFValue(mean + 3 * stddev, time);
                case "μ - 3σ":
                    return new AFValue(mean - 3 * stddev, time);
                case "μ + 2σ":
                    return new AFValue(mean + 2 * stddev, time);
                case "μ - 2σ":
                    return new AFValue(mean - 2 * stddev, time);
                case "μ + offset":
                    return new AFValue(mean + offset);
                case "μ - offset":
                    return new AFValue(mean - offset);
                case "μ + σ":
                    return new AFValue(mean + stddev, time);
                case "μ - σ":
                    return new AFValue(mean - stddev, time);*/
                case "μ":
                    return new AFValue(mean, time);
                /*
                case "Maximum":
                    return new AFValue(maximum, time);
                case "Minimum":
                    return new AFValue(minimum, time);
                    */
            }
            logger.Error($"{calculationName} The specified calculation: {equation} method is unknown");
            return null;
        }

        internal void InitialRun()
        {
            currentFrameQuery = currentEventFrame(eventFrameQuery);
            ComputeStatistics();
            //AFEventFrameSearch currentEventFrameQuery = currentEventFrame(eventFrameQuery);

            IEnumerable<AFEventFrame> currentEventFrames = currentFrameQuery.FindEventFrames(0, true, int.MaxValue);
            try { 
                foreach (AFEventFrame currentEventFrame in currentEventFrames)
                {
                    WriteValues(currentEventFrame.StartTime);
                }
            }
            catch (System.Exception e)
            {
                logger.Error($"{calculationName} : Was not able to write initial data due {e.Message}");
            }
        }

        internal void ComputeStatistics()
        {
            logger.Info($"{calculationName} Starting some recalcuations");
            IEnumerable<AFEventFrame> eventFrames = eventFrameQuery.FindEventFrames(0, true, int.MaxValue);
            //if (eventFrames)

            List<AFValues> trends = new List<AFValues>();
            try
            {
                foreach (AFEventFrame EF in eventFrames)
                {
                    trends.Add(sensor.Data.InterpolatedValues(EF.TimeRange, interval, null, "", true));
                }
                logger.Debug($"{calculationName} : Succefully captured data and eventframes");
            }
            catch (System.Exception e)
            {
                logger.Error($"{calculationName} : Was not succesfull in querying data or event frames, {e.Message}");
                return;
            }
            List<AFValues> slices = GetSlices(trends);

            foreach (KeyValuePair<AFAttributeTrait, AFValues> bound in bounds)
            {
                bound.Value.Clear();
            }
            foreach (AFValues slice in slices)
            {
                foreach (KeyValuePair<AFAttributeTrait, AFValues> bound in bounds)
                {
                    bound.Value.Add(performCalculation(calculationsToPerform[bound.Key], slice));
                }
            }
            logger.Info($"{calculationName} : Finishing some recalculation");
        }

        internal void WriteValues(AFTime startTime)
        {
            logger.Info($"{calculationName}: Writing data with {startTime}");
            AFValue nodataValue = new AFValue(nodata);
            foreach (KeyValuePair<AFAttributeTrait, AFValues> boundPair in bounds)
            {
                AFValues bound = boundPair.Value;
                nodataValue.Timestamp = timeShift(bound, startTime);
                if (boundAttributes[boundPair.Key] != null)
                {
                    boundAttributes[boundPair.Key].PIPoint.UpdateValues(bound, AFUpdateOption.Insert);
                    boundAttributes[boundPair.Key].PIPoint.UpdateValue(nodataValue, AFUpdateOption.Insert);
                }
            }
        }

        public static IDictionary<AFSummaryTypes, AFValue> GetStatistics(AFValues values)
        {
            if (values.Count != 1)
            {
                AFTimeRange range = new AFTimeRange(values[0].Timestamp, values[values.Count - 1].Timestamp);
                return values.Summary(range, AFSummaryTypes.All, AFCalculationBasis.EventWeighted, AFTimestampCalculation.MostRecentTime);
            }
            else
            {
                IDictionary<AFSummaryTypes, AFValue> dict = new Dictionary<AFSummaryTypes, AFValue>();
                dict[AFSummaryTypes.Average] = values[0];
                //dict[AFSummaryTypes.Maximum] = values[0];
                //dict[AFSummaryTypes.Minimum] = values[0];
                dict[AFSummaryTypes.StdDev] = new AFValue(0, values[0].Timestamp);
                return dict;
            }
        }

        internal static AFTime timeShift(AFValues values, AFTime startTime)
        {
            foreach (AFValue value in values)
            {
                value.Timestamp = startTime;
                startTime += interval;
            }
            return startTime;
        }

        public static List<AFValues> GetSlices(List<AFValues> trends)
        {
            List<AFValues> outer = new List<AFValues>();
            for (int j = 0; j < trends.Count; j++)
            {
                for (int i = 0; i < trends[j].Count; i++)
                {
                    if (outer.Count <= i)
                        outer.Add(new AFValues());
                    outer[i].Add(trends[j][i]);
                }
            }
            return outer;
        }

        public void performAction(AFEventFrame lastestEventFrame, AFChangeInfoAction action)
        {
            logger.Debug($"{calculationName} About to perform a match");
            if (timelessMatch(eventFrameQuery, lastestEventFrame))
            {
                if (action == AFChangeInfoAction.Added)
                {
                    WriteValues(lastestEventFrame.StartTime);
                }
                else if (action == AFChangeInfoAction.Updated || action == AFChangeInfoAction.Removed)
                {
                    ComputeStatistics();
                }
            }
        }
    }
}
