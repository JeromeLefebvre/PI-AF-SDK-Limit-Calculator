using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OSIsoft.AF;
using OSIsoft.AF.UI;
using OSIsoft.AF.Asset;
using LimitCalculatorSDK;

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

        EventFrameSearch h;
        OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage search;

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
            search.EventFrameCriteria.SaveCriteria(calculationName.Text);
        }


        private void displaySearch_Click(object sender, EventArgs e)
        {
            refreshSearchWindow();
            h.Show();
        }

        private void refreshSearchWindow()
        {
            h = new EventFrameSearch(this, db);
            search = (OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage)h.Controls["eventFrameSearchPage"];
            search.EventFrameCriteria.RestoreCriteria(calculationName.Text);
            search.EventFrameCriteria.LastFullSearchString = queryTextBox.Text;
            search.CriteriaInitiallyOpened = true;
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
