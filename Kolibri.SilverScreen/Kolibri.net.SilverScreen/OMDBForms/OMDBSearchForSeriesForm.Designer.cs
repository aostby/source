namespace Kolibri.net.SilverScreen.OMDBForms
{
    partial class OMDBSearchForSeriesForm
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
            components = new System.ComponentModel.Container();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelStatus = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            groupBoxDetails = new GroupBox();
            labelPath = new Label();
            pictureBoxCurrent = new PictureBox();
            groupBoxChangePath = new GroupBox();
            labelTo = new Label();
            labelFrom = new Label();
            textBox2 = new TextBox();
            buttonExecuteChange = new Button();
            textBox1 = new TextBox();
            labelFilsti = new Label();
            buttonOpenFolder = new Button();
            groupBoxManual = new GroupBox();
            textBoxYearValue = new TextBox();
            buttonTmdbSearch = new Button();
            buttonImdbIdSearch = new Button();
            buttonManual = new Button();
            textBoxSearchValue = new TextBox();
            groupBoxSearchType = new GroupBox();
            buttonLog = new Button();
            radioButtonLocal = new RadioButton();
            radioButtonCombo = new RadioButton();
            buttonSearch = new Button();
            dataGridView1 = new DataGridView();
            contextMenuStrip1 = new ContextMenuStrip(components);
            toolStripMenuItemNavn = new ToolStripMenuItem();
            toolStripMenuItemImdbId = new ToolStripMenuItem();
            toolStripMenuItemSearchThisFolder = new ToolStripMenuItem();
            buttonFindAndUpdateByIMDBID = new Button();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBoxDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCurrent).BeginInit();
            groupBoxChangePath.SuspendLayout();
            groupBoxManual.SuspendLayout();
            groupBoxSearchType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelStatus });
            statusStrip1.Location = new Point(0, 681);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(958, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelStatus
            // 
            toolStripStatusLabelStatus.Name = "toolStripStatusLabelStatus";
            toolStripStatusLabelStatus.Size = new Size(39, 17);
            toolStripStatusLabelStatus.Text = "Status";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(groupBoxDetails);
            splitContainer1.Panel1.Controls.Add(groupBoxChangePath);
            splitContainer1.Panel1.Controls.Add(labelFilsti);
            splitContainer1.Panel1.Controls.Add(buttonOpenFolder);
            splitContainer1.Panel1.Controls.Add(groupBoxManual);
            splitContainer1.Panel1.Controls.Add(groupBoxSearchType);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(dataGridView1);
            splitContainer1.Size = new Size(958, 681);
            splitContainer1.SplitterDistance = 318;
            splitContainer1.TabIndex = 2;
            // 
            // groupBoxDetails
            // 
            groupBoxDetails.Controls.Add(labelPath);
            groupBoxDetails.Controls.Add(pictureBoxCurrent);
            groupBoxDetails.Location = new Point(17, 427);
            groupBoxDetails.Name = "groupBoxDetails";
            groupBoxDetails.Size = new Size(228, 242);
            groupBoxDetails.TabIndex = 7;
            groupBoxDetails.TabStop = false;
            groupBoxDetails.Text = "Details";
            // 
            // labelPath
            // 
            labelPath.AutoSize = true;
            labelPath.Location = new Point(13, 205);
            labelPath.Name = "labelPath";
            labelPath.Size = new Size(34, 15);
            labelPath.TabIndex = 7;
            labelPath.Text = "Path:";
            labelPath.Click += labelPath_Click;
            // 
            // pictureBoxCurrent
            // 
            pictureBoxCurrent.Location = new Point(12, 22);
            pictureBoxCurrent.Name = "pictureBoxCurrent";
            pictureBoxCurrent.Size = new Size(209, 177);
            pictureBoxCurrent.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxCurrent.TabIndex = 6;
            pictureBoxCurrent.TabStop = false;
            // 
            // groupBoxChangePath
            // 
            groupBoxChangePath.Controls.Add(buttonFindAndUpdateByIMDBID);
            groupBoxChangePath.Controls.Add(labelTo);
            groupBoxChangePath.Controls.Add(labelFrom);
            groupBoxChangePath.Controls.Add(textBox2);
            groupBoxChangePath.Controls.Add(buttonExecuteChange);
            groupBoxChangePath.Controls.Add(textBox1);
            groupBoxChangePath.Location = new Point(14, 302);
            groupBoxChangePath.Name = "groupBoxChangePath";
            groupBoxChangePath.Size = new Size(231, 116);
            groupBoxChangePath.TabIndex = 6;
            groupBoxChangePath.TabStop = false;
            groupBoxChangePath.Text = "ChangePath";
            // 
            // labelTo
            // 
            labelTo.AutoSize = true;
            labelTo.Location = new Point(15, 58);
            labelTo.Name = "labelTo";
            labelTo.Size = new Size(19, 15);
            labelTo.TabIndex = 4;
            labelTo.Text = "To";
            // 
            // labelFrom
            // 
            labelFrom.AutoSize = true;
            labelFrom.Location = new Point(15, 29);
            labelFrom.Name = "labelFrom";
            labelFrom.Size = new Size(35, 15);
            labelFrom.TabIndex = 3;
            labelFrom.Text = "From";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(56, 55);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(169, 23);
            textBox2.TabIndex = 1;
            // 
            // buttonExecuteChange
            // 
            buttonExecuteChange.Location = new Point(150, 84);
            buttonExecuteChange.Name = "buttonExecuteChange";
            buttonExecuteChange.Size = new Size(75, 23);
            buttonExecuteChange.TabIndex = 2;
            buttonExecuteChange.Text = "Execute";
            buttonExecuteChange.UseVisualStyleBackColor = true;
            buttonExecuteChange.Click += buttonExecuteChange_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(56, 26);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(169, 23);
            textBox1.TabIndex = 0;
            // 
            // labelFilsti
            // 
            labelFilsti.AutoSize = true;
            labelFilsti.Location = new Point(14, 15);
            labelFilsti.Name = "labelFilsti";
            labelFilsti.Size = new Size(31, 15);
            labelFilsti.TabIndex = 4;
            labelFilsti.Text = "Filsti";
            // 
            // buttonOpenFolder
            // 
            buttonOpenFolder.Location = new Point(12, 33);
            buttonOpenFolder.Name = "buttonOpenFolder";
            buttonOpenFolder.Size = new Size(75, 23);
            buttonOpenFolder.TabIndex = 3;
            buttonOpenFolder.Text = "Mappe";
            buttonOpenFolder.UseVisualStyleBackColor = true;
            buttonOpenFolder.Click += buttonOpenFolder_Click;
            // 
            // groupBoxManual
            // 
            groupBoxManual.Controls.Add(textBoxYearValue);
            groupBoxManual.Controls.Add(buttonTmdbSearch);
            groupBoxManual.Controls.Add(buttonImdbIdSearch);
            groupBoxManual.Controls.Add(buttonManual);
            groupBoxManual.Controls.Add(textBoxSearchValue);
            groupBoxManual.Location = new Point(12, 159);
            groupBoxManual.Name = "groupBoxManual";
            groupBoxManual.Size = new Size(241, 137);
            groupBoxManual.TabIndex = 2;
            groupBoxManual.TabStop = false;
            groupBoxManual.Text = "Manuelt søk (tittel, år)";
            // 
            // textBoxYearValue
            // 
            textBoxYearValue.Location = new Point(7, 45);
            textBoxYearValue.Name = "textBoxYearValue";
            textBoxYearValue.Size = new Size(219, 23);
            textBoxYearValue.TabIndex = 6;
            // 
            // buttonTmdbSearch
            // 
            buttonTmdbSearch.Location = new Point(11, 105);
            buttonTmdbSearch.Name = "buttonTmdbSearch";
            buttonTmdbSearch.Size = new Size(75, 23);
            buttonTmdbSearch.TabIndex = 5;
            buttonTmdbSearch.Text = "TMDB søk";
            buttonTmdbSearch.UseVisualStyleBackColor = true;
            buttonTmdbSearch.Click += buttonTmdbSearch_Click;
            // 
            // buttonImdbIdSearch
            // 
            buttonImdbIdSearch.Location = new Point(11, 76);
            buttonImdbIdSearch.Name = "buttonImdbIdSearch";
            buttonImdbIdSearch.Size = new Size(75, 23);
            buttonImdbIdSearch.TabIndex = 4;
            buttonImdbIdSearch.Text = "ImdbId søk";
            buttonImdbIdSearch.UseVisualStyleBackColor = true;
            buttonImdbIdSearch.Click += buttonImdbIdSearch_Click;
            // 
            // buttonManual
            // 
            buttonManual.Location = new Point(129, 75);
            buttonManual.Name = "buttonManual";
            buttonManual.Size = new Size(100, 23);
            buttonManual.TabIndex = 3;
            buttonManual.Text = "Manuelt søk";
            buttonManual.UseVisualStyleBackColor = true;
            buttonManual.Click += buttonManual_Click;
            // 
            // textBoxSearchValue
            // 
            textBoxSearchValue.Location = new Point(7, 16);
            textBoxSearchValue.Name = "textBoxSearchValue";
            textBoxSearchValue.Size = new Size(219, 23);
            textBoxSearchValue.TabIndex = 0;
            textBoxSearchValue.TextChanged += textBoxManual_TextChanged;
            // 
            // groupBoxSearchType
            // 
            groupBoxSearchType.Controls.Add(buttonLog);
            groupBoxSearchType.Controls.Add(radioButtonLocal);
            groupBoxSearchType.Controls.Add(radioButtonCombo);
            groupBoxSearchType.Controls.Add(buttonSearch);
            groupBoxSearchType.Location = new Point(12, 62);
            groupBoxSearchType.Name = "groupBoxSearchType";
            groupBoxSearchType.Size = new Size(241, 91);
            groupBoxSearchType.TabIndex = 1;
            groupBoxSearchType.TabStop = false;
            groupBoxSearchType.Text = "Kilde for detalj søk";
            // 
            // buttonLog
            // 
            buttonLog.Location = new Point(11, 58);
            buttonLog.Name = "buttonLog";
            buttonLog.Size = new Size(215, 23);
            buttonLog.TabIndex = 3;
            buttonLog.Text = "Vis logg";
            buttonLog.UseVisualStyleBackColor = true;
            buttonLog.Click += buttonLog_Click;
            // 
            // radioButtonLocal
            // 
            radioButtonLocal.AutoSize = true;
            radioButtonLocal.Checked = true;
            radioButtonLocal.Location = new Point(100, 26);
            radioButtonLocal.Name = "radioButtonLocal";
            radioButtonLocal.Size = new Size(57, 19);
            radioButtonLocal.TabIndex = 2;
            radioButtonLocal.TabStop = true;
            radioButtonLocal.Text = "Lokalt";
            radioButtonLocal.UseVisualStyleBackColor = true;
            // 
            // radioButtonCombo
            // 
            radioButtonCombo.AutoSize = true;
            radioButtonCombo.Location = new Point(11, 26);
            radioButtonCombo.Name = "radioButtonCombo";
            radioButtonCombo.Size = new Size(81, 19);
            radioButtonCombo.TabIndex = 1;
            radioButtonCombo.TabStop = true;
            radioButtonCombo.Text = "Kombinert";
            radioButtonCombo.UseVisualStyleBackColor = true;
            // 
            // buttonSearch
            // 
            buttonSearch.Location = new Point(163, 24);
            buttonSearch.Name = "buttonSearch";
            buttonSearch.Size = new Size(63, 23);
            buttonSearch.TabIndex = 0;
            buttonSearch.Text = "Søk";
            buttonSearch.UseVisualStyleBackColor = true;
            buttonSearch.Click += buttonSearch_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.ContextMenuStrip = contextMenuStrip1;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(636, 681);
            dataGridView1.TabIndex = 0;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            dataGridView1.DoubleClick += dataGridView1_DoubleClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItemNavn, toolStripMenuItemImdbId, toolStripMenuItemSearchThisFolder });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(204, 70);
            // 
            // toolStripMenuItemNavn
            // 
            toolStripMenuItemNavn.Name = "toolStripMenuItemNavn";
            toolStripMenuItemNavn.Size = new Size(203, 22);
            toolStripMenuItemNavn.Text = "Let opp Navn";
            toolStripMenuItemNavn.Click += ContextMenuEvent_Click;
            // 
            // toolStripMenuItemImdbId
            // 
            toolStripMenuItemImdbId.Name = "toolStripMenuItemImdbId";
            toolStripMenuItemImdbId.Size = new Size(203, 22);
            toolStripMenuItemImdbId.Text = "Sett ImdbId";
            toolStripMenuItemImdbId.Click += ContextMenuEvent_Click;
            // 
            // toolStripMenuItemSearchThisFolder
            // 
            toolStripMenuItemSearchThisFolder.Name = "toolStripMenuItemSearchThisFolder";
            toolStripMenuItemSearchThisFolder.Size = new Size(203, 22);
            toolStripMenuItemSearchThisFolder.Text = "Søk etter denne mappen";
            toolStripMenuItemSearchThisFolder.Click += ContextMenuEvent_Click;
            // 
            // buttonFindAndUpdateByIMDBID
            // 
            buttonFindAndUpdateByIMDBID.Location = new Point(10, 84);
            buttonFindAndUpdateByIMDBID.Name = "buttonFindAndUpdateByIMDBID";
            buttonFindAndUpdateByIMDBID.Size = new Size(105, 23);
            buttonFindAndUpdateByIMDBID.TabIndex = 5;
            buttonFindAndUpdateByIMDBID.Text = "Find And Update By IMDBId";
            buttonFindAndUpdateByIMDBID.UseVisualStyleBackColor = true;
            buttonFindAndUpdateByIMDBID.Click += buttonFindAndUpdateByIMDBID_Click;
            // 
            // OMDBSearchForSeriesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(958, 703);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Name = "OMDBSearchForSeriesForm";
            Text = "OMDBSearchForSeries";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBoxDetails.ResumeLayout(false);
            groupBoxDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxCurrent).EndInit();
            groupBoxChangePath.ResumeLayout(false);
            groupBoxChangePath.PerformLayout();
            groupBoxManual.ResumeLayout(false);
            groupBoxManual.PerformLayout();
            groupBoxSearchType.ResumeLayout(false);
            groupBoxSearchType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelStatus;
        private SplitContainer splitContainer1;
        private Button buttonSearch;
        private DataGridView dataGridView1;
        private GroupBox groupBoxSearchType;
        private RadioButton radioButtonLocal;
        private RadioButton radioButtonCombo;
        private GroupBox groupBoxManual;
        private Button buttonManual;
        private TextBox textBoxSearchValue;
        private Button buttonLog;
        private Button buttonImdbIdSearch;
        private Button buttonTmdbSearch;
        private Label labelFilsti;
        private Button buttonOpenFolder;
        private ContextMenuStrip contextMenuStrip1;
        private TextBox textBoxYearValue;
        private ToolStripMenuItem toolStripMenuItemNavn;
        private ToolStripMenuItem toolStripMenuItemImdbId;
        private ToolStripMenuItem toolStripMenuItemSearchThisFolder;
        private GroupBox groupBoxChangePath;
        private Label labelTo;
        private Label labelFrom;
        private TextBox textBox2;
        private Button buttonExecuteChange;
        private TextBox textBox1;
        private GroupBox groupBoxDetails;
        private Label labelPath;
        private PictureBox pictureBoxCurrent;
        private Button buttonFindAndUpdateByIMDBID;
    }
}