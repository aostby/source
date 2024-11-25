namespace Kolibri.Common.Beer.Forms
{
    partial class BeerDescriptionForm
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.showListsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allWithoutDescriptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hopsDescriptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonUpdateDescription = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.hopsFindAllWeirdosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(29, 26);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(217, 21);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(252, 27);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(536, 379);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(29, 56);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(37, 13);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "Status";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Enabled = false;
            this.dataGridView1.Location = new System.Drawing.Point(32, 77);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(214, 361);
            this.dataGridView1.TabIndex = 3;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showListsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // showListsToolStripMenuItem
            // 
            this.showListsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allWithoutDescriptionToolStripMenuItem,
            this.hopsDescriptionsToolStripMenuItem,
            this.hopsFindAllWeirdosToolStripMenuItem});
            this.showListsToolStripMenuItem.Name = "showListsToolStripMenuItem";
            this.showListsToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.showListsToolStripMenuItem.Text = "Show lists";
            // 
            // allWithoutDescriptionToolStripMenuItem
            // 
            this.allWithoutDescriptionToolStripMenuItem.Name = "allWithoutDescriptionToolStripMenuItem";
            this.allWithoutDescriptionToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.allWithoutDescriptionToolStripMenuItem.Text = "All without description";
            this.allWithoutDescriptionToolStripMenuItem.Click += new System.EventHandler(this.allWithoutDescriptionToolStripMenuItem_Click);
            // 
            // hopsDescriptionsToolStripMenuItem
            // 
            this.hopsDescriptionsToolStripMenuItem.Name = "hopsDescriptionsToolStripMenuItem";
            this.hopsDescriptionsToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.hopsDescriptionsToolStripMenuItem.Text = "Hops descriptions";
            this.hopsDescriptionsToolStripMenuItem.Click += new System.EventHandler(this.hopsDescriptionsToolStripMenuItem_Click);
            // 
            // buttonUpdateDescription
            // 
            this.buttonUpdateDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpdateDescription.Location = new System.Drawing.Point(384, 415);
            this.buttonUpdateDescription.Name = "buttonUpdateDescription";
            this.buttonUpdateDescription.Size = new System.Drawing.Size(404, 23);
            this.buttonUpdateDescription.TabIndex = 5;
            this.buttonUpdateDescription.Text = "Update description";
            this.buttonUpdateDescription.UseVisualStyleBackColor = true;
            this.buttonUpdateDescription.Click += new System.EventHandler(this.buttonUpdateDescription_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Enabled = false;
            this.buttonSave.Location = new System.Drawing.Point(253, 415);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(125, 23);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // hopsFindAllWeirdosToolStripMenuItem
            // 
            this.hopsFindAllWeirdosToolStripMenuItem.Name = "hopsFindAllWeirdosToolStripMenuItem";
            this.hopsFindAllWeirdosToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.hopsFindAllWeirdosToolStripMenuItem.Text = "Hops Find All Weirdos";
            this.hopsFindAllWeirdosToolStripMenuItem.Click += new System.EventHandler(this.hopsFindAllWeirdosToolStripMenuItem_Click);
            // 
            // BeerDescriptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonUpdateDescription);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BeerDescriptionForm";
            this.Text = "BeerDescriptionForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showListsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allWithoutDescriptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hopsDescriptionsToolStripMenuItem;
        private System.Windows.Forms.Button buttonUpdateDescription;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.ToolStripMenuItem hopsFindAllWeirdosToolStripMenuItem;
    }
}