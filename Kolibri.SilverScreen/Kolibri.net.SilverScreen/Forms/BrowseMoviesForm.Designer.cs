
namespace Kolibri.net.Common.MovieAPI.Forms
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
            splitContainer1 = new SplitContainer();
            panelFound = new Panel();
            Visualize = new GroupBox();
            buttonVisualize = new Button();
            groupBoxSearch = new GroupBox();
            buttonSearch = new Button();
            checkBoxDecending = new CheckBox();
            linkLabelOpenInBrowser = new LinkLabel();
            radioButtonRating = new RadioButton();
            linkLabelYear = new LinkLabel();
            radioButtonYear = new RadioButton();
            linkLabelGenre = new LinkLabel();
            tbSearch = new TextBox();
            comboBoxYear = new ComboBox();
            comboBoxGenre = new ComboBox();
            labelInfo = new Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.SuspendLayout();
            Visualize.SuspendLayout();
            groupBoxSearch.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(panelFound);
            splitContainer1.Panel1.Controls.Add(Visualize);
            splitContainer1.Panel1.Controls.Add(groupBoxSearch);
            splitContainer1.Panel1.Controls.Add(labelInfo);
            splitContainer1.Size = new Size(1211, 674);
            splitContainer1.SplitterDistance = 161;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 0;
            // 
            // panelFound
            // 
            panelFound.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelFound.AutoScroll = true;
            panelFound.Location = new Point(679, 58);
            panelFound.Name = "panelFound";
            panelFound.Size = new Size(518, 90);
            panelFound.TabIndex = 13;
            // 
            // Visualize
            // 
            Visualize.Controls.Add(buttonVisualize);
            Visualize.Location = new Point(430, 15);
            Visualize.Margin = new Padding(4, 3, 4, 3);
            Visualize.Name = "Visualize";
            Visualize.Padding = new Padding(4, 3, 4, 3);
            Visualize.Size = new Size(233, 133);
            Visualize.TabIndex = 12;
            Visualize.TabStop = false;
            Visualize.Text = "Visualize";
            // 
            // buttonVisualize
            // 
            buttonVisualize.Location = new Point(8, 23);
            buttonVisualize.Margin = new Padding(4, 3, 4, 3);
            buttonVisualize.Name = "buttonVisualize";
            buttonVisualize.Size = new Size(218, 27);
            buttonVisualize.TabIndex = 0;
            buttonVisualize.Text = "Visualize";
            buttonVisualize.UseVisualStyleBackColor = true;
            buttonVisualize.Click += buttonVisualize_Click;
            // 
            // groupBoxSearch
            // 
            groupBoxSearch.Controls.Add(buttonSearch);
            groupBoxSearch.Controls.Add(checkBoxDecending);
            groupBoxSearch.Controls.Add(linkLabelOpenInBrowser);
            groupBoxSearch.Controls.Add(radioButtonRating);
            groupBoxSearch.Controls.Add(linkLabelYear);
            groupBoxSearch.Controls.Add(radioButtonYear);
            groupBoxSearch.Controls.Add(linkLabelGenre);
            groupBoxSearch.Controls.Add(tbSearch);
            groupBoxSearch.Controls.Add(comboBoxYear);
            groupBoxSearch.Controls.Add(comboBoxGenre);
            groupBoxSearch.Location = new Point(8, 8);
            groupBoxSearch.Margin = new Padding(4, 3, 4, 3);
            groupBoxSearch.Name = "groupBoxSearch";
            groupBoxSearch.Padding = new Padding(4, 3, 4, 3);
            groupBoxSearch.Size = new Size(414, 142);
            groupBoxSearch.TabIndex = 11;
            groupBoxSearch.TabStop = false;
            groupBoxSearch.Text = "Search";
            // 
            // buttonSearch
            // 
            buttonSearch.Location = new Point(315, 15);
            buttonSearch.Margin = new Padding(4, 3, 4, 3);
            buttonSearch.Name = "buttonSearch";
            buttonSearch.Size = new Size(88, 27);
            buttonSearch.TabIndex = 0;
            buttonSearch.Text = "Search";
            buttonSearch.UseVisualStyleBackColor = true;
            buttonSearch.Click += buttonSearch_Click;
            // 
            // checkBoxDecending
            // 
            checkBoxDecending.AutoSize = true;
            checkBoxDecending.Checked = true;
            checkBoxDecending.CheckState = CheckState.Checked;
            checkBoxDecending.Location = new Point(315, 102);
            checkBoxDecending.Margin = new Padding(4, 3, 4, 3);
            checkBoxDecending.Name = "checkBoxDecending";
            checkBoxDecending.Size = new Size(83, 19);
            checkBoxDecending.TabIndex = 6;
            checkBoxDecending.Text = "Decending";
            checkBoxDecending.UseVisualStyleBackColor = true;
            // 
            // linkLabelOpenInBrowser
            // 
            linkLabelOpenInBrowser.AutoSize = true;
            linkLabelOpenInBrowser.Location = new Point(40, 118);
            linkLabelOpenInBrowser.Margin = new Padding(4, 0, 4, 0);
            linkLabelOpenInBrowser.Name = "linkLabelOpenInBrowser";
            linkLabelOpenInBrowser.Size = new Size(94, 15);
            linkLabelOpenInBrowser.TabIndex = 9;
            linkLabelOpenInBrowser.TabStop = true;
            linkLabelOpenInBrowser.Text = "Open In Browser";
            linkLabelOpenInBrowser.LinkClicked += linkLabelOpenInBrowser_LinkClicked;
            // 
            // radioButtonRating
            // 
            radioButtonRating.AutoSize = true;
            radioButtonRating.Checked = true;
            radioButtonRating.Location = new Point(315, 52);
            radioButtonRating.Margin = new Padding(4, 3, 4, 3);
            radioButtonRating.Name = "radioButtonRating";
            radioButtonRating.Size = new Size(59, 19);
            radioButtonRating.TabIndex = 7;
            radioButtonRating.TabStop = true;
            radioButtonRating.Text = "Rating";
            radioButtonRating.UseVisualStyleBackColor = true;
            // 
            // linkLabelYear
            // 
            linkLabelYear.AutoSize = true;
            linkLabelYear.Location = new Point(40, 90);
            linkLabelYear.Margin = new Padding(4, 0, 4, 0);
            linkLabelYear.Name = "linkLabelYear";
            linkLabelYear.Size = new Size(29, 15);
            linkLabelYear.TabIndex = 5;
            linkLabelYear.TabStop = true;
            linkLabelYear.Text = "Year";
            // 
            // radioButtonYear
            // 
            radioButtonYear.AutoSize = true;
            radioButtonYear.Location = new Point(315, 75);
            radioButtonYear.Margin = new Padding(4, 3, 4, 3);
            radioButtonYear.Name = "radioButtonYear";
            radioButtonYear.Size = new Size(47, 19);
            radioButtonYear.TabIndex = 8;
            radioButtonYear.Text = "Year";
            radioButtonYear.UseVisualStyleBackColor = true;
            // 
            // linkLabelGenre
            // 
            linkLabelGenre.AutoSize = true;
            linkLabelGenre.Location = new Point(40, 59);
            linkLabelGenre.Margin = new Padding(4, 0, 4, 0);
            linkLabelGenre.Name = "linkLabelGenre";
            linkLabelGenre.Size = new Size(38, 15);
            linkLabelGenre.TabIndex = 4;
            linkLabelGenre.TabStop = true;
            linkLabelGenre.Text = "Genre";
            // 
            // tbSearch
            // 
            tbSearch.Location = new Point(31, 20);
            tbSearch.Margin = new Padding(4, 3, 4, 3);
            tbSearch.Name = "tbSearch";
            tbSearch.Size = new Size(247, 23);
            tbSearch.TabIndex = 1;
            // 
            // comboBoxYear
            // 
            comboBoxYear.FormattingEnabled = true;
            comboBoxYear.Location = new Point(138, 81);
            comboBoxYear.Margin = new Padding(4, 3, 4, 3);
            comboBoxYear.Name = "comboBoxYear";
            comboBoxYear.Size = new Size(140, 23);
            comboBoxYear.TabIndex = 3;
            // 
            // comboBoxGenre
            // 
            comboBoxGenre.FormattingEnabled = true;
            comboBoxGenre.Location = new Point(138, 50);
            comboBoxGenre.Margin = new Padding(4, 3, 4, 3);
            comboBoxGenre.Name = "comboBoxGenre";
            comboBoxGenre.Size = new Size(140, 23);
            comboBoxGenre.TabIndex = 2;
            // 
            // labelInfo
            // 
            labelInfo.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            labelInfo.Font = new Font("Marlett", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelInfo.ForeColor = Color.Blue;
            labelInfo.Location = new Point(671, 16);
            labelInfo.Margin = new Padding(4, 0, 4, 0);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(526, 21);
            labelInfo.TabIndex = 10;
            labelInfo.Text = "Search for a database item.";
            // 
            // BrowseMoviesForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1211, 674);
            Controls.Add(splitContainer1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "BrowseMoviesForm";
            Text = "Browse for Movies";
            FormClosing += BrowseMoviesForm_FormClosing;
            splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            Visualize.ResumeLayout(false);
            groupBoxSearch.ResumeLayout(false);
            groupBoxSearch.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox comboBoxGenre;
        private System.Windows.Forms.TextBox tbSearch;
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
        private Panel panelFound;
    }
}