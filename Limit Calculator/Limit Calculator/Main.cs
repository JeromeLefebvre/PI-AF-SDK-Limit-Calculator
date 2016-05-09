using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OSIsoft.AF;
using OSIsoft.AF.UI;
using OSIsoft.AF.Asset;
using LimitCalculatorSDK;
using System.Security.Principal;
using System.Net;

namespace Limit_Calculator
{
    public partial class Main : Form
    {
        static readonly List<string> possibleOperations = new List<string> { "None",
                                                                    "Minimum",
                                                                    "μ - 3σ",
                                                                    "μ - 2σ",
                                                                    "μ - σ",
                                                                    "μ",
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

        private void Main_Load(object sender, EventArgs e)
        {
            //queryTextBox.Text = @"Start:>*-10h End:<*-1h Inprogress:=False";
            // TODO: Create the LimitCalculator Element if it does not exists 
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
            AFTreeNode node = (AFTreeNode)afTreeView.SelectedNode;
            string path = node.AFPath;
            string query = queryTextBox.Text;
            Dictionary<string, string> limits = new Dictionary<string, string> { };

            foreach (AFAttributeTrait limit in AFAttributeTrait.AllLimits)
            {
                ComboBox comboBox = (ComboBox)panel1.Controls[limit.Name];
                if (comboBox.Text != "None")
                {
                    limits[limit.Name] = comboBox.Text;
                }
            }
            CalculationPreference preference = new CalculationPreference(path, query, limits);

            AFElement preferenceRoot = ((AFElement)configurationTreeView.AFRoot);
            AFElement newCalculation = new AFElement(calculationName.Text);

            newCalculation.Attributes.Add("Configuration");
            newCalculation.Attributes["Configuration"].Type = typeof(string);
            newCalculation.Attributes["Configuration"].SetValue(new AFValue(preference.JSON()));
            AFElement alreadyThere = preferenceRoot.Elements[calculationName.Text];
            if (alreadyThere != null)
                preferenceRoot.Elements.Remove(alreadyThere);
            
            preferenceRoot.Elements.Add(newCalculation);
            preferenceRoot.CheckIn();
        }

        private void displaySearch_Click(object sender, EventArgs e)
        {
            EventFrameSearch h = new EventFrameSearch(this, db);

            OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage search = (OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage)h.Controls["eventFrameSearchPage"];
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

            criteria.LastFullSearchString = query.ToString();
            search.EventFrameCriteria = criteria;

            h.Show();
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
                AFAttribute sensor = AFAttribute.FindAttribute(preference.sensorPath, db);
                afTreeView.AFSelect(sensor, db, preference.sensorPath);
            }
        }

        private void piSystemPicker_ConnectionChange(object sender, SelectionChangeEventArgs e)
        {
            updatePreferenceTree();
        }
    }
}
