namespace Kolibri.net.SilverScreen.Forms
{
    partial class IMDbDataFilesForm
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
            linkLabelOnline = new LinkLabel();
            flowLayoutPanelGZ = new FlowLayoutPanel();
            flowLayoutPanelDataFiles = new FlowLayoutPanel();
            toolStrip1 = new ToolStrip();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripLabel1 = new ToolStripLabel();
            linkLabelLocal = new LinkLabel();
            flowLayoutPanelUpdate = new FlowLayoutPanel();
            linkLabelIMDBdb = new LinkLabel();
            groupBoxGZ = new GroupBox();
            groupBoxDataFiles = new GroupBox();
            groupBoxUpdate = new GroupBox();
            panelInfo = new Panel();
            buttonCancel = new Button();
            toolStrip1.SuspendLayout();
            groupBoxGZ.SuspendLayout();
            groupBoxDataFiles.SuspendLayout();
            groupBoxUpdate.SuspendLayout();
            SuspendLayout();
            // 
            // linkLabelOnline
            // 
            linkLabelOnline.AutoSize = true;
            linkLabelOnline.Location = new Point(23, 7);
            linkLabelOnline.Name = "linkLabelOnline";
            linkLabelOnline.Size = new Size(89, 15);
            linkLabelOnline.TabIndex = 0;
            linkLabelOnline.TabStop = true;
            linkLabelOnline.Text = "linkLabelOnline";
            linkLabelOnline.LinkClicked += linkLabel1_LinkClicked;
            // 
            // flowLayoutPanelGZ
            // 
            flowLayoutPanelGZ.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelGZ.Location = new Point(6, 22);
            flowLayoutPanelGZ.Name = "flowLayoutPanelGZ";
            flowLayoutPanelGZ.Size = new Size(205, 372);
            flowLayoutPanelGZ.TabIndex = 1;
            // 
            // flowLayoutPanelDataFiles
            // 
            flowLayoutPanelDataFiles.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelDataFiles.Location = new Point(10, 22);
            flowLayoutPanelDataFiles.Name = "flowLayoutPanelDataFiles";
            flowLayoutPanelDataFiles.Size = new Size(205, 372);
            flowLayoutPanelDataFiles.TabIndex = 2;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripProgressBar1, toolStripLabel1 });
            toolStrip1.Location = new Point(0, 542);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1451, 25);
            toolStrip1.TabIndex = 3;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 22);
            toolStripProgressBar1.Visible = false;
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(86, 22);
            toolStripLabel1.Text = "toolStripLabel1";
            // 
            // linkLabelLocal
            // 
            linkLabelLocal.AutoSize = true;
            linkLabelLocal.Location = new Point(23, 30);
            linkLabelLocal.Name = "linkLabelLocal";
            linkLabelLocal.Size = new Size(82, 15);
            linkLabelLocal.TabIndex = 4;
            linkLabelLocal.TabStop = true;
            linkLabelLocal.Text = "linkLabelLocal";
            linkLabelLocal.LinkClicked += linkLabel1_LinkClicked;
            // 
            // flowLayoutPanelUpdate
            // 
            flowLayoutPanelUpdate.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelUpdate.Location = new Point(10, 22);
            flowLayoutPanelUpdate.Name = "flowLayoutPanelUpdate";
            flowLayoutPanelUpdate.Size = new Size(205, 372);
            flowLayoutPanelUpdate.TabIndex = 3;
            // 
            // linkLabelIMDBdb
            // 
            linkLabelIMDBdb.AutoSize = true;
            linkLabelIMDBdb.Location = new Point(23, 51);
            linkLabelIMDBdb.Name = "linkLabelIMDBdb";
            linkLabelIMDBdb.Size = new Size(53, 15);
            linkLabelIMDBdb.TabIndex = 5;
            linkLabelIMDBdb.TabStop = true;
            linkLabelIMDBdb.Text = "IMDB db";
            linkLabelIMDBdb.LinkClicked += linkLabel1_LinkClicked;
            // 
            // groupBoxGZ
            // 
            groupBoxGZ.Controls.Add(flowLayoutPanelGZ);
            groupBoxGZ.Location = new Point(23, 82);
            groupBoxGZ.Name = "groupBoxGZ";
            groupBoxGZ.Size = new Size(221, 444);
            groupBoxGZ.TabIndex = 6;
            groupBoxGZ.TabStop = false;
            groupBoxGZ.Text = "IMDB datafiles GZ";
            // 
            // groupBoxDataFiles
            // 
            groupBoxDataFiles.Controls.Add(flowLayoutPanelDataFiles);
            groupBoxDataFiles.Location = new Point(261, 82);
            groupBoxDataFiles.Name = "groupBoxDataFiles";
            groupBoxDataFiles.Size = new Size(221, 444);
            groupBoxDataFiles.TabIndex = 7;
            groupBoxDataFiles.TabStop = false;
            groupBoxDataFiles.Text = "IMDB Unzipped DataFiles";
            // 
            // groupBoxUpdate
            // 
            groupBoxUpdate.Controls.Add(buttonCancel);
            groupBoxUpdate.Controls.Add(flowLayoutPanelUpdate);
            groupBoxUpdate.Location = new Point(499, 82);
            groupBoxUpdate.Name = "groupBoxUpdate";
            groupBoxUpdate.Size = new Size(221, 444);
            groupBoxUpdate.TabIndex = 8;
            groupBoxUpdate.TabStop = false;
            groupBoxUpdate.Text = "Update Local Temp Database";
            // 
            // panelInfo
            // 
            panelInfo.Location = new Point(740, 5);
            panelInfo.Name = "panelInfo";
            panelInfo.Size = new Size(699, 521);
            panelInfo.TabIndex = 9;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(14, 403);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // IMDbDataFilesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1451, 567);
            Controls.Add(panelInfo);
            Controls.Add(groupBoxUpdate);
            Controls.Add(groupBoxDataFiles);
            Controls.Add(groupBoxGZ);
            Controls.Add(linkLabelIMDBdb);
            Controls.Add(linkLabelLocal);
            Controls.Add(toolStrip1);
            Controls.Add(linkLabelOnline);
            Name = "IMDbDataFilesForm";
            Text = "IMDbDataFilesForm";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            groupBoxGZ.ResumeLayout(false);
            groupBoxDataFiles.ResumeLayout(false);
            groupBoxUpdate.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private LinkLabel linkLabelOnline;
        private FlowLayoutPanel flowLayoutPanelGZ;
        private FlowLayoutPanel flowLayoutPanelDataFiles;
        private ToolStrip toolStrip1;
        private ToolStripLabel toolStripLabel1;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStrip toolStrip2;
        private LinkLabel linkLabelLocal;
        private FlowLayoutPanel flowLayoutPanelUpdate;
        private LinkLabel linkLabelIMDBdb;
        private GroupBox groupBoxGZ;
        private GroupBox groupBoxDataFiles;
        private GroupBox groupBoxUpdate;
        private Panel panelInfo;
        private Button buttonCancel;
    }
}