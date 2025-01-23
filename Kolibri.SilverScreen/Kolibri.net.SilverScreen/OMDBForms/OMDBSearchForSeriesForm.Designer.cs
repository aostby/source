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
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelStatus = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            groupBoxManual = new GroupBox();
            buttonManual = new Button();
            textBoxManual = new TextBox();
            groupBoxSearchType = new GroupBox();
            radioButtonLocal = new RadioButton();
            radioButtonCombo = new RadioButton();
            buttonSearch = new Button();
            dataGridView1 = new DataGridView();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBoxManual.SuspendLayout();
            groupBoxSearchType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelStatus });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
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
            splitContainer1.Panel1.Controls.Add(groupBoxManual);
            splitContainer1.Panel1.Controls.Add(groupBoxSearchType);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(dataGridView1);
            splitContainer1.Size = new Size(800, 428);
            splitContainer1.SplitterDistance = 266;
            splitContainer1.TabIndex = 2;
            // 
            // groupBoxManual
            // 
            groupBoxManual.Controls.Add(buttonManual);
            groupBoxManual.Controls.Add(textBoxManual);
            groupBoxManual.Location = new Point(12, 78);
            groupBoxManual.Name = "groupBoxManual";
            groupBoxManual.Size = new Size(241, 80);
            groupBoxManual.TabIndex = 2;
            groupBoxManual.TabStop = false;
            groupBoxManual.Text = "Manuelt søk";
            // 
            // buttonManual
            // 
            buttonManual.Location = new Point(126, 45);
            buttonManual.Name = "buttonManual";
            buttonManual.Size = new Size(100, 23);
            buttonManual.TabIndex = 3;
            buttonManual.Text = "Manuelt søk";
            buttonManual.UseVisualStyleBackColor = true;
            buttonManual.Click += buttonManual_Click;
            // 
            // textBoxManual
            // 
            textBoxManual.Location = new Point(7, 16);
            textBoxManual.Name = "textBoxManual";
            textBoxManual.Size = new Size(219, 23);
            textBoxManual.TabIndex = 0;
            // 
            // groupBoxSearchType
            // 
            groupBoxSearchType.Controls.Add(radioButtonLocal);
            groupBoxSearchType.Controls.Add(radioButtonCombo);
            groupBoxSearchType.Controls.Add(buttonSearch);
            groupBoxSearchType.Location = new Point(12, 15);
            groupBoxSearchType.Name = "groupBoxSearchType";
            groupBoxSearchType.Size = new Size(241, 56);
            groupBoxSearchType.TabIndex = 1;
            groupBoxSearchType.TabStop = false;
            groupBoxSearchType.Text = "Kilde for detalj søk";
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
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(530, 428);
            dataGridView1.TabIndex = 0;
            // 
            // OMDBSearchForSeriesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Name = "OMDBSearchForSeriesForm";
            Text = "OMDBSearchForSeries";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBoxManual.ResumeLayout(false);
            groupBoxManual.PerformLayout();
            groupBoxSearchType.ResumeLayout(false);
            groupBoxSearchType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
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
        private TextBox textBoxManual;
    }
}