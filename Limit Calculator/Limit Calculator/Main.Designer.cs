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
            this.add = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.calculationPreferenceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.eventLog1 = new System.Diagnostics.EventLog();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.calculationPreferenceBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).BeginInit();
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
            this.queryTextBox.Location = new System.Drawing.Point(403, 32);
            this.queryTextBox.Multiline = true;
            this.queryTextBox.Name = "queryTextBox";
            this.queryTextBox.Size = new System.Drawing.Size(420, 50);
            this.queryTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(401, 12);
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
            this.afTreeView.ShowNodeToolTips = true;
            this.afTreeView.Size = new System.Drawing.Size(372, 289);
            this.afTreeView.TabIndex = 6;
            // 
            // add
            // 
            this.add.Location = new System.Drawing.Point(403, 337);
            this.add.Name = "add";
            this.add.Size = new System.Drawing.Size(75, 23);
            this.add.TabIndex = 7;
            this.add.Text = "Add Calculation";
            this.add.UseVisualStyleBackColor = true;
            this.add.Click += new System.EventHandler(this.add_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(403, 145);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(282, 175);
            this.panel1.TabIndex = 25;
            // 
            // calculationPreferenceBindingSource
            // 
            this.calculationPreferenceBindingSource.DataSource = typeof(Limit_Calculator.CalculationPreference);
            // 
            // eventLog1
            // 
            this.eventLog1.SynchronizingObject = this;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(403, 88);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 27;
            this.button1.Text = "Search";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 392);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.add);
            this.Controls.Add(this.afTreeView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.queryTextBox);
            this.Controls.Add(this.afDatabasePicker);
            this.Controls.Add(this.piSystemPicker);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "Main";
            this.Text = "Limit Calculator";
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.calculationPreferenceBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OSIsoft.AF.UI.PISystemPicker piSystemPicker;
        private OSIsoft.AF.UI.AFDatabasePicker afDatabasePicker;
        private System.Windows.Forms.TextBox queryTextBox;
        private System.Windows.Forms.Label label1;
        private OSIsoft.AF.UI.AFTreeView afTreeView;
        private System.Windows.Forms.Button add;
        private System.Windows.Forms.BindingSource calculationPreferenceBindingSource;
        private System.Windows.Forms.Panel panel1;
        private System.Diagnostics.EventLog eventLog1;
        private System.Windows.Forms.Button button1;
    }
}

