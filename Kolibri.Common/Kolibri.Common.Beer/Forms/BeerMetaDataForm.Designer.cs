namespace Kolibri.Common.Beer.Forms
{
    partial class BeerMetaDataForm
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
            this.buttonDownloadFermentables = new System.Windows.Forms.Button();
            this.buttonVisualizeFermentables = new System.Windows.Forms.Button();
            this.buttonSaveAs = new System.Windows.Forms.Button();
            this.buttonFermentablesManufacturer = new System.Windows.Forms.Button();
            this.buttonFermentablesDistinctValues = new System.Windows.Forms.Button();
            this.buttonDownloadYeasts = new System.Windows.Forms.Button();
            this.buttonDownloadHops = new System.Windows.Forms.Button();
            this.buttonHopsGetTheWeirdos = new System.Windows.Forms.Button();
            this.buttonFixYeast = new System.Windows.Forms.Button();
            this.buttonHopsDescription = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonNameDescription = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBoxHopsWeirdo = new System.Windows.Forms.TextBox();
            this.buttonHopsWeirdo = new System.Windows.Forms.Button();
            this.buttonBRLists = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonDownloadFermentables
            // 
            this.buttonDownloadFermentables.Location = new System.Drawing.Point(24, 2);
            this.buttonDownloadFermentables.Name = "buttonDownloadFermentables";
            this.buttonDownloadFermentables.Size = new System.Drawing.Size(75, 23);
            this.buttonDownloadFermentables.TabIndex = 0;
            this.buttonDownloadFermentables.Text = "Download";
            this.buttonDownloadFermentables.UseVisualStyleBackColor = true;
            this.buttonDownloadFermentables.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // buttonVisualizeFermentables
            // 
            this.buttonVisualizeFermentables.Location = new System.Drawing.Point(127, 1);
            this.buttonVisualizeFermentables.Name = "buttonVisualizeFermentables";
            this.buttonVisualizeFermentables.Size = new System.Drawing.Size(75, 23);
            this.buttonVisualizeFermentables.TabIndex = 1;
            this.buttonVisualizeFermentables.Text = "Visualize";
            this.buttonVisualizeFermentables.UseVisualStyleBackColor = true;
            this.buttonVisualizeFermentables.Click += new System.EventHandler(this.buttonVisualize_Click);
            // 
            // buttonSaveAs
            // 
            this.buttonSaveAs.Location = new System.Drawing.Point(227, 2);
            this.buttonSaveAs.Name = "buttonSaveAs";
            this.buttonSaveAs.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveAs.TabIndex = 2;
            this.buttonSaveAs.Text = "Save as";
            this.buttonSaveAs.UseVisualStyleBackColor = true;
            this.buttonSaveAs.Click += new System.EventHandler(this.buttonSaveAs_Click);
            // 
            // buttonFermentablesManufacturer
            // 
            this.buttonFermentablesManufacturer.Location = new System.Drawing.Point(319, 1);
            this.buttonFermentablesManufacturer.Name = "buttonFermentablesManufacturer";
            this.buttonFermentablesManufacturer.Size = new System.Drawing.Size(191, 23);
            this.buttonFermentablesManufacturer.TabIndex = 3;
            this.buttonFermentablesManufacturer.Text = "Read JSON and create manufacturer";
            this.buttonFermentablesManufacturer.UseVisualStyleBackColor = true;
            this.buttonFermentablesManufacturer.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonFermentablesDistinctValues
            // 
            this.buttonFermentablesDistinctValues.Location = new System.Drawing.Point(516, 2);
            this.buttonFermentablesDistinctValues.Name = "buttonFermentablesDistinctValues";
            this.buttonFermentablesDistinctValues.Size = new System.Drawing.Size(191, 23);
            this.buttonFermentablesDistinctValues.TabIndex = 4;
            this.buttonFermentablesDistinctValues.Text = "Read JSON and find distinct values";
            this.buttonFermentablesDistinctValues.UseVisualStyleBackColor = true;
            this.buttonFermentablesDistinctValues.Click += new System.EventHandler(this.buttonFermentablesDistinctValues_Click);
            // 
            // buttonDownloadYeasts
            // 
            this.buttonDownloadYeasts.Location = new System.Drawing.Point(24, 102);
            this.buttonDownloadYeasts.Name = "buttonDownloadYeasts";
            this.buttonDownloadYeasts.Size = new System.Drawing.Size(75, 23);
            this.buttonDownloadYeasts.TabIndex = 5;
            this.buttonDownloadYeasts.Text = "DL Yeasts";
            this.buttonDownloadYeasts.UseVisualStyleBackColor = true;
            this.buttonDownloadYeasts.Click += new System.EventHandler(this.downloadYeasts_Click);
            // 
            // buttonDownloadHops
            // 
            this.buttonDownloadHops.Location = new System.Drawing.Point(27, 49);
            this.buttonDownloadHops.Name = "buttonDownloadHops";
            this.buttonDownloadHops.Size = new System.Drawing.Size(75, 23);
            this.buttonDownloadHops.TabIndex = 6;
            this.buttonDownloadHops.Text = "DL Hops";
            this.buttonDownloadHops.UseVisualStyleBackColor = true;
            this.buttonDownloadHops.Click += new System.EventHandler(this.buttonDownloadHops_Click);
            // 
            // buttonHopsGetTheWeirdos
            // 
            this.buttonHopsGetTheWeirdos.Location = new System.Drawing.Point(127, 49);
            this.buttonHopsGetTheWeirdos.Name = "buttonHopsGetTheWeirdos";
            this.buttonHopsGetTheWeirdos.Size = new System.Drawing.Size(75, 23);
            this.buttonHopsGetTheWeirdos.TabIndex = 7;
            this.buttonHopsGetTheWeirdos.Text = "HopsWeirdos";
            this.buttonHopsGetTheWeirdos.UseVisualStyleBackColor = true;
            this.buttonHopsGetTheWeirdos.Click += new System.EventHandler(this.buttonHopsGetTheWeirdos_Click);
            // 
            // buttonFixYeast
            // 
            this.buttonFixYeast.Location = new System.Drawing.Point(127, 102);
            this.buttonFixYeast.Name = "buttonFixYeast";
            this.buttonFixYeast.Size = new System.Drawing.Size(75, 23);
            this.buttonFixYeast.TabIndex = 8;
            this.buttonFixYeast.Text = "FixYeastList";
            this.buttonFixYeast.UseVisualStyleBackColor = true;
            this.buttonFixYeast.Click += new System.EventHandler(this.buttonFixYeast_Click);
            // 
            // buttonHopsDescription
            // 
            this.buttonHopsDescription.Location = new System.Drawing.Point(254, 48);
            this.buttonHopsDescription.Name = "buttonHopsDescription";
            this.buttonHopsDescription.Size = new System.Drawing.Size(75, 23);
            this.buttonHopsDescription.TabIndex = 9;
            this.buttonHopsDescription.Text = "Hops Description";
            this.buttonHopsDescription.UseVisualStyleBackColor = true;
            this.buttonHopsDescription.Click += new System.EventHandler(this.buttonHopsDescription_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonNameDescription);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Location = new System.Drawing.Point(27, 176);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(521, 193);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // buttonNameDescription
            // 
            this.buttonNameDescription.Location = new System.Drawing.Point(376, 57);
            this.buttonNameDescription.Name = "buttonNameDescription";
            this.buttonNameDescription.Size = new System.Drawing.Size(119, 23);
            this.buttonNameDescription.TabIndex = 1;
            this.buttonNameDescription.Text = "Name - Description";
            this.buttonNameDescription.UseVisualStyleBackColor = true;
            this.buttonNameDescription.Click += new System.EventHandler(this.buttonNameDescription_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(16, 29);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(479, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // textBoxHopsWeirdo
            // 
            this.textBoxHopsWeirdo.Location = new System.Drawing.Point(371, 51);
            this.textBoxHopsWeirdo.Name = "textBoxHopsWeirdo";
            this.textBoxHopsWeirdo.Size = new System.Drawing.Size(177, 20);
            this.textBoxHopsWeirdo.TabIndex = 11;
            this.textBoxHopsWeirdo.Text = "https://www.hopslist.com/hops/dual-purpose-hops/hueller-bitterer/";
            // 
            // buttonHopsWeirdo
            // 
            this.buttonHopsWeirdo.Location = new System.Drawing.Point(555, 48);
            this.buttonHopsWeirdo.Name = "buttonHopsWeirdo";
            this.buttonHopsWeirdo.Size = new System.Drawing.Size(177, 23);
            this.buttonHopsWeirdo.TabIndex = 12;
            this.buttonHopsWeirdo.Text = "Get Hops Weirdo";
            this.buttonHopsWeirdo.UseVisualStyleBackColor = true;
            this.buttonHopsWeirdo.Click += new System.EventHandler(this.buttonHopsWeirdo_Click);
            // 
            // buttonBRLists
            // 
            this.buttonBRLists.Location = new System.Drawing.Point(24, 147);
            this.buttonBRLists.Name = "buttonBRLists";
            this.buttonBRLists.Size = new System.Drawing.Size(119, 23);
            this.buttonBRLists.TabIndex = 13;
            this.buttonBRLists.Text = "BeerRecipatorLists";
            this.buttonBRLists.UseVisualStyleBackColor = true;
            this.buttonBRLists.Click += new System.EventHandler(this.buttonBRLists_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(150, 147);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "buttonBRList";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonBRlistRead_click);
            // 
            // BeerMetaDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonBRLists);
            this.Controls.Add(this.buttonHopsWeirdo);
            this.Controls.Add(this.textBoxHopsWeirdo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonHopsDescription);
            this.Controls.Add(this.buttonFixYeast);
            this.Controls.Add(this.buttonHopsGetTheWeirdos);
            this.Controls.Add(this.buttonDownloadHops);
            this.Controls.Add(this.buttonDownloadYeasts);
            this.Controls.Add(this.buttonFermentablesDistinctValues);
            this.Controls.Add(this.buttonFermentablesManufacturer);
            this.Controls.Add(this.buttonSaveAs);
            this.Controls.Add(this.buttonVisualizeFermentables);
            this.Controls.Add(this.buttonDownloadFermentables);
            this.Name = "BeerMetaDataForm";
            this.Text = "Beer MetaData";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonDownloadFermentables;
        private System.Windows.Forms.Button buttonVisualizeFermentables;
        private System.Windows.Forms.Button buttonSaveAs;
        private System.Windows.Forms.Button buttonFermentablesManufacturer;
        private System.Windows.Forms.Button buttonFermentablesDistinctValues;
        private System.Windows.Forms.Button buttonDownloadYeasts;
        private System.Windows.Forms.Button buttonDownloadHops;
        private System.Windows.Forms.Button buttonHopsGetTheWeirdos;
        private System.Windows.Forms.Button buttonFixYeast;
        private System.Windows.Forms.Button buttonHopsDescription;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonNameDescription;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox textBoxHopsWeirdo;
        private System.Windows.Forms.Button buttonHopsWeirdo;
        private System.Windows.Forms.Button buttonBRLists;
        private System.Windows.Forms.Button button1;
    }
}