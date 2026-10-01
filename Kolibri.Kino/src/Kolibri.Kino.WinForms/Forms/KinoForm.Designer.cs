namespace Kolibri.Kino.WinForms.Forms;

partial class KinoForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        toolbar = new FlowLayoutPanel();
        txtSearch = new TextBox();
        btnSearchLocal = new Button();
        btnSearchOnline = new Button();
        btnImport = new Button();
        btnAddToWatchlist = new Button();
        btnLocalMovies = new Button();
        btnLocalSeries = new Button();
        btnWatchlists = new Button();
        btnSettings = new Button();
        typeFilter = new FlowLayoutPanel();
        sortRow = new FlowLayoutPanel();
        lblSort = new Label();
        rbSortNone = new RadioButton();
        rbSortTitle = new RadioButton();
        rbSortYear = new RadioButton();
        rbSortRating = new RadioButton();
        rbSortGenre = new RadioButton();
        defaultKeysBanner = new Panel();
        lblDefaultKeys = new Label();
        lnkSetKeys = new LinkLabel();
        dgvItems = new DataGridView();
        itemMenu = new ContextMenuStrip(components);
        menuDetails = new ToolStripMenuItem();
        menuSeparator1 = new ToolStripSeparator();
        menuSeparator2 = new ToolStripSeparator();
        menuOpenOnDisk = new ToolStripMenuItem();
        menuOpenImdb = new ToolStripMenuItem();
        menuOpenTmdb = new ToolStripMenuItem();
        menuRemove = new ToolStripMenuItem();
        bindingSource = new BindingSource(components);
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        toolbar.SuspendLayout();
        defaultKeysBanner.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvItems).BeginInit();
        itemMenu.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)bindingSource).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();
        // 
        // toolbar
        // 
        toolbar.AutoSize = true;
        toolbar.Controls.Add(txtSearch);
        toolbar.Controls.Add(btnSearchLocal);
        toolbar.Controls.Add(btnSearchOnline);
        toolbar.Controls.Add(btnImport);
        toolbar.Controls.Add(btnAddToWatchlist);
        toolbar.Controls.Add(btnLocalMovies);
        toolbar.Controls.Add(btnLocalSeries);
        toolbar.Controls.Add(btnWatchlists);
        toolbar.Controls.Add(btnSettings);
        toolbar.Dock = DockStyle.Top;
        toolbar.Location = new Point(0, 44);
        toolbar.Name = "toolbar";
        toolbar.Padding = new Padding(6);
        toolbar.Size = new Size(984, 74);
        toolbar.TabIndex = 0;
        // 
        // txtSearch
        // 
        txtSearch.Location = new Point(9, 9);
        txtSearch.Name = "txtSearch";
        txtSearch.PlaceholderText = "Title…";
        txtSearch.Size = new Size(320, 23);
        txtSearch.TabIndex = 0;
        txtSearch.KeyDown += txtSearch_KeyDown;
        // 
        // btnSearchLocal
        // 
        btnSearchLocal.AutoSize = true;
        btnSearchLocal.Location = new Point(335, 9);
        btnSearchLocal.Name = "btnSearchLocal";
        btnSearchLocal.Size = new Size(100, 25);
        btnSearchLocal.TabIndex = 1;
        btnSearchLocal.Text = "Search library";
        btnSearchLocal.Click += btnSearchLocal_Click;
        // 
        // btnSearchOnline
        // 
        btnSearchOnline.AutoSize = true;
        btnSearchOnline.Location = new Point(441, 9);
        btnSearchOnline.Name = "btnSearchOnline";
        btnSearchOnline.Size = new Size(100, 25);
        btnSearchOnline.TabIndex = 2;
        btnSearchOnline.Text = "Search OMDb";
        btnSearchOnline.Click += btnSearchOnline_Click;
        // 
        // btnImport
        // 
        btnImport.AutoSize = true;
        btnImport.Enabled = false;
        btnImport.Location = new Point(547, 9);
        btnImport.Name = "btnImport";
        btnImport.Size = new Size(100, 25);
        btnImport.TabIndex = 3;
        btnImport.Text = "Import selected";
        btnImport.Click += btnImport_Click;
        // 
        // btnAddToWatchlist
        // 
        btnAddToWatchlist.AutoSize = true;
        btnAddToWatchlist.Location = new Point(653, 9);
        btnAddToWatchlist.Name = "btnAddToWatchlist";
        btnAddToWatchlist.Size = new Size(112, 25);
        btnAddToWatchlist.TabIndex = 5;
        btnAddToWatchlist.Text = "Add to watchlist…";
        btnAddToWatchlist.Click += btnAddToWatchlist_Click;
        // 
        // btnLocalMovies
        // 
        btnLocalMovies.AutoSize = true;
        btnLocalMovies.Location = new Point(792, 9);
        btnLocalMovies.Margin = new Padding(24, 3, 3, 3);
        btnLocalMovies.Name = "btnLocalMovies";
        btnLocalMovies.Size = new Size(100, 25);
        btnLocalMovies.TabIndex = 4;
        btnLocalMovies.Text = "Local movies…";
        btnLocalMovies.Click += btnLocalMovies_Click;
        // 
        // btnLocalSeries
        // 
        btnLocalSeries.AutoSize = true;
        btnLocalSeries.Location = new Point(9, 40);
        btnLocalSeries.Name = "btnLocalSeries";
        btnLocalSeries.Size = new Size(100, 25);
        btnLocalSeries.TabIndex = 8;
        btnLocalSeries.Text = "Local series…";
        btnLocalSeries.Click += btnLocalSeries_Click;
        // 
        // btnWatchlists
        // 
        btnWatchlists.AutoSize = true;
        btnWatchlists.Location = new Point(115, 40);
        btnWatchlists.Name = "btnWatchlists";
        btnWatchlists.Size = new Size(90, 25);
        btnWatchlists.TabIndex = 6;
        btnWatchlists.Text = "Watchlists…";
        btnWatchlists.Click += btnWatchlists_Click;
        // 
        // btnSettings
        // 
        btnSettings.AutoSize = true;
        btnSettings.Location = new Point(211, 40);
        btnSettings.Name = "btnSettings";
        btnSettings.Size = new Size(80, 25);
        btnSettings.TabIndex = 7;
        btnSettings.Text = "Settings…";
        btnSettings.Click += btnSettings_Click;
        // 
        // typeFilter
        // 
        typeFilter.AutoSize = true;
        typeFilter.Dock = DockStyle.Top;
        typeFilter.Location = new Point(0, 118);
        typeFilter.Name = "typeFilter";
        typeFilter.Padding = new Padding(6, 0, 6, 3);
        typeFilter.Size = new Size(984, 3);
        typeFilter.TabIndex = 3;
        //
        // sortRow (Sort: As listed / Title / Year / Rating / Genre)
        //
        sortRow.AutoSize = true;
        sortRow.Controls.Add(lblSort);
        sortRow.Controls.Add(rbSortNone);
        sortRow.Controls.Add(rbSortTitle);
        sortRow.Controls.Add(rbSortYear);
        sortRow.Controls.Add(rbSortRating);
        sortRow.Controls.Add(rbSortGenre);
        sortRow.Dock = DockStyle.Top;
        sortRow.Name = "sortRow";
        sortRow.Padding = new Padding(6, 0, 6, 3);
        sortRow.TabIndex = 5;
        //
        // lblSort
        //
        lblSort.AutoSize = true;
        lblSort.Margin = new Padding(3, 5, 3, 0);
        lblSort.Name = "lblSort";
        lblSort.Text = "Sort:";
        //
        // rbSortNone
        //
        rbSortNone.AutoSize = true;
        rbSortNone.Checked = true;
        rbSortNone.Margin = new Padding(3, 3, 9, 3);
        rbSortNone.Name = "rbSortNone";
        rbSortNone.TabStop = true;
        rbSortNone.Text = "As listed";
        rbSortNone.CheckedChanged += Sort_CheckedChanged;
        //
        // rbSortTitle
        //
        rbSortTitle.AutoSize = true;
        rbSortTitle.Margin = new Padding(3, 3, 9, 3);
        rbSortTitle.Name = "rbSortTitle";
        rbSortTitle.Text = "Title";
        rbSortTitle.CheckedChanged += Sort_CheckedChanged;
        //
        // rbSortYear
        //
        rbSortYear.AutoSize = true;
        rbSortYear.Margin = new Padding(3, 3, 9, 3);
        rbSortYear.Name = "rbSortYear";
        rbSortYear.Text = "Year (newest first)";
        rbSortYear.CheckedChanged += Sort_CheckedChanged;
        //
        // rbSortRating
        //
        rbSortRating.AutoSize = true;
        rbSortRating.Margin = new Padding(3, 3, 9, 3);
        rbSortRating.Name = "rbSortRating";
        rbSortRating.Text = "Rating (best first)";
        rbSortRating.CheckedChanged += Sort_CheckedChanged;
        //
        // rbSortGenre
        //
        rbSortGenre.AutoSize = true;
        rbSortGenre.Margin = new Padding(3, 3, 9, 3);
        rbSortGenre.Name = "rbSortGenre";
        rbSortGenre.Text = "Genre";
        rbSortGenre.CheckedChanged += Sort_CheckedChanged;
        //
        // defaultKeysBanner
        // 
        defaultKeysBanner.BackColor = Color.LightYellow;
        defaultKeysBanner.Controls.Add(lblDefaultKeys);
        defaultKeysBanner.Controls.Add(lnkSetKeys);
        defaultKeysBanner.Dock = DockStyle.Top;
        defaultKeysBanner.Location = new Point(0, 0);
        defaultKeysBanner.Name = "defaultKeysBanner";
        defaultKeysBanner.Padding = new Padding(9, 4, 9, 4);
        defaultKeysBanner.Size = new Size(984, 44);
        defaultKeysBanner.TabIndex = 4;
        // 
        // lblDefaultKeys
        // 
        lblDefaultKeys.Dock = DockStyle.Fill;
        lblDefaultKeys.Location = new Point(9, 4);
        lblDefaultKeys.Name = "lblDefaultKeys";
        lblDefaultKeys.Size = new Size(843, 36);
        lblDefaultKeys.TabIndex = 0;
        lblDefaultKeys.Text = "You're using the shared default keys.";
        lblDefaultKeys.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lnkSetKeys
        // 
        lnkSetKeys.AutoSize = true;
        lnkSetKeys.Dock = DockStyle.Right;
        lnkSetKeys.Location = new Point(852, 4);
        lnkSetKeys.Name = "lnkSetKeys";
        lnkSetKeys.Padding = new Padding(12, 10, 0, 0);
        lnkSetKeys.Size = new Size(123, 25);
        lnkSetKeys.TabIndex = 1;
        lnkSetKeys.TabStop = true;
        lnkSetKeys.Text = "Set your own keys…";
        lnkSetKeys.LinkClicked += lnkSetKeys_LinkClicked;
        // 
        // dgvItems
        // 
        dgvItems.AllowUserToAddRows = false;
        dgvItems.AllowUserToDeleteRows = false;
        dgvItems.AutoGenerateColumns = false;
        dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvItems.ContextMenuStrip = itemMenu;
        dgvItems.DataSource = bindingSource;
        dgvItems.Dock = DockStyle.Fill;
        dgvItems.Location = new Point(0, 121);
        dgvItems.MultiSelect = false;
        dgvItems.Name = "dgvItems";
        dgvItems.ReadOnly = true;
        dgvItems.RowHeadersVisible = false;
        dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvItems.Size = new Size(984, 418);
        dgvItems.TabIndex = 1;
        dgvItems.CellMouseDown += dgvItems_CellMouseDown;
        dgvItems.CellDoubleClick += dgvItems_CellDoubleClick;
        dgvItems.CellFormatting += dgvItems_CellFormatting;
        //
        // itemMenu
        //
        itemMenu.Items.AddRange(new ToolStripItem[] { menuDetails, menuSeparator1, menuOpenOnDisk, menuOpenImdb, menuOpenTmdb, menuSeparator2, menuRemove });
        itemMenu.Name = "itemMenu";
        itemMenu.Size = new Size(192, 92);
        itemMenu.Opening += itemMenu_Opening;
        //
        // menuDetails (same as double-clicking a row)
        //
        menuDetails.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        menuDetails.Name = "menuDetails";
        menuDetails.Size = new Size(191, 22);
        menuDetails.Text = "Show details";
        menuDetails.Click += menuDetails_Click;
        //
        // menuSeparator1
        //
        menuSeparator1.Name = "menuSeparator1";
        menuSeparator1.Size = new Size(188, 6);
        //
        // menuSeparator2
        //
        menuSeparator2.Name = "menuSeparator2";
        menuSeparator2.Size = new Size(188, 6);
        //
        // menuOpenOnDisk
        //
        menuOpenOnDisk.Name = "menuOpenOnDisk";
        menuOpenOnDisk.Size = new Size(191, 22);
        menuOpenOnDisk.Text = "Open on disk";
        menuOpenOnDisk.Click += menuOpenOnDisk_Click;
        // 
        // menuOpenImdb
        // 
        menuOpenImdb.Name = "menuOpenImdb";
        menuOpenImdb.Size = new Size(191, 22);
        menuOpenImdb.Text = "Open on IMDb";
        menuOpenImdb.Click += menuOpenImdb_Click;
        // 
        // menuOpenTmdb
        // 
        menuOpenTmdb.Name = "menuOpenTmdb";
        menuOpenTmdb.Size = new Size(191, 22);
        menuOpenTmdb.Text = "Open on TMDb";
        menuOpenTmdb.Click += menuOpenTmdb_Click;
        // 
        // menuRemove
        // 
        menuRemove.Name = "menuRemove";
        menuRemove.Size = new Size(191, 22);
        menuRemove.Text = "Remove from library…";
        menuRemove.Click += menuRemove_Click;
        // 
        // statusStrip
        // 
        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus });
        statusStrip.Location = new Point(0, 539);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(984, 22);
        statusStrip.TabIndex = 2;
        // 
        // lblStatus
        // 
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(39, 17);
        lblStatus.Text = "Ready";
        // 
        // KinoForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(984, 561);
        Controls.Add(dgvItems);
        Controls.Add(statusStrip);
        Controls.Add(sortRow);
        Controls.Add(typeFilter);
        Controls.Add(toolbar);
        Controls.Add(defaultKeysBanner);
        Name = "KinoForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Kolibri.Kino";
        Load += KinoForm_Load;
        toolbar.ResumeLayout(false);
        toolbar.PerformLayout();
        defaultKeysBanner.ResumeLayout(false);
        defaultKeysBanner.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvItems).EndInit();
        itemMenu.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)bindingSource).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private FlowLayoutPanel toolbar;
    private TextBox txtSearch;
    private Button btnSearchLocal;
    private Button btnSearchOnline;
    private Button btnImport;
    private Button btnLocalMovies;
    private Button btnAddToWatchlist;
    private Button btnWatchlists;
    private Button btnLocalSeries;
    private Button btnSettings;
    private FlowLayoutPanel typeFilter;
    private FlowLayoutPanel sortRow;
    private Label lblSort;
    private RadioButton rbSortNone;
    private RadioButton rbSortTitle;
    private RadioButton rbSortYear;
    private RadioButton rbSortRating;
    private RadioButton rbSortGenre;
    private Panel defaultKeysBanner;
    private Label lblDefaultKeys;
    private LinkLabel lnkSetKeys;
    private DataGridView dgvItems;
    private ContextMenuStrip itemMenu;
    private ToolStripMenuItem menuRemove;
    private ToolStripMenuItem menuOpenImdb;
    private ToolStripMenuItem menuOpenTmdb;
    private ToolStripMenuItem menuOpenOnDisk;
    private ToolStripMenuItem menuDetails;
    private ToolStripSeparator menuSeparator1;
    private ToolStripSeparator menuSeparator2;
    private BindingSource bindingSource;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
}
