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
            groupBoxtype = new GroupBox();
            radioButtonDrive = new RadioButton();
            radioButtonUltiSid = new RadioButton();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            groupBoxtype.SuspendLayout();
            SuspendLayout();
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(12, 8);
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
            buttonToggle.Location = new Point(362, 50);
            buttonToggle.Name = "buttonToggle";
            buttonToggle.Size = new Size(75, 37);
            buttonToggle.TabIndex = 1;
            buttonToggle.Text = "Enabled";
            buttonToggle.UseVisualStyleBackColor = true;
            buttonToggle.Click += toggleSound_Click;
            // 
            // groupBoxtype
            // 
            groupBoxtype.Controls.Add(radioButtonDrive);
            groupBoxtype.Controls.Add(radioButtonUltiSid);
            groupBoxtype.Location = new Point(185, 45);
            groupBoxtype.Name = "groupBoxtype";
            groupBoxtype.Size = new Size(145, 46);
            groupBoxtype.TabIndex = 2;
            groupBoxtype.TabStop = false;
            groupBoxtype.Text = "Volume for";
            // 
            // radioButtonDrive
            // 
            radioButtonDrive.AutoSize = true;
            radioButtonDrive.Location = new Point(81, 18);
            radioButtonDrive.Name = "radioButtonDrive";
            radioButtonDrive.Size = new Size(52, 19);
            radioButtonDrive.TabIndex = 1;
            radioButtonDrive.Text = "Drive";
            radioButtonDrive.UseVisualStyleBackColor = true;
            // 
            // radioButtonUltiSid
            // 
            radioButtonUltiSid.AutoSize = true;
            radioButtonUltiSid.Checked = true;
            radioButtonUltiSid.Location = new Point(16, 18);
            radioButtonUltiSid.Name = "radioButtonUltiSid";
            radioButtonUltiSid.Size = new Size(59, 19);
            radioButtonUltiSid.TabIndex = 0;
            radioButtonUltiSid.TabStop = true;
            radioButtonUltiSid.Text = "UltiSid";
            radioButtonUltiSid.UseVisualStyleBackColor = true;
            // 
            // VolumeControlForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(443, 98);
            Controls.Add(groupBoxtype);
            Controls.Add(buttonToggle);
            Controls.Add(trackBar1);
            MaximumSize = new Size(459, 137);
            MinimumSize = new Size(459, 137);
            Name = "VolumeControlForm";
            Text = "VolumeControlForm";
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            groupBoxtype.ResumeLayout(false);
            groupBoxtype.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TrackBar trackBar1;
        private Button buttonToggle;
        private ToolTip toolTip1;
        private GroupBox groupBoxtype;
        private RadioButton radioButtonDrive;
        private RadioButton radioButtonUltiSid;
    }
}