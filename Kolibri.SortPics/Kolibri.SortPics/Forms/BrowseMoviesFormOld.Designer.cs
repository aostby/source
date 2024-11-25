
namespace SortPics.Forms
{
    partial class BrowseMoviesFormOld
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
            this.radioButtonYear = new System.Windows.Forms.RadioButton();
            this.radioButtonRating = new System.Windows.Forms.RadioButton();
            this.checkBoxDecending = new System.Windows.Forms.CheckBox();
            this.linkLabelYear = new System.Windows.Forms.LinkLabel();
            this.linkLabelGenre = new System.Windows.Forms.LinkLabel();
            this.comboBoxYear = new System.Windows.Forms.ComboBox();
            this.comboBoxGenre = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonSearch = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.radioButtonYear);
            this.splitContainer1.Panel1.Controls.Add(this.radioButtonRating);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxDecending);
            this.splitContainer1.Panel1.Controls.Add(this.linkLabelYear);
            this.splitContainer1.Panel1.Controls.Add(this.linkLabelGenre);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxYear);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxGenre);
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            this.splitContainer1.Panel1.Controls.Add(this.buttonSearch);
            this.splitContainer1.Size = new System.Drawing.Size(1168, 693);
            this.splitContainer1.SplitterDistance = 184;
            this.splitContainer1.TabIndex = 0;
            // 
            // radioButtonYear
            // 
            this.radioButtonYear.AutoSize = true;
            this.radioButtonYear.Location = new System.Drawing.Point(299, 64);
            this.radioButtonYear.Name = "radioButtonYear";
            this.radioButtonYear.Size = new System.Drawing.Size(47, 17);
            this.radioButtonYear.TabIndex = 8;
            this.radioButtonYear.Text = "Year";
            this.radioButtonYear.UseVisualStyleBackColor = true;
            // 
            // radioButtonRating
            // 
            this.radioButtonRating.AutoSize = true;
            this.radioButtonRating.Checked = true;
            this.radioButtonRating.Location = new System.Drawing.Point(299, 44);
            this.radioButtonRating.Name = "radioButtonRating";
            this.radioButtonRating.Size = new System.Drawing.Size(56, 17);
            this.radioButtonRating.TabIndex = 7;
            this.radioButtonRating.TabStop = true;
            this.radioButtonRating.Text = "Rating";
            this.radioButtonRating.UseVisualStyleBackColor = true;
            // 
            // checkBoxDecending
            // 
            this.checkBoxDecending.AutoSize = true;
            this.checkBoxDecending.Checked = true;
            this.checkBoxDecending.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDecending.Location = new System.Drawing.Point(299, 87);
            this.checkBoxDecending.Name = "checkBoxDecending";
            this.checkBoxDecending.Size = new System.Drawing.Size(78, 17);
            this.checkBoxDecending.TabIndex = 6;
            this.checkBoxDecending.Text = "Decending";
            this.checkBoxDecending.UseVisualStyleBackColor = true;
            // 
            // linkLabelYear
            // 
            this.linkLabelYear.AutoSize = true;
            this.linkLabelYear.Location = new System.Drawing.Point(36, 73);
            this.linkLabelYear.Name = "linkLabelYear";
            this.linkLabelYear.Size = new System.Drawing.Size(29, 13);
            this.linkLabelYear.TabIndex = 5;
            this.linkLabelYear.TabStop = true;
            this.linkLabelYear.Text = "Year";
            // 
            // linkLabelGenre
            // 
            this.linkLabelGenre.AutoSize = true;
            this.linkLabelGenre.Location = new System.Drawing.Point(36, 46);
            this.linkLabelGenre.Name = "linkLabelGenre";
            this.linkLabelGenre.Size = new System.Drawing.Size(36, 13);
            this.linkLabelGenre.TabIndex = 4;
            this.linkLabelGenre.TabStop = true;
            this.linkLabelGenre.Text = "Genre";
            // 
            // comboBoxYear
            // 
            this.comboBoxYear.FormattingEnabled = true;
            this.comboBoxYear.Location = new System.Drawing.Point(120, 65);
            this.comboBoxYear.Name = "comboBoxYear";
            this.comboBoxYear.Size = new System.Drawing.Size(121, 21);
            this.comboBoxYear.TabIndex = 3;
            // 
            // comboBoxGenre
            // 
            this.comboBoxGenre.FormattingEnabled = true;
            this.comboBoxGenre.Location = new System.Drawing.Point(120, 38);
            this.comboBoxGenre.Name = "comboBoxGenre";
            this.comboBoxGenre.Size = new System.Drawing.Size(121, 21);
            this.comboBoxGenre.TabIndex = 2;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(29, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(247, 20);
            this.textBox1.TabIndex = 1;
            // 
            // buttonSearch
            // 
            this.buttonSearch.Location = new System.Drawing.Point(299, 12);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(75, 23);
            this.buttonSearch.TabIndex = 0;
            this.buttonSearch.Text = "Search";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // BrowseMoviesFormOld
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1168, 693);
            this.Controls.Add(this.splitContainer1);
            this.Name = "BrowseMoviesFormOld";
            this.Text = "Browse for Movies";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BrowseMoviesForm_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
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
    }
}