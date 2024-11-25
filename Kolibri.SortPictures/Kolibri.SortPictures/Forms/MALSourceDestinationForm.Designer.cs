namespace Kolibri.SortPictures.Forms
{
    partial class MALSourceDestinationForm
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
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.buttonSource = new System.Windows.Forms.Button();
            this.buttonDestination = new System.Windows.Forms.Button();
            this.textBoxDestination = new System.Windows.Forms.TextBox();
            this.buttonExecute = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelFilnavn = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelCustom = new System.Windows.Forms.Panel();
            this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonOpenDirD = new System.Windows.Forms.Button();
            this.buttonOpenDirS = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxSource
            // 
            this.textBoxSource.Location = new System.Drawing.Point(13, 13);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.Size = new System.Drawing.Size(622, 20);
            this.textBoxSource.TabIndex = 0;
            this.textBoxSource.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // buttonSource
            // 
            this.buttonSource.Location = new System.Drawing.Point(664, 9);
            this.buttonSource.Name = "buttonSource";
            this.buttonSource.Size = new System.Drawing.Size(75, 23);
            this.buttonSource.TabIndex = 1;
            this.buttonSource.Text = "Let opp";
            this.buttonSource.UseVisualStyleBackColor = true;
            this.buttonSource.Click += new System.EventHandler(this.button_Click);
            // 
            // buttonDestination
            // 
            this.buttonDestination.Location = new System.Drawing.Point(664, 35);
            this.buttonDestination.Name = "buttonDestination";
            this.buttonDestination.Size = new System.Drawing.Size(75, 23);
            this.buttonDestination.TabIndex = 3;
            this.buttonDestination.Text = "Let opp";
            this.buttonDestination.UseVisualStyleBackColor = true;
            this.buttonDestination.Click += new System.EventHandler(this.button_Click);
            // 
            // textBoxDestination
            // 
            this.textBoxDestination.Location = new System.Drawing.Point(13, 39);
            this.textBoxDestination.Name = "textBoxDestination";
            this.textBoxDestination.Size = new System.Drawing.Size(622, 20);
            this.textBoxDestination.TabIndex = 2;
            this.textBoxDestination.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // buttonExecute
            // 
            this.buttonExecute.Location = new System.Drawing.Point(663, 72);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(75, 41);
            this.buttonExecute.TabIndex = 4;
            this.buttonExecute.Text = "Utfør";
            this.buttonExecute.UseVisualStyleBackColor = true;
            this.buttonExecute.Click += new System.EventHandler(this.buttonExecute_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelFilnavn});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(756, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelFilnavn
            // 
            this.toolStripStatusLabelFilnavn.Name = "toolStripStatusLabelFilnavn";
            this.toolStripStatusLabelFilnavn.Size = new System.Drawing.Size(150, 17);
            this.toolStripStatusLabelFilnavn.Text = "toolStripStatusLabelFilnavn";
            // 
            // panelCustom
            // 
            this.panelCustom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCustom.Location = new System.Drawing.Point(13, 66);
            this.panelCustom.Name = "panelCustom";
            this.panelCustom.Size = new System.Drawing.Size(644, 359);
            this.panelCustom.TabIndex = 7;
            // 
            // flowLayoutPanelButtons
            // 
            this.flowLayoutPanelButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanelButtons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelButtons.Location = new System.Drawing.Point(664, 120);
            this.flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            this.flowLayoutPanelButtons.Size = new System.Drawing.Size(74, 305);
            this.flowLayoutPanelButtons.TabIndex = 8;
            // 
            // buttonOpenDirD
            // 
            this.buttonOpenDirD.Location = new System.Drawing.Point(637, 35);
            this.buttonOpenDirD.Name = "buttonOpenDirD";
            this.buttonOpenDirD.Size = new System.Drawing.Size(25, 23);
            this.buttonOpenDirD.TabIndex = 12;
            this.buttonOpenDirD.Text = "Let opp";
            this.buttonOpenDirD.UseVisualStyleBackColor = true;
            this.buttonOpenDirD.Click += new System.EventHandler(this.buttonOpenDir_Click);
            // 
            // buttonOpenDirS
            // 
            this.buttonOpenDirS.Location = new System.Drawing.Point(637, 9);
            this.buttonOpenDirS.Name = "buttonOpenDirS";
            this.buttonOpenDirS.Size = new System.Drawing.Size(25, 23);
            this.buttonOpenDirS.TabIndex = 11;
            this.buttonOpenDirS.Text = "Let opp";
            this.buttonOpenDirS.UseVisualStyleBackColor = true;
            this.buttonOpenDirS.Click += new System.EventHandler(this.buttonOpenDir_Click);
            // 
            // MALSourceDestinationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 450);
            this.Controls.Add(this.buttonOpenDirD);
            this.Controls.Add(this.buttonOpenDirS);
            this.Controls.Add(this.flowLayoutPanelButtons);
            this.Controls.Add(this.panelCustom);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.buttonExecute);
            this.Controls.Add(this.buttonDestination);
            this.Controls.Add(this.textBoxDestination);
            this.Controls.Add(this.buttonSource);
            this.Controls.Add(this.textBoxSource);
            this.Name = "MALSourceDestinationForm";
            this.Text = "SortPicsDesktopForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSource;
        private System.Windows.Forms.Button buttonSource;
        private System.Windows.Forms.Button buttonDestination;
        private System.Windows.Forms.TextBox textBoxDestination;
        private System.Windows.Forms.Button buttonExecute;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFilnavn;
        private System.Windows.Forms.Panel panelCustom;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;
        private System.Windows.Forms.Button buttonOpenDirD;
        private System.Windows.Forms.Button buttonOpenDirS;
    }
}