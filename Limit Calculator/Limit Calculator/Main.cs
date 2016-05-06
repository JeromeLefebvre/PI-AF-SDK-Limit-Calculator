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
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = calculations;
            //afAttributeTraitsPage1.A
            //bindingSource.DataSource = possibleOperations;

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
            //comboBox1.ValueMember = "Name";
        }

        private void Main_Load(object sender, EventArgs e)
        {
            queryTextBox.Text = @"Start:>*-10h Inprogress:=False Template:=""Batch""";
        }

        private void findEventFrames(object sender, EventArgs e)
        {
            eventframeGridView.Rows.Clear();
            db = afDatabasePicker.AFDatabase;
            //string queryString = @"Start:>*-10m Inprogress:=True Template:=""Batch"" ""|Golden"":=True  ";
            //string queryString = @"Start:>*-10h Inprogress:=False Template:=""Batch""";
            try
            {
                /*
                AFEventFrameCriteria criteria = eventFrameSearchPage1.EventFrameCriteria;
                OSIsoft.AF.Search.AFEventFrameSearch query = new OSIsoft.AF.Search.AFEventFrameSearch(db, "search", eventFrameSearchPage1.EventFrameCriteria.LastFullSearchString);
                OSIsoft.AF.Search.AFSearchToken startTime = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.Start, OSIsoft.AF.Search.AFSearchOperator.GreaterThanOrEqual, criteria.StartTime);
                OSIsoft.AF.Search.AFSearchToken endTime = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.End, OSIsoft.AF.Search.AFSearchOperator.LessThanOrEqual, criteria.EndTime);
                OSIsoft.AF.Search.AFSearchToken inProgess = new OSIsoft.AF.Search.AFSearchToken(OSIsoft.AF.Search.AFSearchFilter.InProgress, OSIsoft.AF.Search.AFSearchOperator.Equal, criteria.InProgress.ToString());

                query.Tokens.Add(startTime);
                query.Tokens.Add(endTime);
                query.Tokens.Add(inProgess);

                queryTextBox.Text = eventFrameSearchPage1.EventFrameCriteria.LastFullSearchString;
                IEnumerable<AFEventFrame> found = query.FindEventFrames();
                foreach (AFEventFrame f in found)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(eventframeGridView);
                    row.Cells[0].Value = f.Name;
                    row.Cells[1].Value = f.Template.Name;
                    row.Cells[2].Value = f.TimeRange.StartTime.LocalTime;
                    row.Cells[3].Value = f.TimeRange.EndTime.LocalTime;
                    eventframeGridView.Rows.Add(row);
                }
                */
            }
            catch (System.FormatException exception)
            {
                MessageBox.Show(exception.Message);
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

        private void add_Click(object sender, EventArgs e)
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
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = calculations;
            save();
        }

        private void save()
        {

            string output = JsonConvert.SerializeObject(calculations, Formatting.Indented);
            string homedirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            File.WriteAllText(homedirectory + @"\\" + "LimitCalculatorSetting.json", output);
        }


        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
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
