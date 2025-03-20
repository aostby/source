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
            toolStrip1.SuspendLayout();
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
            flowLayoutPanelGZ.Location = new Point(23, 50);
            flowLayoutPanelGZ.Name = "flowLayoutPanelGZ";
            flowLayoutPanelGZ.Size = new Size(205, 372);
            flowLayoutPanelGZ.TabIndex = 1;
            // 
            // flowLayoutPanelDataFiles
            // 
            flowLayoutPanelDataFiles.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelDataFiles.Location = new Point(234, 50);
            flowLayoutPanelDataFiles.Name = "flowLayoutPanelDataFiles";
            flowLayoutPanelDataFiles.Size = new Size(205, 372);
            flowLayoutPanelDataFiles.TabIndex = 2;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripProgressBar1, toolStripLabel1 });
            toolStrip1.Location = new Point(0, 425);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
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
            linkLabelLocal.Location = new Point(23, 27);
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
            flowLayoutPanelUpdate.Location = new Point(445, 50);
            flowLayoutPanelUpdate.Name = "flowLayoutPanelUpdate";
            flowLayoutPanelUpdate.Size = new Size(205, 372);
            flowLayoutPanelUpdate.TabIndex = 3;
            // 
            // IMDbDataFilesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(flowLayoutPanelUpdate);
            Controls.Add(linkLabelLocal);
            Controls.Add(toolStrip1);
            Controls.Add(flowLayoutPanelDataFiles);
            Controls.Add(flowLayoutPanelGZ);
            Controls.Add(linkLabelOnline);
            Name = "IMDbDataFilesForm";
            Text = "IMDbDataFilesForm";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
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
    }
}