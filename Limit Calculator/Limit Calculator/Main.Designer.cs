namespace Limit_Calculator
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.piSystemPicker = new OSIsoft.AF.UI.PISystemPicker();
            this.afDatabasePicker = new OSIsoft.AF.UI.AFDatabasePicker();
            this.queryTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.afTreeView = new OSIsoft.AF.UI.AFTreeView();
            this.addToPreference = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.eventLog1 = new System.Diagnostics.EventLog();
            this.displaySearch = new System.Windows.Forms.Button();
            this.calculationPreferenceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.configurationTreeView = new OSIsoft.AF.UI.AFTreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.calculationName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.offsetSetting = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.calculationPreferenceBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // piSystemPicker
            // 
            this.piSystemPicker.AccessibleDescription = "PI System Picker";
            this.piSystemPicker.AccessibleName = "PI System Picker";
            this.piSystemPicker.ConnectAutomatically = true;
            this.piSystemPicker.Cursor = System.Windows.Forms.Cursors.Default;
            this.piSystemPicker.Location = new System.Drawing.Point(12, 12);
            this.piSystemPicker.LoginPromptSetting = OSIsoft.AF.UI.PISystemPicker.LoginPromptSettingOptions.Default;
            this.piSystemPicker.Name = "piSystemPicker";
            this.piSystemPicker.ShowBegin = false;
            this.piSystemPicker.ShowDelete = false;
            this.piSystemPicker.ShowEnd = false;
            this.piSystemPicker.ShowFind = false;
            this.piSystemPicker.ShowList = false;
            this.piSystemPicker.ShowNavigation = false;
            this.piSystemPicker.ShowNew = false;
            this.piSystemPicker.ShowNext = false;
            this.piSystemPicker.ShowPrevious = false;
            this.piSystemPicker.ShowProperties = false;
            this.piSystemPicker.Size = new System.Drawing.Size(370, 22);
            this.piSystemPicker.TabIndex = 0;
            this.piSystemPicker.ConnectionChange += new OSIsoft.AF.UI.ConnectionChangeEventHandler(this.piSystemPicker_ConnectionChange);
            this.piSystemPicker.SelectionChange += new OSIsoft.AF.UI.SelectionChangeEventHandler(this.piSystemPicker_SelectionChange);
            // 
            // afDatabasePicker
            // 
            this.afDatabasePicker.AccessibleDescription = "Database Picker";
            this.afDatabasePicker.AccessibleName = "Database Picker";
            this.afDatabasePicker.Location = new System.Drawing.Point(12, 40);
            this.afDatabasePicker.Name = "afDatabasePicker";
            this.afDatabasePicker.ShowBegin = false;
            this.afDatabasePicker.ShowConfigurationDatabase = OSIsoft.AF.UI.ShowConfigurationDatabase.Hide;
            this.afDatabasePicker.ShowDelete = false;
            this.afDatabasePicker.ShowEnd = false;
            this.afDatabasePicker.ShowFind = false;
            this.afDatabasePicker.ShowList = false;
            this.afDatabasePicker.ShowNavigation = false;
            this.afDatabasePicker.ShowNew = false;
            this.afDatabasePicker.ShowNext = false;
            this.afDatabasePicker.ShowPrevious = false;
            this.afDatabasePicker.ShowProperties = false;
            this.afDatabasePicker.Size = new System.Drawing.Size(370, 22);
            this.afDatabasePicker.TabIndex = 1;
            this.afDatabasePicker.SelectionChange += new OSIsoft.AF.UI.SelectionChangeEventHandler(this.afDatabasePicker_SelectionChange);
            // 
            // queryTextBox
            // 
            this.queryTextBox.Enabled = false;
            this.queryTextBox.Location = new System.Drawing.Point(404, 60);
            this.queryTextBox.Multiline = true;
            this.queryTextBox.Name = "queryTextBox";
            this.queryTextBox.Size = new System.Drawing.Size(379, 50);
            this.queryTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(402, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Event Frame Query:";
            // 
            // afTreeView
            // 
            this.afTreeView.HideSelection = false;
            this.afTreeView.Location = new System.Drawing.Point(10, 72);
            this.afTreeView.Name = "afTreeView";
            this.afTreeView.ShowAttributes = true;
            this.afTreeView.ShowContacts = false;
            this.afTreeView.ShowDatabases = false;
            this.afTreeView.ShowElementTemplates = false;
            this.afTreeView.ShowEnumerations = false;
            this.afTreeView.ShowNodeToolTips = true;
            this.afTreeView.ShowPlugIns = false;
            this.afTreeView.ShowUOMs = true;
            this.afTreeView.Size = new System.Drawing.Size(372, 267);
            this.afTreeView.TabIndex = 6;
            // 
            // addToPreference
            // 
            this.addToPreference.Location = new System.Drawing.Point(402, 406);
            this.addToPreference.Name = "addToPreference";
            this.addToPreference.Size = new System.Drawing.Size(104, 23);
            this.addToPreference.TabIndex = 7;
            this.addToPreference.Text = "Save Calculation";
            this.addToPreference.UseVisualStyleBackColor = true;
            this.addToPreference.Click += new System.EventHandler(this.addToPreference_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(402, 169);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(282, 187);
            this.panel1.TabIndex = 25;
            // 
            // eventLog1
            // 
            this.eventLog1.SynchronizingObject = this;
            // 
            // displaySearch
            // 
            this.displaySearch.Location = new System.Drawing.Point(402, 116);
            this.displaySearch.Name = "displaySearch";
            this.displaySearch.Size = new System.Drawing.Size(121, 23);
            this.displaySearch.TabIndex = 27;
            this.displaySearch.Text = "Event Frame Search";
            this.displaySearch.UseVisualStyleBackColor = true;
            this.displaySearch.Click += new System.EventHandler(this.displaySearch_Click);
            // 
            // configurationTreeView
            // 
            this.configurationTreeView.HideSelection = false;
            this.configurationTreeView.Location = new System.Drawing.Point(10, 345);
            this.configurationTreeView.Name = "configurationTreeView";
            this.configurationTreeView.ShowNodeToolTips = true;
            this.configurationTreeView.Size = new System.Drawing.Size(372, 151);
            this.configurationTreeView.TabIndex = 28;
            this.configurationTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.configurationTreeView_AfterSelect);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(400, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 12);
            this.label2.TabIndex = 29;
            this.label2.Text = "Name:";
            // 
            // calculationName
            // 
            this.calculationName.Location = new System.Drawing.Point(442, 12);
            this.calculationName.Name = "calculationName";
            this.calculationName.Size = new System.Drawing.Size(338, 19);
            this.calculationName.TabIndex = 30;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(400, 154);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(327, 12);
            this.label3.TabIndex = 31;
            this.label3.Text = "Select the calculations that need to be perform for each slices";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(402, 373);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 12);
            this.label4.TabIndex = 32;
            this.label4.Text = "Offset:";
            // 
            // offsetSetting
            // 
            this.offsetSetting.Location = new System.Drawing.Point(447, 370);
            this.offsetSetting.Name = "offsetSetting";
            this.offsetSetting.Size = new System.Drawing.Size(100, 19);
            this.offsetSetting.TabIndex = 33;
            this.offsetSetting.Text = "0";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 542);
            this.Controls.Add(this.offsetSetting);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.calculationName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.configurationTreeView);
            this.Controls.Add(this.displaySearch);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.addToPreference);
            this.Controls.Add(this.afTreeView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.queryTextBox);
            this.Controls.Add(this.afDatabasePicker);
            this.Controls.Add(this.piSystemPicker);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "Main";
            this.Text = "Limit Calculator";
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.calculationPreferenceBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OSIsoft.AF.UI.PISystemPicker piSystemPicker;
        private OSIsoft.AF.UI.AFDatabasePicker afDatabasePicker;
        private System.Windows.Forms.Label label1;
        private OSIsoft.AF.UI.AFTreeView afTreeView;
        private System.Windows.Forms.Button addToPreference;
        private System.Windows.Forms.BindingSource calculationPreferenceBindingSource;
        private System.Windows.Forms.Panel panel1;
        private System.Diagnostics.EventLog eventLog1;
        private System.Windows.Forms.Button displaySearch;
        private OSIsoft.AF.UI.AFTreeView configurationTreeView;
        private System.Windows.Forms.TextBox calculationName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox offsetSetting;
        private System.Windows.Forms.TextBox queryTextBox;
    }
}

