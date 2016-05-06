using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OSIsoft.AF;
using OSIsoft.AF.EventFrame;
using OSIsoft.AF.UI;
using Newtonsoft.Json;
using OSIsoft.AF.Asset;
using OSIsoft.AF.UI.Search;

namespace Limit_Calculator
{
    public partial class Main : Form
    {
        static List<CalculationPreference> calculations = new List<CalculationPreference> { };
        static string path = @"C:\Users\jlefebvre\Desktop\setting.json";

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
            if (File.Exists(path))
            {
                string preferenceText = File.ReadAllText(path);
                // Need to add in a way to properly deserlialize the object or at least, manually append it.
                //calculations = JsonConvert.DeserializeObject<List<CalculationPreference>>(preferenceText);
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
            queryTextBox.Text = @"Start:>*-10h Inprogress:=False Template:=""Batch""";
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
            string homedirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            File.WriteAllText(homedirectory + @"\\" + "LimitCalculatorSetting.json", output);
        }

        private void displaySearch_Click(object sender, EventArgs e)
        {
            EventFrameSearch h = new EventFrameSearch(this, db);
            h.Show();
        }
    }
    class CalculationPreference
    {
        public string sensorPath { get; set; }
        public string eventFrameQuery { get; set; }
        public Dictionary<AFAttributeTrait, string> calculationsToPerform = new Dictionary<AFAttributeTrait, string> { };
        public CalculationPreference(string path, string query)
        {
            sensorPath = path;
            eventFrameQuery = query;
        }
    }
}
