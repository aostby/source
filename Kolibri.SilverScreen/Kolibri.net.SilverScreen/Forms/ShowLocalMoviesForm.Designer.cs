namespace Kolibri.net.SilverScreen.Forms
{
    partial class ShowLocalMoviesForm
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
            splitContainer1 = new SplitContainer();
            buttonManual = new Button();
            groupBoxFilter = new GroupBox();
            buttonMissing = new Button();
            radioButtonFilterNotMatched = new RadioButton();
            radioButtonFilterNoneExistant = new RadioButton();
            radioButtonFilterAlle = new RadioButton();
            checkBoxSimple = new CheckBox();
            groupBoxFileSearch = new GroupBox();
            radioButtonUpdateNew = new RadioButton();
            radioButtonUpdateNone = new RadioButton();
            radioButtonUpdateAll = new RadioButton();
            labelNumItemsDB = new Label();
            textBoxSource = new TextBox();
            buttonOpenFolder = new Button();
            splitContainer2 = new SplitContainer();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelStatus = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolTipLocalMovies = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBoxFilter.SuspendLayout();
            groupBoxFileSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(buttonManual);
            splitContainer1.Panel1.Controls.Add(groupBoxFilter);
            splitContainer1.Panel1.Controls.Add(checkBoxSimple);
            splitContainer1.Panel1.Controls.Add(groupBoxFileSearch);
            splitContainer1.Panel1.Controls.Add(labelNumItemsDB);
            splitContainer1.Panel1.Controls.Add(textBoxSource);
            splitContainer1.Panel1.Controls.Add(buttonOpenFolder);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1266, 731);
            splitContainer1.SplitterDistance = 79;
            splitContainer1.TabIndex = 0;
            // 
            // buttonManual
            // 
            buttonManual.ImageAlign = ContentAlignment.MiddleLeft;
            buttonManual.Location = new Point(722, 50);
            buttonManual.Name = "buttonManual";
            buttonManual.Size = new Size(252, 23);
            buttonManual.TabIndex = 7;
            buttonManual.Text = "Let opp og finn film manuelt";
            buttonManual.TextAlign = ContentAlignment.MiddleRight;
            buttonManual.UseVisualStyleBackColor = true;
            buttonManual.Click += buttonManual_Click;
            // 
            // groupBoxFilter
            // 
            groupBoxFilter.Controls.Add(buttonMissing);
            groupBoxFilter.Controls.Add(radioButtonFilterNotMatched);
            groupBoxFilter.Controls.Add(radioButtonFilterNoneExistant);
            groupBoxFilter.Controls.Add(radioButtonFilterAlle);
            groupBoxFilter.Location = new Point(389, 34);
            groupBoxFilter.Name = "groupBoxFilter";
            groupBoxFilter.Size = new Size(327, 45);
            groupBoxFilter.TabIndex = 6;
            groupBoxFilter.TabStop = false;
            groupBoxFilter.Text = "Filter";
            // 
            // buttonMissing
            // 
            buttonMissing.Enabled = false;
            buttonMissing.Location = new Point(272, 16);
            buttonMissing.Name = "buttonMissing";
            buttonMissing.Size = new Size(49, 23);
            buttonMissing.TabIndex = 3;
            buttonMissing.Text = "Find";
            buttonMissing.UseVisualStyleBackColor = true;
            buttonMissing.Click += buttonMissing_Click;
            // 
            // radioButtonFilterNotMatched
            // 
            radioButtonFilterNotMatched.AutoSize = true;
            radioButtonFilterNotMatched.Location = new Point(191, 20);
            radioButtonFilterNotMatched.Name = "radioButtonFilterNotMatched";
            radioButtonFilterNotMatched.Size = new Size(75, 19);
            radioButtonFilterNotMatched.TabIndex = 2;
            radioButtonFilterNotMatched.Text = "Uten treff";
            radioButtonFilterNotMatched.UseVisualStyleBackColor = true;
            radioButtonFilterNotMatched.CheckedChanged += radioButtonFilter_CheckedChanged;
            // 
            // radioButtonFilterNoneExistant
            // 
            radioButtonFilterNoneExistant.AutoSize = true;
            radioButtonFilterNoneExistant.Location = new Point(70, 20);
            radioButtonFilterNoneExistant.Name = "radioButtonFilterNoneExistant";
            radioButtonFilterNoneExistant.Size = new Size(111, 19);
            radioButtonFilterNoneExistant.TabIndex = 1;
            radioButtonFilterNoneExistant.Text = "IkkeEksisterende";
            radioButtonFilterNoneExistant.UseVisualStyleBackColor = true;
            radioButtonFilterNoneExistant.CheckedChanged += radioButtonFilter_CheckedChanged;
            // 
            // radioButtonFilterAlle
            // 
            radioButtonFilterAlle.AutoSize = true;
            radioButtonFilterAlle.Checked = true;
            radioButtonFilterAlle.Location = new Point(15, 20);
            radioButtonFilterAlle.Name = "radioButtonFilterAlle";
            radioButtonFilterAlle.Size = new Size(45, 19);
            radioButtonFilterAlle.TabIndex = 0;
            radioButtonFilterAlle.TabStop = true;
            radioButtonFilterAlle.Text = "Alle";
            radioButtonFilterAlle.UseVisualStyleBackColor = true;
            radioButtonFilterAlle.CheckedChanged += radioButtonFilter_CheckedChanged;
            // 
            // checkBoxSimple
            // 
            checkBoxSimple.AutoSize = true;
            checkBoxSimple.Checked = true;
            checkBoxSimple.CheckState = CheckState.Checked;
            checkBoxSimple.Location = new Point(722, 34);
            checkBoxSimple.Name = "checkBoxSimple";
            checkBoxSimple.Size = new Size(75, 19);
            checkBoxSimple.TabIndex = 5;
            checkBoxSimple.Text = "Forenklet";
            checkBoxSimple.UseVisualStyleBackColor = true;
            // 
            // groupBoxFileSearch
            // 
            groupBoxFileSearch.Controls.Add(radioButtonUpdateNew);
            groupBoxFileSearch.Controls.Add(radioButtonUpdateNone);
            groupBoxFileSearch.Controls.Add(radioButtonUpdateAll);
            groupBoxFileSearch.Location = new Point(803, 10);
            groupBoxFileSearch.Name = "groupBoxFileSearch";
            groupBoxFileSearch.Size = new Size(171, 39);
            groupBoxFileSearch.TabIndex = 4;
            groupBoxFileSearch.TabStop = false;
            groupBoxFileSearch.Text = "Oppdater";
            // 
            // radioButtonUpdateNew
            // 
            radioButtonUpdateNew.AutoSize = true;
            radioButtonUpdateNew.Location = new Point(6, 14);
            radioButtonUpdateNew.Name = "radioButtonUpdateNew";
            radioButtonUpdateNew.Size = new Size(46, 19);
            radioButtonUpdateNew.TabIndex = 2;
            radioButtonUpdateNew.Text = "Nye";
            radioButtonUpdateNew.UseVisualStyleBackColor = true;
            // 
            // radioButtonUpdateNone
            // 
            radioButtonUpdateNone.AutoSize = true;
            radioButtonUpdateNone.Checked = true;
            radioButtonUpdateNone.Location = new Point(58, 14);
            radioButtonUpdateNone.Name = "radioButtonUpdateNone";
            radioButtonUpdateNone.Size = new Size(55, 19);
            radioButtonUpdateNone.TabIndex = 1;
            radioButtonUpdateNone.TabStop = true;
            radioButtonUpdateNone.Text = "Ingen";
            radioButtonUpdateNone.UseVisualStyleBackColor = true;
            // 
            // radioButtonUpdateAll
            // 
            radioButtonUpdateAll.AutoSize = true;
            radioButtonUpdateAll.Location = new Point(119, 14);
            radioButtonUpdateAll.Name = "radioButtonUpdateAll";
            radioButtonUpdateAll.Size = new Size(45, 19);
            radioButtonUpdateAll.TabIndex = 0;
            radioButtonUpdateAll.Text = "Alle";
            toolTipLocalMovies.SetToolTip(radioButtonUpdateAll, "Sletter alle Items og FileItems før søk (beholder filen på disk)");
            radioButtonUpdateAll.UseVisualStyleBackColor = true;
            // 
            // labelNumItemsDB
            // 
            labelNumItemsDB.AutoSize = true;
            labelNumItemsDB.Location = new Point(12, 34);
            labelNumItemsDB.Name = "labelNumItemsDB";
            labelNumItemsDB.Size = new Size(64, 15);
            labelNumItemsDB.TabIndex = 2;
            labelNumItemsDB.Text = "Antall (DB)";
            // 
            // textBoxSource
            // 
            textBoxSource.Location = new Point(12, 10);
            textBoxSource.Name = "textBoxSource";
            textBoxSource.Size = new Size(704, 23);
            textBoxSource.TabIndex = 1;
            // 
            // buttonOpenFolder
            // 
            buttonOpenFolder.ImageAlign = ContentAlignment.MiddleLeft;
            buttonOpenFolder.Location = new Point(722, 10);
            buttonOpenFolder.Name = "buttonOpenFolder";
            buttonOpenFolder.Size = new Size(75, 23);
            buttonOpenFolder.TabIndex = 0;
            buttonOpenFolder.Text = "Let opp mappe";
            buttonOpenFolder.TextAlign = ContentAlignment.MiddleRight;
            buttonOpenFolder.UseVisualStyleBackColor = true;
            buttonOpenFolder.Click += buttonOpenFolder_Click;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(statusStrip1);
            splitContainer2.Size = new Size(1266, 648);
            splitContainer2.SplitterDistance = 390;
            splitContainer2.TabIndex = 0;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelStatus, toolStripProgressBar1 });
            statusStrip1.Location = new Point(0, 626);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(390, 22);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelStatus
            // 
            toolStripStatusLabelStatus.Name = "toolStripStatusLabelStatus";
            toolStripStatusLabelStatus.Size = new Size(273, 17);
            toolStripStatusLabelStatus.Spring = true;
            toolStripStatusLabelStatus.Text = "Status";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 16);
            // 
            // ShowLocalMoviesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1266, 731);
            Controls.Add(splitContainer1);
            Name = "ShowLocalMoviesForm";
            Text = "Show local movies";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBoxFilter.ResumeLayout(false);
            groupBoxFilter.PerformLayout();
            groupBoxFileSearch.ResumeLayout(false);
            groupBoxFileSearch.PerformLayout();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private TextBox textBoxSource;
        private Button buttonOpenFolder;
        private Label labelNumItemsDB;
        private SplitContainer splitContainer2;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelStatus;
        private GroupBox groupBoxFileSearch;
        private RadioButton radioButtonUpdateNew;
        private RadioButton radioButtonUpdateNone;
        private RadioButton radioButtonUpdateAll;
        private CheckBox checkBoxSimple;
        private GroupBox groupBoxFilter;
        private RadioButton radioButtonFilterNoneExistant;
        private RadioButton radioButtonFilterAlle;
        private RadioButton radioButtonFilterNotMatched;
        private ToolTip toolTipLocalMovies;
        private Button buttonMissing;
        private ToolStripProgressBar toolStripProgressBar1;
        private Button buttonManual;
    }
}