namespace Kolibri.Kino.WinForms.Forms;

partial class LocalSeriesForm
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
        txtFilter = new TextBox();
        btnRefresh = new Button();
        btnSetFolder = new Button();
        btnAddToWatchlist = new Button();
        btnFindNew = new Button();
        mainSplit = new SplitContainer();
        dgvSeries = new DataGridView();
        seriesMenu = new ContextMenuStrip(components);
        menuChangeImdbId = new ToolStripMenuItem();
        menuOpenImdb = new ToolStripMenuItem();
        bindingSource = new BindingSource(components);
        rightSplit = new SplitContainer();
        seriesDetails = new Kolibri.Kino.WinForms.Controls.ItemDetailsControl();
        seasonSplit = new SplitContainer();
        tabSeasons = new TabControl();
        episodeDetails = new Kolibri.Kino.WinForms.Controls.ItemDetailsControl();
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        progress = new ToolStripProgressBar();
        toolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)mainSplit).BeginInit();
        mainSplit.Panel1.SuspendLayout();
        mainSplit.Panel2.SuspendLayout();
        mainSplit.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvSeries).BeginInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource).BeginInit();
        ((System.ComponentModel.ISupportInitialize)rightSplit).BeginInit();
        rightSplit.Panel1.SuspendLayout();
        rightSplit.Panel2.SuspendLayout();
        rightSplit.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)seasonSplit).BeginInit();
        seasonSplit.Panel1.SuspendLayout();
        seasonSplit.Panel2.SuspendLayout();
        seasonSplit.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // toolbar
        //
        toolbar.AutoSize = true;
        toolbar.Controls.Add(txtFilter);
        toolbar.Controls.Add(btnRefresh);
        toolbar.Controls.Add(btnSetFolder);
        toolbar.Controls.Add(btnAddToWatchlist);
        toolbar.Controls.Add(btnFindNew);
        toolbar.Dock = DockStyle.Top;
        toolbar.Name = "toolbar";
        toolbar.Padding = new Padding(6);
        toolbar.TabIndex = 0;
        //
        // txtFilter
        //
        txtFilter.Margin = new Padding(3, 7, 3, 3);
        txtFilter.Name = "txtFilter";
        txtFilter.PlaceholderText = "Filter title, then Enter…";
        txtFilter.Size = new Size(260, 23);
        txtFilter.TabIndex = 0;
        txtFilter.KeyDown += txtFilter_KeyDown;
        //
        // btnRefresh
        //
        btnRefresh.AutoSize = true;
        btnRefresh.Margin = new Padding(24, 6, 3, 3);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(110, 25);
        btnRefresh.TabIndex = 1;
        btnRefresh.Text = "Refresh episodes";
        btnRefresh.Click += btnRefresh_Click;
        //
        // btnSetFolder
        //
        btnSetFolder.AutoSize = true;
        btnSetFolder.Margin = new Padding(3, 6, 3, 3);
        btnSetFolder.Name = "btnSetFolder";
        btnSetFolder.Size = new Size(85, 25);
        btnSetFolder.TabIndex = 2;
        btnSetFolder.Text = "Set folder…";
        btnSetFolder.Click += btnSetFolder_Click;
        //
        // btnAddToWatchlist
        //
        btnAddToWatchlist.AutoSize = true;
        btnAddToWatchlist.Margin = new Padding(3, 6, 3, 3);
        btnAddToWatchlist.Name = "btnAddToWatchlist";
        btnAddToWatchlist.Size = new Size(110, 25);
        btnAddToWatchlist.TabIndex = 3;
        btnAddToWatchlist.Text = "Add to watchlist…";
        btnAddToWatchlist.Click += btnAddToWatchlist_Click;
        //
        // btnFindNew
        //
        btnFindNew.AutoSize = true;
        btnFindNew.Margin = new Padding(24, 6, 3, 3);
        btnFindNew.Name = "btnFindNew";
        btnFindNew.Size = new Size(110, 25);
        btnFindNew.TabIndex = 4;
        btnFindNew.Text = "Find new series…";
        btnFindNew.Click += btnFindNew_Click;
        //
        // mainSplit: series list | details
        //
        mainSplit.Dock = DockStyle.Fill;
        mainSplit.Name = "mainSplit";
        mainSplit.Panel1.Controls.Add(dgvSeries);
        mainSplit.Panel2.Controls.Add(rightSplit);
        mainSplit.Size = new Size(1384, 772);
        mainSplit.SplitterDistance = 430;
        mainSplit.TabIndex = 1;
        //
        // dgvSeries
        //
        dgvSeries.AllowUserToAddRows = false;
        dgvSeries.AllowUserToDeleteRows = false;
        dgvSeries.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvSeries.DataSource = bindingSource;
        dgvSeries.Dock = DockStyle.Fill;
        dgvSeries.MultiSelect = false;
        dgvSeries.Name = "dgvSeries";
        dgvSeries.ReadOnly = true;
        dgvSeries.RowHeadersVisible = false;
        dgvSeries.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvSeries.TabIndex = 0;
        dgvSeries.CellFormatting += dgvSeries_CellFormatting;
        dgvSeries.CellMouseDown += dgvSeries_CellMouseDown;
        dgvSeries.ContextMenuStrip = seriesMenu;
        dgvSeries.SelectionChanged += dgvSeries_SelectionChanged;
        //
        // seriesMenu (right-click on a series)
        //
        seriesMenu.Items.AddRange(new ToolStripItem[] { menuChangeImdbId, menuOpenImdb });
        seriesMenu.Name = "seriesMenu";
        seriesMenu.Opening += seriesMenu_Opening;
        //
        // menuChangeImdbId
        //
        menuChangeImdbId.Name = "menuChangeImdbId";
        menuChangeImdbId.Text = "Change IMDb id…";
        menuChangeImdbId.Click += menuChangeImdbId_Click;
        //
        // menuOpenImdb
        //
        menuOpenImdb.Name = "menuOpenImdb";
        menuOpenImdb.Text = "Open on IMDb";
        menuOpenImdb.Click += menuOpenImdb_Click;
        //
        // rightSplit: series details above, seasons below
        //
        rightSplit.Dock = DockStyle.Fill;
        rightSplit.Name = "rightSplit";
        rightSplit.Orientation = Orientation.Horizontal;
        rightSplit.Panel1.Controls.Add(seriesDetails);
        rightSplit.Panel2.Controls.Add(seasonSplit);
        rightSplit.Size = new Size(950, 772);
        rightSplit.SplitterDistance = 330;
        rightSplit.TabIndex = 0;
        //
        // seriesDetails
        //
        seriesDetails.Dock = DockStyle.Fill;
        seriesDetails.Name = "seriesDetails";
        seriesDetails.TabIndex = 0;
        //
        // seasonSplit: season tabs | episode details
        //
        seasonSplit.Dock = DockStyle.Fill;
        seasonSplit.Name = "seasonSplit";
        seasonSplit.Panel1.Controls.Add(tabSeasons);
        seasonSplit.Panel2.Controls.Add(episodeDetails);
        seasonSplit.Size = new Size(950, 438);
        seasonSplit.SplitterDistance = 500;
        seasonSplit.TabIndex = 0;
        //
        // tabSeasons (one page per season, added in code)
        //
        tabSeasons.Dock = DockStyle.Fill;
        tabSeasons.Name = "tabSeasons";
        tabSeasons.SelectedIndex = 0;
        tabSeasons.TabIndex = 0;
        tabSeasons.SelectedIndexChanged += tabSeasons_SelectedIndexChanged;
        //
        // episodeDetails
        //
        episodeDetails.Dock = DockStyle.Fill;
        episodeDetails.Name = "episodeDetails";
        episodeDetails.TabIndex = 0;
        //
        // statusStrip
        //
        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus, progress });
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
        // progress
        //
        progress.MarqueeAnimationSpeed = 30;
        progress.Name = "progress";
        progress.Size = new Size(120, 16);
        progress.Style = ProgressBarStyle.Marquee;
        progress.Visible = false;
        //
        // LocalSeriesForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1384, 861);
        Controls.Add(mainSplit);
        Controls.Add(statusStrip);
        Controls.Add(toolbar);
        Name = "LocalSeriesForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Local series";
        Load += LocalSeriesForm_Load;
        toolbar.ResumeLayout(false);
        toolbar.PerformLayout();
        mainSplit.Panel1.ResumeLayout(false);
        mainSplit.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)mainSplit).EndInit();
        mainSplit.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvSeries).EndInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource).EndInit();
        rightSplit.Panel1.ResumeLayout(false);
        rightSplit.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)rightSplit).EndInit();
        rightSplit.ResumeLayout(false);
        seasonSplit.Panel1.ResumeLayout(false);
        seasonSplit.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)seasonSplit).EndInit();
        seasonSplit.ResumeLayout(false);
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private FlowLayoutPanel toolbar;
    private TextBox txtFilter;
    private Button btnRefresh;
    private Button btnSetFolder;
    private Button btnAddToWatchlist;
    private Button btnFindNew;
    private SplitContainer mainSplit;
    private DataGridView dgvSeries;
    private ContextMenuStrip seriesMenu;
    private ToolStripMenuItem menuChangeImdbId;
    private ToolStripMenuItem menuOpenImdb;
    private BindingSource bindingSource;
    private SplitContainer rightSplit;
    private Kolibri.Kino.WinForms.Controls.ItemDetailsControl seriesDetails;
    private SplitContainer seasonSplit;
    private TabControl tabSeasons;
    private Kolibri.Kino.WinForms.Controls.ItemDetailsControl episodeDetails;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
    private ToolStripProgressBar progress;
}
