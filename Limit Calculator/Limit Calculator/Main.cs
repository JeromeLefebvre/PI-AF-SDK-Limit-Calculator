using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OSIsoft.AF;
using OSIsoft.AF.UI;
using OSIsoft.AF.Asset;
using LimitCalculatorSDK;
using System.Threading;

namespace Limit_Calculator
{
    public partial class Main : Form
    {
        static readonly List<string> possibleOperations = new List<string> { "None",
                                                                    "Minimum",
                                                                    "μ - 3σ",
                                                                    "μ - 2σ",
                                                                    "μ - σ",
                                                                    "μ - offset",
                                                                    "μ",
                                                                    "μ + offset",
                                                                    "μ + σ",
                                                                    "μ + 2σ",
                                                                    "μ + 3σ",
                                                                    "Maximum"};
        private AFDatabase db = null;

        public Main()
        {
            InitializeComponent();
            afDatabasePicker.SystemPicker = piSystemPicker;

            CreatePanel();
        }

        private void CreatePanel()
        {
            int horizontal = 10;
            int vertical = 25;
            foreach (AFAttributeTrait limit in AFAttributeTrait.AllLimits)
            {
                panel1.Controls.Add(new Label
                {
                    Text = limit.Abbreviation,
                    Location = new System.Drawing.Point(horizontal, vertical)
                });

                panel1.Controls.Add(new ComboBox
                {
                    Location = new System.Drawing.Point(horizontal + 100, vertical),
                    Name = limit.Name,
                    DataSource = possibleOperations.ToArray()
                });
                vertical += 22;
            }
        }

        private void afDatabasePicker_SelectionChange(object sender, OSIsoft.AF.UI.SelectionChangeEventArgs e)
        {
            db = afDatabasePicker.AFDatabase;
            if (db != null)
            {
                afTreeView.AFRoot = db.Elements;
                
            }
        }

        private void addToPreference_Click(object sender, EventArgs e)
        {
            if (calculationName.Text == "")
            {
                MessageBox.Show("Please specify the name of the calculation", "No Name specified", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            AFTreeNode node = (AFTreeNode)afTreeView.SelectedNode;
            string path = node.AFPath;
            string query = queryTextBox.Text;
            double offset = Convert.ToDouble(offsetSetting.Text);
            Dictionary<string, string> limits = new Dictionary<string, string> { };

            foreach (AFAttributeTrait limit in AFAttributeTrait.AllLimits)
            {
                ComboBox comboBox = (ComboBox)panel1.Controls[limit.Name];
                if (comboBox.Text != "None")
                {
                    limits[limit.Name] = comboBox.Text;
                }
            }
            CalculationPreference preference = new CalculationPreference(path, query, offset, limits);

            AFElement preferenceRoot = ((AFElement)configurationTreeView.AFRoot);
            AFValue configurationValue = new AFValue(preference.JSON());
            AFElement preferenceElement = preferenceRoot.Elements[calculationName.Text];

            if (preferenceElement == null)
            {
                preferenceElement = new AFElement(calculationName.Text);
                preferenceElement.Attributes.Add("Configuration");
                preferenceElement.Attributes["Configuration"].Type = typeof(string);
                preferenceRoot.Elements.Add(preferenceElement);
                preferenceRoot.CheckIn();
            }
            preferenceElement.Attributes["Configuration"].SetValue(configurationValue);
            preferenceElement.CheckIn();
            configurationTreeView.Refresh();
            configurationTreeView.AFSelect(preferenceElement, preferenceElement.Database, preferenceElement.GetPath());
        }

        private void displaySearch_Click(object sender, EventArgs e)
        {
            EventFrameSearch h = new EventFrameSearch(this, db);

            OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage search = (OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage)h.Controls["eventFrameSearchPage"];
            search.Database = db;
            /*
            AFEventFrameCriteria criteria = search.EventFrameCriteria;

            OSIsoft.AF.Search.AFEventFrameSearch query = new OSIsoft.AF.Search.AFEventFrameSearch(db, "search", queryTextBox.Text);

            OSIsoft.AF.Search.AFSearchToken token = new OSIsoft.AF.Search.AFSearchToken();
            query.TryFindSearchToken(OSIsoft.AF.Search.AFSearchFilter.Start, out token);
            if (token.Value != null)
                criteria.StartTime = token.Value;
            query.Tokens.Remove(token);

            query.TryFindSearchToken(OSIsoft.AF.Search.AFSearchFilter.End, out token);
            if (token.Value != null)
                criteria.EndTime = token.Value;
            query.Tokens.Remove(token);

            query.TryFindSearchToken(OSIsoft.AF.Search.AFSearchFilter.InProgress, out token);
            if (token.Value != null)
                criteria.InProgress = token.Value == "True" ? true : false;
            query.Tokens.Remove(token);

            criteria.LastFullSearchString = query.ToString();  */
            search.EventFrameCriteria = queryToCriteria(queryTextBox.Text);

            h.Show();
        }


        private AFEventFrameCriteria queryToCriteria(string queryText)
        {
            AFEventFrameCriteria criteria = new AFEventFrameCriteria();
            OSIsoft.AF.Search.AFEventFrameSearch query = new OSIsoft.AF.Search.AFEventFrameSearch(db, "search", queryText);

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

        private void piSystemPicker_SelectionChange(object sender, SelectionChangeEventArgs e)
        {    
            PISystem pisystem = piSystemPicker.PISystem;
            if (pisystem != null)
            {
                AFElement limitcalculator = piSystemPicker.PISystem.Databases["Configuration"].Elements["LimitCalculator"];
                if (limitcalculator == null)
                {
                    piSystemPicker.PISystem.Databases["Configuration"].Elements.Add(new AFElement("LimitCalculator"));
                    piSystemPicker.PISystem.Databases["Configuration"].CheckIn();
                }
                configurationTreeView.AFRoot = piSystemPicker.PISystem.Databases["Configuration"].Elements["LimitCalculator"];
            }
        }

        private void updatePreferenceTree()
        {
            configurationTreeView.AFRoot = piSystemPicker.PISystem.Databases["Configuration"].Elements["LimitCalculator"];
        }

        private void configurationTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            AFTreeNode node = (AFTreeNode)e.Node;
            AFElement selectedCalculation = (AFElement)node.AFObject;
            if (!selectedCalculation.IsRoot)
            {
                calculationName.Text = selectedCalculation.Name;
                CalculationPreference preference = CalculationPreference.CalculationPreferenceFromJSON((string)selectedCalculation.Attributes["configuration"].GetValue().Value);
                queryTextBox.Text = preference.eventFrameQuery;
                Dictionary<AFAttributeTrait, string> limitCalculations = preference.getTraitDictionary();
                foreach (AFAttributeTrait trait in AFAttributeTrait.AllLimits)
                {
                    ComboBox comboBox = (ComboBox)panel1.Controls[trait.Name];
                    comboBox.Text = "None";
                }
                foreach (KeyValuePair<AFAttributeTrait, string> pair in limitCalculations)
                {
                    ComboBox comboBox = (ComboBox)panel1.Controls[pair.Key.Name];
                    comboBox.Text = pair.Value;
                }
                offsetSetting.Text = preference.offset.ToString();
                AFAttribute sensor = AFAttribute.FindAttribute(preference.sensorPath, db);
                afDatabasePicker.AFDatabase = sensor.Database;
                afTreeView.AFRoot = sensor.Database.Elements;

                afTreeView.AFSelect(sensor, db, preference.sensorPath);
            }
        }

        private void piSystemPicker_ConnectionChange(object sender, SelectionChangeEventArgs e)
        {
            updatePreferenceTree();
        }
    }
}
