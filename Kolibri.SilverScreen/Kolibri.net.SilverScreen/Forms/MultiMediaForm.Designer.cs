namespace Kolibri.net.SilverScreen.Forms
{
    partial class MultiMediaForm
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
            splitContainer1 = new SplitContainer();
            groupBoxFileSearch = new GroupBox();
            radioButtonNew = new RadioButton();
            radioButtonNone = new RadioButton();
            radioButtonAll = new RadioButton();
            labelNumItemsDB = new Label();
            textBoxSource = new TextBox();
            buttonOpenFolder = new Button();
            splitContainer2 = new SplitContainer();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelStatus = new ToolStripStatusLabel();
            checkBoxSimple = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
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
            splitContainer1.Panel1.Controls.Add(checkBoxSimple);
            splitContainer1.Panel1.Controls.Add(groupBoxFileSearch);
            splitContainer1.Panel1.Controls.Add(labelNumItemsDB);
            splitContainer1.Panel1.Controls.Add(textBoxSource);
            splitContainer1.Panel1.Controls.Add(buttonOpenFolder);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1633, 637);
            splitContainer1.SplitterDistance = 63;
            splitContainer1.TabIndex = 0;
            // 
            // groupBoxFileSearch
            // 
            groupBoxFileSearch.Controls.Add(radioButtonNew);
            groupBoxFileSearch.Controls.Add(radioButtonNone);
            groupBoxFileSearch.Controls.Add(radioButtonAll);
            groupBoxFileSearch.Location = new Point(803, 10);
            groupBoxFileSearch.Name = "groupBoxFileSearch";
            groupBoxFileSearch.Size = new Size(302, 39);
            groupBoxFileSearch.TabIndex = 4;
            groupBoxFileSearch.TabStop = false;
            groupBoxFileSearch.Text = "Oppdater";
            // 
            // radioButtonNew
            // 
            radioButtonNew.AutoSize = true;
            radioButtonNew.Location = new Point(6, 14);
            radioButtonNew.Name = "radioButtonNew";
            radioButtonNew.Size = new Size(46, 19);
            radioButtonNew.TabIndex = 2;
            radioButtonNew.Text = "Nye";
            radioButtonNew.UseVisualStyleBackColor = true;
            // 
            // radioButtonNone
            // 
            radioButtonNone.AutoSize = true;
            radioButtonNone.Checked = true;
            radioButtonNone.Location = new Point(58, 14);
            radioButtonNone.Name = "radioButtonNone";
            radioButtonNone.Size = new Size(55, 19);
            radioButtonNone.TabIndex = 1;
            radioButtonNone.TabStop = true;
            radioButtonNone.Text = "Ingen";
            radioButtonNone.UseVisualStyleBackColor = true;
            // 
            // radioButtonAll
            // 
            radioButtonAll.AutoSize = true;
            radioButtonAll.Location = new Point(119, 14);
            radioButtonAll.Name = "radioButtonAll";
            radioButtonAll.Size = new Size(45, 19);
            radioButtonAll.TabIndex = 0;
            radioButtonAll.Text = "Alle";
            radioButtonAll.UseVisualStyleBackColor = true;
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
            splitContainer2.Size = new Size(1633, 570);
            splitContainer2.SplitterDistance = 645;
            splitContainer2.TabIndex = 0;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelStatus });
            statusStrip1.Location = new Point(0, 548);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(645, 22);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelStatus
            // 
            toolStripStatusLabelStatus.Name = "toolStripStatusLabelStatus";
            toolStripStatusLabelStatus.Size = new Size(630, 17);
            toolStripStatusLabelStatus.Spring = true;
            toolStripStatusLabelStatus.Text = "Status";
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
            // MultiMediaForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1633, 637);
            Controls.Add(splitContainer1);
            Name = "MultiMediaForm";
            Text = "MultiMediaForm";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
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
        private RadioButton radioButtonNew;
        private RadioButton radioButtonNone;
        private RadioButton radioButtonAll;
        private CheckBox checkBoxSimple;
    }
}