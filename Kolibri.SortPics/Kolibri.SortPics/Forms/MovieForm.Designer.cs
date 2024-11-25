namespace MoviesFromImdb
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MovieForm));
            this.tbSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.tbTitle = new System.Windows.Forms.TextBox();
            this.tbYear = new System.Windows.Forms.TextBox();
            this.tbRated = new System.Windows.Forms.TextBox();
            this.tbRuntime = new System.Windows.Forms.TextBox();
            this.tbGenre = new System.Windows.Forms.TextBox();
            this.tbActors = new System.Windows.Forms.TextBox();
            this.tbPlot = new System.Windows.Forms.TextBox();
            this.tbMetascore = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnAddToWatchlist = new System.Windows.Forms.Button();
            this.btnWatchList = new System.Windows.Forms.Button();
            this.brnRefresh = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbYearParameter = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnTop100 = new System.Windows.Forms.Button();
            this.btnRecommend = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pbPoster = new System.Windows.Forms.PictureBox();
            this.linkTrailer = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPoster)).BeginInit();
            this.SuspendLayout();
            // 
            // tbSearch
            // 
            this.tbSearch.Location = new System.Drawing.Point(132, 13);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(337, 20);
            this.tbSearch.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(238, 65);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(105, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // tbTitle
            // 
            this.tbTitle.Location = new System.Drawing.Point(79, 115);
            this.tbTitle.Name = "tbTitle";
            this.tbTitle.ReadOnly = true;
            this.tbTitle.Size = new System.Drawing.Size(451, 20);
            this.tbTitle.TabIndex = 3;
            // 
            // tbYear
            // 
            this.tbYear.Location = new System.Drawing.Point(79, 151);
            this.tbYear.Name = "tbYear";
            this.tbYear.ReadOnly = true;
            this.tbYear.Size = new System.Drawing.Size(451, 20);
            this.tbYear.TabIndex = 4;
            // 
            // tbRated
            // 
            this.tbRated.Location = new System.Drawing.Point(79, 229);
            this.tbRated.Name = "tbRated";
            this.tbRated.ReadOnly = true;
            this.tbRated.Size = new System.Drawing.Size(451, 20);
            this.tbRated.TabIndex = 5;
            // 
            // tbRuntime
            // 
            this.tbRuntime.Location = new System.Drawing.Point(79, 192);
            this.tbRuntime.Name = "tbRuntime";
            this.tbRuntime.ReadOnly = true;
            this.tbRuntime.Size = new System.Drawing.Size(451, 20);
            this.tbRuntime.TabIndex = 6;
            // 
            // tbGenre
            // 
            this.tbGenre.Location = new System.Drawing.Point(79, 334);
            this.tbGenre.Name = "tbGenre";
            this.tbGenre.ReadOnly = true;
            this.tbGenre.Size = new System.Drawing.Size(451, 20);
            this.tbGenre.TabIndex = 7;
            // 
            // tbActors
            // 
            this.tbActors.Location = new System.Drawing.Point(79, 300);
            this.tbActors.Name = "tbActors";
            this.tbActors.ReadOnly = true;
            this.tbActors.Size = new System.Drawing.Size(451, 20);
            this.tbActors.TabIndex = 8;
            // 
            // tbPlot
            // 
            this.tbPlot.Location = new System.Drawing.Point(79, 368);
            this.tbPlot.Multiline = true;
            this.tbPlot.Name = "tbPlot";
            this.tbPlot.ReadOnly = true;
            this.tbPlot.Size = new System.Drawing.Size(451, 75);
            this.tbPlot.TabIndex = 10;
            // 
            // tbMetascore
            // 
            this.tbMetascore.Location = new System.Drawing.Point(79, 263);
            this.tbMetascore.Name = "tbMetascore";
            this.tbMetascore.ReadOnly = true;
            this.tbMetascore.Size = new System.Drawing.Size(451, 20);
            this.tbMetascore.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 115);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Title";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Year";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 229);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Rated";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 192);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Runtime";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 334);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Genre";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 300);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Actors";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 263);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Metascore";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 368);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(25, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Plot";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(120, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "*Enter the movie name :";
            // 
            // btnAddToWatchlist
            // 
            this.btnAddToWatchlist.Location = new System.Drawing.Point(250, 465);
            this.btnAddToWatchlist.Name = "btnAddToWatchlist";
            this.btnAddToWatchlist.Size = new System.Drawing.Size(105, 23);
            this.btnAddToWatchlist.TabIndex = 21;
            this.btnAddToWatchlist.Text = "Add to Watchlist";
            this.btnAddToWatchlist.UseVisualStyleBackColor = true;
            this.btnAddToWatchlist.Click += new System.EventHandler(this.btnAddToWatchlist_Click);
            // 
            // btnWatchList
            // 
            this.btnWatchList.Location = new System.Drawing.Point(425, 465);
            this.btnWatchList.Name = "btnWatchList";
            this.btnWatchList.Size = new System.Drawing.Size(105, 23);
            this.btnWatchList.TabIndex = 22;
            this.btnWatchList.Text = "Watchlist";
            this.btnWatchList.UseVisualStyleBackColor = true;
            this.btnWatchList.Click += new System.EventHandler(this.btnWatchList_Click);
            // 
            // brnRefresh
            // 
            this.brnRefresh.Location = new System.Drawing.Point(79, 465);
            this.brnRefresh.Name = "brnRefresh";
            this.brnRefresh.Size = new System.Drawing.Size(105, 23);
            this.brnRefresh.TabIndex = 24;
            this.brnRefresh.Text = "Refresh";
            this.brnRefresh.UseVisualStyleBackColor = true;
            this.brnRefresh.Click += new System.EventHandler(this.brnRefresh_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbYearParameter);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.tbSearch);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Location = new System.Drawing.Point(12, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(518, 104);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            // 
            // tbYearParameter
            // 
            this.tbYearParameter.Location = new System.Drawing.Point(132, 39);
            this.tbYearParameter.Name = "tbYearParameter";
            this.tbYearParameter.Size = new System.Drawing.Size(100, 20);
            this.tbYearParameter.TabIndex = 22;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 39);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "Year :";
            // 
            // btnTop100
            // 
            this.btnTop100.Location = new System.Drawing.Point(560, 465);
            this.btnTop100.Name = "btnTop100";
            this.btnTop100.Size = new System.Drawing.Size(106, 23);
            this.btnTop100.TabIndex = 26;
            this.btnTop100.Text = "Top 100 IMDb";
            this.btnTop100.UseVisualStyleBackColor = true;
            this.btnTop100.Click += new System.EventHandler(this.btnTop100_Click);
            // 
            // btnRecommend
            // 
            this.btnRecommend.Location = new System.Drawing.Point(672, 465);
            this.btnRecommend.Name = "btnRecommend";
            this.btnRecommend.Size = new System.Drawing.Size(116, 23);
            this.btnRecommend.TabIndex = 27;
            this.btnRecommend.Text = "Recommend movie";
            this.btnRecommend.UseVisualStyleBackColor = true;
            this.btnRecommend.Click += new System.EventHandler(this.btnRecommend_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MoviesFromImdb.Properties.Resources.IMDB_Logo_2016;
            this.pictureBox1.Location = new System.Drawing.Point(576, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(202, 72);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 23;
            this.pictureBox1.TabStop = false;
            // 
            // pbPoster
            // 
            this.pbPoster.Location = new System.Drawing.Point(560, 106);
            this.pbPoster.Name = "pbPoster";
            this.pbPoster.Size = new System.Drawing.Size(228, 328);
            this.pbPoster.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbPoster.TabIndex = 9;
            this.pbPoster.TabStop = false;
            // 
            // linkTrailer
            // 
            this.linkTrailer.AutoSize = true;
            this.linkTrailer.Location = new System.Drawing.Point(643, 442);
            this.linkTrailer.Name = "linkTrailer";
            this.linkTrailer.Size = new System.Drawing.Size(67, 13);
            this.linkTrailer.TabIndex = 29;
            this.linkTrailer.TabStop = true;
            this.linkTrailer.Text = "Watch trailer";
            this.linkTrailer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkTrailer_LinkClicked);
            // 
            // MovieForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.linkTrailer);
            this.Controls.Add(this.btnRecommend);
            this.Controls.Add(this.btnTop100);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.brnRefresh);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnWatchList);
            this.Controls.Add(this.btnAddToWatchlist);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbMetascore);
            this.Controls.Add(this.tbPlot);
            this.Controls.Add(this.pbPoster);
            this.Controls.Add(this.tbActors);
            this.Controls.Add(this.tbGenre);
            this.Controls.Add(this.tbRuntime);
            this.Controls.Add(this.tbRated);
            this.Controls.Add(this.tbYear);
            this.Controls.Add(this.tbTitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MovieForm";
            this.Text = "Search for Movie";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MovieForm_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPoster)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}

