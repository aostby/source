namespace Kolibri.net.Common.Formutilities
{
    partial class DuplicatesForm
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
            labelB = new Label();
            labelA = new Label();
            btnDeleteASelected = new Button();
            btnDeleteBSelected = new Button();
            lstDuplicates = new ListBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            picImageA = new PictureBox();
            picImageB = new PictureBox();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picImageA).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picImageB).BeginInit();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(labelB);
            splitContainer1.Panel1.Controls.Add(labelA);
            splitContainer1.Panel1.Controls.Add(btnDeleteASelected);
            splitContainer1.Panel1.Controls.Add(btnDeleteBSelected);
            splitContainer1.Panel1.Controls.Add(lstDuplicates);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tableLayoutPanel1);
            splitContainer1.Size = new Size(800, 450);
            splitContainer1.SplitterDistance = 191;
            splitContainer1.TabIndex = 0;
            // 
            // labelB
            // 
            labelB.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelB.AutoSize = true;
            labelB.Location = new Point(153, 397);
            labelB.Name = "labelB";
            labelB.Size = new Size(35, 15);
            labelB.TabIndex = 4;
            labelB.Text = "B -->";
            // 
            // labelA
            // 
            labelA.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelA.AutoSize = true;
            labelA.Location = new Point(12, 397);
            labelA.Name = "labelA";
            labelA.Size = new Size(36, 15);
            labelA.TabIndex = 3;
            labelA.Text = "<-- A";
            // 
            // btnDeleteASelected
            // 
            btnDeleteASelected.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnDeleteASelected.Location = new Point(3, 415);
            btnDeleteASelected.Name = "btnDeleteASelected";
            btnDeleteASelected.Size = new Size(75, 23);
            btnDeleteASelected.TabIndex = 2;
            btnDeleteASelected.Text = "Slett A";
            btnDeleteASelected.UseVisualStyleBackColor = true;
            btnDeleteASelected.Click += btnDeleteSelected_Click;
            // 
            // btnDeleteBSelected
            // 
            btnDeleteBSelected.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnDeleteBSelected.Location = new Point(114, 415);
            btnDeleteBSelected.Name = "btnDeleteBSelected";
            btnDeleteBSelected.Size = new Size(75, 23);
            btnDeleteBSelected.TabIndex = 0;
            btnDeleteBSelected.Text = "Slett B";
            btnDeleteBSelected.UseVisualStyleBackColor = true;
            btnDeleteBSelected.Click += btnDeleteSelected_Click;
            // 
            // lstDuplicates
            // 
            lstDuplicates.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstDuplicates.FormattingEnabled = true;
            lstDuplicates.Location = new Point(12, 8);
            lstDuplicates.Name = "lstDuplicates";
            lstDuplicates.Size = new Size(176, 394);
            lstDuplicates.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(picImageA, 0, 0);
            tableLayoutPanel1.Controls.Add(picImageB, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(605, 450);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // picImageA
            // 
            picImageA.Dock = DockStyle.Fill;
            picImageA.Location = new Point(3, 3);
            picImageA.Name = "picImageA";
            picImageA.Size = new Size(296, 444);
            picImageA.TabIndex = 0;
            picImageA.TabStop = false;
            picImageA.MouseClick += picImage_MouseClick;
            // 
            // picImageB
            // 
            picImageB.Dock = DockStyle.Fill;
            picImageB.Location = new Point(305, 3);
            picImageB.Name = "picImageB";
            picImageB.Size = new Size(297, 444);
            picImageB.TabIndex = 1;
            picImageB.TabStop = false;
            picImageB.MouseClick += picImage_MouseClick;
            // 
            // DuplicatesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Name = "DuplicatesForm";
            Text = "DuplicatesForm";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picImageA).EndInit();
            ((System.ComponentModel.ISupportInitialize)picImageB).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private TableLayoutPanel tableLayoutPanel1;
        private ListBox lstDuplicates;
        private Button btnDeleteBSelected;
        private PictureBox picImageA;
        private PictureBox picImageB;
        private Button btnDeleteASelected;
        private Label labelB;
        private Label labelA;
        private ToolTip toolTip1;
    }
}