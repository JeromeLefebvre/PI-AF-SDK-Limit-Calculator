namespace Limit_Calculator
{
    partial class EventFrameSearch
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
            this.eventFrameSearchPage = new OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage();
            this.SuspendLayout();
            // 
            // eventFrameSearchPage
            // 
            this.eventFrameSearchPage.AccessibleDescription = "Event Frame Query Search Property Page";
            this.eventFrameSearchPage.AccessibleName = "Event Frame Search Page";
            this.eventFrameSearchPage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eventFrameSearchPage.AutoSize = true;
            this.eventFrameSearchPage.HelpContext = ((long)(0));
            this.eventFrameSearchPage.Location = new System.Drawing.Point(1, 0);
            this.eventFrameSearchPage.Name = "eventFrameSearchPage";
            this.eventFrameSearchPage.Size = new System.Drawing.Size(1163, 721);
            this.eventFrameSearchPage.TabIndex = 0;
            this.eventFrameSearchPage.SearchCompleted += new OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage.SearchCompletedHandler(this.eventFrameSearchPage_SearchCompleted);
            this.eventFrameSearchPage.Load += new System.EventHandler(this.eventFrameSearchPage_Load);
            // 
            // EventFrameSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1186, 733);
            this.Controls.Add(this.eventFrameSearchPage);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "EventFrameSearch";
            this.Text = "EventFrameSearch";
            this.Load += new System.EventHandler(this.EventFrameSearch_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OSIsoft.AF.UI.PropertyPage.EventFrameSearchPage eventFrameSearchPage;
    }
}