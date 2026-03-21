namespace Kolibri.net.SilverScreen.IMDBForms
{
    partial class WatchlistForm
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WatchlistForm));
            gridMovies = new DataGridView();
            Image = new DataGridViewImageColumn();
            Title = new DataGridViewTextBoxColumn();
            Plot = new DataGridViewTextBoxColumn();
            imdbRating = new DataGridViewTextBoxColumn();
            Metascore = new DataGridViewTextBoxColumn();
            Genre = new DataGridViewTextBoxColumn();
            Actors = new DataGridViewTextBoxColumn();
            Year = new DataGridViewTextBoxColumn();
            Trailer = new DataGridViewTextBoxColumn();
            Runtime = new DataGridViewTextBoxColumn();
            Poster = new DataGridViewTextBoxColumn();
            Watched = new DataGridViewTextBoxColumn();
            ImdbId = new DataGridViewTextBoxColumn();
            cmsOptions = new ContextMenuStrip(components);
            miMovieDetails = new ToolStripMenuItem();
            miChangeStatus = new ToolStripMenuItem();
            miDeleteMovie = new ToolStripMenuItem();
            bsMovies = new BindingSource(components);
            lblFound = new Label();
            chbWatched = new CheckBox();
            chbNotWatched = new CheckBox();
            tbTitle = new TextBox();
            btnExcel = new Button();
            btnShare = new Button();
            btnWa = new Button();
            btnPrint = new Button();
            btnClipboard = new Button();
            btnLoadWatchlist = new Button();
            openFileDialog1 = new OpenFileDialog();
            btnChart = new Button();
            chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            btnGrid = new Button();
            buttonPlex = new Button();
            ((System.ComponentModel.ISupportInitialize)gridMovies).BeginInit();
            cmsOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bsMovies).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chart).BeginInit();
            SuspendLayout();
            // 
            // gridMovies
            // 
            gridMovies.AllowUserToAddRows = false;
            gridMovies.AllowUserToDeleteRows = false;
            gridMovies.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridMovies.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 238);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            gridMovies.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            gridMovies.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridMovies.Columns.AddRange(new DataGridViewColumn[] { Image, Title, Plot, imdbRating, Metascore, Genre, Actors, Year, Trailer, Runtime, Poster, Watched, ImdbId });
            gridMovies.ContextMenuStrip = cmsOptions;
            gridMovies.DataSource = bsMovies;
            gridMovies.Location = new Point(14, 35);
            gridMovies.Margin = new Padding(4);
            gridMovies.Name = "gridMovies";
            gridMovies.ReadOnly = true;
            gridMovies.RowHeadersVisible = false;
            gridMovies.RowHeadersWidth = 51;
            gridMovies.RowTemplate.Height = 100;
            gridMovies.Size = new Size(1424, 391);
            gridMovies.TabIndex = 0;
            gridMovies.CellFormatting += gridMovies_CellFormatting;
            gridMovies.CellMouseDoubleClick += gridMovies_CellMouseDoubleClick;
            gridMovies.CellMouseDown += gridMovies_CellMouseDown;
            // 
            // Image
            // 
            Image.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            Image.DataPropertyName = "Image";
            Image.HeaderText = "Image";
            Image.MinimumWidth = 6;
            Image.Name = "Image";
            Image.ReadOnly = true;
            Image.Resizable = DataGridViewTriState.True;
            Image.SortMode = DataGridViewColumnSortMode.Automatic;
            Image.Width = 61;
            // 
            // Title
            // 
            Title.DataPropertyName = "Title";
            Title.HeaderText = "Title";
            Title.MinimumWidth = 6;
            Title.Name = "Title";
            Title.ReadOnly = true;
            Title.Resizable = DataGridViewTriState.True;
            Title.Width = 210;
            // 
            // Plot
            // 
            Plot.DataPropertyName = "Plot";
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            Plot.DefaultCellStyle = dataGridViewCellStyle2;
            Plot.HeaderText = "Plot";
            Plot.MinimumWidth = 6;
            Plot.Name = "Plot";
            Plot.ReadOnly = true;
            Plot.Width = 400;
            // 
            // imdbRating
            // 
            imdbRating.DataPropertyName = "imdbRating";
            imdbRating.HeaderText = "IMDb Rating";
            imdbRating.MinimumWidth = 6;
            imdbRating.Name = "imdbRating";
            imdbRating.ReadOnly = true;
            imdbRating.Width = 50;
            // 
            // Metascore
            // 
            Metascore.DataPropertyName = "Metascore";
            Metascore.HeaderText = "Metascore";
            Metascore.MinimumWidth = 6;
            Metascore.Name = "Metascore";
            Metascore.ReadOnly = true;
            Metascore.Width = 80;
            // 
            // Genre
            // 
            Genre.DataPropertyName = "Genre";
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            Genre.DefaultCellStyle = dataGridViewCellStyle3;
            Genre.HeaderText = "Genre";
            Genre.MinimumWidth = 6;
            Genre.Name = "Genre";
            Genre.ReadOnly = true;
            Genre.Width = 150;
            // 
            // Actors
            // 
            Actors.DataPropertyName = "Actors";
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            Actors.DefaultCellStyle = dataGridViewCellStyle4;
            Actors.HeaderText = "Actors";
            Actors.MinimumWidth = 6;
            Actors.Name = "Actors";
            Actors.ReadOnly = true;
            Actors.Width = 210;
            // 
            // Year
            // 
            Year.DataPropertyName = "Year";
            Year.HeaderText = "Year";
            Year.MinimumWidth = 6;
            Year.Name = "Year";
            Year.ReadOnly = true;
            Year.Width = 50;
            // 
            // Trailer
            // 
            Trailer.DataPropertyName = "Trailer";
            Trailer.HeaderText = "Trailer";
            Trailer.MinimumWidth = 6;
            Trailer.Name = "Trailer";
            Trailer.ReadOnly = true;
            Trailer.Width = 125;
            // 
            // Runtime
            // 
            Runtime.DataPropertyName = "Runtime";
            Runtime.HeaderText = "Runtime";
            Runtime.MinimumWidth = 6;
            Runtime.Name = "Runtime";
            Runtime.ReadOnly = true;
            Runtime.Width = 125;
            // 
            // Poster
            // 
            Poster.DataPropertyName = "Poster";
            Poster.HeaderText = "Poster";
            Poster.MinimumWidth = 6;
            Poster.Name = "Poster";
            Poster.ReadOnly = true;
            Poster.Resizable = DataGridViewTriState.True;
            Poster.SortMode = DataGridViewColumnSortMode.NotSortable;
            Poster.Visible = false;
            Poster.Width = 125;
            // 
            // Watched
            // 
            Watched.DataPropertyName = "Watched";
            Watched.HeaderText = "Watched";
            Watched.MinimumWidth = 6;
            Watched.Name = "Watched";
            Watched.ReadOnly = true;
            Watched.Visible = false;
            Watched.Width = 70;
            // 
            // ImdbId
            // 
            ImdbId.DataPropertyName = "ImdbId";
            ImdbId.HeaderText = "ImdbId";
            ImdbId.MinimumWidth = 6;
            ImdbId.Name = "ImdbId";
            ImdbId.ReadOnly = true;
            ImdbId.Width = 125;
            // 
            // cmsOptions
            // 
            cmsOptions.ImageScalingSize = new Size(20, 20);
            cmsOptions.Items.AddRange(new ToolStripItem[] { miMovieDetails, miChangeStatus, miDeleteMovie });
            cmsOptions.Name = "cmsOptions";
            cmsOptions.Size = new Size(191, 70);
            cmsOptions.Opening += cmsOptions_Opening;
            // 
            // miMovieDetails
            // 
            miMovieDetails.Name = "miMovieDetails";
            miMovieDetails.Size = new Size(190, 22);
            miMovieDetails.Text = "Show Movie Details";
            miMovieDetails.Click += miMovieDetails_Click;
            // 
            // miChangeStatus
            // 
            miChangeStatus.Name = "miChangeStatus";
            miChangeStatus.Size = new Size(190, 22);
            miChangeStatus.Text = "Change to watched";
            miChangeStatus.Click += miChangeStatus_Click;
            // 
            // miDeleteMovie
            // 
            miDeleteMovie.Name = "miDeleteMovie";
            miDeleteMovie.Size = new Size(190, 22);
            miDeleteMovie.Text = "Delete movie from list";
            miDeleteMovie.Click += miDeleteMovie_Click;
            // 
            // bsMovies
            // 
            bsMovies.CurrentChanged += bsMovies_CurrentChanged;
            // 
            // lblFound
            // 
            lblFound.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblFound.AutoSize = true;
            lblFound.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblFound.Location = new Point(1205, 11);
            lblFound.Margin = new Padding(4, 0, 4, 0);
            lblFound.MinimumSize = new Size(234, 15);
            lblFound.Name = "lblFound";
            lblFound.Size = new Size(234, 15);
            lblFound.TabIndex = 68;
            lblFound.Text = "0 / 0";
            lblFound.TextAlign = ContentAlignment.TopRight;
            // 
            // chbWatched
            // 
            chbWatched.AutoSize = true;
            chbWatched.BackColor = Color.LightGreen;
            chbWatched.Location = new Point(14, 7);
            chbWatched.Margin = new Padding(4);
            chbWatched.Name = "chbWatched";
            chbWatched.Size = new Size(73, 19);
            chbWatched.TabIndex = 69;
            chbWatched.Text = "Watched";
            chbWatched.UseVisualStyleBackColor = false;
            chbWatched.CheckedChanged += chbWatched_CheckedChanged;
            // 
            // chbNotWatched
            // 
            chbNotWatched.AutoSize = true;
            chbNotWatched.BackColor = Color.LightPink;
            chbNotWatched.Location = new Point(102, 7);
            chbNotWatched.Margin = new Padding(4);
            chbNotWatched.Name = "chbNotWatched";
            chbNotWatched.Size = new Size(94, 19);
            chbNotWatched.TabIndex = 70;
            chbNotWatched.Text = "Not watched";
            chbNotWatched.UseVisualStyleBackColor = false;
            chbNotWatched.CheckedChanged += chbWatched_CheckedChanged;
            // 
            // tbTitle
            // 
            tbTitle.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            tbTitle.BackColor = Color.LemonChiffon;
            tbTitle.ForeColor = Color.DarkGray;
            tbTitle.Location = new Point(14, 449);
            tbTitle.Margin = new Padding(4);
            tbTitle.MinimumSize = new Size(162, 20);
            tbTitle.Name = "tbTitle";
            tbTitle.Size = new Size(260, 23);
            tbTitle.TabIndex = 71;
            tbTitle.Text = "Title...";
            tbTitle.TextChanged += tbTitle_TextChanged;
            tbTitle.Enter += tbTitle_Enter;
            tbTitle.Leave += tbTitle_Leave;
            // 
            // btnExcel
            // 
            btnExcel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnExcel.Location = new Point(1029, 450);
            btnExcel.Margin = new Padding(4);
            btnExcel.Name = "btnExcel";
            btnExcel.Size = new Size(87, 26);
            btnExcel.TabIndex = 72;
            btnExcel.Text = "Make excel";
            btnExcel.UseVisualStyleBackColor = true;
            btnExcel.Click += btnExcel_Click;
            // 
            // btnShare
            // 
            btnShare.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnShare.Location = new Point(1152, 450);
            btnShare.Margin = new Padding(4);
            btnShare.Name = "btnShare";
            btnShare.Size = new Size(150, 26);
            btnShare.TabIndex = 73;
            btnShare.Text = "Share watchlist to friend";
            btnShare.UseVisualStyleBackColor = true;
            btnShare.Click += btnShare_Click;
            // 
            // btnWa
            // 
            btnWa.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnWa.Location = new Point(1310, 450);
            btnWa.Margin = new Padding(4);
            btnWa.Name = "btnWa";
            btnWa.Size = new Size(129, 26);
            btnWa.TabIndex = 74;
            btnWa.Text = "Send to WhatsApp";
            btnWa.UseVisualStyleBackColor = true;
            btnWa.Click += btnWa_Click;
            // 
            // btnPrint
            // 
            btnPrint.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnPrint.Location = new Point(781, 450);
            btnPrint.Margin = new Padding(4);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(97, 26);
            btnPrint.TabIndex = 75;
            btnPrint.Text = "Print watchlist";
            btnPrint.UseVisualStyleBackColor = true;
            btnPrint.Click += btnPrint_Click;
            // 
            // btnClipboard
            // 
            btnClipboard.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClipboard.Location = new Point(603, 450);
            btnClipboard.Margin = new Padding(4);
            btnClipboard.Name = "btnClipboard";
            btnClipboard.Size = new Size(172, 26);
            btnClipboard.TabIndex = 76;
            btnClipboard.Text = "Copy watchlist to clipboard";
            btnClipboard.UseVisualStyleBackColor = true;
            btnClipboard.Click += btnClipboard_Click;
            // 
            // btnLoadWatchlist
            // 
            btnLoadWatchlist.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnLoadWatchlist.Location = new Point(917, 450);
            btnLoadWatchlist.Margin = new Padding(4);
            btnLoadWatchlist.Name = "btnLoadWatchlist";
            btnLoadWatchlist.Size = new Size(105, 26);
            btnLoadWatchlist.TabIndex = 77;
            btnLoadWatchlist.Text = "Load watchlist";
            btnLoadWatchlist.UseVisualStyleBackColor = true;
            btnLoadWatchlist.Click += btnLoadWatchlist_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnChart
            // 
            btnChart.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnChart.Location = new Point(298, 449);
            btnChart.Margin = new Padding(4);
            btnChart.Name = "btnChart";
            btnChart.Size = new Size(172, 26);
            btnChart.TabIndex = 78;
            btnChart.Text = "Show chart by Rating";
            btnChart.UseVisualStyleBackColor = true;
            btnChart.Click += btnChart_Click;
            // 
            // chart
            // 
            chartArea1.Name = "ChartArea1";
            chart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            chart.Legends.Add(legend1);
            chart.Location = new Point(14, 35);
            chart.Margin = new Padding(4);
            chart.Name = "chart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "IMDb Rating";
            chart.Series.Add(series1);
            chart.Size = new Size(1424, 391);
            chart.TabIndex = 79;
            chart.Text = "chart1";
            title1.Name = "Title";
            title1.Text = "Best movies by rating";
            chart.Titles.Add(title1);
            // 
            // btnGrid
            // 
            btnGrid.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnGrid.Location = new Point(298, 450);
            btnGrid.Margin = new Padding(4);
            btnGrid.Name = "btnGrid";
            btnGrid.Size = new Size(172, 26);
            btnGrid.TabIndex = 80;
            btnGrid.Text = "Back to watchlist";
            btnGrid.UseVisualStyleBackColor = true;
            btnGrid.Click += btnGrid_Click;
            // 
            // buttonPlex
            // 
            buttonPlex.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonPlex.Location = new Point(498, 451);
            buttonPlex.Margin = new Padding(4);
            buttonPlex.Name = "buttonPlex";
            buttonPlex.Size = new Size(97, 26);
            buttonPlex.TabIndex = 81;
            buttonPlex.Text = "Copy to Plex";
            buttonPlex.UseVisualStyleBackColor = true;
            buttonPlex.Click += buttonPlex_Click;
            // 
            // WatchlistForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1452, 490);
            Controls.Add(buttonPlex);
            Controls.Add(btnLoadWatchlist);
            Controls.Add(btnClipboard);
            Controls.Add(btnPrint);
            Controls.Add(btnWa);
            Controls.Add(btnShare);
            Controls.Add(btnExcel);
            Controls.Add(tbTitle);
            Controls.Add(chbNotWatched);
            Controls.Add(chbWatched);
            Controls.Add(lblFound);
            Controls.Add(btnChart);
            Controls.Add(btnGrid);
            Controls.Add(gridMovies);
            Controls.Add(chart);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Margin = new Padding(4);
            MinimumSize = new Size(1468, 527);
            Name = "WatchlistForm";
            Text = "Watchlist";
            Load += WatchlistForm_Load;
            KeyDown += WatchlistForm_KeyDown;
            ((System.ComponentModel.ISupportInitialize)gridMovies).EndInit();
            cmsOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)bsMovies).EndInit();
            ((System.ComponentModel.ISupportInitialize)chart).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gridMovies;
        private System.Windows.Forms.BindingSource bsMovies;
        private System.Windows.Forms.ContextMenuStrip cmsOptions;
        private System.Windows.Forms.ToolStripMenuItem miMovieDetails;
        private System.Windows.Forms.ToolStripMenuItem miChangeStatus;
        private System.Windows.Forms.ToolStripMenuItem miDeleteMovie;
        private System.Windows.Forms.Label lblFound;
        private System.Windows.Forms.CheckBox chbWatched;
        private System.Windows.Forms.CheckBox chbNotWatched;
        private System.Windows.Forms.TextBox tbTitle;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Button btnShare;
        private System.Windows.Forms.Button btnWa;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClipboard;
        private System.Windows.Forms.Button btnLoadWatchlist;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.Button btnGrid;
        private System.Windows.Forms.DataGridViewImageColumn Image;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title;
        private System.Windows.Forms.DataGridViewTextBoxColumn Plot;
        private System.Windows.Forms.DataGridViewTextBoxColumn imdbRating;
        private System.Windows.Forms.DataGridViewTextBoxColumn Metascore;
        private System.Windows.Forms.DataGridViewTextBoxColumn Genre;
        private System.Windows.Forms.DataGridViewTextBoxColumn Actors;
        private System.Windows.Forms.DataGridViewTextBoxColumn Year;
        private System.Windows.Forms.DataGridViewTextBoxColumn Trailer;
        private System.Windows.Forms.DataGridViewTextBoxColumn Runtime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Poster;
        private System.Windows.Forms.DataGridViewTextBoxColumn Watched;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImdbId;
        private Button buttonPlex;
    }
}