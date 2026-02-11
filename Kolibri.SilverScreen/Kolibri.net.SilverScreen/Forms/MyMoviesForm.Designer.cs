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
            buttonVelg = new Button();
            radioButtonShowGrid = new RadioButton();
            splitContainer1 = new SplitContainer();
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
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
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
            groupBoxValg.Controls.Add(buttonVelg);
            groupBoxValg.Controls.Add(radioButtonShowGrid);
            groupBoxValg.Location = new Point(291, 41);
            groupBoxValg.Name = "groupBoxValg";
            groupBoxValg.Size = new Size(506, 41);
            groupBoxValg.TabIndex = 7;
            groupBoxValg.TabStop = false;
            groupBoxValg.Text = "Velg handling";
            // 
            // buttonVelg
            // 
            buttonVelg.Anchor = AnchorStyles.Right;
            buttonVelg.Location = new Point(422, 13);
            buttonVelg.Name = "buttonVelg";
            buttonVelg.Size = new Size(75, 23);
            buttonVelg.TabIndex = 1;
            buttonVelg.Text = "Velg";
            buttonVelg.UseVisualStyleBackColor = true;
            buttonVelg.Click += button1_Click;
            // 
            // radioButtonShowGrid
            // 
            radioButtonShowGrid.AutoSize = true;
            radioButtonShowGrid.Checked = true;
            radioButtonShowGrid.Location = new Point(6, 16);
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
            splitContainer1.Size = new Size(779, 343);
            splitContainer1.SplitterDistance = 344;
            splitContainer1.TabIndex = 8;
            // 
            // MyMoviesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
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
    }
}