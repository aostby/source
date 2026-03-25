namespace Kolibri.net.SilverScreen.Forms
{
    partial class MyMoviesForm
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
            textBoxSource = new TextBox();
            buttonOpenFolder = new Button();
            statusStrip1 = new StatusStrip();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripStatusLabelStatus = new ToolStripStatusLabel();
            labelNumItemsDB = new LinkLabel();
            groupBoxValg = new GroupBox();
            radioButtonDuplicates = new RadioButton();
            radioButtonEditDiff = new RadioButton();
            radioButtonShowDiff = new RadioButton();
            radioButtonShowLog = new RadioButton();
            buttonVelg = new Button();
            radioButtonShowGrid = new RadioButton();
            splitContainer1 = new SplitContainer();
            checkBoxDetailType = new CheckBox();
            statusStrip1.SuspendLayout();
            groupBoxValg.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // textBoxSource
            // 
            textBoxSource.Location = new Point(12, 12);
            textBoxSource.Name = "textBoxSource";
            textBoxSource.Size = new Size(704, 23);
            textBoxSource.TabIndex = 3;
            // 
            // buttonOpenFolder
            // 
            buttonOpenFolder.ImageAlign = ContentAlignment.MiddleLeft;
            buttonOpenFolder.Location = new Point(722, 12);
            buttonOpenFolder.Name = "buttonOpenFolder";
            buttonOpenFolder.Size = new Size(75, 23);
            buttonOpenFolder.TabIndex = 2;
            buttonOpenFolder.Text = "Let opp mappe";
            buttonOpenFolder.TextAlign = ContentAlignment.MiddleRight;
            buttonOpenFolder.UseVisualStyleBackColor = true;
            buttonOpenFolder.Click += buttonOpenFolder_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripProgressBar1, toolStripStatusLabelStatus });
            statusStrip1.Location = new Point(0, 747);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1201, 22);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 16);
            // 
            // toolStripStatusLabelStatus
            // 
            toolStripStatusLabelStatus.Name = "toolStripStatusLabelStatus";
            toolStripStatusLabelStatus.Size = new Size(60, 17);
            toolStripStatusLabelStatus.Text = "Welcome!";
            // 
            // labelNumItemsDB
            // 
            labelNumItemsDB.AutoSize = true;
            labelNumItemsDB.Location = new Point(12, 38);
            labelNumItemsDB.Name = "labelNumItemsDB";
            labelNumItemsDB.Size = new Size(64, 15);
            labelNumItemsDB.TabIndex = 5;
            labelNumItemsDB.TabStop = true;
            labelNumItemsDB.Text = "Antall (DB)";
            labelNumItemsDB.LinkClicked += labelNumItemsDB_LinkClicked;
            // 
            // groupBoxValg
            // 
            groupBoxValg.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBoxValg.Controls.Add(radioButtonDuplicates);
            groupBoxValg.Controls.Add(radioButtonEditDiff);
            groupBoxValg.Controls.Add(radioButtonShowDiff);
            groupBoxValg.Controls.Add(radioButtonShowLog);
            groupBoxValg.Controls.Add(buttonVelg);
            groupBoxValg.Controls.Add(radioButtonShowGrid);
            groupBoxValg.Location = new Point(621, 41);
            groupBoxValg.Name = "groupBoxValg";
            groupBoxValg.Size = new Size(577, 41);
            groupBoxValg.TabIndex = 7;
            groupBoxValg.TabStop = false;
            groupBoxValg.Text = "Velg handling";
            // 
            // radioButtonDuplicates
            // 
            radioButtonDuplicates.AutoSize = true;
            radioButtonDuplicates.Location = new Point(352, 17);
            radioButtonDuplicates.Name = "radioButtonDuplicates";
            radioButtonDuplicates.Size = new Size(79, 19);
            radioButtonDuplicates.TabIndex = 5;
            radioButtonDuplicates.Text = "Duplikater";
            radioButtonDuplicates.UseVisualStyleBackColor = true;
            // 
            // radioButtonEditDiff
            // 
            radioButtonEditDiff.AutoSize = true;
            radioButtonEditDiff.Location = new Point(260, 17);
            radioButtonEditDiff.Name = "radioButtonEditDiff";
            radioButtonEditDiff.Size = new Size(86, 19);
            radioButtonEditDiff.TabIndex = 4;
            radioButtonEditDiff.Text = "Rediger diff";
            radioButtonEditDiff.UseVisualStyleBackColor = true;
            // 
            // radioButtonShowDiff
            // 
            radioButtonShowDiff.AutoSize = true;
            radioButtonShowDiff.Location = new Point(193, 17);
            radioButtonShowDiff.Name = "radioButtonShowDiff";
            radioButtonShowDiff.Size = new Size(61, 19);
            radioButtonShowDiff.TabIndex = 3;
            radioButtonShowDiff.Text = "Vis diff";
            radioButtonShowDiff.UseVisualStyleBackColor = true;
            // 
            // radioButtonShowLog
            // 
            radioButtonShowLog.AutoSize = true;
            radioButtonShowLog.Location = new Point(103, 17);
            radioButtonShowLog.Name = "radioButtonShowLog";
            radioButtonShowLog.Size = new Size(67, 19);
            radioButtonShowLog.TabIndex = 2;
            radioButtonShowLog.Text = "Vis logg";
            radioButtonShowLog.UseVisualStyleBackColor = true;
            // 
            // buttonVelg
            // 
            buttonVelg.Anchor = AnchorStyles.Right;
            buttonVelg.Location = new Point(493, 13);
            buttonVelg.Name = "buttonVelg";
            buttonVelg.Size = new Size(75, 23);
            buttonVelg.TabIndex = 1;
            buttonVelg.Text = "Velg";
            buttonVelg.UseVisualStyleBackColor = true;
            buttonVelg.Click += buttonVelg_Click;
            // 
            // radioButtonShowGrid
            // 
            radioButtonShowGrid.AutoSize = true;
            radioButtonShowGrid.Checked = true;
            radioButtonShowGrid.Location = new Point(17, 17);
            radioButtonShowGrid.Name = "radioButtonShowGrid";
            radioButtonShowGrid.Size = new Size(58, 19);
            radioButtonShowGrid.TabIndex = 0;
            radioButtonShowGrid.TabStop = true;
            radioButtonShowGrid.Text = "Vis DB";
            radioButtonShowGrid.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(12, 82);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Size = new Size(1180, 662);
            splitContainer1.SplitterDistance = 520;
            splitContainer1.TabIndex = 8;
            // 
            // checkBoxDetailType
            // 
            checkBoxDetailType.AutoSize = true;
            checkBoxDetailType.Checked = true;
            checkBoxDetailType.CheckState = CheckState.Checked;
            checkBoxDetailType.Location = new Point(12, 57);
            checkBoxDetailType.Name = "checkBoxDetailType";
            checkBoxDetailType.Size = new Size(134, 19);
            checkBoxDetailType.TabIndex = 9;
            checkBoxDetailType.Text = "Utvidet detaljvisning";
            checkBoxDetailType.UseVisualStyleBackColor = true;
            // 
            // MyMoviesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1201, 769);
            Controls.Add(checkBoxDetailType);
            Controls.Add(splitContainer1);
            Controls.Add(groupBoxValg);
            Controls.Add(labelNumItemsDB);
            Controls.Add(statusStrip1);
            Controls.Add(textBoxSource);
            Controls.Add(buttonOpenFolder);
            Name = "MyMoviesForm";
            Text = "Movies I Have - MyMovies";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            groupBoxValg.ResumeLayout(false);
            groupBoxValg.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxSource;
        private Button buttonOpenFolder;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelStatus;
        private ToolStripProgressBar toolStripProgressBar1;
        private LinkLabel labelNumItemsDB;
        private GroupBox groupBoxValg;
        private Button buttonVelg;
        private RadioButton radioButtonShowGrid;
        private SplitContainer splitContainer1;
        private CheckBox checkBoxDetailType;
        private RadioButton radioButtonShowLog;
        private RadioButton radioButton1;
        private RadioButton radioButtonShowDiff;
        private RadioButton radioButtonEditDiff;
        private RadioButton radioButtonDuplicates;
    }
}