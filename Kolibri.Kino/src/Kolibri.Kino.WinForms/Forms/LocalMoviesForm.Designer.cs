namespace Kolibri.Kino.WinForms.Forms;

partial class LocalMoviesForm
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
        lblFolderCaption = new Label();
        txtFolder = new TextBox();
        btnBrowse = new Button();
        rbAll = new RadioButton();
        rbMissing = new RadioButton();
        rbNotInLibrary = new RadioButton();
        btnRefresh = new Button();
        btnScanNew = new Button();
        btnScanAll = new Button();
        btnRemoveMissing = new Button();
        btnFindMovie = new Button();
        btnCleanup = new Button();
        btnAddToWatchlist = new Button();
        split = new SplitContainer();
        dgvMovies = new DataGridView();
        bindingSource = new BindingSource(components);
        details = new Kolibri.Kino.WinForms.Controls.ItemDetailsControl();
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        progress = new ToolStripProgressBar();
        btnCancel = new ToolStripButton();
        toolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)split).BeginInit();
        split.Panel1.SuspendLayout();
        split.Panel2.SuspendLayout();
        split.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvMovies).BeginInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // toolbar
        //
        toolbar.AutoSize = true;
        toolbar.Controls.Add(lblFolderCaption);
        toolbar.Controls.Add(txtFolder);
        toolbar.Controls.Add(btnBrowse);
        toolbar.Controls.Add(rbAll);
        toolbar.Controls.Add(rbMissing);
        toolbar.Controls.Add(rbNotInLibrary);
        toolbar.Controls.Add(btnRefresh);
        toolbar.Controls.Add(btnScanNew);
        toolbar.Controls.Add(btnScanAll);
        toolbar.Controls.Add(btnRemoveMissing);
        toolbar.Controls.Add(btnFindMovie);
        toolbar.Controls.Add(btnCleanup);
        toolbar.Controls.Add(btnAddToWatchlist);
        toolbar.Dock = DockStyle.Top;
        toolbar.Location = new Point(0, 0);
        toolbar.Name = "toolbar";
        toolbar.Padding = new Padding(6);
        toolbar.Size = new Size(1184, 43);
        toolbar.TabIndex = 0;
        //
        // lblFolderCaption
        //
        lblFolderCaption.AutoSize = true;
        lblFolderCaption.Margin = new Padding(3, 10, 3, 0);
        lblFolderCaption.Name = "lblFolderCaption";
        lblFolderCaption.TabIndex = 0;
        lblFolderCaption.Text = "Folder:";
        //
        // txtFolder
        //
        txtFolder.Margin = new Padding(3, 7, 3, 3);
        txtFolder.Name = "txtFolder";
        txtFolder.ReadOnly = true;
        txtFolder.Size = new Size(420, 23);
        txtFolder.TabIndex = 1;
        //
        // btnBrowse
        //
        btnBrowse.AutoSize = true;
        btnBrowse.Margin = new Padding(3, 6, 3, 3);
        btnBrowse.Name = "btnBrowse";
        btnBrowse.Size = new Size(75, 25);
        btnBrowse.TabIndex = 2;
        btnBrowse.Text = "Browse…";
        btnBrowse.Click += btnBrowse_Click;
        //
        // rbAll
        //
        rbAll.AutoSize = true;
        rbAll.Checked = true;
        rbAll.Margin = new Padding(24, 9, 3, 3);
        rbAll.Name = "rbAll";
        rbAll.TabIndex = 3;
        rbAll.TabStop = true;
        rbAll.Text = "All";
        rbAll.CheckedChanged += Filter_CheckedChanged;
        //
        // rbMissing
        //
        rbMissing.AutoSize = true;
        rbMissing.Margin = new Padding(3, 9, 3, 3);
        rbMissing.Name = "rbMissing";
        rbMissing.TabIndex = 4;
        rbMissing.Text = "Missing on disk";
        rbMissing.CheckedChanged += Filter_CheckedChanged;
        //
        // rbNotInLibrary
        //
        rbNotInLibrary.AutoSize = true;
        rbNotInLibrary.Margin = new Padding(3, 9, 3, 3);
        rbNotInLibrary.Name = "rbNotInLibrary";
        rbNotInLibrary.TabIndex = 5;
        rbNotInLibrary.Text = "Not in library";
        rbNotInLibrary.CheckedChanged += Filter_CheckedChanged;
        //
        // btnRefresh
        //
        btnRefresh.AutoSize = true;
        btnRefresh.Margin = new Padding(24, 6, 3, 3);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(75, 25);
        btnRefresh.TabIndex = 6;
        btnRefresh.Text = "Refresh";
        btnRefresh.Click += btnRefresh_Click;
        //
        // btnScanNew
        //
        btnScanNew.AutoSize = true;
        btnScanNew.Margin = new Padding(24, 6, 3, 3);
        btnScanNew.Name = "btnScanNew";
        btnScanNew.Size = new Size(100, 25);
        btnScanNew.TabIndex = 7;
        btnScanNew.Text = "Scan new files";
        btnScanNew.Click += btnScanNew_Click;
        //
        // btnScanAll
        //
        btnScanAll.AutoSize = true;
        btnScanAll.Margin = new Padding(3, 6, 3, 3);
        btnScanAll.Name = "btnScanAll";
        btnScanAll.Size = new Size(140, 25);
        btnScanAll.TabIndex = 8;
        btnScanAll.Text = "Scan all + refresh details";
        btnScanAll.Click += btnScanAll_Click;
        //
        // btnRemoveMissing
        //
        btnRemoveMissing.AutoSize = true;
        btnRemoveMissing.Margin = new Padding(3, 6, 3, 3);
        btnRemoveMissing.Name = "btnRemoveMissing";
        btnRemoveMissing.Size = new Size(110, 25);
        btnRemoveMissing.TabIndex = 9;
        btnRemoveMissing.Text = "Remove missing…";
        btnRemoveMissing.Click += btnRemoveMissing_Click;
        //
        // btnFindMovie
        //
        btnFindMovie.AutoSize = true;
        btnFindMovie.Margin = new Padding(3, 6, 3, 3);
        btnFindMovie.Name = "btnFindMovie";
        btnFindMovie.Size = new Size(120, 25);
        btnFindMovie.TabIndex = 10;
        btnFindMovie.Text = "Find movie for file…";
        btnFindMovie.Click += btnFindMovie_Click;
        //
        // btnCleanup
        //
        btnCleanup.AutoSize = true;
        btnCleanup.Margin = new Padding(3, 6, 3, 3);
        btnCleanup.Name = "btnCleanup";
        btnCleanup.Size = new Size(110, 25);
        btnCleanup.TabIndex = 11;
        btnCleanup.Text = "Clean up folder…";
        btnCleanup.Click += btnCleanup_Click;
        //
        // btnAddToWatchlist
        //
        btnAddToWatchlist.AutoSize = true;
        btnAddToWatchlist.Margin = new Padding(3, 6, 3, 3);
        btnAddToWatchlist.Name = "btnAddToWatchlist";
        btnAddToWatchlist.Size = new Size(110, 25);
        btnAddToWatchlist.TabIndex = 12;
        btnAddToWatchlist.Text = "Add to watchlist…";
        btnAddToWatchlist.Click += btnAddToWatchlist_Click;
        //
        // split
        //
        split.Dock = DockStyle.Fill;
        split.Location = new Point(0, 43);
        split.Name = "split";
        //
        // split.Panel1
        //
        split.Panel1.Controls.Add(dgvMovies);
        //
        // split.Panel2
        //
        split.Panel2.Controls.Add(details);
        split.Size = new Size(1184, 596);
        split.SplitterDistance = 600;
        split.TabIndex = 1;
        //
        // dgvMovies
        //
        dgvMovies.AllowUserToAddRows = false;
        dgvMovies.AllowUserToDeleteRows = false;
        dgvMovies.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvMovies.DataSource = bindingSource;
        dgvMovies.Dock = DockStyle.Fill;
        dgvMovies.MultiSelect = false;
        dgvMovies.Name = "dgvMovies";
        dgvMovies.ReadOnly = true;
        dgvMovies.RowHeadersVisible = false;
        dgvMovies.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvMovies.TabIndex = 0;
        dgvMovies.CellFormatting += dgvMovies_CellFormatting;
        dgvMovies.CellDoubleClick += dgvMovies_CellDoubleClick;
        dgvMovies.SelectionChanged += dgvMovies_SelectionChanged;
        //
        // details
        //
        details.Dock = DockStyle.Fill;
        details.Name = "details";
        details.TabIndex = 0;
        //
        // statusStrip
        //
        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus, progress, btnCancel });
        statusStrip.Location = new Point(0, 639);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1184, 22);
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
        // btnCancel
        //
        btnCancel.DisplayStyle = ToolStripItemDisplayStyle.Text;
        btnCancel.Name = "btnCancel";
        btnCancel.Text = "Cancel";
        btnCancel.Visible = false;
        btnCancel.Click += btnCancel_Click;
        //
        // LocalMoviesForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1184, 661);
        Controls.Add(split);
        Controls.Add(statusStrip);
        Controls.Add(toolbar);
        Name = "LocalMoviesForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Local movies";
        Load += LocalMoviesForm_Load;
        toolbar.ResumeLayout(false);
        toolbar.PerformLayout();
        split.Panel1.ResumeLayout(false);
        split.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)split).EndInit();
        split.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvMovies).EndInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private FlowLayoutPanel toolbar;
    private Label lblFolderCaption;
    private TextBox txtFolder;
    private Button btnBrowse;
    private RadioButton rbAll;
    private RadioButton rbMissing;
    private RadioButton rbNotInLibrary;
    private Button btnRefresh;
    private Button btnScanNew;
    private Button btnScanAll;
    private Button btnRemoveMissing;
    private Button btnFindMovie;
    private Button btnCleanup;
    private Button btnAddToWatchlist;
    private SplitContainer split;
    private DataGridView dgvMovies;
    private BindingSource bindingSource;
    private Kolibri.Kino.WinForms.Controls.ItemDetailsControl details;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
    private ToolStripProgressBar progress;
    private ToolStripButton btnCancel;
}
