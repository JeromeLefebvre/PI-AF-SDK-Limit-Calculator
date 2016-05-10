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
        private AFDatabase afdatabse;
        static readonly AFEnumerationValue nodata = (new PIServers().DefaultPIServer).StateSets["SYSTEM"]["NO Data"];
        static AFTimeSpan interval = new AFTimeSpan(seconds: 1);

        private readonly AFEventFrameSearch eventFrameQuery;
        private readonly AFAttribute sensor;
        private Dictionary<AFAttributeTrait, AFValues> bounds = new Dictionary<AFAttributeTrait, AFValues> { };
        private Dictionary<AFAttributeTrait, AFAttribute> boundAttributes = new Dictionary<AFAttributeTrait, AFAttribute> { };

        Dictionary<AFAttributeTrait, string> calculationsToPerform;

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public LimitCalculation(CalculationPreference preference)
        {
            string afattributepath = preference.sensorPath;
            string eventQuery = preference.eventFrameQuery;
            calculationsToPerform = preference.getTraitDictionary();

            sensor = AFAttribute.FindAttribute(afattributepath, null);
            pisystem = sensor.PISystem;
            afdatabse = sensor.Database;
            foreach (KeyValuePair<AFAttributeTrait, string> pair in calculationsToPerform)
            {
                bounds[pair.Key] = new AFValues();
                boundAttributes[pair.Key] = sensor.GetAttributeByTrait(pair.Key);
            }
            eventFrameQuery = new AFEventFrameSearch(afdatabse, "eventFrameSearch", eventQuery);
            InitialRun();
        }

        internal static bool timelessMatch(AFEventFrameSearch query, AFEventFrame ef)
        {
            List<AFSearchToken> tokens = query.Tokens.ToList();
            tokens.RemoveAll(t => t.Filter == AFSearchFilter.InProgress || t.Filter == AFSearchFilter.Start || t.Filter == AFSearchFilter.End);
            AFEventFrameSearch timeless = new AFEventFrameSearch(query.Database, "AllEventFrames", tokens);
            return timeless.IsMatch(ef);
        }

        internal static AFEventFrameSearch currentEventFrame(AFEventFrameSearch query)
        {
            List<AFSearchToken> tokens = query.Tokens.ToList();
            tokens.RemoveAll(t => t.Filter == AFSearchFilter.InProgress || t.Filter == AFSearchFilter.AllDescendants || t.Filter == AFSearchFilter.End);
            AFSearchToken inprogress = new AFSearchToken(AFSearchFilter.InProgress, AFSearchOperator.Equal, "True");
            tokens.Add(inprogress);
            return new AFEventFrameSearch(query.Database, "CurrentEventFrame", tokens);
        }

        internal AFValue performCalculation(string calculationName, AFValues slice)
        {
            IDictionary<AFSummaryTypes, AFValue> statisticForSlice = GetStatistics(slice);
            AFTime time = statisticForSlice[AFSummaryTypes.Average].Timestamp;
            double mean = statisticForSlice[AFSummaryTypes.Average].ValueAsDouble();
            double stddev = statisticForSlice[AFSummaryTypes.StdDev].ValueAsDouble();
            double maximum = statisticForSlice[AFSummaryTypes.Maximum].ValueAsDouble();
            double minimum = statisticForSlice[AFSummaryTypes.Minimum].ValueAsDouble();
            switch (calculationName)
            {
                case "μ + 3σ":
                    return new AFValue(mean + 3 * stddev, time);
                case "μ - 3σ":
                    return new AFValue(mean - 3 * stddev, time);
                case "μ + 2σ":
                    return new AFValue(mean + 2 * stddev, time);
                case "μ - 2σ":
                    return new AFValue(mean - 2 * stddev, time);
                case "μ + σ":
                    return new AFValue(mean + stddev, time);
                case "μ - σ":
                    return new AFValue(mean - stddev, time);
                case "μ":
                    return new AFValue(mean, time);
                case "Maximum":
                    return new AFValue(maximum, time);
                case "Minimum":
                    return new AFValue(minimum, time);
            }
            // this should throw an exception
            return null;
        }

        internal void InitialRun()
        {
            ComputeStatistics();
            AFEventFrameSearch currentEventFrameQuery = currentEventFrame(eventFrameQuery);

            IEnumerable<AFEventFrame> currentEventFrames = currentEventFrameQuery.FindEventFrames(0, true, int.MaxValue);
            foreach (AFEventFrame currentEventFrame in currentEventFrames)
            {
                WriteValues(currentEventFrame.StartTime);
            }
        }

        internal void ComputeStatistics()
        {
            IEnumerable<AFEventFrame> eventFrames = eventFrameQuery.FindEventFrames(0, true, int.MaxValue);
            List<AFValues> trends = new List<AFValues>();
            foreach (AFEventFrame EF in eventFrames)
            {
                trends.Add(sensor.Data.InterpolatedValues(EF.TimeRange, interval, null, "", true));
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
        }

        internal void WriteValues(AFTime startTime)
        {
            logger.Info($"Writing data with {startTime}");
            AFValue nodataValue = new AFValue(nodata);
            foreach (KeyValuePair<AFAttributeTrait, AFValues> boundPair in bounds)
            {
                AFValues bound = boundPair.Value;
                nodataValue.Timestamp = timeShift(bound, startTime);
                boundAttributes[boundPair.Key].PIPoint.UpdateValues(bound, AFUpdateOption.Insert);
                boundAttributes[boundPair.Key].PIPoint.UpdateValue(nodataValue, AFUpdateOption.Insert);
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
                dict[AFSummaryTypes.Maximum] = values[0];
                dict[AFSummaryTypes.Minimum] = values[0];
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
