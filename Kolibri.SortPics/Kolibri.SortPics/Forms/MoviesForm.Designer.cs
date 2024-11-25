
namespace SortPics.Forms
{
    partial class MoviesForm
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
            this.toolStripStatusLabelFilnavn = new System.Windows.Forms.Label();
            this.groupBoxShow = new System.Windows.Forms.GroupBox();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.checkBoxHTML = new System.Windows.Forms.CheckBox();
            this.checkBoxDynamicHTML = new System.Windows.Forms.CheckBox();
            this.buttonVis = new System.Windows.Forms.Button();
            this.groupBoxSearch = new System.Windows.Forms.GroupBox();
            this.buttonExcel = new System.Windows.Forms.Button();
            this.buttonSubtitleSearch = new System.Windows.Forms.Button();
            this.checkBoxExcel = new System.Windows.Forms.CheckBox();
            this.checkBoxFileName = new System.Windows.Forms.CheckBox();
            this.checkBoxDB = new System.Windows.Forms.CheckBox();
            this.buttonFind = new System.Windows.Forms.Button();
            this.panelDetails = new System.Windows.Forms.Panel();
            this.groupBoxMovieLinks = new System.Windows.Forms.GroupBox();
            this.groupBoxDBPath = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.linkLabelLiteDB = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBoxShow.SuspendLayout();
            this.groupBoxSearch.SuspendLayout();
            this.panelDetails.SuspendLayout();
            this.groupBoxDBPath.SuspendLayout();
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
            this.splitContainer1.Size = new System.Drawing.Size(1168, 693);
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
            this.splitContainer2.Panel1.Controls.Add(this.toolStripStatusLabelFilnavn);
            this.splitContainer2.Panel1.Controls.Add(this.groupBoxShow);
            this.splitContainer2.Panel1.Controls.Add(this.groupBoxSearch);
            this.splitContainer2.Panel1.Controls.Add(this.panelDetails);
            this.splitContainer2.Size = new System.Drawing.Size(1168, 619);
            this.splitContainer2.SplitterDistance = 361;
            this.splitContainer2.TabIndex = 1;
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
            this.groupBoxShow.Controls.Add(this.checkBoxDynamicHTML);
            this.groupBoxShow.Controls.Add(this.buttonVis);
            this.groupBoxShow.Location = new System.Drawing.Point(8, 159);
            this.groupBoxShow.Name = "groupBoxShow";
            this.groupBoxShow.Size = new System.Drawing.Size(148, 105);
            this.groupBoxShow.TabIndex = 2;
            this.groupBoxShow.TabStop = false;
            this.groupBoxShow.Text = "Vis data i min ODBC";
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(7, 73);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(128, 23);
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
            this.checkBoxHTML.Location = new System.Drawing.Point(82, 50);
            this.checkBoxHTML.Name = "checkBoxHTML";
            this.checkBoxHTML.Size = new System.Drawing.Size(53, 17);
            this.checkBoxHTML.TabIndex = 4;
            this.checkBoxHTML.Text = "Static";
            this.checkBoxHTML.UseVisualStyleBackColor = true;
            // 
            // checkBoxDynamicHTML
            // 
            this.checkBoxDynamicHTML.AutoSize = true;
            this.checkBoxDynamicHTML.Checked = true;
            this.checkBoxDynamicHTML.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDynamicHTML.Location = new System.Drawing.Point(7, 50);
            this.checkBoxDynamicHTML.Name = "checkBoxDynamicHTML";
            this.checkBoxDynamicHTML.Size = new System.Drawing.Size(76, 17);
            this.checkBoxDynamicHTML.TabIndex = 3;
            this.checkBoxDynamicHTML.Text = "Interactive";
            this.checkBoxDynamicHTML.UseVisualStyleBackColor = true;
            // 
            // buttonVis
            // 
            this.buttonVis.Location = new System.Drawing.Point(7, 20);
            this.buttonVis.Name = "buttonVis";
            this.buttonVis.Size = new System.Drawing.Size(128, 23);
            this.buttonVis.TabIndex = 0;
            this.buttonVis.Text = "Vis OMDB data";
            this.buttonVis.UseVisualStyleBackColor = true;
            this.buttonVis.Click += new System.EventHandler(this.buttonVis_Click);
            // 
            // groupBoxSearch
            // 
            this.groupBoxSearch.Controls.Add(this.buttonExcel);
            this.groupBoxSearch.Controls.Add(this.buttonSubtitleSearch);
            this.groupBoxSearch.Controls.Add(this.checkBoxExcel);
            this.groupBoxSearch.Controls.Add(this.checkBoxFileName);
            this.groupBoxSearch.Controls.Add(this.checkBoxDB);
            this.groupBoxSearch.Controls.Add(this.buttonFind);
            this.groupBoxSearch.Location = new System.Drawing.Point(8, 5);
            this.groupBoxSearch.Name = "groupBoxSearch";
            this.groupBoxSearch.Size = new System.Drawing.Size(148, 150);
            this.groupBoxSearch.TabIndex = 1;
            this.groupBoxSearch.TabStop = false;
            this.groupBoxSearch.Text = "Søk etter filer i OMDB";
            // 
            // buttonExcel
            // 
            this.buttonExcel.Font = new System.Drawing.Font("Microsoft Sans Serif", 5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExcel.Location = new System.Drawing.Point(96, 92);
            this.buttonExcel.Name = "buttonExcel";
            this.buttonExcel.Size = new System.Drawing.Size(34, 18);
            this.buttonExcel.TabIndex = 5;
            this.buttonExcel.Text = "Excel";
            this.buttonExcel.UseVisualStyleBackColor = true;
            this.buttonExcel.Click += new System.EventHandler(this.buttonExcel_Click);
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
            this.checkBoxDB.CheckedChanged += new System.EventHandler(this.checkBoxDB_CheckedChanged);
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
            // panelDetails
            // 
            this.panelDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDetails.Controls.Add(this.groupBoxMovieLinks);
            this.panelDetails.Controls.Add(this.groupBoxDBPath);
            this.panelDetails.Location = new System.Drawing.Point(8, 3);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Size = new System.Drawing.Size(342, 587);
            this.panelDetails.TabIndex = 4;
            // 
            // groupBoxMovieLinks
            // 
            this.groupBoxMovieLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMovieLinks.Location = new System.Drawing.Point(8, 351);
            this.groupBoxMovieLinks.Name = "groupBoxMovieLinks";
            this.groupBoxMovieLinks.Size = new System.Drawing.Size(298, 216);
            this.groupBoxMovieLinks.TabIndex = 5;
            this.groupBoxMovieLinks.TabStop = false;
            this.groupBoxMovieLinks.Text = "MovieLinks";
            // 
            // groupBoxDBPath
            // 
            this.groupBoxDBPath.Controls.Add(this.button1);
            this.groupBoxDBPath.Controls.Add(this.linkLabelLiteDB);
            this.groupBoxDBPath.Location = new System.Drawing.Point(8, 268);
            this.groupBoxDBPath.Name = "groupBoxDBPath";
            this.groupBoxDBPath.Size = new System.Drawing.Size(148, 77);
            this.groupBoxDBPath.TabIndex = 0;
            this.groupBoxDBPath.TabStop = false;
            this.groupBoxDBPath.Text = "LiteDB Path";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(7, 37);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Choose Lite DB";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // linkLabelLiteDB
            // 
            this.linkLabelLiteDB.AutoSize = true;
            this.linkLabelLiteDB.Location = new System.Drawing.Point(7, 20);
            this.linkLabelLiteDB.Name = "linkLabelLiteDB";
            this.linkLabelLiteDB.Size = new System.Drawing.Size(81, 13);
            this.linkLabelLiteDB.TabIndex = 0;
            this.linkLabelLiteDB.TabStop = true;
            this.linkLabelLiteDB.Text = "linkLabelLiteDB";
            this.linkLabelLiteDB.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            // 
            // MoviesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1168, 693);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MoviesForm";
            this.Text = "MoviesOMDBForm";
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
            this.panelDetails.ResumeLayout(false);
            this.groupBoxDBPath.ResumeLayout(false);
            this.groupBoxDBPath.PerformLayout();
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
        private System.Windows.Forms.CheckBox checkBoxDynamicHTML;
        private System.Windows.Forms.Button buttonSubtitleSearch;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.GroupBox groupBoxDBPath;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.LinkLabel linkLabelLiteDB;
        private System.Windows.Forms.Button buttonExcel;
        private System.Windows.Forms.GroupBox groupBoxMovieLinks;
    }
}