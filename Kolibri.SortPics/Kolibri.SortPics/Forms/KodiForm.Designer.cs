namespace SortPics.Forms
{
    partial class KodiForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panelDetails = new System.Windows.Forms.Panel();
            this.checkBoxSeries = new System.Windows.Forms.CheckBox();
            this.checkBoxMovies = new System.Windows.Forms.CheckBox();
            this.buttonMapKodi2IMDB = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripStatusLabelFilnavn = new System.Windows.Forms.ToolStripLabel();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panelDetails.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 107;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panelDetails);
            this.splitContainer2.Size = new System.Drawing.Size(800, 314);
            this.splitContainer2.SplitterDistance = 266;
            this.splitContainer2.TabIndex = 1;
            // 
            // panelDetails
            // 
            this.panelDetails.Controls.Add(this.button1);
            this.panelDetails.Controls.Add(this.checkBoxSeries);
            this.panelDetails.Controls.Add(this.checkBoxMovies);
            this.panelDetails.Controls.Add(this.buttonMapKodi2IMDB);
            this.panelDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDetails.Location = new System.Drawing.Point(0, 0);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Size = new System.Drawing.Size(266, 314);
            this.panelDetails.TabIndex = 0;
            // 
            // checkBoxSeries
            // 
            this.checkBoxSeries.AutoSize = true;
            this.checkBoxSeries.Checked = true;
            this.checkBoxSeries.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSeries.Location = new System.Drawing.Point(13, 38);
            this.checkBoxSeries.Name = "checkBoxSeries";
            this.checkBoxSeries.Size = new System.Drawing.Size(55, 17);
            this.checkBoxSeries.TabIndex = 2;
            this.checkBoxSeries.Text = "Series";
            this.checkBoxSeries.UseVisualStyleBackColor = true;
            // 
            // checkBoxMovies
            // 
            this.checkBoxMovies.AutoSize = true;
            this.checkBoxMovies.Checked = true;
            this.checkBoxMovies.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMovies.Location = new System.Drawing.Point(13, 14);
            this.checkBoxMovies.Name = "checkBoxMovies";
            this.checkBoxMovies.Size = new System.Drawing.Size(60, 17);
            this.checkBoxMovies.TabIndex = 1;
            this.checkBoxMovies.Text = "Movies";
            this.checkBoxMovies.UseVisualStyleBackColor = true;
            // 
            // buttonMapKodi2IMDB
            // 
            this.buttonMapKodi2IMDB.Location = new System.Drawing.Point(12, 66);
            this.buttonMapKodi2IMDB.Name = "buttonMapKodi2IMDB";
            this.buttonMapKodi2IMDB.Size = new System.Drawing.Size(188, 23);
            this.buttonMapKodi2IMDB.TabIndex = 0;
            this.buttonMapKodi2IMDB.Text = "Map Kodi 2 IMDB";
            this.buttonMapKodi2IMDB.UseVisualStyleBackColor = true;
            this.buttonMapKodi2IMDB.Click += new System.EventHandler(this.buttonMapKodi2IMDB_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelFilnavn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 314);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripStatusLabelFilnavn
            // 
            this.toolStripStatusLabelFilnavn.Name = "toolStripStatusLabelFilnavn";
            this.toolStripStatusLabelFilnavn.Size = new System.Drawing.Size(150, 22);
            this.toolStripStatusLabelFilnavn.Text = "toolStripStatusLabelFilnavn";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(61, 130);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // KodiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "KodiForm";
            this.Text = "KodiForm";
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripStatusLabelFilnavn;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.Button buttonMapKodi2IMDB;
        private System.Windows.Forms.CheckBox checkBoxSeries;
        private System.Windows.Forms.CheckBox checkBoxMovies;
        private System.Windows.Forms.Button button1;
    }
}