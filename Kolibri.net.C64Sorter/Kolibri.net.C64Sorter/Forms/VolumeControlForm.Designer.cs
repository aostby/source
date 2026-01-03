namespace Kolibri.net.C64Sorter.Forms
{
    partial class VolumeControlForm
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
            trackBar1 = new TrackBar();
            buttonToggle = new Button();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(12, 12);
            trackBar1.Maximum = 6;
            trackBar1.Minimum = -42;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(425, 45);
            trackBar1.TabIndex = 0;
            trackBar1.TickFrequency = 2;
            trackBar1.Scroll += trackBar1_Scroll;
            trackBar1.MouseHover += trackBar1_MouseHover;
            // 
            // buttonToggle
            // 
            buttonToggle.Location = new Point(362, 63);
            buttonToggle.Name = "buttonToggle";
            buttonToggle.Size = new Size(75, 37);
            buttonToggle.TabIndex = 1;
            buttonToggle.Text = "Enabled";
            buttonToggle.UseVisualStyleBackColor = true;
            buttonToggle.Click += toggleSound_Click;
            // 
            // VolumeControlForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(443, 98);
            Controls.Add(buttonToggle);
            Controls.Add(trackBar1);
            MaximumSize = new Size(459, 137);
            MinimumSize = new Size(459, 137);
            Name = "VolumeControlForm";
            Text = "VolumeControlForm";
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TrackBar trackBar1;
        private Button buttonToggle;
        private ToolTip toolTip1;
    }
}