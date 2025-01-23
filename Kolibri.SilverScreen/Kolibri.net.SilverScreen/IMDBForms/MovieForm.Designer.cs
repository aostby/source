using javax.swing.text;
using Kolibri.net.Common.Utilities;

namespace Kolibri.net.SilverScreen.IMDBForms
{
    partial class MovieForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MovieForm));
            tbSearch = new TextBox();
            btnSearch = new Button();
            tbTitle = new TextBox();
            tbYear = new TextBox();
            tbRated = new TextBox();
            tbRuntime = new TextBox();
            tbGenre = new TextBox();
            tbActors = new TextBox();
            tbPlot = new TextBox();
            tbMetascore = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            btnAddToWatchlist = new Button();
            btnWatchList = new Button();
            brnRefresh = new Button();
            groupBox1 = new GroupBox();
            buttonNewList = new Button();
            labelWatchListName = new Label();
            comboBox1 = new ComboBox();
            tbYearParameter = new TextBox();
            label10 = new Label();
            btnTop100 = new Button();
            btnRecommend = new Button();
            pictureBox1 = new PictureBox();
            pbPoster = new PictureBox();
            linkTrailer = new LinkLabel();
            labelImdbId = new Label();
            labelImdbRating = new Label();
            linkLabelOpenFilePath = new LinkLabel();
            toolTip1 = new ToolTip(components);
            buttonUpdate = new Button();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbPoster).BeginInit();
            SuspendLayout();
            // 
            // tbSearch
            // 
            tbSearch.Location = new Point(154, 15);
            tbSearch.Margin = new Padding(4);
            tbSearch.Name = "tbSearch";
            tbSearch.Size = new Size(392, 23);
            tbSearch.TabIndex = 1;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(362, 46);
            btnSearch.Margin = new Padding(4);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(184, 26);
            btnSearch.TabIndex = 3;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // tbTitle
            // 
            tbTitle.Location = new Point(92, 133);
            tbTitle.Margin = new Padding(4);
            tbTitle.Name = "tbTitle";
            tbTitle.ReadOnly = true;
            tbTitle.Size = new Size(526, 23);
            tbTitle.TabIndex = 3;
            // 
            // tbYear
            // 
            tbYear.Location = new Point(92, 174);
            tbYear.Margin = new Padding(4);
            tbYear.Name = "tbYear";
            tbYear.ReadOnly = true;
            tbYear.Size = new Size(526, 23);
            tbYear.TabIndex = 4;
            // 
            // tbRated
            // 
            tbRated.Location = new Point(92, 264);
            tbRated.Margin = new Padding(4);
            tbRated.Name = "tbRated";
            tbRated.ReadOnly = true;
            tbRated.Size = new Size(526, 23);
            tbRated.TabIndex = 5;
            // 
            // tbRuntime
            // 
            tbRuntime.Location = new Point(92, 221);
            tbRuntime.Margin = new Padding(4);
            tbRuntime.Name = "tbRuntime";
            tbRuntime.ReadOnly = true;
            tbRuntime.Size = new Size(526, 23);
            tbRuntime.TabIndex = 6;
            // 
            // tbGenre
            // 
            tbGenre.Location = new Point(92, 385);
            tbGenre.Margin = new Padding(4);
            tbGenre.Name = "tbGenre";
            tbGenre.ReadOnly = true;
            tbGenre.Size = new Size(526, 23);
            tbGenre.TabIndex = 7;
            // 
            // tbActors
            // 
            tbActors.Location = new Point(92, 346);
            tbActors.Margin = new Padding(4);
            tbActors.Name = "tbActors";
            tbActors.ReadOnly = true;
            tbActors.Size = new Size(526, 23);
            tbActors.TabIndex = 8;
            // 
            // tbPlot
            // 
            tbPlot.Location = new Point(92, 425);
            tbPlot.Margin = new Padding(4);
            tbPlot.Multiline = true;
            tbPlot.Name = "tbPlot";
            tbPlot.ReadOnly = true;
            tbPlot.Size = new Size(526, 86);
            tbPlot.TabIndex = 10;
            // 
            // tbMetascore
            // 
            tbMetascore.Location = new Point(92, 304);
            tbMetascore.Margin = new Padding(4);
            tbMetascore.Name = "tbMetascore";
            tbMetascore.ReadOnly = true;
            tbMetascore.Size = new Size(526, 23);
            tbMetascore.TabIndex = 11;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(11, 133);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(29, 15);
            label1.TabIndex = 12;
            label1.Text = "Title";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(11, 174);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(29, 15);
            label2.TabIndex = 13;
            label2.Text = "Year";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(11, 264);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(37, 15);
            label3.TabIndex = 14;
            label3.Text = "Rated";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(11, 221);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(52, 15);
            label4.TabIndex = 15;
            label4.Text = "Runtime";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(10, 385);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(38, 15);
            label5.TabIndex = 16;
            label5.Text = "Genre";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(10, 346);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(41, 15);
            label6.TabIndex = 17;
            label6.Text = "Actors";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(10, 304);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(62, 15);
            label7.TabIndex = 18;
            label7.Text = "Metascore";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(10, 425);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(28, 15);
            label8.TabIndex = 19;
            label8.Text = "Plot";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(7, 19);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(134, 15);
            label9.TabIndex = 20;
            label9.Text = "*Enter the movie name :";
            // 
            // btnAddToWatchlist
            // 
            btnAddToWatchlist.Location = new Point(362, 82);
            btnAddToWatchlist.Margin = new Padding(4);
            btnAddToWatchlist.Name = "btnAddToWatchlist";
            btnAddToWatchlist.Size = new Size(48, 26);
            btnAddToWatchlist.TabIndex = 21;
            btnAddToWatchlist.Text = "Add to Watchlist";
            toolTip1.SetToolTip(btnAddToWatchlist, "Add item to WatchList");
            btnAddToWatchlist.UseVisualStyleBackColor = true;
            btnAddToWatchlist.Click += btnAddToWatchlist_Click;
            // 
            // btnWatchList
            // 
            btnWatchList.Location = new Point(422, 82);
            btnWatchList.Margin = new Padding(4);
            btnWatchList.Name = "btnWatchList";
            btnWatchList.Size = new Size(66, 26);
            btnWatchList.TabIndex = 22;
            btnWatchList.Text = "Watchlist";
            toolTip1.SetToolTip(btnWatchList, "View elements in WatchList");
            btnWatchList.UseVisualStyleBackColor = true;
            btnWatchList.Click += btnWatchList_Click;
            // 
            // brnRefresh
            // 
            brnRefresh.Location = new Point(92, 536);
            brnRefresh.Margin = new Padding(4);
            brnRefresh.Name = "brnRefresh";
            brnRefresh.Size = new Size(122, 26);
            brnRefresh.TabIndex = 24;
            brnRefresh.Text = "Refresh";
            brnRefresh.UseVisualStyleBackColor = true;
            brnRefresh.Click += brnRefresh_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(buttonUpdate);
            groupBox1.Controls.Add(buttonNewList);
            groupBox1.Controls.Add(labelWatchListName);
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(tbYearParameter);
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(tbSearch);
            groupBox1.Controls.Add(btnSearch);
            groupBox1.Controls.Add(btnWatchList);
            groupBox1.Controls.Add(btnAddToWatchlist);
            groupBox1.Location = new Point(14, 6);
            groupBox1.Margin = new Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4);
            groupBox1.Size = new Size(605, 120);
            groupBox1.TabIndex = 25;
            groupBox1.TabStop = false;
            // 
            // buttonNewList
            // 
            buttonNewList.Location = new Point(501, 82);
            buttonNewList.Name = "buttonNewList";
            buttonNewList.Size = new Size(45, 26);
            buttonNewList.TabIndex = 24;
            buttonNewList.Text = "New List";
            toolTip1.SetToolTip(buttonNewList, "Create new WatchList");
            buttonNewList.UseVisualStyleBackColor = true;
            buttonNewList.Click += buttonNewList_Click;
            // 
            // labelWatchListName
            // 
            labelWatchListName.AutoSize = true;
            labelWatchListName.Location = new Point(7, 88);
            labelWatchListName.Name = "labelWatchListName";
            labelWatchListName.Size = new Size(97, 15);
            labelWatchListName.TabIndex = 23;
            labelWatchListName.Text = "WatchList Name:";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(154, 84);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(198, 23);
            comboBox1.TabIndex = 22;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // tbYearParameter
            // 
            tbYearParameter.Location = new Point(154, 45);
            tbYearParameter.Margin = new Padding(4);
            tbYearParameter.Name = "tbYearParameter";
            tbYearParameter.Size = new Size(116, 23);
            tbYearParameter.TabIndex = 2;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(7, 45);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(35, 15);
            label10.TabIndex = 21;
            label10.Text = "Year :";
            // 
            // btnTop100
            // 
            btnTop100.Location = new Point(654, 536);
            btnTop100.Margin = new Padding(4);
            btnTop100.Name = "btnTop100";
            btnTop100.Size = new Size(123, 26);
            btnTop100.TabIndex = 26;
            btnTop100.Text = "Top 100 IMDb";
            btnTop100.UseVisualStyleBackColor = true;
            btnTop100.Click += btnTop100_Click;
            // 
            // btnRecommend
            // 
            btnRecommend.Location = new Point(784, 536);
            btnRecommend.Margin = new Padding(4);
            btnRecommend.Name = "btnRecommend";
            btnRecommend.Size = new Size(136, 26);
            btnRecommend.TabIndex = 27;
            btnRecommend.Text = "Recommend movie";
            btnRecommend.UseVisualStyleBackColor = true;
            btnRecommend.Click += btnRecommend_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(672, 14);
            pictureBox1.Margin = new Padding(4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(235, 83);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 23;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // pbPoster
            // 
            pbPoster.Location = new Point(654, 122);
            pbPoster.Margin = new Padding(4);
            pbPoster.Name = "pbPoster";
            pbPoster.Size = new Size(266, 379);
            pbPoster.SizeMode = PictureBoxSizeMode.StretchImage;
            pbPoster.TabIndex = 9;
            pbPoster.TabStop = false;
            // 
            // linkTrailer
            // 
            linkTrailer.AutoSize = true;
            linkTrailer.Location = new Point(660, 510);
            linkTrailer.Margin = new Padding(4, 0, 4, 0);
            linkTrailer.Name = "linkTrailer";
            linkTrailer.Size = new Size(74, 15);
            linkTrailer.TabIndex = 29;
            linkTrailer.TabStop = true;
            linkTrailer.Text = "Watch trailer";
            linkTrailer.LinkClicked += linkTrailer_LinkClicked;
            // 
            // labelImdbId
            // 
            labelImdbId.AutoSize = true;
            labelImdbId.Location = new Point(14, 148);
            labelImdbId.Margin = new Padding(4, 0, 4, 0);
            labelImdbId.Name = "labelImdbId";
            labelImdbId.Size = new Size(45, 15);
            labelImdbId.TabIndex = 30;
            labelImdbId.Text = "ImdbId";
            // 
            // labelImdbRating
            // 
            labelImdbRating.AutoSize = true;
            labelImdbRating.Location = new Point(16, 319);
            labelImdbRating.Margin = new Padding(4, 0, 4, 0);
            labelImdbRating.Name = "labelImdbRating";
            labelImdbRating.Size = new Size(69, 15);
            labelImdbRating.TabIndex = 31;
            labelImdbRating.Text = "ImdbRating";
            // 
            // linkLabelOpenFilePath
            // 
            linkLabelOpenFilePath.AutoSize = true;
            linkLabelOpenFilePath.Location = new Point(738, 510);
            linkLabelOpenFilePath.Margin = new Padding(4, 0, 4, 0);
            linkLabelOpenFilePath.Name = "linkLabelOpenFilePath";
            linkLabelOpenFilePath.Size = new Size(84, 15);
            linkLabelOpenFilePath.TabIndex = 32;
            linkLabelOpenFilePath.TabStop = true;
            linkLabelOpenFilePath.Text = "Open File Path";
            linkLabelOpenFilePath.LinkClicked += linkLabelOpenFilePath_LinkClicked;
            // 
            // buttonUpdate
            // 
            buttonUpdate.Location = new Point(551, 46);
            buttonUpdate.Name = "buttonUpdate";
            buttonUpdate.Size = new Size(47, 26);
            buttonUpdate.TabIndex = 25;
            buttonUpdate.Text = "Update local";
            buttonUpdate.UseVisualStyleBackColor = true;
            buttonUpdate.Visible = false;
            buttonUpdate.Click += buttonUpdate_Click;
            // 
            // MovieForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(934, 577);
            Controls.Add(linkLabelOpenFilePath);
            Controls.Add(labelImdbRating);
            Controls.Add(labelImdbId);
            Controls.Add(linkTrailer);
            Controls.Add(btnRecommend);
            Controls.Add(btnTop100);
            Controls.Add(groupBox1);
            Controls.Add(brnRefresh);
            Controls.Add(pictureBox1);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(tbMetascore);
            Controls.Add(tbPlot);
            Controls.Add(pbPoster);
            Controls.Add(tbActors);
            Controls.Add(tbGenre);
            Controls.Add(tbRuntime);
            Controls.Add(tbRated);
            Controls.Add(tbYear);
            Controls.Add(tbTitle);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Margin = new Padding(4);
            Name = "MovieForm";
            Text = "Search for Movie";
            KeyDown += MovieForm_KeyDown;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbPoster).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.TextBox tbSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox tbTitle;
        private System.Windows.Forms.TextBox tbYear;
        private System.Windows.Forms.TextBox tbRated;
        private System.Windows.Forms.TextBox tbRuntime;
        private System.Windows.Forms.TextBox tbGenre;
        private System.Windows.Forms.TextBox tbActors;
        private System.Windows.Forms.PictureBox pbPoster;
        private System.Windows.Forms.TextBox tbPlot;
        private System.Windows.Forms.TextBox tbMetascore;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnAddToWatchlist;
        private System.Windows.Forms.Button btnWatchList;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button brnRefresh;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbYearParameter;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnTop100;
        private System.Windows.Forms.Button btnRecommend;
        private System.Windows.Forms.LinkLabel linkTrailer;
        private System.Windows.Forms.Label labelImdbId;
        private System.Windows.Forms.Label labelImdbRating;
        private System.Windows.Forms.LinkLabel linkLabelOpenFilePath;
        private ComboBox comboBox1;
        private Label labelWatchListName;
        private ToolTip toolTip1;
        private Button buttonNewList;
        private Button buttonUpdate;
    }
}

