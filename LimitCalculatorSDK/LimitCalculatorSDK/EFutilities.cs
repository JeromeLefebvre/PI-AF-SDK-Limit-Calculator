using OSIsoft.AF.UI;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF.EventFrame;
using System.IO;
using OSIsoft.AF.Time;

namespace LimitCalculatorSDK
{
    public class EFutilities
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static AFEventFrameCriteria queryToCriteria(OSIsoft.AF.Search.AFEventFrameSearch query)
        {
            AFEventFrameCriteria criteria = new AFEventFrameCriteria();

            IList<OSIsoft.AF.Search.AFSearchToken> starttimes;
            query.TryFindSearchTokens(OSIsoft.AF.Search.AFSearchFilter.Start, out starttimes);

            if (starttimes.Count == 2)
            {
                criteria.SearchMode = AFSearchMode.StartInclusive;
                AFTime start = new AFTime(starttimes[0].Value);
                AFTime end = new AFTime(starttimes[1].Value);
                if (start < end)
                {
                    criteria.StartTime = starttimes[0].Value;
                    criteria.EndTime = starttimes[1].Value;
                }
                else
                {
                    criteria.StartTime = starttimes[1].Value;
                    criteria.EndTime = starttimes[0].Value;
                }
            }
            else if (starttimes.Count == 1) {
                criteria.StartTime = starttimes[0].Value;
            }

            OSIsoft.AF.Search.AFSearchToken endtime = new OSIsoft.AF.Search.AFSearchToken();
            query.TryFindSearchToken(OSIsoft.AF.Search.AFSearchFilter.End, out endtime);
            if (endtime.Value != null)
                criteria.EndTime = endtime.Value;
            query.Tokens.Remove(endtime);

            OSIsoft.AF.Search.AFSearchToken inprogress = new OSIsoft.AF.Search.AFSearchToken();
            query.TryFindSearchToken(OSIsoft.AF.Search.AFSearchFilter.InProgress, out inprogress);
            if (inprogress.Value != null)
                criteria.InProgress = inprogress.Value == "True" ? true : false;
            query.Tokens.Remove(inprogress);



            /*
            //
            // Summary:
            //     This is the value of an uninitialized search mode.
            None = 0,
        //
        // Summary:
        //     Includes all objects whose start time is within the specified range. Also known
        //     as "Starting Between".
        StartInclusive = 1,
        //
        // Summary:
        //     Includes all objects whose end time is within the specified range. Also known
        //     as "Ending Between".
        EndInclusive = 2,
        //
        // Summary:
        //     Includes all objects whose start and end time are within the specified range.
        //     Also know as "Entirely Between".
        Inclusive = 3,
        //
        // Summary:
        //     Includes all objects whose time range overlaps with the specified range at any
        //     point in time. Also known as "Active Between".
        Overlapped = 4,
        //
        // Summary:
        //     Includes all objects whose start time is within the specified range and end time
        //     is OSIsoft.AF.Time.AFTime.MaxValue. Also known as "Starting Between and In Progress".
        InProgress = 5
                */
            criteria.LastFullSearchString = query.ToString();
            return criteria;
        }
        public static OSIsoft.AF.Search.AFEventFrameSearch criteriaToQuery(AFEventFrameCriteria criteria)
        {
            OSIsoft.AF.Search.AFSearchFilter startFilter = OSIsoft.AF.Search.AFSearchFilter.Start;
            OSIsoft.AF.Search.AFSearchOperator ge = OSIsoft.AF.Search.AFSearchOperator.GreaterThanOrEqual;
            OSIsoft.AF.Search.AFSearchOperator le = OSIsoft.AF.Search.AFSearchOperator.LessThanOrEqual;
            OSIsoft.AF.Search.AFSearchToken start;
            OSIsoft.AF.Search.AFSearchToken startGE = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.Start, ge, "*");
            OSIsoft.AF.Search.AFEventFrameSearch query = new OSIsoft.AF.Search.AFEventFrameSearch(criteria.Database, "search", criteria.LastFullSearchString);
            OSIsoft.AF.Search.AFSearchToken startTime;
            OSIsoft.AF.Search.AFSearchToken endTime = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.End, OSIsoft.AF.Search.AFSearchOperator.LessThanOrEqual, criteria.EndTime);
            OSIsoft.AF.Search.AFSearchToken inProgess = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.InProgress, OSIsoft.AF.Search.AFSearchOperator.Equal, criteria.InProgress.ToString());

            if (criteria.SearchMode == AFSearchMode.StartInclusive)
            {
                start = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.Start, le, criteria.StartTime);
                query.Tokens.Add(start);
                start = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.Start, le, criteria.EndTime);
                query.Tokens.Add(start);
            }

            query.Tokens.Add(inProgess);
            return query;
        }

    }
}