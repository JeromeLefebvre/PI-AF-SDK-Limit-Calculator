using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OSIsoft.AF;
using OSIsoft.AF.UI;
using OSIsoft.AF.Asset;
using LimitCalculatorSDK;
using OSIsoft.AF.Search;
using System.Linq;
using OSIsoft.AF.Time;

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
            if (path == "" || path == null)
            {
                MessageBox.Show("Please select a attribute", "No attribute specified", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
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
            h = new EventFrameSearch(this, db);
            search = (OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage)h.Controls["eventFrameSearchPage"];
            //search.EventFrameCriteria.RestoreCriteria(calculationName.Text);
            //search.EventFrameCriteria.LastFullSearchString = queryTextBox.Text;*/
            search.CriteriaInitiallyOpened = true;
            OSIsoft.AF.Search.AFEventFrameSearch query = new OSIsoft.AF.Search.AFEventFrameSearch(db, "search", queryTextBox.Text);
            //OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage search = (OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage)h.Controls["eventFrameSearchPage"];
            AFEventFrameCriteria criteria = queryToCriteria(query);
            search.EventFrameCriteria = criteria;
            //refreshSearchWindow();
            h.Show();
        }

        static AFEventFrameCriteria queryToCriteria(OSIsoft.AF.Search.AFEventFrameSearch query)
        {
            AFEventFrameCriteria criteria = new AFEventFrameCriteria();
            criteria.Database = query.Database;
            IList<AFSearchToken> starts;
            query.TryFindSearchTokens(OSIsoft.AF.Search.AFSearchFilter.Start, out starts);
            IList<AFSearchToken> ends;
            query.TryFindSearchTokens(OSIsoft.AF.Search.AFSearchFilter.End, out ends);
            AFSearchToken templatename;
            query.TryFindSearchToken(OSIsoft.AF.Search.AFSearchFilter.Template, out templatename);
            IList<AFSearchToken> values;
            query.TryFindSearchTokens(OSIsoft.AF.Search.AFSearchFilter.Value, out values);

            if (values.Count != 0)
            {
                AFAttributeValueQuery[] queries = new AFAttributeValueQuery[values.Count];
                criteria.AttributeValueQueries = new AFAttributeValueQuery[values.Count];
                criteria.TemplateName = templatename.Value;
                for (int i = 0; i < values.Count; i++)
                {
                    AFSearchToken value = values[i];
                    string attributeName = value.Path.TrimStart(new char[] { '|' });
                    AFElementTemplate template = query.Database.ElementTemplates[templatename.Value];
                    AFAttributeTemplate templateAttribute = template.AttributeTemplates[attributeName];
                    queries[i] = new AFAttributeValueQuery(templateAttribute, value.Operator, value.Value, templateAttribute.DefaultUOM);
                    criteria.AttributeValueQueries[i] = new AFAttributeValueQuery(templateAttribute, value.Operator, value.Value, templateAttribute.DefaultUOM);
                }

                //criteria.AttributeValueQueries = queries;
                //criteria.ValueQueryString = value.ToString();
                //criteria.
            }
            if (ends.Count == 2)
            {
                criteria.SearchType = OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.EndingBetween;
                criteria.StartTime = ends[0].Value;
                criteria.EndTime = ends[1].Value;
            }
            else if (starts.Count == 2)
            {
                criteria.SearchType = OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.StartingBetween;
                criteria.StartTime = starts[0].Value;
                criteria.EndTime = starts[1].Value;
            }
            else if (starts.Count == 1 && ends.Count == 1)
            {
                AFSearchToken start = starts[0];
                AFSearchToken end = ends[0];
                if (start.Operator == AFSearchOperator.LessThanOrEqual && end.Operator == AFSearchOperator.GreaterThanOrEqual)
                {
                    criteria.SearchType = OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.ActiveBetween;
                    criteria.StartTime = end.Value;
                    criteria.EndTime = start.Value;
                }
                else if (start.Operator == AFSearchOperator.GreaterThanOrEqual && end.Operator == AFSearchOperator.LessThanOrEqual)
                {
                    criteria.SearchType = OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.EntirelyBetween;
                    criteria.StartTime = start.Value;
                    criteria.EndTime = end.Value;
                }
            }
            else if (starts.Count == 1)
            {
                AFSearchToken start = starts[0];
                if (start.Operator == AFSearchOperator.GreaterThanOrEqual)
                {
                    criteria.SearchType = OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.StartingAfter;
                }
                else
                {
                    criteria.SearchType = OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.StartingBefore;
                }
                criteria.AFStartTimeString = start.Value;
            }
            else if (ends.Count == 1)
            {
                AFSearchToken end = ends[0];
                if (end.Operator == AFSearchOperator.GreaterThanOrEqual)
                {
                    criteria.SearchType = OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.EndingAfter;
                }
                else
                {
                    criteria.SearchType = OSIsoft.AF.UI.Search.AFBaseEventFrameCriteria.EventFrameSearchType.EndingBefore;
                }
                criteria.AFStartTimeString = end.Value;
            }

            criteria.LastFullSearchString = stripTokens(query);
            return criteria;
        }

        internal static string stripTokens(OSIsoft.AF.Search.AFEventFrameSearch query)
        {
            List<AFSearchToken> tokens = query.Tokens.ToList();
            tokens.RemoveAll(t => t.Filter == AFSearchFilter.InProgress || t.Filter == AFSearchFilter.Start || t.Filter == AFSearchFilter.End || t.Filter == AFSearchFilter.Value);
            OSIsoft.AF.Search.AFEventFrameSearch timeless = new OSIsoft.AF.Search.AFEventFrameSearch(query.Database, "TimeLess", tokens);
            return timeless.ToString();
        }

        private void refreshSearchWindow()
        {
            h = new EventFrameSearch(this, db);
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
                afTreeView.SelectedNode.EnsureVisible();
                afTreeView.Focus();
            }
        }

        private void piSystemPicker_ConnectionChange(object sender, SelectionChangeEventArgs e)
        {
            updatePreferenceTree();
        }

        private void queryTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
