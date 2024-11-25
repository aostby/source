
namespace SortPics.Forms
{
    partial class OMDBForm_org
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
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panelDetails = new System.Windows.Forms.Panel();
            this.toolStripStatusLabelFilnavn = new System.Windows.Forms.Label();
            this.groupBoxShow = new System.Windows.Forms.GroupBox();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.checkBoxHTML = new System.Windows.Forms.CheckBox();
            this.checkBoxEntireDB = new System.Windows.Forms.CheckBox();
            this.buttonVis = new System.Windows.Forms.Button();
            this.groupBoxSearch = new System.Windows.Forms.GroupBox();
            this.buttonSubtitleSearch = new System.Windows.Forms.Button();
            this.checkBoxExcel = new System.Windows.Forms.CheckBox();
            this.checkBoxFileName = new System.Windows.Forms.CheckBox();
            this.checkBoxDB = new System.Windows.Forms.CheckBox();
            this.buttonFind = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBoxShow.SuspendLayout();
            this.groupBoxSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitter2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(800, 693);
            this.splitContainer1.SplitterDistance = 70;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(0, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 70);
            this.splitter2.TabIndex = 1;
            this.splitter2.TabStop = false;
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
            this.splitContainer2.Panel1.Controls.Add(this.toolStripStatusLabelFilnavn);
            this.splitContainer2.Panel1.Controls.Add(this.groupBoxShow);
            this.splitContainer2.Panel1.Controls.Add(this.groupBoxSearch);
            this.splitContainer2.Size = new System.Drawing.Size(800, 619);
            this.splitContainer2.SplitterDistance = 217;
            this.splitContainer2.TabIndex = 1;
            // 
            // panelDetails
            // 
            this.panelDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDetails.Location = new System.Drawing.Point(13, 270);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Size = new System.Drawing.Size(198, 320);
            this.panelDetails.TabIndex = 4;
            // 
            // toolStripStatusLabelFilnavn
            // 
            this.toolStripStatusLabelFilnavn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.toolStripStatusLabelFilnavn.AutoSize = true;
            this.toolStripStatusLabelFilnavn.Location = new System.Drawing.Point(9, 598);
            this.toolStripStatusLabelFilnavn.Name = "toolStripStatusLabelFilnavn";
            this.toolStripStatusLabelFilnavn.Size = new System.Drawing.Size(35, 13);
            this.toolStripStatusLabelFilnavn.TabIndex = 3;
            this.toolStripStatusLabelFilnavn.Text = "label1";
            // 
            // groupBoxShow
            // 
            this.groupBoxShow.Controls.Add(this.buttonRemove);
            this.groupBoxShow.Controls.Add(this.checkBoxHTML);
            this.groupBoxShow.Controls.Add(this.checkBoxEntireDB);
            this.groupBoxShow.Controls.Add(this.buttonVis);
            this.groupBoxShow.Location = new System.Drawing.Point(3, 159);
            this.groupBoxShow.Name = "groupBoxShow";
            this.groupBoxShow.Size = new System.Drawing.Size(192, 105);
            this.groupBoxShow.TabIndex = 2;
            this.groupBoxShow.TabStop = false;
            this.groupBoxShow.Text = "Vis data i min ODBC";
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(7, 73);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(179, 23);
            this.buttonRemove.TabIndex = 4;
            this.buttonRemove.Text = "Fjern ikke-eksisternde";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // checkBoxHTML
            // 
            this.checkBoxHTML.AutoSize = true;
            this.checkBoxHTML.Checked = true;
            this.checkBoxHTML.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHTML.Location = new System.Drawing.Point(113, 50);
            this.checkBoxHTML.Name = "checkBoxHTML";
            this.checkBoxHTML.Size = new System.Drawing.Size(56, 17);
            this.checkBoxHTML.TabIndex = 4;
            this.checkBoxHTML.Text = "HTML";
            this.checkBoxHTML.UseVisualStyleBackColor = true;
            // 
            // checkBoxEntireDB
            // 
            this.checkBoxEntireDB.AutoSize = true;
            this.checkBoxEntireDB.Location = new System.Drawing.Point(14, 50);
            this.checkBoxEntireDB.Name = "checkBoxEntireDB";
            this.checkBoxEntireDB.Size = new System.Drawing.Size(101, 17);
            this.checkBoxEntireDB.TabIndex = 3;
            this.checkBoxEntireDB.Text = "Hele databasen";
            this.checkBoxEntireDB.UseVisualStyleBackColor = true;
            // 
            // buttonVis
            // 
            this.buttonVis.Location = new System.Drawing.Point(7, 20);
            this.buttonVis.Name = "buttonVis";
            this.buttonVis.Size = new System.Drawing.Size(179, 23);
            this.buttonVis.TabIndex = 0;
            this.buttonVis.Text = "Vis OMDB data";
            this.buttonVis.UseVisualStyleBackColor = true;
            this.buttonVis.Click += new System.EventHandler(this.buttonVis_Click);
            // 
            // groupBoxSearch
            // 
            this.groupBoxSearch.Controls.Add(this.buttonSubtitleSearch);
            this.groupBoxSearch.Controls.Add(this.checkBoxExcel);
            this.groupBoxSearch.Controls.Add(this.checkBoxFileName);
            this.groupBoxSearch.Controls.Add(this.checkBoxDB);
            this.groupBoxSearch.Controls.Add(this.buttonFind);
            this.groupBoxSearch.Location = new System.Drawing.Point(3, 3);
            this.groupBoxSearch.Name = "groupBoxSearch";
            this.groupBoxSearch.Size = new System.Drawing.Size(192, 150);
            this.groupBoxSearch.TabIndex = 1;
            this.groupBoxSearch.TabStop = false;
            this.groupBoxSearch.Text = "Søk etter filmer i OMDB databasen";
            // 
            // buttonSubtitleSearch
            // 
            this.buttonSubtitleSearch.Location = new System.Drawing.Point(7, 116);
            this.buttonSubtitleSearch.Name = "buttonSubtitleSearch";
            this.buttonSubtitleSearch.Size = new System.Drawing.Size(128, 23);
            this.buttonSubtitleSearch.TabIndex = 4;
            this.buttonSubtitleSearch.Text = "Subtitle Search";
            this.buttonSubtitleSearch.UseVisualStyleBackColor = true;
            this.buttonSubtitleSearch.Click += new System.EventHandler(this.buttonSubtitleSearch_Click);
            // 
            // checkBoxExcel
            // 
            this.checkBoxExcel.AutoSize = true;
            this.checkBoxExcel.Location = new System.Drawing.Point(7, 93);
            this.checkBoxExcel.Name = "checkBoxExcel";
            this.checkBoxExcel.Size = new System.Drawing.Size(83, 17);
            this.checkBoxExcel.TabIndex = 3;
            this.checkBoxExcel.Text = "Lag Excel fil";
            this.checkBoxExcel.UseVisualStyleBackColor = true;
            // 
            // checkBoxFileName
            // 
            this.checkBoxFileName.AutoSize = true;
            this.checkBoxFileName.Checked = true;
            this.checkBoxFileName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxFileName.Location = new System.Drawing.Point(7, 72);
            this.checkBoxFileName.Name = "checkBoxFileName";
            this.checkBoxFileName.Size = new System.Drawing.Size(138, 17);
            this.checkBoxFileName.TabIndex = 2;
            this.checkBoxFileName.Text = "Oppdater filsti i lokal DB";
            this.checkBoxFileName.UseVisualStyleBackColor = true;
            // 
            // checkBoxDB
            // 
            this.checkBoxDB.AutoSize = true;
            this.checkBoxDB.Checked = true;
            this.checkBoxDB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDB.Location = new System.Drawing.Point(7, 49);
            this.checkBoxDB.Name = "checkBoxDB";
            this.checkBoxDB.Size = new System.Drawing.Size(130, 17);
            this.checkBoxDB.TabIndex = 1;
            this.checkBoxDB.Text = "Lagre i lokal database";
            this.checkBoxDB.UseVisualStyleBackColor = true;
            // 
            // buttonFind
            // 
            this.buttonFind.Location = new System.Drawing.Point(6, 19);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(128, 23);
            this.buttonFind.TabIndex = 0;
            this.buttonFind.Text = "Find movies by filename";
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // OMDBForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 693);
            this.Controls.Add(this.splitContainer1);
            this.Name = "OMDBForm";
            this.Text = "OMDBForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OMDBForm_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBoxShow.ResumeLayout(false);
            this.groupBoxShow.PerformLayout();
            this.groupBoxSearch.ResumeLayout(false);
            this.groupBoxSearch.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBoxSearch;
        private System.Windows.Forms.CheckBox checkBoxFileName;
        private System.Windows.Forms.CheckBox checkBoxDB;
        private System.Windows.Forms.CheckBox checkBoxExcel;
        private System.Windows.Forms.GroupBox groupBoxShow;
        private System.Windows.Forms.Button buttonVis;
        private System.Windows.Forms.Label toolStripStatusLabelFilnavn;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.CheckBox checkBoxHTML;
        private System.Windows.Forms.CheckBox checkBoxEntireDB;
        private System.Windows.Forms.Button buttonSubtitleSearch;
        private System.Windows.Forms.Button buttonRemove;
    }
}