using OSIsoft.AF;
using OSIsoft.AF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace Limit_Calculator
{
    public partial class EventFrameSearch : Form
    {
        private Main main;
        private AFDatabase db;

        public EventFrameSearch(Main ParentForm, AFDatabase db)
        {
            InitializeComponent();
            main = ParentForm;
            this.db = db;
        }

        private void eventFrameSearchPage_SearchCompleted(object sender, EventArgs e)
        {
            AFEventFrameCriteria criteria = eventFrameSearchPage.EventFrameCriteria;
            var searchType = criteria.SearchType;
            if (eventFrameSearchPage.EventFrameCriteria.LastFullSearchString.Contains("Root:"))
            {
                MessageBox.Show("Root element search is not supported", "Root filter is not supported", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else { 
                OSIsoft.AF.Search.AFEventFrameSearch query = new OSIsoft.AF.Search.AFEventFrameSearch(db, "search", eventFrameSearchPage.EventFrameCriteria.LastFullSearchString);
                OSIsoft.AF.Search.AFSearchToken startTime = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.Start, OSIsoft.AF.Search.AFSearchOperator.GreaterThanOrEqual, criteria.StartTime);
                OSIsoft.AF.Search.AFSearchToken endTime = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.End, OSIsoft.AF.Search.AFSearchOperator.LessThanOrEqual, criteria.EndTime);
                OSIsoft.AF.Search.AFSearchToken inProgess = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.InProgress, OSIsoft.AF.Search.AFSearchOperator.Equal, criteria.InProgress.ToString());

                query.Tokens.Add(startTime);
                query.Tokens.Add(endTime);
                query.Tokens.Add(inProgess);
                main.Controls["queryTextBox"].Text = query.ToString();
                AFNamedCollectionList <OSIsoft.AF.EventFrame.AFEventFrame> frames = eventFrameSearchPage.EventFrames;
                IEnumerable<OSIsoft.AF.EventFrame.AFEventFrame> queryFrames = query.FindEventFrames();
                foreach (var frame in queryFrames)
                {

                    if (!frames.Contains(frame))
                    {
                        
                    }
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
            }
        }

        private void EventFrameSearch_Load(object sender, EventArgs e)
        {
            this.eventFrameSearchPage.Dock = DockStyle.Fill;
        }
    }
}
