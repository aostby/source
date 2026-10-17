namespace Kolibri.Kino.WinForms.Forms;

partial class WishlistForm
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
        searchBar = new FlowLayoutPanel();
        txtSearch = new TextBox();
        btnSearch = new Button();
        btnAdd = new Button();
        split = new SplitContainer();
        grpHits = new GroupBox();
        dgvHits = new DataGridView();
        grpWishlist = new GroupBox();
        dgvWishlist = new DataGridView();
        listButtons = new FlowLayoutPanel();
        btnRemove = new Button();
        btnImdb = new Button();
        btnImport = new Button();
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        hitsSource = new BindingSource(components);
        wishlistSource = new BindingSource(components);
        searchBar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)split).BeginInit();
        split.Panel1.SuspendLayout();
        split.Panel2.SuspendLayout();
        split.SuspendLayout();
        grpHits.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvHits).BeginInit();
        grpWishlist.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvWishlist).BeginInit();
        listButtons.SuspendLayout();
        statusStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)hitsSource).BeginInit();
        ((System.ComponentModel.ISupportInitialize)wishlistSource).BeginInit();
        SuspendLayout();
        //
        // searchBar
        //
        searchBar.AutoSize = true;
        searchBar.Controls.Add(txtSearch);
        searchBar.Controls.Add(btnSearch);
        searchBar.Controls.Add(btnAdd);
        searchBar.Dock = DockStyle.Top;
        searchBar.Name = "searchBar";
        searchBar.Padding = new Padding(6);
        searchBar.TabIndex = 0;
        //
        // txtSearch
        //
        txtSearch.Name = "txtSearch";
        txtSearch.PlaceholderText = "Title, or title and year (Kabul, 2025), or IMDb id…";
        txtSearch.Size = new Size(320, 23);
        txtSearch.TabIndex = 0;
        txtSearch.KeyDown += txtSearch_KeyDown;
        //
        // btnSearch
        //
        btnSearch.AutoSize = true;
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(75, 25);
        btnSearch.TabIndex = 1;
        btnSearch.Text = "Search";
        btnSearch.UseVisualStyleBackColor = true;
        btnSearch.Click += btnSearch_Click;
        //
        // btnAdd
        //
        btnAdd.AutoSize = true;
        btnAdd.Margin = new Padding(24, 3, 3, 3);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(110, 25);
        btnAdd.TabIndex = 2;
        btnAdd.Text = "Add to wishlist";
        btnAdd.UseVisualStyleBackColor = true;
        btnAdd.Click += btnAdd_Click;
        //
        // split
        //
        split.Dock = DockStyle.Fill;
        split.Name = "split";
        split.Orientation = Orientation.Horizontal;
        split.Panel1.Controls.Add(grpHits);
        split.Panel2.Controls.Add(grpWishlist);
        split.Size = new Size(884, 480);
        split.SplitterDistance = 200;
        split.TabIndex = 1;
        //
        // grpHits
        //
        grpHits.Controls.Add(dgvHits);
        grpHits.Dock = DockStyle.Fill;
        grpHits.Name = "grpHits";
        grpHits.Padding = new Padding(6);
        grpHits.TabIndex = 0;
        grpHits.TabStop = false;
        grpHits.Text = "Search: SilverScreen first, then OMDb and TMDb";
        //
        // dgvHits
        //
        dgvHits.AllowUserToAddRows = false;
        dgvHits.AllowUserToDeleteRows = false;
        dgvHits.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvHits.BackgroundColor = SystemColors.Window;
        dgvHits.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvHits.DataSource = hitsSource;
        dgvHits.Dock = DockStyle.Fill;
        dgvHits.MultiSelect = false;
        dgvHits.Name = "dgvHits";
        dgvHits.ReadOnly = true;
        dgvHits.RowHeadersVisible = false;
        dgvHits.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvHits.TabIndex = 0;
        dgvHits.CellDoubleClick += Grid_CellDoubleClick;
        dgvHits.CellFormatting += Grid_CellFormatting;
        //
        // grpWishlist
        //
        grpWishlist.Controls.Add(dgvWishlist);
        grpWishlist.Controls.Add(listButtons);
        grpWishlist.Dock = DockStyle.Fill;
        grpWishlist.Name = "grpWishlist";
        grpWishlist.Padding = new Padding(6);
        grpWishlist.TabIndex = 0;
        grpWishlist.TabStop = false;
        grpWishlist.Text = "Wishlist (to get first)";
        //
        // dgvWishlist
        //
        dgvWishlist.AllowUserToAddRows = false;
        dgvWishlist.AllowUserToDeleteRows = false;
        dgvWishlist.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvWishlist.BackgroundColor = SystemColors.Window;
        dgvWishlist.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvWishlist.DataSource = wishlistSource;
        dgvWishlist.Dock = DockStyle.Fill;
        dgvWishlist.MultiSelect = false;
        dgvWishlist.Name = "dgvWishlist";
        dgvWishlist.ReadOnly = true;
        dgvWishlist.RowHeadersVisible = false;
        dgvWishlist.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvWishlist.TabIndex = 0;
        dgvWishlist.CellDoubleClick += Grid_CellDoubleClick;
        dgvWishlist.CellFormatting += Grid_CellFormatting;
        //
        // listButtons
        //
        listButtons.AutoSize = true;
        listButtons.Controls.Add(btnRemove);
        listButtons.Controls.Add(btnImdb);
        listButtons.Controls.Add(btnImport);
        listButtons.Dock = DockStyle.Bottom;
        listButtons.Name = "listButtons";
        listButtons.TabIndex = 1;
        //
        // btnRemove
        //
        btnRemove.AutoSize = true;
        btnRemove.Name = "btnRemove";
        btnRemove.Size = new Size(75, 25);
        btnRemove.TabIndex = 0;
        btnRemove.Text = "Remove";
        btnRemove.UseVisualStyleBackColor = true;
        btnRemove.Click += btnRemove_Click;
        //
        // btnImdb
        //
        btnImdb.AutoSize = true;
        btnImdb.Name = "btnImdb";
        btnImdb.Size = new Size(100, 25);
        btnImdb.TabIndex = 1;
        btnImdb.Text = "Open on IMDb";
        btnImdb.UseVisualStyleBackColor = true;
        btnImdb.Click += btnImdb_Click;
        //
        // btnImport
        //
        btnImport.AutoSize = true;
        btnImport.Margin = new Padding(24, 3, 3, 3);
        btnImport.Name = "btnImport";
        btnImport.Size = new Size(100, 25);
        btnImport.TabIndex = 2;
        btnImport.Text = "Import CSV…";
        btnImport.UseVisualStyleBackColor = true;
        btnImport.Click += btnImport_Click;
        //
        // statusStrip
        //
        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus });
        statusStrip.Name = "statusStrip";
        statusStrip.TabIndex = 2;
        //
        // lblStatus
        //
        lblStatus.Name = "lblStatus";
        lblStatus.Spring = true;
        lblStatus.Text = "Ready";
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        //
        // WishlistForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(884, 561);
        Controls.Add(split);
        Controls.Add(searchBar);
        Controls.Add(statusStrip);
        Name = "WishlistForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Wishlist";
        Load += WishlistForm_Load;
        searchBar.ResumeLayout(false);
        searchBar.PerformLayout();
        split.Panel1.ResumeLayout(false);
        split.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)split).EndInit();
        split.ResumeLayout(false);
        grpHits.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvHits).EndInit();
        grpWishlist.ResumeLayout(false);
        grpWishlist.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvWishlist).EndInit();
        listButtons.ResumeLayout(false);
        listButtons.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)hitsSource).EndInit();
        ((System.ComponentModel.ISupportInitialize)wishlistSource).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private FlowLayoutPanel searchBar;
    private TextBox txtSearch;
    private Button btnSearch;
    private Button btnAdd;
    private SplitContainer split;
    private GroupBox grpHits;
    private DataGridView dgvHits;
    private GroupBox grpWishlist;
    private DataGridView dgvWishlist;
    private FlowLayoutPanel listButtons;
    private Button btnRemove;
    private Button btnImdb;
    private Button btnImport;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
    private BindingSource hitsSource;
    private BindingSource wishlistSource;
}
