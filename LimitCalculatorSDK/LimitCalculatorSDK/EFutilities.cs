using OSIsoft.AF.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LimitCalculatorSDK
{
    class EFutilities
    {
        static AFEventFrameCriteria queryToCriteria(OSIsoft.AF.Search.AFEventFrameSearch query)
        {
            AFEventFrameCriteria criteria = new AFEventFrameCriteria();
            //OSIsoft.AF.Search.AFEventFrameSearch query = new OSIsoft.AF.Search.AFEventFrameSearch(db, "search", queryText);

            OSIsoft.AF.Search.AFSearchToken starttime = new OSIsoft.AF.Search.AFSearchToken();
            query.TryFindSearchToken(OSIsoft.AF.Search.AFSearchFilter.Start, out starttime);
            if (starttime.Value != null)
                criteria.StartTime = starttime.Value;
            query.Tokens.Remove(starttime);

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


            // Pick the search mode
            if (starttime != null && endtime != null)
            {

            }

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
        static OSIsoft.AF.Search.AFEventFrameSearch criteriaToQuery(AFEventFrameCriteria criteria)
        {
            OSIsoft.AF.Search.AFEventFrameSearch query = new OSIsoft.AF.Search.AFEventFrameSearch(criteria.Database, "search", criteria.LastFullSearchString);
            OSIsoft.AF.Search.AFSearchToken startTime = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.Start, OSIsoft.AF.Search.AFSearchOperator.GreaterThanOrEqual, criteria.StartTime);
            OSIsoft.AF.Search.AFSearchToken endTime = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.End, OSIsoft.AF.Search.AFSearchOperator.LessThanOrEqual, criteria.EndTime);
            OSIsoft.AF.Search.AFSearchToken inProgess = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.InProgress, OSIsoft.AF.Search.AFSearchOperator.Equal, criteria.InProgress.ToString());

            query.Tokens.Add(startTime);
            query.Tokens.Add(endTime);
            query.Tokens.Add(inProgess);
            return query;
        }
    }
}