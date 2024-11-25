
namespace SortPics.Forms
{
    partial class BrowseMoviesForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.Visualize = new System.Windows.Forms.GroupBox();
            this.buttonVisualize = new System.Windows.Forms.Button();
            this.groupBoxSearch = new System.Windows.Forms.GroupBox();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.checkBoxDecending = new System.Windows.Forms.CheckBox();
            this.linkLabelOpenInBrowser = new System.Windows.Forms.LinkLabel();
            this.radioButtonRating = new System.Windows.Forms.RadioButton();
            this.linkLabelYear = new System.Windows.Forms.LinkLabel();
            this.radioButtonYear = new System.Windows.Forms.RadioButton();
            this.linkLabelGenre = new System.Windows.Forms.LinkLabel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBoxYear = new System.Windows.Forms.ComboBox();
            this.comboBoxGenre = new System.Windows.Forms.ComboBox();
            this.labelInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.Visualize.SuspendLayout();
            this.groupBoxSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.Visualize);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxSearch);
            this.splitContainer1.Panel1.Controls.Add(this.labelInfo);
            this.splitContainer1.Size = new System.Drawing.Size(934, 584);
            this.splitContainer1.SplitterDistance = 140;
            this.splitContainer1.TabIndex = 0;
            // 
            // Visualize
            // 
            this.Visualize.Controls.Add(this.buttonVisualize);
            this.Visualize.Location = new System.Drawing.Point(369, 13);
            this.Visualize.Name = "Visualize";
            this.Visualize.Size = new System.Drawing.Size(200, 115);
            this.Visualize.TabIndex = 12;
            this.Visualize.TabStop = false;
            this.Visualize.Text = "Visualize";
            // 
            // buttonVisualize
            // 
            this.buttonVisualize.Location = new System.Drawing.Point(7, 20);
            this.buttonVisualize.Name = "buttonVisualize";
            this.buttonVisualize.Size = new System.Drawing.Size(187, 23);
            this.buttonVisualize.TabIndex = 0;
            this.buttonVisualize.Text = "Visualize";
            this.buttonVisualize.UseVisualStyleBackColor = true;
            this.buttonVisualize.Click += new System.EventHandler(this.buttonVisualize_Click);
            // 
            // groupBoxSearch
            // 
            this.groupBoxSearch.Controls.Add(this.buttonSearch);
            this.groupBoxSearch.Controls.Add(this.checkBoxDecending);
            this.groupBoxSearch.Controls.Add(this.linkLabelOpenInBrowser);
            this.groupBoxSearch.Controls.Add(this.radioButtonRating);
            this.groupBoxSearch.Controls.Add(this.linkLabelYear);
            this.groupBoxSearch.Controls.Add(this.radioButtonYear);
            this.groupBoxSearch.Controls.Add(this.linkLabelGenre);
            this.groupBoxSearch.Controls.Add(this.textBox1);
            this.groupBoxSearch.Controls.Add(this.comboBoxYear);
            this.groupBoxSearch.Controls.Add(this.comboBoxGenre);
            this.groupBoxSearch.Location = new System.Drawing.Point(7, 7);
            this.groupBoxSearch.Name = "groupBoxSearch";
            this.groupBoxSearch.Size = new System.Drawing.Size(355, 123);
            this.groupBoxSearch.TabIndex = 11;
            this.groupBoxSearch.TabStop = false;
            this.groupBoxSearch.Text = "Search";
            // 
            // buttonSearch
            // 
            this.buttonSearch.Location = new System.Drawing.Point(270, 13);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(75, 23);
            this.buttonSearch.TabIndex = 0;
            this.buttonSearch.Text = "Search";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // checkBoxDecending
            // 
            this.checkBoxDecending.AutoSize = true;
            this.checkBoxDecending.Checked = true;
            this.checkBoxDecending.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDecending.Location = new System.Drawing.Point(270, 88);
            this.checkBoxDecending.Name = "checkBoxDecending";
            this.checkBoxDecending.Size = new System.Drawing.Size(78, 17);
            this.checkBoxDecending.TabIndex = 6;
            this.checkBoxDecending.Text = "Decending";
            this.checkBoxDecending.UseVisualStyleBackColor = true;
            // 
            // linkLabelOpenInBrowser
            // 
            this.linkLabelOpenInBrowser.AutoSize = true;
            this.linkLabelOpenInBrowser.Location = new System.Drawing.Point(34, 102);
            this.linkLabelOpenInBrowser.Name = "linkLabelOpenInBrowser";
            this.linkLabelOpenInBrowser.Size = new System.Drawing.Size(86, 13);
            this.linkLabelOpenInBrowser.TabIndex = 9;
            this.linkLabelOpenInBrowser.TabStop = true;
            this.linkLabelOpenInBrowser.Text = "Open In Browser";
            this.linkLabelOpenInBrowser.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelOpenInBrowser_LinkClicked);
            // 
            // radioButtonRating
            // 
            this.radioButtonRating.AutoSize = true;
            this.radioButtonRating.Checked = true;
            this.radioButtonRating.Location = new System.Drawing.Point(270, 45);
            this.radioButtonRating.Name = "radioButtonRating";
            this.radioButtonRating.Size = new System.Drawing.Size(56, 17);
            this.radioButtonRating.TabIndex = 7;
            this.radioButtonRating.TabStop = true;
            this.radioButtonRating.Text = "Rating";
            this.radioButtonRating.UseVisualStyleBackColor = true;
            // 
            // linkLabelYear
            // 
            this.linkLabelYear.AutoSize = true;
            this.linkLabelYear.Location = new System.Drawing.Point(34, 78);
            this.linkLabelYear.Name = "linkLabelYear";
            this.linkLabelYear.Size = new System.Drawing.Size(29, 13);
            this.linkLabelYear.TabIndex = 5;
            this.linkLabelYear.TabStop = true;
            this.linkLabelYear.Text = "Year";
            // 
            // radioButtonYear
            // 
            this.radioButtonYear.AutoSize = true;
            this.radioButtonYear.Location = new System.Drawing.Point(270, 65);
            this.radioButtonYear.Name = "radioButtonYear";
            this.radioButtonYear.Size = new System.Drawing.Size(47, 17);
            this.radioButtonYear.TabIndex = 8;
            this.radioButtonYear.Text = "Year";
            this.radioButtonYear.UseVisualStyleBackColor = true;
            // 
            // linkLabelGenre
            // 
            this.linkLabelGenre.AutoSize = true;
            this.linkLabelGenre.Location = new System.Drawing.Point(34, 51);
            this.linkLabelGenre.Name = "linkLabelGenre";
            this.linkLabelGenre.Size = new System.Drawing.Size(36, 13);
            this.linkLabelGenre.TabIndex = 4;
            this.linkLabelGenre.TabStop = true;
            this.linkLabelGenre.Text = "Genre";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(27, 17);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(212, 20);
            this.textBox1.TabIndex = 1;
            // 
            // comboBoxYear
            // 
            this.comboBoxYear.FormattingEnabled = true;
            this.comboBoxYear.Location = new System.Drawing.Point(118, 70);
            this.comboBoxYear.Name = "comboBoxYear";
            this.comboBoxYear.Size = new System.Drawing.Size(121, 21);
            this.comboBoxYear.TabIndex = 3;
            // 
            // comboBoxGenre
            // 
            this.comboBoxGenre.FormattingEnabled = true;
            this.comboBoxGenre.Location = new System.Drawing.Point(118, 43);
            this.comboBoxGenre.Name = "comboBoxGenre";
            this.comboBoxGenre.Size = new System.Drawing.Size(121, 21);
            this.comboBoxGenre.TabIndex = 2;
            // 
            // labelInfo
            // 
            this.labelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelInfo.Font = new System.Drawing.Font("Marlett", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelInfo.ForeColor = System.Drawing.Color.Blue;
            this.labelInfo.Location = new System.Drawing.Point(575, 15);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(347, 106);
            this.labelInfo.TabIndex = 10;
            this.labelInfo.Text = "Search for a database item.";
            // 
            // BrowseMoviesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 584);
            this.Controls.Add(this.splitContainer1);
            this.Name = "BrowseMoviesForm";
            this.Text = "Browse for Movies";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BrowseMoviesForm_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.Visualize.ResumeLayout(false);
            this.groupBoxSearch.ResumeLayout(false);
            this.groupBoxSearch.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox comboBoxGenre;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.RadioButton radioButtonYear;
        private System.Windows.Forms.RadioButton radioButtonRating;
        private System.Windows.Forms.CheckBox checkBoxDecending;
        private System.Windows.Forms.LinkLabel linkLabelYear;
        private System.Windows.Forms.LinkLabel linkLabelGenre;
        private System.Windows.Forms.ComboBox comboBoxYear;
        private System.Windows.Forms.LinkLabel linkLabelOpenInBrowser;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.GroupBox Visualize;
        private System.Windows.Forms.Button buttonVisualize;
        private System.Windows.Forms.GroupBox groupBoxSearch;
    }
}