namespace Kolibri.Common.Utilities.Forms
{
    partial class RenameFilesForm
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
            this.buttonOpenDirD = new System.Windows.Forms.Button();
            this.buttonOpenDirS = new System.Windows.Forms.Button();
            this.buttonDestination = new System.Windows.Forms.Button();
            this.textBoxDestination = new System.Windows.Forms.TextBox();
            this.buttonSource = new System.Windows.Forms.Button();
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.groupBoxRename = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.buttonExecute = new System.Windows.Forms.Button();
            this.buttonPreview = new System.Windows.Forms.Button();
            this.linkLabelTil = new System.Windows.Forms.LinkLabel();
            this.textBoxTil = new System.Windows.Forms.TextBox();
            this.linkLabelFra = new System.Windows.Forms.LinkLabel();
            this.textBoxFra = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonCompare = new System.Windows.Forms.Button();
            this.groupBoxRename.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOpenDirD
            // 
            this.buttonOpenDirD.Location = new System.Drawing.Point(636, 34);
            this.buttonOpenDirD.Name = "buttonOpenDirD";
            this.buttonOpenDirD.Size = new System.Drawing.Size(25, 23);
            this.buttonOpenDirD.TabIndex = 24;
            this.buttonOpenDirD.Text = "Let opp";
            this.buttonOpenDirD.UseVisualStyleBackColor = true;
            this.buttonOpenDirD.Click += new System.EventHandler(this.buttonOpenDir_Click);
            // 
            // buttonOpenDirS
            // 
            this.buttonOpenDirS.Location = new System.Drawing.Point(636, 8);
            this.buttonOpenDirS.Name = "buttonOpenDirS";
            this.buttonOpenDirS.Size = new System.Drawing.Size(25, 23);
            this.buttonOpenDirS.TabIndex = 23;
            this.buttonOpenDirS.Text = "Let opp";
            this.buttonOpenDirS.UseVisualStyleBackColor = true;
            this.buttonOpenDirS.Click += new System.EventHandler(this.buttonOpenDir_Click);
            // 
            // buttonDestination
            // 
            this.buttonDestination.Location = new System.Drawing.Point(663, 34);
            this.buttonDestination.Name = "buttonDestination";
            this.buttonDestination.Size = new System.Drawing.Size(75, 23);
            this.buttonDestination.TabIndex = 22;
            this.buttonDestination.Text = "Let opp";
            this.buttonDestination.UseVisualStyleBackColor = true;
            this.buttonDestination.Click += new System.EventHandler(this.button_Click);
            // 
            // textBoxDestination
            // 
            this.textBoxDestination.Location = new System.Drawing.Point(12, 38);
            this.textBoxDestination.Name = "textBoxDestination";
            this.textBoxDestination.Size = new System.Drawing.Size(622, 20);
            this.textBoxDestination.TabIndex = 21;
            this.textBoxDestination.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // buttonSource
            // 
            this.buttonSource.Location = new System.Drawing.Point(663, 8);
            this.buttonSource.Name = "buttonSource";
            this.buttonSource.Size = new System.Drawing.Size(75, 23);
            this.buttonSource.TabIndex = 20;
            this.buttonSource.Text = "Let opp";
            this.buttonSource.UseVisualStyleBackColor = true;
            this.buttonSource.Click += new System.EventHandler(this.button_Click);
            // 
            // textBoxSource
            // 
            this.textBoxSource.Location = new System.Drawing.Point(12, 12);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.Size = new System.Drawing.Size(622, 20);
            this.textBoxSource.TabIndex = 19;
            this.textBoxSource.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // groupBoxRename
            // 
            this.groupBoxRename.Controls.Add(this.buttonCompare);
            this.groupBoxRename.Controls.Add(this.progressBar1);
            this.groupBoxRename.Controls.Add(this.buttonExecute);
            this.groupBoxRename.Controls.Add(this.buttonPreview);
            this.groupBoxRename.Controls.Add(this.linkLabelTil);
            this.groupBoxRename.Controls.Add(this.textBoxTil);
            this.groupBoxRename.Controls.Add(this.linkLabelFra);
            this.groupBoxRename.Controls.Add(this.textBoxFra);
            this.groupBoxRename.Location = new System.Drawing.Point(13, 65);
            this.groupBoxRename.Name = "groupBoxRename";
            this.groupBoxRename.Size = new System.Drawing.Size(725, 73);
            this.groupBoxRename.TabIndex = 25;
            this.groupBoxRename.TabStop = false;
            this.groupBoxRename.Text = "Gi nytt navn";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(421, 51);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(183, 13);
            this.progressBar1.TabIndex = 6;
            // 
            // buttonExecute
            // 
            this.buttonExecute.Location = new System.Drawing.Point(421, 19);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(183, 28);
            this.buttonExecute.TabIndex = 5;
            this.buttonExecute.Text = "Utfør";
            this.buttonExecute.UseVisualStyleBackColor = true;
            this.buttonExecute.Click += new System.EventHandler(this.buttonExecute_Click);
            // 
            // buttonPreview
            // 
            this.buttonPreview.Location = new System.Drawing.Point(231, 19);
            this.buttonPreview.Name = "buttonPreview";
            this.buttonPreview.Size = new System.Drawing.Size(183, 45);
            this.buttonPreview.TabIndex = 4;
            this.buttonPreview.Text = "Forhåndsvis";
            this.buttonPreview.UseVisualStyleBackColor = true;
            this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
            // 
            // linkLabelTil
            // 
            this.linkLabelTil.AutoSize = true;
            this.linkLabelTil.Location = new System.Drawing.Point(7, 51);
            this.linkLabelTil.Name = "linkLabelTil";
            this.linkLabelTil.Size = new System.Drawing.Size(18, 13);
            this.linkLabelTil.TabIndex = 3;
            this.linkLabelTil.TabStop = true;
            this.linkLabelTil.Text = "Til";
            // 
            // textBoxTil
            // 
            this.textBoxTil.Location = new System.Drawing.Point(35, 45);
            this.textBoxTil.Name = "textBoxTil";
            this.textBoxTil.Size = new System.Drawing.Size(190, 20);
            this.textBoxTil.TabIndex = 2;
            this.textBoxTil.TextChanged += new System.EventHandler(this.textBoxRename_TextChanged);
            // 
            // linkLabelFra
            // 
            this.linkLabelFra.AutoSize = true;
            this.linkLabelFra.Location = new System.Drawing.Point(7, 25);
            this.linkLabelFra.Name = "linkLabelFra";
            this.linkLabelFra.Size = new System.Drawing.Size(22, 13);
            this.linkLabelFra.TabIndex = 1;
            this.linkLabelFra.TabStop = true;
            this.linkLabelFra.Text = "Fra";
            // 
            // textBoxFra
            // 
            this.textBoxFra.Location = new System.Drawing.Point(35, 19);
            this.textBoxFra.Name = "textBoxFra";
            this.textBoxFra.Size = new System.Drawing.Size(190, 20);
            this.textBoxFra.TabIndex = 0;
            this.textBoxFra.TextChanged += new System.EventHandler(this.textBoxRename_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(13, 145);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(728, 308);
            this.panel1.TabIndex = 26;
            // 
            // buttonCompare
            // 
            this.buttonCompare.Location = new System.Drawing.Point(623, 19);
            this.buttonCompare.Name = "buttonCompare";
            this.buttonCompare.Size = new System.Drawing.Size(96, 45);
            this.buttonCompare.TabIndex = 7;
            this.buttonCompare.Text = "Compare";
            this.buttonCompare.UseVisualStyleBackColor = true;
            this.buttonCompare.Click += new System.EventHandler(this.buttonCompare_Click);
            // 
            // RenameFilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 450);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBoxRename);
            this.Controls.Add(this.buttonOpenDirD);
            this.Controls.Add(this.buttonOpenDirS);
            this.Controls.Add(this.buttonDestination);
            this.Controls.Add(this.textBoxDestination);
            this.Controls.Add(this.buttonSource);
            this.Controls.Add(this.textBoxSource);
            this.Name = "RenameFilesForm";
            this.Text = "RenameFilesForm";
            this.groupBoxRename.ResumeLayout(false);
            this.groupBoxRename.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOpenDirD;
        private System.Windows.Forms.Button buttonOpenDirS;
        private System.Windows.Forms.Button buttonDestination;
        private System.Windows.Forms.TextBox textBoxDestination;
        private System.Windows.Forms.Button buttonSource;
        private System.Windows.Forms.TextBox textBoxSource;
        private System.Windows.Forms.GroupBox groupBoxRename;
        private System.Windows.Forms.Button buttonExecute;
        private System.Windows.Forms.Button buttonPreview;
        private System.Windows.Forms.LinkLabel linkLabelTil;
        private System.Windows.Forms.TextBox textBoxTil;
        private System.Windows.Forms.LinkLabel linkLabelFra;
        private System.Windows.Forms.TextBox textBoxFra;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button buttonCompare;
    }
}