namespace Kolibri.Common.Beer.Forms
{
    partial class BeerXmlDisplayForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupControlBeerXMLCheck = new DevExpress.XtraEditors.GroupControl();
            this.buttonShowFile = new System.Windows.Forms.Button();
            this.buttonBeerXMLIssuesReport = new System.Windows.Forms.Button();
            this.groupBoxBeerXML = new System.Windows.Forms.GroupBox();
            this.radioButtonExternal = new System.Windows.Forms.RadioButton();
            this.radioButtonLocal = new System.Windows.Forms.RadioButton();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.groupBoxFile = new System.Windows.Forms.GroupBox();
            this.buttonOpenDirS = new System.Windows.Forms.Button();
            this.buttonSource = new System.Windows.Forms.Button();
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonIssuesReportsAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControlBeerXMLCheck)).BeginInit();
            this.groupControlBeerXMLCheck.SuspendLayout();
            this.groupBoxBeerXML.SuspendLayout();
            this.groupBoxFile.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupControlBeerXMLCheck);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxBeerXML);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxFile);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.webBrowser1);
            this.splitContainer1.Size = new System.Drawing.Size(940, 630);
            this.splitContainer1.SplitterDistance = 313;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupControlBeerXMLCheck
            // 
            this.groupControlBeerXMLCheck.Controls.Add(this.buttonIssuesReportsAll);
            this.groupControlBeerXMLCheck.Controls.Add(this.buttonShowFile);
            this.groupControlBeerXMLCheck.Controls.Add(this.buttonBeerXMLIssuesReport);
            this.groupControlBeerXMLCheck.Location = new System.Drawing.Point(4, 183);
            this.groupControlBeerXMLCheck.Name = "groupControlBeerXMLCheck";
            this.groupControlBeerXMLCheck.Size = new System.Drawing.Size(298, 414);
            this.groupControlBeerXMLCheck.TabIndex = 2;
            this.groupControlBeerXMLCheck.Text = "BeerXML Check (not xsd validation)";
            // 
            // buttonShowFile
            // 
            this.buttonShowFile.Location = new System.Drawing.Point(24, 41);
            this.buttonShowFile.Name = "buttonShowFile";
            this.buttonShowFile.Size = new System.Drawing.Size(75, 23);
            this.buttonShowFile.TabIndex = 1;
            this.buttonShowFile.Text = "Show file";
            this.buttonShowFile.UseVisualStyleBackColor = true;
            this.buttonShowFile.Click += new System.EventHandler(this.buttonShowFile_Click);
            // 
            // buttonBeerXMLIssuesReport
            // 
            this.buttonBeerXMLIssuesReport.Location = new System.Drawing.Point(199, 41);
            this.buttonBeerXMLIssuesReport.Name = "buttonBeerXMLIssuesReport";
            this.buttonBeerXMLIssuesReport.Size = new System.Drawing.Size(94, 23);
            this.buttonBeerXMLIssuesReport.TabIndex = 0;
            this.buttonBeerXMLIssuesReport.Text = "Issues report";
            this.buttonBeerXMLIssuesReport.UseVisualStyleBackColor = true;
            this.buttonBeerXMLIssuesReport.Click += new System.EventHandler(this.buttonBeerXMLIssuesReport_Click);
            // 
            // groupBoxBeerXML
            // 
            this.groupBoxBeerXML.Controls.Add(this.radioButtonExternal);
            this.groupBoxBeerXML.Controls.Add(this.radioButtonLocal);
            this.groupBoxBeerXML.Controls.Add(this.comboBox1);
            this.groupBoxBeerXML.Controls.Add(this.buttonRefresh);
            this.groupBoxBeerXML.Location = new System.Drawing.Point(4, 94);
            this.groupBoxBeerXML.Name = "groupBoxBeerXML";
            this.groupBoxBeerXML.Size = new System.Drawing.Size(306, 83);
            this.groupBoxBeerXML.TabIndex = 1;
            this.groupBoxBeerXML.TabStop = false;
            this.groupBoxBeerXML.Text = "BeerXML\'s";
            // 
            // radioButtonExternal
            // 
            this.radioButtonExternal.AutoSize = true;
            this.radioButtonExternal.Location = new System.Drawing.Point(112, 54);
            this.radioButtonExternal.Name = "radioButtonExternal";
            this.radioButtonExternal.Size = new System.Drawing.Size(103, 17);
            this.radioButtonExternal.TabIndex = 3;
            this.radioButtonExternal.Text = "External browser";
            this.radioButtonExternal.UseVisualStyleBackColor = true;
            // 
            // radioButtonLocal
            // 
            this.radioButtonLocal.AutoSize = true;
            this.radioButtonLocal.Checked = true;
            this.radioButtonLocal.Location = new System.Drawing.Point(9, 54);
            this.radioButtonLocal.Name = "radioButtonLocal";
            this.radioButtonLocal.Size = new System.Drawing.Size(91, 17);
            this.radioButtonLocal.TabIndex = 2;
            this.radioButtonLocal.TabStop = true;
            this.radioButtonLocal.Text = "Local browser";
            this.radioButtonLocal.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(9, 20);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(289, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(273, 54);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(25, 23);
            this.buttonRefresh.TabIndex = 0;
            this.buttonRefresh.Text = "refresh";
            this.toolTip1.SetToolTip(this.buttonRefresh, "Oppdater, les BeerXML filer på nytt");
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // groupBoxFile
            // 
            this.groupBoxFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxFile.Controls.Add(this.buttonOpenDirS);
            this.groupBoxFile.Controls.Add(this.buttonSource);
            this.groupBoxFile.Controls.Add(this.textBoxSource);
            this.groupBoxFile.Location = new System.Drawing.Point(4, 4);
            this.groupBoxFile.Name = "groupBoxFile";
            this.groupBoxFile.Size = new System.Drawing.Size(306, 83);
            this.groupBoxFile.TabIndex = 0;
            this.groupBoxFile.TabStop = false;
            this.groupBoxFile.Text = "Source folder";
            // 
            // buttonOpenDirS
            // 
            this.buttonOpenDirS.Location = new System.Drawing.Point(172, 47);
            this.buttonOpenDirS.Name = "buttonOpenDirS";
            this.buttonOpenDirS.Size = new System.Drawing.Size(25, 23);
            this.buttonOpenDirS.TabIndex = 20;
            this.buttonOpenDirS.Text = "Let opp";
            this.toolTip1.SetToolTip(this.buttonOpenDirS, "Åpne mappe med BeerXML");
            this.buttonOpenDirS.UseVisualStyleBackColor = true;
            this.buttonOpenDirS.Click += new System.EventHandler(this.buttonOpenDir_Click);
            // 
            // buttonSource
            // 
            this.buttonSource.Location = new System.Drawing.Point(199, 47);
            this.buttonSource.Name = "buttonSource";
            this.buttonSource.Size = new System.Drawing.Size(75, 23);
            this.buttonSource.TabIndex = 19;
            this.buttonSource.Text = "Let opp";
            this.toolTip1.SetToolTip(this.buttonSource, "Velg/Sett mappe med BeerXML filer i");
            this.buttonSource.UseVisualStyleBackColor = true;
            this.buttonSource.Click += new System.EventHandler(this.button_Click);
            // 
            // textBoxSource
            // 
            this.textBoxSource.Location = new System.Drawing.Point(8, 19);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.Size = new System.Drawing.Size(271, 20);
            this.textBoxSource.TabIndex = 18;
            this.textBoxSource.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(623, 630);
            this.webBrowser1.TabIndex = 0;
            // 
            // buttonIssuesReportsAll
            // 
            this.buttonIssuesReportsAll.Location = new System.Drawing.Point(24, 70);
            this.buttonIssuesReportsAll.Name = "buttonIssuesReportsAll";
            this.buttonIssuesReportsAll.Size = new System.Drawing.Size(269, 27);
            this.buttonIssuesReportsAll.TabIndex = 2;
            this.buttonIssuesReportsAll.Text = "Issues reports for all files";
            this.buttonIssuesReportsAll.UseVisualStyleBackColor = true;
            this.buttonIssuesReportsAll.Click += new System.EventHandler(this.buttonIssuesReportsAll_Click);
            // 
            // BeerXmlDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(940, 630);
            this.Controls.Add(this.splitContainer1);
            this.Name = "BeerXmlDisplayForm";
            this.Text = "BeerXmlDisplayForm";
            this.Shown += new System.EventHandler(this.BeerXmlDisplayForm_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupControlBeerXMLCheck)).EndInit();
            this.groupControlBeerXMLCheck.ResumeLayout(false);
            this.groupBoxBeerXML.ResumeLayout(false);
            this.groupBoxBeerXML.PerformLayout();
            this.groupBoxFile.ResumeLayout(false);
            this.groupBoxFile.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBoxFile;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Button buttonOpenDirS;
        private System.Windows.Forms.Button buttonSource;
        private System.Windows.Forms.TextBox textBoxSource;
        private System.Windows.Forms.GroupBox groupBoxBeerXML;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.RadioButton radioButtonExternal;
        private System.Windows.Forms.RadioButton radioButtonLocal;
        private System.Windows.Forms.ToolTip toolTip1;
        private DevExpress.XtraEditors.GroupControl groupControlBeerXMLCheck;
        private System.Windows.Forms.Button buttonBeerXMLIssuesReport;
        private System.Windows.Forms.Button buttonShowFile;
        private System.Windows.Forms.Button buttonIssuesReportsAll;
    }
}