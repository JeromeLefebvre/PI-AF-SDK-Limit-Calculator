using OSIsoft.AF;
using OSIsoft.AF.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            OSIsoft.AF.Search.AFEventFrameSearch query = new OSIsoft.AF.Search.AFEventFrameSearch(db, "search", eventFrameSearchPage.EventFrameCriteria.LastFullSearchString);
            OSIsoft.AF.Search.AFSearchToken startTime = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.Start, OSIsoft.AF.Search.AFSearchOperator.GreaterThanOrEqual, criteria.StartTime);
            OSIsoft.AF.Search.AFSearchToken endTime = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.End, OSIsoft.AF.Search.AFSearchOperator.LessThanOrEqual, criteria.EndTime);
            OSIsoft.AF.Search.AFSearchToken inProgess = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.InProgress, OSIsoft.AF.Search.AFSearchOperator.Equal, criteria.InProgress.ToString());

            query.Tokens.Add(startTime);
            query.Tokens.Add(endTime);
            query.Tokens.Add(inProgess);
            main.Controls["queryTextBox"].Text = query.ToString();
        }

        private void EventFrameSearch_Load(object sender, EventArgs e)
        {
            this.eventFrameSearchPage.Dock = DockStyle.Fill;
        }
    }
}
