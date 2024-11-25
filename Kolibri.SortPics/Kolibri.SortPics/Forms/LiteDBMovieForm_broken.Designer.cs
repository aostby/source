
namespace SortPics.Forms
{
    partial class LiteDBMovieForm
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
            this.groupBoxSearch = new System.Windows.Forms.GroupBox();
            this.buttonFilter = new System.Windows.Forms.Button();
            this.buttonYear = new System.Windows.Forms.Button();
            this.textBoxYear = new System.Windows.Forms.TextBox();
            this.labelYear = new System.Windows.Forms.Label();
            this.buttonRating = new System.Windows.Forms.Button();
            this.textBoxRating = new System.Windows.Forms.TextBox();
            this.labelRating = new System.Windows.Forms.Label();
            this.buttonTitle = new System.Windows.Forms.Button();
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelGenre = new System.Windows.Forms.Label();
            this.comboBoxGenre = new System.Windows.Forms.ComboBox();
            this.groupBoxShow = new System.Windows.Forms.GroupBox();
            this.groupBoxHTML = new System.Windows.Forms.GroupBox();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.buttonDisplayCurrentHTML = new System.Windows.Forms.Button();
            this.buttonVis = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panelDetails = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBoxSearch.SuspendLayout();
            this.groupBoxShow.SuspendLayout();
            this.groupBoxHTML.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxSearch);
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxShow);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(800, 505);
            this.splitContainer1.SplitterDistance = 145;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBoxSearch
            // 
            this.groupBoxSearch.Controls.Add(this.buttonFilter);
            this.groupBoxSearch.Controls.Add(this.buttonYear);
            this.groupBoxSearch.Controls.Add(this.textBoxYear);
            this.groupBoxSearch.Controls.Add(this.labelYear);
            this.groupBoxSearch.Controls.Add(this.buttonRating);
            this.groupBoxSearch.Controls.Add(this.textBoxRating);
            this.groupBoxSearch.Controls.Add(this.labelRating);
            this.groupBoxSearch.Controls.Add(this.buttonTitle);
            this.groupBoxSearch.Controls.Add(this.textBoxTitle);
            this.groupBoxSearch.Controls.Add(this.labelTitle);
            this.groupBoxSearch.Controls.Add(this.labelGenre);
            this.groupBoxSearch.Controls.Add(this.comboBoxGenre);
            this.groupBoxSearch.Location = new System.Drawing.Point(218, 7);
            this.groupBoxSearch.Name = "groupBoxSearch";
            this.groupBoxSearch.Size = new System.Drawing.Size(570, 133);
            this.groupBoxSearch.TabIndex = 3;
            this.groupBoxSearch.TabStop = false;
            this.groupBoxSearch.Text = "Search by";
            // 
            // buttonFilter
            // 
            this.buttonFilter.Location = new System.Drawing.Point(264, 19);
            this.buttonFilter.Name = "buttonFilter";
            this.buttonFilter.Size = new System.Drawing.Size(255, 92);
            this.buttonFilter.TabIndex = 11;
            this.buttonFilter.Text = "Search by set filter";
            this.buttonFilter.UseVisualStyleBackColor = true;
            this.buttonFilter.Click += new System.EventHandler(this.buttonFilter_Click);
            // 
            // buttonYear
            // 
            this.buttonYear.Location = new System.Drawing.Point(214, 68);
            this.buttonYear.Name = "buttonYear";
            this.buttonYear.Size = new System.Drawing.Size(35, 20);
            this.buttonYear.TabIndex = 10;
            this.buttonYear.Text = "Søk";
            this.buttonYear.UseVisualStyleBackColor = true;
            this.buttonYear.Click += new System.EventHandler(this.buttonYear_Click);
            // 
            // textBoxYear
            // 
            this.textBoxYear.Location = new System.Drawing.Point(48, 68);
            this.textBoxYear.Name = "textBoxYear";
            this.textBoxYear.Size = new System.Drawing.Size(159, 20);
            this.textBoxYear.TabIndex = 9;
            this.textBoxYear.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.textBoxYear.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxYear_KeyPress);
            // 
            // labelYear
            // 
            this.labelYear.AutoSize = true;
            this.labelYear.Location = new System.Drawing.Point(7, 72);
            this.labelYear.Name = "labelYear";
            this.labelYear.Size = new System.Drawing.Size(29, 13);
            this.labelYear.TabIndex = 8;
            this.labelYear.Text = "Year";
            // 
            // buttonRating
            // 
            this.buttonRating.Location = new System.Drawing.Point(214, 91);
            this.buttonRating.Name = "buttonRating";
            this.buttonRating.Size = new System.Drawing.Size(35, 20);
            this.buttonRating.TabIndex = 7;
            this.buttonRating.Text = "Søk";
            this.buttonRating.UseVisualStyleBackColor = true;
            this.buttonRating.Click += new System.EventHandler(this.buttonRating_Click);
            // 
            // textBoxRating
            // 
            this.textBoxRating.Location = new System.Drawing.Point(48, 91);
            this.textBoxRating.Name = "textBoxRating";
            this.textBoxRating.Size = new System.Drawing.Size(159, 20);
            this.textBoxRating.TabIndex = 6;
            this.textBoxRating.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // labelRating
            // 
            this.labelRating.AutoSize = true;
            this.labelRating.Location = new System.Drawing.Point(7, 95);
            this.labelRating.Name = "labelRating";
            this.labelRating.Size = new System.Drawing.Size(38, 13);
            this.labelRating.TabIndex = 5;
            this.labelRating.Text = "Rating";
            // 
            // buttonTitle
            // 
            this.buttonTitle.Location = new System.Drawing.Point(214, 45);
            this.buttonTitle.Name = "buttonTitle";
            this.buttonTitle.Size = new System.Drawing.Size(35, 20);
            this.buttonTitle.TabIndex = 4;
            this.buttonTitle.Text = "Søk";
            this.buttonTitle.UseVisualStyleBackColor = true;
            this.buttonTitle.Click += new System.EventHandler(this.buttonTitle_Click);
            // 
            // textBoxTitle
            // 
            this.textBoxTitle.Location = new System.Drawing.Point(48, 45);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(159, 20);
            this.textBoxTitle.TabIndex = 3;
            this.textBoxTitle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(7, 49);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(27, 13);
            this.labelTitle.TabIndex = 2;
            this.labelTitle.Text = "Title";
            // 
            // labelGenre
            // 
            this.labelGenre.AutoSize = true;
            this.labelGenre.Location = new System.Drawing.Point(6, 25);
            this.labelGenre.Name = "labelGenre";
            this.labelGenre.Size = new System.Drawing.Size(36, 13);
            this.labelGenre.TabIndex = 1;
            this.labelGenre.Text = "Genre";
            // 
            // comboBoxGenre
            // 
            this.comboBoxGenre.FormattingEnabled = true;
            this.comboBoxGenre.Location = new System.Drawing.Point(48, 19);
            this.comboBoxGenre.Name = "comboBoxGenre";
            this.comboBoxGenre.Size = new System.Drawing.Size(201, 21);
            this.comboBoxGenre.TabIndex = 0;
            this.comboBoxGenre.SelectedIndexChanged += new System.EventHandler(this.comboBoxGenre_SelectedIndexChanged);
            // 
            // groupBoxShow
            // 
            this.groupBoxShow.Controls.Add(this.groupBoxHTML);
            this.groupBoxShow.Controls.Add(this.buttonVis);
            this.groupBoxShow.Location = new System.Drawing.Point(12, 7);
            this.groupBoxShow.Name = "groupBoxShow";
            this.groupBoxShow.Size = new System.Drawing.Size(200, 133);
            this.groupBoxShow.TabIndex = 2;
            this.groupBoxShow.TabStop = false;
            this.groupBoxShow.Text = "Vis data i min ODBC";
            // 
            // groupBoxHTML
            // 
            this.groupBoxHTML.Controls.Add(this.buttonPrint);
            this.groupBoxHTML.Controls.Add(this.buttonDisplayCurrentHTML);
            this.groupBoxHTML.Location = new System.Drawing.Point(7, 49);
            this.groupBoxHTML.Name = "groupBoxHTML";
            this.groupBoxHTML.Size = new System.Drawing.Size(183, 62);
            this.groupBoxHTML.TabIndex = 3;
            this.groupBoxHTML.TabStop = false;
            this.groupBoxHTML.Text = "Show as HTML";
            // 
            // buttonPrint
            // 
            this.buttonPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPrint.Location = new System.Drawing.Point(6, 39);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(57, 17);
            this.buttonPrint.TabIndex = 6;
            this.buttonPrint.Text = "Print HTML";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // buttonDisplayCurrentHTML
            // 
            this.buttonDisplayCurrentHTML.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDisplayCurrentHTML.Location = new System.Drawing.Point(6, 19);
            this.buttonDisplayCurrentHTML.Name = "buttonDisplayCurrentHTML";
            this.buttonDisplayCurrentHTML.Size = new System.Drawing.Size(171, 17);
            this.buttonDisplayCurrentHTML.TabIndex = 5;
            this.buttonDisplayCurrentHTML.Text = "Display HTML";
            this.buttonDisplayCurrentHTML.UseVisualStyleBackColor = true;
            this.buttonDisplayCurrentHTML.Click += new System.EventHandler(this.buttonDisplayCurrent_Click);
            // 
            // buttonVis
            // 
            this.buttonVis.Location = new System.Drawing.Point(7, 20);
            this.buttonVis.Name = "buttonVis";
            this.buttonVis.Size = new System.Drawing.Size(183, 23);
            this.buttonVis.TabIndex = 0;
            this.buttonVis.Text = "Vis OMDB data";
            this.buttonVis.UseVisualStyleBackColor = true;
            this.buttonVis.Click += new System.EventHandler(this.buttonVis_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 334);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(150, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabelFilnavn";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panelDetails);
            this.splitContainer2.Size = new System.Drawing.Size(800, 328);
            this.splitContainer2.SplitterDistance = 253;
            this.splitContainer2.TabIndex = 1;
            // 
            // panelDetails
            // 
            this.panelDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDetails.Location = new System.Drawing.Point(12, 3);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Size = new System.Drawing.Size(234, 343);
            this.panelDetails.TabIndex = 4;
            // 
            // LiteDBMovieForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 505);
            this.Controls.Add(this.splitContainer1);
            this.Name = "LiteDBMovieForm";
            this.Text = "OMDBForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LiteDBMovieForm_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBoxSearch.ResumeLayout(false);
            this.groupBoxSearch.PerformLayout();
            this.groupBoxShow.ResumeLayout(false);
            this.groupBoxHTML.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBoxShow;
        private System.Windows.Forms.Button buttonVis;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.GroupBox groupBoxHTML;
        private System.Windows.Forms.GroupBox groupBoxSearch;
        private System.Windows.Forms.ComboBox comboBoxGenre;
        private System.Windows.Forms.Button buttonTitle;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelGenre;
        private System.Windows.Forms.Button buttonDisplayCurrentHTML;
        private System.Windows.Forms.Button buttonRating;
        private System.Windows.Forms.TextBox textBoxRating;
        private System.Windows.Forms.Label labelRating;
        private System.Windows.Forms.Button buttonYear;
        private System.Windows.Forms.TextBox textBoxYear;
        private System.Windows.Forms.Label labelYear;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.Button buttonFilter;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}