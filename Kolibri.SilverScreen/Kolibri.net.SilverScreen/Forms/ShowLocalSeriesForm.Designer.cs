using Kolibri.net.Common.Utilities;

namespace Kolibri.Common.VisualizeOMDbItem
{
    partial class ShowLocalSeriesForm
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowLocalSeriesForm));
            mainLayout = new TableLayoutPanel();
            headerLayout = new TableLayoutPanel();
            buttonLookUp = new Button();
            searchTxt = new TextBox();
            searchBtn = new Button();
            omdbLabel = new Label();
            leftLayout = new TableLayoutPanel();
            movieList = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            contextMenuStrip1 = new ContextMenuStrip(components);
            toolStripMenuItemDelete = new ToolStripMenuItem();
            toolStripMenuItemSetIMDBTagOnFolder = new ToolStripMenuItem();
            movieImageList = new ImageList(components);
            tableLayoutPanel1 = new TableLayoutPanel();
            buttonEdit = new Button();
            detailsViewBtn = new Button();
            label1 = new Label();
            tileViewBtn = new Button();
            rightLayout = new TableLayoutPanel();
            labelOpen = new Label();
            ratingContentLabel = new Label();
            labelRating = new Label();
            countryContentLabel = new Label();
            directorContentLabel = new Label();
            imdbContentLabel = new Label();
            titleContentLabel = new Label();
            pictureBox = new PictureBox();
            titleLabel = new Label();
            imdbLabel = new Label();
            directorLabel = new Label();
            countryLabel = new Label();
            plotLabel = new Label();
            runtimeLabel = new Label();
            typeLabel = new Label();
            totalLabel = new Label();
            yearLabel = new Label();
            yearContentLabel = new Label();
            totalContentLabel = new Label();
            typeContentLabel = new Label();
            runtimeContentLabel = new Label();
            plotContentLabel = new Label();
            button1 = new Button();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            mainLayout.SuspendLayout();
            headerLayout.SuspendLayout();
            leftLayout.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            rightLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayout
            // 
            mainLayout.BackColor = Color.AliceBlue;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 43.9583321F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56.0416679F));
            mainLayout.Controls.Add(headerLayout, 0, 0);
            mainLayout.Controls.Add(leftLayout, 0, 1);
            mainLayout.Controls.Add(rightLayout, 1, 1);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(0, 0);
            mainLayout.Margin = new Padding(4, 3, 4, 3);
            mainLayout.Name = "mainLayout";
            mainLayout.RowCount = 2;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 69F));
            mainLayout.RowStyles.Add(new RowStyle());
            mainLayout.Size = new Size(1440, 764);
            mainLayout.TabIndex = 1;
            // 
            // headerLayout
            // 
            headerLayout.BackColor = Color.DarkBlue;
            headerLayout.ColumnCount = 4;
            mainLayout.SetColumnSpan(headerLayout, 2);
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350F));
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            headerLayout.Controls.Add(buttonLookUp, 2, 0);
            headerLayout.Controls.Add(searchTxt, 0, 0);
            headerLayout.Controls.Add(searchBtn, 1, 0);
            headerLayout.Controls.Add(omdbLabel, 4, 0);
            headerLayout.Dock = DockStyle.Fill;
            headerLayout.Location = new Point(4, 3);
            headerLayout.Margin = new Padding(4, 3, 4, 3);
            headerLayout.Name = "headerLayout";
            headerLayout.RowCount = 1;
            headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            headerLayout.Size = new Size(1432, 63);
            headerLayout.TabIndex = 1;
            // 
            // buttonLookUp
            // 
            buttonLookUp.BackColor = Color.DarkBlue;
            buttonLookUp.BackgroundImage = (Image)resources.GetObject("buttonLookUp.BackgroundImage");
            buttonLookUp.BackgroundImageLayout = ImageLayout.Stretch;
            buttonLookUp.Cursor = Cursors.Hand;
            buttonLookUp.Dock = DockStyle.Fill;
            buttonLookUp.FlatStyle = FlatStyle.Flat;
            buttonLookUp.ForeColor = Color.DarkBlue;
            buttonLookUp.Location = new Point(422, 2);
            buttonLookUp.Margin = new Padding(2);
            buttonLookUp.Name = "buttonLookUp";
            buttonLookUp.Size = new Size(66, 59);
            buttonLookUp.TabIndex = 3;
            buttonLookUp.UseVisualStyleBackColor = false;
            buttonLookUp.Click += buttonLookUp_Click;
            // 
            // searchTxt
            // 
            searchTxt.Dock = DockStyle.Fill;
            searchTxt.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            searchTxt.Location = new Point(9, 14);
            searchTxt.Margin = new Padding(9, 14, 7, 14);
            searchTxt.Name = "searchTxt";
            searchTxt.Size = new Size(334, 24);
            searchTxt.TabIndex = 0;
            // 
            // searchBtn
            // 
            searchBtn.BackColor = Color.DarkBlue;
            searchBtn.BackgroundImage = (Image)resources.GetObject("searchBtn.BackgroundImage");
            searchBtn.BackgroundImageLayout = ImageLayout.Stretch;
            searchBtn.Cursor = Cursors.Hand;
            searchBtn.Dock = DockStyle.Fill;
            searchBtn.FlatStyle = FlatStyle.Flat;
            searchBtn.ForeColor = Color.DarkBlue;
            searchBtn.Location = new Point(352, 2);
            searchBtn.Margin = new Padding(2);
            searchBtn.Name = "searchBtn";
            searchBtn.Size = new Size(66, 59);
            searchBtn.TabIndex = 1;
            searchBtn.UseVisualStyleBackColor = false;
            searchBtn.Click += searchBtn_Click;
            // 
            // omdbLabel
            // 
            omdbLabel.AutoSize = true;
            omdbLabel.Dock = DockStyle.Fill;
            omdbLabel.Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Italic, GraphicsUnit.Point, 0);
            omdbLabel.ForeColor = Color.White;
            omdbLabel.Location = new Point(494, 0);
            omdbLabel.Margin = new Padding(4, 0, 4, 0);
            omdbLabel.Name = "omdbLabel";
            omdbLabel.Size = new Size(934, 63);
            omdbLabel.TabIndex = 2;
            omdbLabel.Text = "Search local series";
            omdbLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // leftLayout
            // 
            leftLayout.ColumnCount = 1;
            leftLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            leftLayout.Controls.Add(movieList, 0, 0);
            leftLayout.Controls.Add(tableLayoutPanel1, 0, 1);
            leftLayout.Dock = DockStyle.Fill;
            leftLayout.Location = new Point(4, 72);
            leftLayout.Margin = new Padding(4, 3, 4, 3);
            leftLayout.Name = "leftLayout";
            leftLayout.RowCount = 2;
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            leftLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 69F));
            leftLayout.Size = new Size(625, 689);
            leftLayout.TabIndex = 3;
            // 
            // movieList
            // 
            movieList.Alignment = ListViewAlignment.Default;
            movieList.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5 });
            movieList.ContextMenuStrip = contextMenuStrip1;
            movieList.Dock = DockStyle.Fill;
            movieList.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            movieList.FullRowSelect = true;
            movieList.GridLines = true;
            movieList.LargeImageList = movieImageList;
            movieList.Location = new Point(4, 3);
            movieList.Margin = new Padding(4, 3, 4, 3);
            movieList.MultiSelect = false;
            movieList.Name = "movieList";
            movieList.Size = new Size(617, 614);
            movieList.TabIndex = 2;
            movieList.TileSize = new Size(100, 200);
            movieList.UseCompatibleStateImageBehavior = false;
            movieList.ColumnClick += movieList_ColumnClick;
            movieList.ItemSelectionChanged += movieList_ItemSelectionChanged;
            movieList.DoubleClick += movieList_DoubleClick;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Title";
            columnHeader1.Width = 230;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Year";
            columnHeader2.Width = 90;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Seasons";
            columnHeader3.Width = 80;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "ImdbRating";
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "ImdbId";
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItemDelete, toolStripMenuItemSetIMDBTagOnFolder });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(194, 48);
            // 
            // toolStripMenuItemDelete
            // 
            toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
            toolStripMenuItemDelete.Size = new Size(193, 22);
            toolStripMenuItemDelete.Text = "Delete item";
            toolStripMenuItemDelete.Click += contextMenu_Click;
            // 
            // toolStripMenuItemSetIMDBTagOnFolder
            // 
            toolStripMenuItemSetIMDBTagOnFolder.Name = "toolStripMenuItemSetIMDBTagOnFolder";
            toolStripMenuItemSetIMDBTagOnFolder.Size = new Size(193, 22);
            toolStripMenuItemSetIMDBTagOnFolder.Text = "Set IMDB tag on folder";
            toolStripMenuItemSetIMDBTagOnFolder.Click += contextMenu_Click;
            // 
            // movieImageList
            // 
            movieImageList.ColorDepth = ColorDepth.Depth8Bit;
            movieImageList.ImageStream = (ImageListStreamer)resources.GetObject("movieImageList.ImageStream");
            movieImageList.TransparentColor = Color.Transparent;
            movieImageList.Images.SetKeyName(0, "match.jpg");
            movieImageList.Images.SetKeyName(1, "match2.png");
            movieImageList.Images.SetKeyName(2, "match3.png");
            movieImageList.Images.SetKeyName(3, "match4.png");
            movieImageList.Images.SetKeyName(4, "MV5BMTg5NzY0MzA2MV5BMl5BanBnXkFtZTYwNDc3NTc2._V1_SX300.jpg");
            movieImageList.Images.SetKeyName(5, "search.png");
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22.5F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22.5F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Controls.Add(buttonEdit, 1, 0);
            tableLayoutPanel1.Controls.Add(detailsViewBtn, 3, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(tileViewBtn, 2, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(4, 623);
            tableLayoutPanel1.Margin = new Padding(4, 3, 4, 25);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(617, 41);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // buttonEdit
            // 
            buttonEdit.BackColor = SystemColors.Control;
            buttonEdit.Dock = DockStyle.Fill;
            buttonEdit.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonEdit.Location = new Point(212, 3);
            buttonEdit.Margin = new Padding(4, 3, 4, 3);
            buttonEdit.Name = "buttonEdit";
            buttonEdit.Size = new Size(111, 35);
            buttonEdit.TabIndex = 4;
            buttonEdit.Text = "Edit item";
            buttonEdit.UseVisualStyleBackColor = false;
            buttonEdit.Click += buttonEdit_Click;
            // 
            // detailsViewBtn
            // 
            detailsViewBtn.BackColor = SystemColors.Control;
            detailsViewBtn.Dock = DockStyle.Fill;
            detailsViewBtn.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            detailsViewBtn.Location = new Point(465, 3);
            detailsViewBtn.Margin = new Padding(4, 3, 4, 3);
            detailsViewBtn.Name = "detailsViewBtn";
            detailsViewBtn.Size = new Size(126, 35);
            detailsViewBtn.TabIndex = 3;
            detailsViewBtn.Text = "Grid";
            detailsViewBtn.UseVisualStyleBackColor = false;
            detailsViewBtn.Click += detailsViewBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(4, 0);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(200, 41);
            label1.TabIndex = 0;
            label1.Text = "Results found";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tileViewBtn
            // 
            tileViewBtn.BackColor = SystemColors.Control;
            tileViewBtn.Dock = DockStyle.Fill;
            tileViewBtn.Enabled = false;
            tileViewBtn.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tileViewBtn.Location = new Point(331, 3);
            tileViewBtn.Margin = new Padding(4, 3, 4, 3);
            tileViewBtn.Name = "tileViewBtn";
            tileViewBtn.Size = new Size(126, 35);
            tileViewBtn.TabIndex = 2;
            tileViewBtn.Text = "Tile";
            tileViewBtn.UseVisualStyleBackColor = false;
            tileViewBtn.Click += tileViewBtn_Click;
            // 
            // rightLayout
            // 
            rightLayout.ColumnCount = 3;
            rightLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 47.61905F));
            rightLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26.19048F));
            rightLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26.19048F));
            rightLayout.Controls.Add(labelOpen, 1, 7);
            rightLayout.Controls.Add(ratingContentLabel, 2, 4);
            rightLayout.Controls.Add(labelRating, 1, 4);
            rightLayout.Controls.Add(countryContentLabel, 2, 3);
            rightLayout.Controls.Add(directorContentLabel, 2, 2);
            rightLayout.Controls.Add(imdbContentLabel, 2, 1);
            rightLayout.Controls.Add(titleContentLabel, 2, 0);
            rightLayout.Controls.Add(pictureBox, 0, 0);
            rightLayout.Controls.Add(titleLabel, 1, 0);
            rightLayout.Controls.Add(imdbLabel, 1, 1);
            rightLayout.Controls.Add(directorLabel, 1, 2);
            rightLayout.Controls.Add(countryLabel, 1, 3);
            rightLayout.Controls.Add(plotLabel, 1, 10);
            rightLayout.Controls.Add(runtimeLabel, 1, 9);
            rightLayout.Controls.Add(typeLabel, 1, 8);
            rightLayout.Controls.Add(totalLabel, 1, 6);
            rightLayout.Controls.Add(yearLabel, 1, 5);
            rightLayout.Controls.Add(yearContentLabel, 2, 5);
            rightLayout.Controls.Add(totalContentLabel, 2, 6);
            rightLayout.Controls.Add(typeContentLabel, 2, 8);
            rightLayout.Controls.Add(runtimeContentLabel, 2, 9);
            rightLayout.Controls.Add(plotContentLabel, 1, 11);
            rightLayout.Controls.Add(button1, 2, 7);
            rightLayout.Dock = DockStyle.Fill;
            rightLayout.Location = new Point(637, 78);
            rightLayout.Margin = new Padding(4, 9, 4, 14);
            rightLayout.Name = "rightLayout";
            rightLayout.RowCount = 12;
            rightLayout.RowStyles.Add(new RowStyle());
            rightLayout.RowStyles.Add(new RowStyle());
            rightLayout.RowStyles.Add(new RowStyle());
            rightLayout.RowStyles.Add(new RowStyle());
            rightLayout.RowStyles.Add(new RowStyle());
            rightLayout.RowStyles.Add(new RowStyle());
            rightLayout.RowStyles.Add(new RowStyle());
            rightLayout.RowStyles.Add(new RowStyle());
            rightLayout.RowStyles.Add(new RowStyle());
            rightLayout.RowStyles.Add(new RowStyle());
            rightLayout.RowStyles.Add(new RowStyle());
            rightLayout.RowStyles.Add(new RowStyle());
            rightLayout.Size = new Size(799, 672);
            rightLayout.TabIndex = 4;
            // 
            // labelOpen
            // 
            labelOpen.AutoSize = true;
            labelOpen.Dock = DockStyle.Fill;
            labelOpen.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelOpen.ImageAlign = ContentAlignment.MiddleLeft;
            labelOpen.Location = new Point(384, 271);
            labelOpen.Margin = new Padding(4, 9, 4, 9);
            labelOpen.Name = "labelOpen";
            labelOpen.Size = new Size(201, 18);
            labelOpen.TabIndex = 21;
            labelOpen.Text = "Open Location:";
            labelOpen.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // ratingContentLabel
            // 
            ratingContentLabel.AutoSize = true;
            ratingContentLabel.BackColor = Color.Azure;
            ratingContentLabel.Dock = DockStyle.Fill;
            ratingContentLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ratingContentLabel.ImageAlign = ContentAlignment.TopLeft;
            ratingContentLabel.Location = new Point(593, 161);
            ratingContentLabel.Margin = new Padding(4, 9, 4, 9);
            ratingContentLabel.Name = "ratingContentLabel";
            ratingContentLabel.Size = new Size(202, 20);
            ratingContentLabel.TabIndex = 20;
            ratingContentLabel.Click += ContentLabel_Click;
            // 
            // labelRating
            // 
            labelRating.AutoSize = true;
            labelRating.Dock = DockStyle.Fill;
            labelRating.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelRating.ImageAlign = ContentAlignment.MiddleLeft;
            labelRating.Location = new Point(384, 161);
            labelRating.Margin = new Padding(4, 9, 4, 9);
            labelRating.Name = "labelRating";
            labelRating.Size = new Size(201, 20);
            labelRating.TabIndex = 19;
            labelRating.Text = "Rating:";
            labelRating.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // countryContentLabel
            // 
            countryContentLabel.AutoSize = true;
            countryContentLabel.Dock = DockStyle.Fill;
            countryContentLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            countryContentLabel.ImageAlign = ContentAlignment.TopLeft;
            countryContentLabel.Location = new Point(593, 123);
            countryContentLabel.Margin = new Padding(4, 9, 4, 9);
            countryContentLabel.Name = "countryContentLabel";
            countryContentLabel.Size = new Size(202, 20);
            countryContentLabel.TabIndex = 13;
            // 
            // directorContentLabel
            // 
            directorContentLabel.AutoSize = true;
            directorContentLabel.Dock = DockStyle.Fill;
            directorContentLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            directorContentLabel.ImageAlign = ContentAlignment.TopLeft;
            directorContentLabel.Location = new Point(593, 85);
            directorContentLabel.Margin = new Padding(4, 9, 4, 9);
            directorContentLabel.Name = "directorContentLabel";
            directorContentLabel.Size = new Size(202, 20);
            directorContentLabel.TabIndex = 12;
            // 
            // imdbContentLabel
            // 
            imdbContentLabel.AutoSize = true;
            imdbContentLabel.Dock = DockStyle.Fill;
            imdbContentLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            imdbContentLabel.ImageAlign = ContentAlignment.TopLeft;
            imdbContentLabel.Location = new Point(593, 47);
            imdbContentLabel.Margin = new Padding(4, 9, 4, 9);
            imdbContentLabel.Name = "imdbContentLabel";
            imdbContentLabel.Size = new Size(202, 20);
            imdbContentLabel.TabIndex = 11;
            imdbContentLabel.Click += pictureBox_Click;
            // 
            // titleContentLabel
            // 
            titleContentLabel.AutoSize = true;
            titleContentLabel.Dock = DockStyle.Fill;
            titleContentLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            titleContentLabel.ImageAlign = ContentAlignment.TopLeft;
            titleContentLabel.Location = new Point(593, 9);
            titleContentLabel.Margin = new Padding(4, 9, 4, 9);
            titleContentLabel.Name = "titleContentLabel";
            titleContentLabel.Size = new Size(202, 20);
            titleContentLabel.TabIndex = 10;
            // 
            // pictureBox
            // 
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.ErrorImage = (Image)resources.GetObject("pictureBox.ErrorImage");
            pictureBox.InitialImage = (Image)resources.GetObject("pictureBox.InitialImage");
            pictureBox.Location = new Point(4, 3);
            pictureBox.Margin = new Padding(4, 3, 4, 3);
            pictureBox.Name = "pictureBox";
            rightLayout.SetRowSpan(pictureBox, 12);
            pictureBox.Size = new Size(372, 666);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            pictureBox.Click += pictureBox_Click;
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            titleLabel.ImageAlign = ContentAlignment.MiddleLeft;
            titleLabel.Location = new Point(384, 9);
            titleLabel.Margin = new Padding(4, 9, 4, 9);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(201, 20);
            titleLabel.TabIndex = 1;
            titleLabel.Text = "Title:";
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // imdbLabel
            // 
            imdbLabel.AutoSize = true;
            imdbLabel.Dock = DockStyle.Fill;
            imdbLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            imdbLabel.ImageAlign = ContentAlignment.MiddleLeft;
            imdbLabel.Location = new Point(384, 47);
            imdbLabel.Margin = new Padding(4, 9, 4, 9);
            imdbLabel.Name = "imdbLabel";
            imdbLabel.Size = new Size(201, 20);
            imdbLabel.TabIndex = 2;
            imdbLabel.Text = "ID:";
            imdbLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // directorLabel
            // 
            directorLabel.AutoSize = true;
            directorLabel.Dock = DockStyle.Fill;
            directorLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            directorLabel.ImageAlign = ContentAlignment.MiddleLeft;
            directorLabel.Location = new Point(384, 85);
            directorLabel.Margin = new Padding(4, 9, 4, 9);
            directorLabel.Name = "directorLabel";
            directorLabel.Size = new Size(201, 20);
            directorLabel.TabIndex = 3;
            directorLabel.Text = "Director:";
            directorLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // countryLabel
            // 
            countryLabel.AutoSize = true;
            countryLabel.Dock = DockStyle.Fill;
            countryLabel.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            countryLabel.ImageAlign = ContentAlignment.MiddleLeft;
            countryLabel.Location = new Point(384, 123);
            countryLabel.Margin = new Padding(4, 9, 4, 9);
            countryLabel.Name = "countryLabel";
            countryLabel.Size = new Size(201, 20);
            countryLabel.TabIndex = 4;
            countryLabel.Text = "Country:";
            countryLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // plotLabel
            // 
            plotLabel.AutoSize = true;
            plotLabel.Dock = DockStyle.Fill;
            plotLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            plotLabel.ImageAlign = ContentAlignment.MiddleLeft;
            plotLabel.Location = new Point(384, 379);
            plotLabel.Margin = new Padding(4, 9, 4, 9);
            plotLabel.Name = "plotLabel";
            plotLabel.Size = new Size(201, 18);
            plotLabel.TabIndex = 9;
            plotLabel.Text = "Plot:";
            plotLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // runtimeLabel
            // 
            runtimeLabel.AutoSize = true;
            runtimeLabel.Dock = DockStyle.Fill;
            runtimeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            runtimeLabel.ImageAlign = ContentAlignment.MiddleLeft;
            runtimeLabel.Location = new Point(384, 343);
            runtimeLabel.Margin = new Padding(4, 9, 4, 9);
            runtimeLabel.Name = "runtimeLabel";
            runtimeLabel.Size = new Size(201, 18);
            runtimeLabel.TabIndex = 8;
            runtimeLabel.Text = "Runtime:";
            runtimeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // typeLabel
            // 
            typeLabel.AutoSize = true;
            typeLabel.Dock = DockStyle.Fill;
            typeLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            typeLabel.ImageAlign = ContentAlignment.MiddleLeft;
            typeLabel.Location = new Point(384, 307);
            typeLabel.Margin = new Padding(4, 9, 4, 9);
            typeLabel.Name = "typeLabel";
            typeLabel.Size = new Size(201, 18);
            typeLabel.TabIndex = 7;
            typeLabel.Text = "Type:";
            typeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // totalLabel
            // 
            totalLabel.AutoSize = true;
            totalLabel.Dock = DockStyle.Fill;
            totalLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            totalLabel.ImageAlign = ContentAlignment.MiddleLeft;
            totalLabel.Location = new Point(384, 235);
            totalLabel.Margin = new Padding(4, 9, 4, 9);
            totalLabel.Name = "totalLabel";
            totalLabel.Size = new Size(201, 18);
            totalLabel.TabIndex = 6;
            totalLabel.Text = "Total seasons:";
            totalLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // yearLabel
            // 
            yearLabel.AutoSize = true;
            yearLabel.Dock = DockStyle.Fill;
            yearLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            yearLabel.ImageAlign = ContentAlignment.MiddleLeft;
            yearLabel.Location = new Point(384, 199);
            yearLabel.Margin = new Padding(4, 9, 4, 9);
            yearLabel.Name = "yearLabel";
            yearLabel.Size = new Size(201, 18);
            yearLabel.TabIndex = 5;
            yearLabel.Text = "Year:";
            yearLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // yearContentLabel
            // 
            yearContentLabel.AutoSize = true;
            yearContentLabel.Dock = DockStyle.Fill;
            yearContentLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            yearContentLabel.ImageAlign = ContentAlignment.TopLeft;
            yearContentLabel.Location = new Point(593, 199);
            yearContentLabel.Margin = new Padding(4, 9, 4, 9);
            yearContentLabel.Name = "yearContentLabel";
            yearContentLabel.Size = new Size(202, 18);
            yearContentLabel.TabIndex = 14;
            // 
            // totalContentLabel
            // 
            totalContentLabel.AutoSize = true;
            totalContentLabel.Dock = DockStyle.Fill;
            totalContentLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            totalContentLabel.ImageAlign = ContentAlignment.TopLeft;
            totalContentLabel.Location = new Point(593, 235);
            totalContentLabel.Margin = new Padding(4, 9, 4, 9);
            totalContentLabel.Name = "totalContentLabel";
            totalContentLabel.Size = new Size(202, 18);
            totalContentLabel.TabIndex = 15;
            // 
            // typeContentLabel
            // 
            typeContentLabel.AutoSize = true;
            typeContentLabel.Dock = DockStyle.Fill;
            typeContentLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            typeContentLabel.ImageAlign = ContentAlignment.TopLeft;
            typeContentLabel.Location = new Point(593, 307);
            typeContentLabel.Margin = new Padding(4, 9, 4, 9);
            typeContentLabel.Name = "typeContentLabel";
            typeContentLabel.Size = new Size(202, 18);
            typeContentLabel.TabIndex = 16;
            // 
            // runtimeContentLabel
            // 
            runtimeContentLabel.AutoSize = true;
            runtimeContentLabel.Dock = DockStyle.Fill;
            runtimeContentLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            runtimeContentLabel.ImageAlign = ContentAlignment.TopLeft;
            runtimeContentLabel.Location = new Point(593, 343);
            runtimeContentLabel.Margin = new Padding(4, 9, 4, 9);
            runtimeContentLabel.Name = "runtimeContentLabel";
            runtimeContentLabel.Size = new Size(202, 18);
            runtimeContentLabel.TabIndex = 17;
            // 
            // plotContentLabel
            // 
            plotContentLabel.AutoSize = true;
            rightLayout.SetColumnSpan(plotContentLabel, 2);
            plotContentLabel.Dock = DockStyle.Fill;
            plotContentLabel.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            plotContentLabel.ImageAlign = ContentAlignment.TopLeft;
            plotContentLabel.Location = new Point(384, 406);
            plotContentLabel.Margin = new Padding(4, 0, 4, 9);
            plotContentLabel.Name = "plotContentLabel";
            plotContentLabel.Size = new Size(411, 257);
            plotContentLabel.TabIndex = 18;
            // 
            // button1
            // 
            button1.BackColor = Color.Azure;
            button1.Location = new Point(592, 265);
            button1.Name = "button1";
            button1.Size = new Size(203, 30);
            button1.TabIndex = 22;
            button1.Text = "Finn";
            button1.UseVisualStyleBackColor = false;
            button1.Click += buttonFinn_Click;
            // 
            // statusStrip
            // 
            statusStrip.BackColor = Color.DarkBlue;
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip.Location = new Point(0, 742);
            statusStrip.Name = "statusStrip";
            statusStrip.Padding = new Padding(1, 0, 16, 0);
            statusStrip.Size = new Size(1440, 22);
            statusStrip.SizingGrip = false;
            statusStrip.TabIndex = 2;
            // 
            // statusLabel
            // 
            statusLabel.Font = new Font("Microsoft Sans Serif", 9F);
            statusLabel.ForeColor = Color.White;
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(1423, 17);
            statusLabel.Spring = true;
            statusLabel.Text = "Clear searchfield to show all items in list";
            // 
            // ShowLocalSeriesForm
            // 
            AcceptButton = searchBtn;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1440, 764);
            Controls.Add(statusStrip);
            Controls.Add(mainLayout);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(989, 686);
            Name = "ShowLocalSeriesForm";
            Text = "OMDb Series";
            mainLayout.ResumeLayout(false);
            headerLayout.ResumeLayout(false);
            headerLayout.PerformLayout();
            leftLayout.ResumeLayout(false);
            contextMenuStrip1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            rightLayout.ResumeLayout(false);
            rightLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private System.Windows.Forms.TextBox searchTxt;
        private System.Windows.Forms.Button searchBtn;
        private System.Windows.Forms.Label omdbLabel;
        private System.Windows.Forms.ListView movieList;
        private System.Windows.Forms.ImageList movieImageList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.TableLayoutPanel leftLayout;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button detailsViewBtn;
        private System.Windows.Forms.Button tileViewBtn;
        private System.Windows.Forms.TableLayoutPanel rightLayout;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label imdbLabel;
        private System.Windows.Forms.Label directorLabel;
        private System.Windows.Forms.Label countryLabel;
        private System.Windows.Forms.Label plotLabel;
        private System.Windows.Forms.Label runtimeLabel;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.Label totalLabel;
        private System.Windows.Forms.Label yearLabel;
        private System.Windows.Forms.Label titleContentLabel;
        private System.Windows.Forms.Label countryContentLabel;
        private System.Windows.Forms.Label directorContentLabel;
        private System.Windows.Forms.Label imdbContentLabel;
        private System.Windows.Forms.Label yearContentLabel;
        private System.Windows.Forms.Label totalContentLabel;
        private System.Windows.Forms.Label typeContentLabel;
        private System.Windows.Forms.Label runtimeContentLabel;
        private System.Windows.Forms.Label plotContentLabel;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private Button buttonLookUp;
        private Label ratingContentLabel;
        private Label labelRating;
        private Label labelOpen;
        private Button button1;
        private Button buttonEdit;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem toolStripMenuItemDelete;
        private ToolStripMenuItem toolStripMenuItemSetIMDBTagOnFolder;
        private ColumnHeader columnHeader5;
    }
}

