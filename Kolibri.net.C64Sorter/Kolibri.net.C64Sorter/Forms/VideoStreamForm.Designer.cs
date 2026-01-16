namespace Kolibri.net.C64Sorter.Forms
{
    partial class VideoStreamForm
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
            toolStripDropDownButtonToggleVideo = new ToolStripDropDownButton();
            buttonOpenStreamWindow = new Button();
            labelOpenStreamWindow = new Label();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelStatus, toolStripDropDownButtonToggleVideo });
            statusStrip1.Location = new Point(0, 139);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(584, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelStatus
            // 
            toolStripStatusLabelStatus.Name = "toolStripStatusLabelStatus";
            toolStripStatusLabelStatus.Size = new Size(118, 17);
            toolStripStatusLabelStatus.Text = "toolStripStatusLabel1";
            // 
            // toolStripDropDownButtonToggleVideo
            // 
            toolStripDropDownButtonToggleVideo.AutoSize = false;
            toolStripDropDownButtonToggleVideo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripDropDownButtonToggleVideo.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButtonToggleVideo.Name = "toolStripDropDownButtonToggleVideo";
            toolStripDropDownButtonToggleVideo.Size = new Size(35, 20);
            toolStripDropDownButtonToggleVideo.Tag = "Enabled";
            toolStripDropDownButtonToggleVideo.Text = "Enabled";
            toolStripDropDownButtonToggleVideo.Click += toolStripDropDownButtonToggleVideo_Click;
            // 
            // buttonOpenStreamWindow
            // 
            buttonOpenStreamWindow.BackgroundImageLayout = ImageLayout.Zoom;
            buttonOpenStreamWindow.Location = new Point(434, 12);
            buttonOpenStreamWindow.Name = "buttonOpenStreamWindow";
            buttonOpenStreamWindow.Size = new Size(138, 61);
            buttonOpenStreamWindow.TabIndex = 2;
            buttonOpenStreamWindow.Text = "Open";
            buttonOpenStreamWindow.UseVisualStyleBackColor = true;
            buttonOpenStreamWindow.Click += buttonOpenStreamWindow_Click;
            // 
            // labelOpenStreamWindow
            // 
            labelOpenStreamWindow.Location = new Point(12, 12);
            labelOpenStreamWindow.Name = "labelOpenStreamWindow";
            labelOpenStreamWindow.Size = new Size(416, 115);
            labelOpenStreamWindow.TabIndex = 3;
            labelOpenStreamWindow.Text = "Opening the stream can be done if you configure the Tools menu";
            // 
            // VideoStreamForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 161);
            Controls.Add(labelOpenStreamWindow);
            Controls.Add(buttonOpenStreamWindow);
            Controls.Add(statusStrip1);
            MaximumSize = new Size(600, 200);
            MinimumSize = new Size(459, 137);
            Name = "VideoStreamForm";
            Text = "VideoStreamForm";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelStatus;
        private ToolStripDropDownButton toolStripDropDownButtonToggleVideo;
        private Button buttonOpenStreamWindow;
        private Label labelOpenStreamWindow;
    }
}