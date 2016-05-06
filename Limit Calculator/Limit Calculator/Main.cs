using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OSIsoft.AF;
using OSIsoft.AF.UI;
using OSIsoft.AF.Asset;
using Newtonsoft.Json;
using LimitCalculatorSDK;

namespace Limit_Calculator
{
    public partial class Main : Form
    {
        static List<CalculationPreference> calculations = new List<CalculationPreference> { };
        static string preferenceFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\" + "LimitCalculatorSetting.json";

        static List<string> possibleOperations = new List<string> { "None",
                                                                    "Tag minimun",
                                                                    "Minimum",
                                                                    "μ　- 3σ",
                                                                    "μ　- 2σ",
                                                                    "μ　- σ",
                                                                    "μ",
                                                                    "μ　+ σ",
                                                                    "μ　+ 2σ",
                                                                    "μ　+ 3σ",
                                                                    "Maximum",
                                                                    "Tag maximum"};
        private BindingSource bindingSource = new BindingSource();
        AFDatabase db = null;

        public Main()
        {
            InitializeComponent();
            afDatabasePicker.SystemPicker = piSystemPicker;
            if (File.Exists(preferenceFilePath))
            {
                string preferenceText = File.ReadAllText(preferenceFilePath);
            }

            ICollection<AFAttributeTrait>  limits = AFAttributeTrait.AllLimits;

            int horizontal = 10;
            int vertical = 25;

            foreach (AFAttributeTrait limit in limits)
            {
                Label limitLabel = new Label();
                limitLabel.Text = limit.Abbreviation;
                limitLabel.Location = new System.Drawing.Point(horizontal, vertical);
                panel1.Controls.Add(limitLabel);

                ComboBox limitCombo = new ComboBox();
                limitCombo.Name = limit.Name;
                limitCombo.Location = new System.Drawing.Point(horizontal + 100, vertical);
                limitCombo.DataSource = possibleOperations.ToArray();
                
                panel1.Controls.Add(limitCombo);
                vertical += limitLabel.Height - 1;
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            queryTextBox.Text = @"Start:>*-10h End:<*-1h Inprogress:=False";
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
            CalculationPreference preference = new CalculationPreference(path, query);
            foreach (AFAttributeTrait limit in AFAttributeTrait.AllLimits)
            {
                ComboBox comboBox = (ComboBox)panel1.Controls[limit.Name];
                if (comboBox.Text != "None")
                {
                    preference.calculationsToPerform[limit] = comboBox.Text;
                }
            }
            calculations.Add(preference);
            save();
        }

        private void save()
        {
            string output = JsonConvert.SerializeObject(calculations, Formatting.Indented);
            File.WriteAllText(preferenceFilePath, output);
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
                criteria.InProgress = token.Value == "true" ? true : false;
            query.Tokens.Remove(token);

            criteria.LastFullSearchString = query.ToString();
            search.EventFrameCriteria = criteria;

            h.Show();
        }
    }
}
