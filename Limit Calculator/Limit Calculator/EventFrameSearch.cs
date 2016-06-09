using OSIsoft.AF;
using OSIsoft.AF.EventFrame;
using OSIsoft.AF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LimitCalculatorSDK;
using OSIsoft.AF.Search;
using OSIsoft.AF.Time;

namespace Limit_Calculator
{
    public partial class EventFrameSearch : Form
    {
        private Main main;
        public AFDatabase db;

        public EventFrameSearch()
        {
            InitializeComponent();
            this.db = PISystem.CreatePISystem("localhost").Databases["JDIData"];
        }

        public EventFrameSearch(Main ParentForm)
        {
            InitializeComponent();
            main = ParentForm;
            this.db = null;
        }

        public EventFrameSearch(Main ParentForm, AFDatabase db)
        {
            InitializeComponent();
            main = ParentForm;
            this.db = db;
        }

        public static void compareEventFrame(OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage uisearch, OSIsoft.AF.Search.AFEventFrameSearch afsdksearch)
        {
            string path = @"C:\Users\jlefebvre\Desktop\differences\";
            //AFNamedCollectionList<AFEventFrame> uiEF_raw = uisearch.EventFrames;
            List<AFEventFrame> asfkEF = afsdksearch.FindEventFrames().ToList<AFEventFrame>();
            List<AFEventFrame> uiEF = uisearch.EventFrames.ToList<AFEventFrame>();//new List<AFEventFrame>();
            List<AFEventFrame> uiExceptafsdk = uiEF.Except(asfkEF).ToList();
            List<AFEventFrame> afsdkExceptui = asfkEF.Except(uiEF).ToList();
            if (uiExceptafsdk.Count != 0)
            {
                List<string> ef_names = uiExceptafsdk.Select(ef => "U " + ef.Name).ToList();
                File.WriteAllLines(path + "InUInotAFSDK.txt", ef_names);
            }
            if (afsdkExceptui.Count != 0)
            {
                List<string> ef_names = afsdkExceptui.Select(ef => "A " + ef.Name).ToList();
                File.WriteAllLines(path + "InASDKnotUI.txt", ef_names);
            }
        }
        private void eventFrameSearchPage_SearchCompleted(object sender, EventArgs e)
        {
            //AFEventFrameCriteria criteria = eventFrameSearchPage.EventFrameCriteria;
            //OSIsoft.AF.Search.AFEventFrameSearch createdQUery = LimitCalculatorSDK.EFutilities.criteriaToQuery(criteria);
            // Convert the criteria to a AFSDK criteria here
            if (eventFrameSearchPage.EventFrameCriteria.LastFullSearchString.Contains("Root:"))
            {
            }
            OSIsoft.AF.Search.AFEventFrameSearch criteria = searchToCriteria(eventFrameSearchPage);
            main.Controls["queryTextBox"].Text = criteria.ToString();
        }

        private static OSIsoft.AF.Search.AFEventFrameSearch searchToCriteria(OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage page)
        {
            //page.EventFrameCriteria.SearchMode;

            AFDatabase db = page.Database;
            string start = page.EventFrameCriteria.StartTime;
            string end = page.EventFrameCriteria.EndTime;
            string AFstart = page.EventFrameCriteria.AFStartTimeString;
            bool inProgress = page.EventFrameCriteria.InProgress;
            string LastFullSeach = page.EventFrameCriteria.LastFullSearchString;
            LastFullSeach = LastFullSeach.Replace(@"\", "").Replace("\'", "'");
            OSIsoft.AF.Search.AFEventFrameSearch timelessTerms = new OSIsoft.AF.Search.AFEventFrameSearch(db, "notime", LastFullSeach);
            List<AFSearchToken> tokens = timelessTerms.Tokens.ToList();
            switch (page.EventFrameCriteria.SearchType)
            {
                case OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.ActiveBetween:
                    tokens.Add(new AFSearchToken(AFSearchFilter.Start, AFSearchOperator.LessThanOrEqual, end));
                    tokens.Add(new AFSearchToken(AFSearchFilter.End, AFSearchOperator.GreaterThanOrEqual, start));
                    break;
                case OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.EndingAfter:
                    tokens.Add(new AFSearchToken(AFSearchFilter.End, AFSearchOperator.GreaterThanOrEqual, AFstart));
                    break;
                case OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.EndingBefore:
                    tokens.Add(new AFSearchToken(AFSearchFilter.End, AFSearchOperator.LessThanOrEqual, AFstart));
                    break;
                case OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.EndingBetween:
                    tokens.Add(new AFSearchToken(AFSearchFilter.End, AFSearchOperator.GreaterThanOrEqual, start));
                    tokens.Add(new AFSearchToken(AFSearchFilter.End, AFSearchOperator.LessThanOrEqual, end));
                    break;
                case OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.EntirelyBetween:
                    tokens.Add(new AFSearchToken(AFSearchFilter.Start, AFSearchOperator.GreaterThanOrEqual, start));
                    tokens.Add(new AFSearchToken(AFSearchFilter.End, AFSearchOperator.LessThanOrEqual, end));
                    break;
                case OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.StartingAfter:
                    tokens.Add(new AFSearchToken(AFSearchFilter.Start, AFSearchOperator.GreaterThanOrEqual, AFstart));
                    if (inProgress)
                        tokens.Add(new AFSearchToken(AFSearchFilter.InProgress, AFSearchOperator.Equal, "true"));
                    break;
                case OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.StartingBefore:
                    tokens.Add(new AFSearchToken(AFSearchFilter.Start, AFSearchOperator.LessThanOrEqual, AFstart));
                    if (inProgress)
                        tokens.Add(new AFSearchToken(AFSearchFilter.InProgress, AFSearchOperator.Equal, "true"));
                    break;
                case OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.StartingBetween:
                    tokens.Add(new AFSearchToken(AFSearchFilter.Start, AFSearchOperator.GreaterThanOrEqual, start));
                    tokens.Add(new AFSearchToken(AFSearchFilter.Start, AFSearchOperator.LessThanOrEqual, end));
                    if (inProgress)
                        tokens.Add(new AFSearchToken(AFSearchFilter.InProgress, AFSearchOperator.Equal, "true"));
                    break;
            }

            OSIsoft.AF.Search.AFEventFrameSearch criteria = new OSIsoft.AF.Search.AFEventFrameSearch(db, "search", tokens);

            AFNamedCollection<AFEventFrame> UIFrames = page.EventFrames;
            
            IEnumerable<AFEventFrame> AFSDKframes = criteria.FindEventFrames();
            try
            {
                int count = AFSDKframes.Count() > 1000 ? 1000 : AFSDKframes.Count();
                if (UIFrames.Count != count)
                    MessageBox.Show("They UI and AFKSDK frames report different variables", "Count error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (System.FormatException e)
            {
                // Bad event frame search formatting for AFSDK UI.
                MessageBox.Show(e.Message);
            }
            return criteria;
        }


        private void EventFrameSearch_Load(object sender, EventArgs e)
        {
            this.eventFrameSearchPage.Dock = DockStyle.Fill;
        }

        private void eventFrameSearchPage_Load(object sender, EventArgs e)
        {

        }
    }
}
