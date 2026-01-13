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
            videoview = new LibVLCSharp.WinForms.VideoView();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelStatus = new ToolStripStatusLabel();
            toolStripDropDownButtonToggleVideo = new ToolStripDropDownButton();
            menuStrip1 = new MenuStrip();
            streamToolStripMenuItem = new ToolStripMenuItem();
            startStreamToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)videoview).BeginInit();
            statusStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // videoview
            // 
            videoview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            videoview.BackColor = Color.Black;
            videoview.Location = new Point(12, 0);
            videoview.MediaPlayer = null;
            videoview.Name = "videoview";
            videoview.Size = new Size(815, 518);
            videoview.TabIndex = 0;
            videoview.Text = "videoView1";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelStatus, toolStripDropDownButtonToggleVideo });
            statusStrip1.Location = new Point(0, 521);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(839, 22);
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
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { streamToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(839, 24);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // streamToolStripMenuItem
            // 
            streamToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { startStreamToolStripMenuItem });
            streamToolStripMenuItem.Name = "streamToolStripMenuItem";
            streamToolStripMenuItem.Size = new Size(56, 20);
            streamToolStripMenuItem.Text = "Stream";
            // 
            // startStreamToolStripMenuItem
            // 
            startStreamToolStripMenuItem.Name = "startStreamToolStripMenuItem";
            startStreamToolStripMenuItem.Size = new Size(137, 22);
            startStreamToolStripMenuItem.Text = "Start stream";
            startStreamToolStripMenuItem.Click += startStreamToolStripMenuItem_Click;
            // 
            // VideoStreamForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(839, 543);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            Controls.Add(videoview);
            MainMenuStrip = menuStrip1;
            Name = "VideoStreamForm";
            Text = "VideoStreamForm";
            ((System.ComponentModel.ISupportInitialize)videoview).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private LibVLCSharp.WinForms.VideoView videoview;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelStatus;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem streamToolStripMenuItem;
        private ToolStripMenuItem startStreamToolStripMenuItem;
        private ToolStripDropDownButton toolStripDropDownButtonToggleVideo;
    }
}