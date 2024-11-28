namespace Kolibri.Common.VisualizeOMDbItem
{
    partial class ShowLocalSeries
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowLocalSeries));
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.searchTxt = new System.Windows.Forms.TextBox();
            this.searchBtn = new System.Windows.Forms.Button();
            this.omdbLabel = new System.Windows.Forms.Label();
            this.leftLayout = new System.Windows.Forms.TableLayoutPanel();
            this.movieList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.movieImageList = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.detailsViewBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.resultsToGet = new System.Windows.Forms.ComboBox();
            this.tileViewBtn = new System.Windows.Forms.Button();
            this.rightLayout = new System.Windows.Forms.TableLayoutPanel();
            this.countryContentLabel = new System.Windows.Forms.Label();
            this.directorContentLabel = new System.Windows.Forms.Label();
            this.productionContentLabel = new System.Windows.Forms.Label();
            this.titleContentLabel = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.titleLabel = new System.Windows.Forms.Label();
            this.productionLabel = new System.Windows.Forms.Label();
            this.directorLabel = new System.Windows.Forms.Label();
            this.countryLabel = new System.Windows.Forms.Label();
            this.plotLabel = new System.Windows.Forms.Label();
            this.runtimeLabel = new System.Windows.Forms.Label();
            this.typeLabel = new System.Windows.Forms.Label();
            this.totalLabel = new System.Windows.Forms.Label();
            this.yearLabel = new System.Windows.Forms.Label();
            this.yearContentLabel = new System.Windows.Forms.Label();
            this.totalContentLabel = new System.Windows.Forms.Label();
            this.typeContentLabel = new System.Windows.Forms.Label();
            this.runtimeContentLabel = new System.Windows.Forms.Label();
            this.plotContentLabel = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mainLayout.SuspendLayout();
            this.headerLayout.SuspendLayout();
            this.leftLayout.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.rightLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.BackColor = System.Drawing.Color.AliceBlue;
            this.mainLayout.ColumnCount = 2;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.mainLayout.Controls.Add(this.headerLayout, 0, 0);
            this.mainLayout.Controls.Add(this.leftLayout, 0, 1);
            this.mainLayout.Controls.Add(this.rightLayout, 1, 1);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.RowCount = 2;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainLayout.Size = new System.Drawing.Size(1234, 662);
            this.mainLayout.TabIndex = 1;
            // 
            // headerLayout
            // 
            this.headerLayout.BackColor = System.Drawing.Color.DarkBlue;
            this.headerLayout.ColumnCount = 3;
            this.mainLayout.SetColumnSpan(this.headerLayout, 2);
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.headerLayout.Controls.Add(this.searchTxt, 0, 0);
            this.headerLayout.Controls.Add(this.searchBtn, 1, 0);
            this.headerLayout.Controls.Add(this.omdbLabel, 2, 0);
            this.headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayout.Location = new System.Drawing.Point(3, 3);
            this.headerLayout.Name = "headerLayout";
            this.headerLayout.RowCount = 1;
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Size = new System.Drawing.Size(1228, 54);
            this.headerLayout.TabIndex = 1;
            // 
            // searchTxt
            // 
            this.searchTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchTxt.Location = new System.Drawing.Point(8, 12);
            this.searchTxt.Margin = new System.Windows.Forms.Padding(8, 12, 6, 12);
            this.searchTxt.Name = "searchTxt";
            this.searchTxt.Size = new System.Drawing.Size(286, 24);
            this.searchTxt.TabIndex = 0;
            // 
            // searchBtn
            // 
            this.searchBtn.BackColor = System.Drawing.Color.DarkBlue;
            this.searchBtn.BackgroundImage = global::Kolibri.Common.VisualizeOMDbItem.Properties.Resources.search;
            this.searchBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.searchBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.searchBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchBtn.ForeColor = System.Drawing.Color.DarkBlue;
            this.searchBtn.Location = new System.Drawing.Point(302, 2);
            this.searchBtn.Margin = new System.Windows.Forms.Padding(2);
            this.searchBtn.Name = "searchBtn";
            this.searchBtn.Size = new System.Drawing.Size(56, 50);
            this.searchBtn.TabIndex = 1;
            this.searchBtn.UseVisualStyleBackColor = false;
            this.searchBtn.Click += new System.EventHandler(this.searchBtn_Click);
            // 
            // omdbLabel
            // 
            this.omdbLabel.AutoSize = true;
            this.omdbLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.omdbLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.omdbLabel.ForeColor = System.Drawing.Color.White;
            this.omdbLabel.Location = new System.Drawing.Point(363, 0);
            this.omdbLabel.Name = "omdbLabel";
            this.omdbLabel.Size = new System.Drawing.Size(862, 54);
            this.omdbLabel.TabIndex = 2;
            this.omdbLabel.Text = "Search local series";
            this.omdbLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // leftLayout
            // 
            this.leftLayout.ColumnCount = 1;
            this.leftLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftLayout.Controls.Add(this.movieList, 0, 0);
            this.leftLayout.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.leftLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftLayout.Location = new System.Drawing.Point(3, 63);
            this.leftLayout.Name = "leftLayout";
            this.leftLayout.RowCount = 2;
            this.leftLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.leftLayout.Size = new System.Drawing.Size(549, 596);
            this.leftLayout.TabIndex = 3;
            // 
            // movieList
            // 
            this.movieList.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.movieList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.movieList.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.movieList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.movieList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.movieList.FullRowSelect = true;
            this.movieList.GridLines = true;
            this.movieList.HideSelection = false;
            this.movieList.LargeImageList = this.movieImageList;
            this.movieList.Location = new System.Drawing.Point(3, 3);
            this.movieList.MultiSelect = false;
            this.movieList.Name = "movieList";
            this.movieList.Size = new System.Drawing.Size(543, 530);
            this.movieList.TabIndex = 2;
            this.movieList.TileSize = new System.Drawing.Size(100, 200);
            this.movieList.UseCompatibleStateImageBehavior = false;
            this.movieList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.movieList_ItemSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Title";
            this.columnHeader1.Width = 230;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Year";
            this.columnHeader2.Width = 90;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Type";
            this.columnHeader3.Width = 80;
            // 
            // movieImageList
            // 
            this.movieImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("movieImageList.ImageStream")));
            this.movieImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.movieImageList.Images.SetKeyName(0, "match.jpg");
            this.movieImageList.Images.SetKeyName(1, "match2.png");
            this.movieImageList.Images.SetKeyName(2, "match3.png");
            this.movieImageList.Images.SetKeyName(3, "match4.png");
            this.movieImageList.Images.SetKeyName(4, "MV5BMTg5NzY0MzA2MV5BMl5BanBnXkFtZTYwNDc3NTc2._V1_SX300.jpg");
            this.movieImageList.Images.SetKeyName(5, "search.png");
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.5F));
            this.tableLayoutPanel1.Controls.Add(this.detailsViewBtn, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.resultsToGet, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tileViewBtn, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 539);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 22);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(543, 35);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // detailsViewBtn
            // 
            this.detailsViewBtn.BackColor = System.Drawing.SystemColors.Control;
            this.detailsViewBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsViewBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detailsViewBtn.Location = new System.Drawing.Point(423, 3);
            this.detailsViewBtn.Name = "detailsViewBtn";
            this.detailsViewBtn.Size = new System.Drawing.Size(117, 29);
            this.detailsViewBtn.TabIndex = 3;
            this.detailsViewBtn.Text = "Grid";
            this.detailsViewBtn.UseVisualStyleBackColor = false;
            this.detailsViewBtn.Click += new System.EventHandler(this.detailsViewBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "Results found";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // resultsToGet
            // 
            this.resultsToGet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsToGet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.resultsToGet.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultsToGet.FormattingEnabled = true;
            this.resultsToGet.Items.AddRange(new object[] {
            "10",
            "20",
            "30",
            "40",
            "50",
            "100"});
            this.resultsToGet.Location = new System.Drawing.Point(193, 3);
            this.resultsToGet.Name = "resultsToGet";
            this.resultsToGet.Size = new System.Drawing.Size(102, 24);
            this.resultsToGet.TabIndex = 1;
            // 
            // tileViewBtn
            // 
            this.tileViewBtn.BackColor = System.Drawing.SystemColors.Control;
            this.tileViewBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileViewBtn.Enabled = false;
            this.tileViewBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tileViewBtn.Location = new System.Drawing.Point(301, 3);
            this.tileViewBtn.Name = "tileViewBtn";
            this.tileViewBtn.Size = new System.Drawing.Size(116, 29);
            this.tileViewBtn.TabIndex = 2;
            this.tileViewBtn.Text = "Tile";
            this.tileViewBtn.UseVisualStyleBackColor = false;
            this.tileViewBtn.Click += new System.EventHandler(this.tileViewBtn_Click);
            // 
            // rightLayout
            // 
            this.rightLayout.ColumnCount = 3;
            this.rightLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.61905F));
            this.rightLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.19048F));
            this.rightLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.19048F));
            this.rightLayout.Controls.Add(this.countryContentLabel, 2, 3);
            this.rightLayout.Controls.Add(this.directorContentLabel, 2, 2);
            this.rightLayout.Controls.Add(this.productionContentLabel, 2, 1);
            this.rightLayout.Controls.Add(this.titleContentLabel, 2, 0);
            this.rightLayout.Controls.Add(this.pictureBox, 0, 0);
            this.rightLayout.Controls.Add(this.titleLabel, 1, 0);
            this.rightLayout.Controls.Add(this.productionLabel, 1, 1);
            this.rightLayout.Controls.Add(this.directorLabel, 1, 2);
            this.rightLayout.Controls.Add(this.countryLabel, 1, 3);
            this.rightLayout.Controls.Add(this.plotLabel, 1, 10);
            this.rightLayout.Controls.Add(this.runtimeLabel, 1, 9);
            this.rightLayout.Controls.Add(this.typeLabel, 1, 8);
            this.rightLayout.Controls.Add(this.totalLabel, 1, 6);
            this.rightLayout.Controls.Add(this.yearLabel, 1, 5);
            this.rightLayout.Controls.Add(this.yearContentLabel, 2, 5);
            this.rightLayout.Controls.Add(this.totalContentLabel, 2, 6);
            this.rightLayout.Controls.Add(this.typeContentLabel, 2, 8);
            this.rightLayout.Controls.Add(this.runtimeContentLabel, 2, 9);
            this.rightLayout.Controls.Add(this.plotContentLabel, 1, 11);
            this.rightLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightLayout.Location = new System.Drawing.Point(558, 68);
            this.rightLayout.Margin = new System.Windows.Forms.Padding(3, 8, 3, 12);
            this.rightLayout.Name = "rightLayout";
            this.rightLayout.RowCount = 12;
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.rightLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.rightLayout.Size = new System.Drawing.Size(673, 582);
            this.rightLayout.TabIndex = 4;
            // 
            // countryContentLabel
            // 
            this.countryContentLabel.AutoSize = true;
            this.countryContentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.countryContentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.countryContentLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.countryContentLabel.Location = new System.Drawing.Point(499, 116);
            this.countryContentLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.countryContentLabel.Name = "countryContentLabel";
            this.countryContentLabel.Size = new System.Drawing.Size(171, 20);
            this.countryContentLabel.TabIndex = 13;
            // 
            // directorContentLabel
            // 
            this.directorContentLabel.AutoSize = true;
            this.directorContentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directorContentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.directorContentLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.directorContentLabel.Location = new System.Drawing.Point(499, 80);
            this.directorContentLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.directorContentLabel.Name = "directorContentLabel";
            this.directorContentLabel.Size = new System.Drawing.Size(171, 20);
            this.directorContentLabel.TabIndex = 12;
            // 
            // productionContentLabel
            // 
            this.productionContentLabel.AutoSize = true;
            this.productionContentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productionContentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.productionContentLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.productionContentLabel.Location = new System.Drawing.Point(499, 44);
            this.productionContentLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.productionContentLabel.Name = "productionContentLabel";
            this.productionContentLabel.Size = new System.Drawing.Size(171, 20);
            this.productionContentLabel.TabIndex = 11;
            // 
            // titleContentLabel
            // 
            this.titleContentLabel.AutoSize = true;
            this.titleContentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleContentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleContentLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.titleContentLabel.Location = new System.Drawing.Point(499, 8);
            this.titleContentLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.titleContentLabel.Name = "titleContentLabel";
            this.titleContentLabel.Size = new System.Drawing.Size(171, 20);
            this.titleContentLabel.TabIndex = 10;
            // 
            // pictureBox
            // 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.ErrorImage = global::Kolibri.Common.VisualizeOMDbItem.Properties.Resources.no_image;
            this.pictureBox.InitialImage = global::Kolibri.Common.VisualizeOMDbItem.Properties.Resources.no_image;
            this.pictureBox.Location = new System.Drawing.Point(3, 3);
            this.pictureBox.Name = "pictureBox";
            this.rightLayout.SetRowSpan(this.pictureBox, 12);
            this.pictureBox.Size = new System.Drawing.Size(314, 576);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.Click += new System.EventHandler(this.pictureBox_Click);
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.titleLabel.Location = new System.Drawing.Point(323, 8);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(170, 20);
            this.titleLabel.TabIndex = 1;
            this.titleLabel.Text = "Title:";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // productionLabel
            // 
            this.productionLabel.AutoSize = true;
            this.productionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.productionLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.productionLabel.Location = new System.Drawing.Point(323, 44);
            this.productionLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.productionLabel.Name = "productionLabel";
            this.productionLabel.Size = new System.Drawing.Size(170, 20);
            this.productionLabel.TabIndex = 2;
            this.productionLabel.Text = "Production:";
            this.productionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // directorLabel
            // 
            this.directorLabel.AutoSize = true;
            this.directorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.directorLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.directorLabel.Location = new System.Drawing.Point(323, 80);
            this.directorLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.directorLabel.Name = "directorLabel";
            this.directorLabel.Size = new System.Drawing.Size(170, 20);
            this.directorLabel.TabIndex = 3;
            this.directorLabel.Text = "Director:";
            this.directorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // countryLabel
            // 
            this.countryLabel.AutoSize = true;
            this.countryLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.countryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.countryLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.countryLabel.Location = new System.Drawing.Point(323, 116);
            this.countryLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.countryLabel.Name = "countryLabel";
            this.countryLabel.Size = new System.Drawing.Size(170, 20);
            this.countryLabel.TabIndex = 4;
            this.countryLabel.Text = "Country:";
            this.countryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // plotLabel
            // 
            this.plotLabel.AutoSize = true;
            this.plotLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plotLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.plotLabel.Location = new System.Drawing.Point(323, 308);
            this.plotLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.plotLabel.Name = "plotLabel";
            this.plotLabel.Size = new System.Drawing.Size(170, 18);
            this.plotLabel.TabIndex = 9;
            this.plotLabel.Text = "Plot:";
            this.plotLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // runtimeLabel
            // 
            this.runtimeLabel.AutoSize = true;
            this.runtimeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.runtimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runtimeLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.runtimeLabel.Location = new System.Drawing.Point(323, 274);
            this.runtimeLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.runtimeLabel.Name = "runtimeLabel";
            this.runtimeLabel.Size = new System.Drawing.Size(170, 18);
            this.runtimeLabel.TabIndex = 8;
            this.runtimeLabel.Text = "Runtime:";
            this.runtimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // typeLabel
            // 
            this.typeLabel.AutoSize = true;
            this.typeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.typeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.typeLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.typeLabel.Location = new System.Drawing.Point(323, 240);
            this.typeLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.typeLabel.Name = "typeLabel";
            this.typeLabel.Size = new System.Drawing.Size(170, 18);
            this.typeLabel.TabIndex = 7;
            this.typeLabel.Text = "Type:";
            this.typeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // totalLabel
            // 
            this.totalLabel.AutoSize = true;
            this.totalLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.totalLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.totalLabel.Location = new System.Drawing.Point(323, 196);
            this.totalLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.totalLabel.Name = "totalLabel";
            this.totalLabel.Size = new System.Drawing.Size(170, 18);
            this.totalLabel.TabIndex = 6;
            this.totalLabel.Text = "Total seasons:";
            this.totalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // yearLabel
            // 
            this.yearLabel.AutoSize = true;
            this.yearLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.yearLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yearLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.yearLabel.Location = new System.Drawing.Point(323, 162);
            this.yearLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.yearLabel.Name = "yearLabel";
            this.yearLabel.Size = new System.Drawing.Size(170, 18);
            this.yearLabel.TabIndex = 5;
            this.yearLabel.Text = "Year:";
            this.yearLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // yearContentLabel
            // 
            this.yearContentLabel.AutoSize = true;
            this.yearContentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.yearContentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yearContentLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.yearContentLabel.Location = new System.Drawing.Point(499, 162);
            this.yearContentLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.yearContentLabel.Name = "yearContentLabel";
            this.yearContentLabel.Size = new System.Drawing.Size(171, 18);
            this.yearContentLabel.TabIndex = 14;
            // 
            // totalContentLabel
            // 
            this.totalContentLabel.AutoSize = true;
            this.totalContentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.totalContentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalContentLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.totalContentLabel.Location = new System.Drawing.Point(499, 196);
            this.totalContentLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.totalContentLabel.Name = "totalContentLabel";
            this.totalContentLabel.Size = new System.Drawing.Size(171, 18);
            this.totalContentLabel.TabIndex = 15;
            // 
            // typeContentLabel
            // 
            this.typeContentLabel.AutoSize = true;
            this.typeContentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.typeContentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.typeContentLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.typeContentLabel.Location = new System.Drawing.Point(499, 240);
            this.typeContentLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.typeContentLabel.Name = "typeContentLabel";
            this.typeContentLabel.Size = new System.Drawing.Size(171, 18);
            this.typeContentLabel.TabIndex = 16;
            // 
            // runtimeContentLabel
            // 
            this.runtimeContentLabel.AutoSize = true;
            this.runtimeContentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.runtimeContentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runtimeContentLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.runtimeContentLabel.Location = new System.Drawing.Point(499, 274);
            this.runtimeContentLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.runtimeContentLabel.Name = "runtimeContentLabel";
            this.runtimeContentLabel.Size = new System.Drawing.Size(171, 18);
            this.runtimeContentLabel.TabIndex = 17;
            // 
            // plotContentLabel
            // 
            this.plotContentLabel.AutoSize = true;
            this.rightLayout.SetColumnSpan(this.plotContentLabel, 2);
            this.plotContentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotContentLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plotContentLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.plotContentLabel.Location = new System.Drawing.Point(323, 334);
            this.plotContentLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 8);
            this.plotContentLabel.Name = "plotContentLabel";
            this.plotContentLabel.Size = new System.Drawing.Size(347, 240);
            this.plotContentLabel.TabIndex = 18;
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.DarkBlue;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 640);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1234, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 2;
            // 
            // statusLabel
            // 
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.statusLabel.ForeColor = System.Drawing.Color.White;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(1219, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "Clear searchfield to show all items in list";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "ImdbRating";
            // 
            // ShowLocalSeries
            // 
            this.AcceptButton = this.searchBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 662);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mainLayout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(850, 600);
            this.Name = "ShowLocalSeries";
            this.Text = "OMDb client";
            this.mainLayout.ResumeLayout(false);
            this.headerLayout.ResumeLayout(false);
            this.headerLayout.PerformLayout();
            this.leftLayout.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.rightLayout.ResumeLayout(false);
            this.rightLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.ComboBox resultsToGet;
        private System.Windows.Forms.Button detailsViewBtn;
        private System.Windows.Forms.Button tileViewBtn;
        private System.Windows.Forms.TableLayoutPanel rightLayout;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label productionLabel;
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
        private System.Windows.Forms.Label productionContentLabel;
        private System.Windows.Forms.Label yearContentLabel;
        private System.Windows.Forms.Label totalContentLabel;
        private System.Windows.Forms.Label typeContentLabel;
        private System.Windows.Forms.Label runtimeContentLabel;
        private System.Windows.Forms.Label plotContentLabel;
        private System.Windows.Forms.ColumnHeader columnHeader4;
    }
}

