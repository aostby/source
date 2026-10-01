namespace Kolibri.Kino.WinForms.Forms;

partial class SeriesLookupDialog
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
        lblFolder = new Label();
        guessBar = new FlowLayoutPanel();
        lblGuess = new Label();
        btnEnterId = new Button();
        btnRemoveTag = new Button();
        searchBar = new FlowLayoutPanel();
        lblQuery = new Label();
        txtQuery = new TextBox();
        lblYear = new Label();
        txtYear = new TextBox();
        btnSearch = new Button();
        split = new SplitContainer();
        dgvCandidates = new DataGridView();
        bindingSource = new BindingSource(components);
        details = new Kolibri.Kino.WinForms.Controls.ItemDetailsControl();
        actions = new FlowLayoutPanel();
        btnLink = new Button();
        btnImdb = new Button();
        btnOpenFolder = new Button();
        btnSkip = new Button();
        btnIgnore = new Button();
        btnStop = new Button();
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        progress = new ToolStripProgressBar();
        guessBar.SuspendLayout();
        searchBar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)split).BeginInit();
        split.Panel1.SuspendLayout();
        split.Panel2.SuspendLayout();
        split.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvCandidates).BeginInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource).BeginInit();
        actions.SuspendLayout();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // lblFolder
        //
        lblFolder.AutoEllipsis = true;
        lblFolder.Dock = DockStyle.Top;
        lblFolder.Name = "lblFolder";
        lblFolder.Padding = new Padding(9, 9, 9, 0);
        lblFolder.Size = new Size(1084, 50);
        lblFolder.TabIndex = 0;
        lblFolder.Text = "Folder";
        //
        // guessBar
        //
        guessBar.AutoSize = true;
        guessBar.BackColor = SystemColors.Info;
        guessBar.Controls.Add(lblGuess);
        guessBar.Controls.Add(btnLink);
        guessBar.Controls.Add(btnEnterId);
        guessBar.Controls.Add(btnRemoveTag);
        guessBar.Dock = DockStyle.Top;
        guessBar.Name = "guessBar";
        guessBar.Padding = new Padding(6);
        guessBar.TabIndex = 1;
        //
        // lblGuess
        //
        lblGuess.AutoSize = true;
        lblGuess.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblGuess.ForeColor = SystemColors.InfoText;
        lblGuess.Margin = new Padding(3, 9, 12, 0);
        lblGuess.MaximumSize = new Size(1060, 0);
        lblGuess.Name = "lblGuess";
        lblGuess.TabIndex = 0;
        lblGuess.Text = "We think this is:";
        //
        // btnEnterId
        //
        btnEnterId.AutoSize = true;
        btnEnterId.Margin = new Padding(3, 6, 3, 3);
        btnEnterId.Name = "btnEnterId";
        btnEnterId.Size = new Size(170, 25);
        btnEnterId.TabIndex = 2;
        btnEnterId.Text = "Wrong? Enter the IMDb id…";
        btnEnterId.Click += btnEnterId_Click;
        //
        // btnRemoveTag
        //
        btnRemoveTag.AutoSize = true;
        btnRemoveTag.Margin = new Padding(3, 6, 3, 3);
        btnRemoveTag.Name = "btnRemoveTag";
        btnRemoveTag.Size = new Size(220, 25);
        btnRemoveTag.TabIndex = 3;
        btnRemoveTag.Text = "Remove the IMDb id from the folder name…";
        btnRemoveTag.Click += btnRemoveTag_Click;
        //
        // searchBar
        //
        searchBar.AutoSize = true;
        searchBar.Controls.Add(lblQuery);
        searchBar.Controls.Add(txtQuery);
        searchBar.Controls.Add(lblYear);
        searchBar.Controls.Add(txtYear);
        searchBar.Controls.Add(btnSearch);
        searchBar.Dock = DockStyle.Top;
        searchBar.Name = "searchBar";
        searchBar.Padding = new Padding(6);
        searchBar.TabIndex = 2;
        //
        // lblQuery
        //
        lblQuery.AutoSize = true;
        lblQuery.Margin = new Padding(3, 10, 3, 0);
        lblQuery.Name = "lblQuery";
        lblQuery.TabIndex = 0;
        lblQuery.Text = "Or search by title:";
        //
        // txtQuery
        //
        txtQuery.Margin = new Padding(3, 7, 3, 3);
        txtQuery.Name = "txtQuery";
        txtQuery.Size = new Size(360, 23);
        txtQuery.TabIndex = 1;
        //
        // lblYear
        //
        lblYear.AutoSize = true;
        lblYear.Margin = new Padding(12, 10, 3, 0);
        lblYear.Name = "lblYear";
        lblYear.TabIndex = 2;
        lblYear.Text = "Year:";
        //
        // txtYear
        //
        txtYear.Margin = new Padding(3, 7, 3, 3);
        txtYear.MaxLength = 4;
        txtYear.Name = "txtYear";
        txtYear.Size = new Size(60, 23);
        txtYear.TabIndex = 3;
        //
        // btnSearch
        //
        btnSearch.AutoSize = true;
        btnSearch.Margin = new Padding(12, 6, 3, 3);
        btnSearch.Name = "btnSearch";
        btnSearch.Size = new Size(75, 25);
        btnSearch.TabIndex = 4;
        btnSearch.Text = "Search";
        btnSearch.Click += btnSearch_Click;
        //
        // split
        //
        split.Dock = DockStyle.Fill;
        split.Name = "split";
        //
        // split.Panel1
        //
        split.Panel1.Controls.Add(dgvCandidates);
        //
        // split.Panel2
        //
        split.Panel2.Controls.Add(details);
        split.Size = new Size(1084, 480);
        split.SplitterDistance = 500;
        split.TabIndex = 2;
        //
        // dgvCandidates
        //
        dgvCandidates.AllowUserToAddRows = false;
        dgvCandidates.AllowUserToDeleteRows = false;
        dgvCandidates.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvCandidates.DataSource = bindingSource;
        dgvCandidates.Dock = DockStyle.Fill;
        dgvCandidates.MultiSelect = false;
        dgvCandidates.Name = "dgvCandidates";
        dgvCandidates.ReadOnly = true;
        dgvCandidates.RowHeadersVisible = false;
        dgvCandidates.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvCandidates.TabIndex = 0;
        dgvCandidates.CellDoubleClick += dgvCandidates_CellDoubleClick;
        dgvCandidates.SelectionChanged += dgvCandidates_SelectionChanged;
        //
        // details
        //
        details.Dock = DockStyle.Fill;
        details.Name = "details";
        details.TabIndex = 0;
        //
        // actions
        //
        actions.AutoSize = true;
        actions.Controls.Add(btnImdb);
        actions.Controls.Add(btnOpenFolder);
        actions.Controls.Add(btnSkip);
        actions.Controls.Add(btnIgnore);
        actions.Controls.Add(btnStop);
        actions.Dock = DockStyle.Bottom;
        actions.Name = "actions";
        actions.Padding = new Padding(6);
        actions.TabIndex = 3;
        //
        // btnLink
        //
        btnLink.AutoSize = true;
        btnLink.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnLink.Margin = new Padding(3, 6, 3, 3);
        btnLink.Name = "btnLink";
        btnLink.Size = new Size(120, 25);
        btnLink.TabIndex = 1;
        btnLink.Text = "Yes, link it";
        btnLink.Click += btnLink_Click;
        //
        // btnImdb
        //
        btnImdb.AutoSize = true;
        btnImdb.Name = "btnImdb";
        btnImdb.Size = new Size(100, 25);
        btnImdb.TabIndex = 0;
        btnImdb.Text = "Open on IMDb";
        btnImdb.Click += btnImdb_Click;
        //
        // btnOpenFolder
        //
        btnOpenFolder.AutoSize = true;
        btnOpenFolder.Name = "btnOpenFolder";
        btnOpenFolder.Size = new Size(90, 25);
        btnOpenFolder.TabIndex = 2;
        btnOpenFolder.Text = "Open folder";
        btnOpenFolder.Click += btnOpenFolder_Click;
        //
        // btnSkip
        //
        btnSkip.AutoSize = true;
        btnSkip.Margin = new Padding(24, 3, 3, 3);
        btnSkip.Name = "btnSkip";
        btnSkip.Size = new Size(75, 25);
        btnSkip.TabIndex = 3;
        btnSkip.Text = "Skip";
        btnSkip.Click += btnSkip_Click;
        //
        // btnIgnore
        //
        btnIgnore.AutoSize = true;
        btnIgnore.Name = "btnIgnore";
        btnIgnore.Size = new Size(110, 25);
        btnIgnore.TabIndex = 4;
        btnIgnore.Text = "Don't ask again";
        btnIgnore.Click += btnIgnore_Click;
        //
        // btnStop
        //
        btnStop.AutoSize = true;
        btnStop.Margin = new Padding(24, 3, 3, 3);
        btnStop.Name = "btnStop";
        btnStop.Size = new Size(90, 25);
        btnStop.TabIndex = 5;
        btnStop.Text = "Stop asking";
        btnStop.Click += btnStop_Click;
        //
        // statusStrip
        //
        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus, progress });
        statusStrip.Name = "statusStrip";
        statusStrip.TabIndex = 4;
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
        // SeriesLookupDialog
        //
        AcceptButton = btnSearch;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1084, 620);
        Controls.Add(split);
        Controls.Add(actions);
        Controls.Add(statusStrip);
        Controls.Add(searchBar);
        Controls.Add(guessBar);
        Controls.Add(lblFolder);
        Name = "SeriesLookupDialog";
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        Text = "Find series";
        Load += SeriesLookupDialog_Load;
        guessBar.ResumeLayout(false);
        guessBar.PerformLayout();
        searchBar.ResumeLayout(false);
        searchBar.PerformLayout();
        split.Panel1.ResumeLayout(false);
        split.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)split).EndInit();
        split.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvCandidates).EndInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource).EndInit();
        actions.ResumeLayout(false);
        actions.PerformLayout();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblFolder;
    private FlowLayoutPanel guessBar;
    private Label lblGuess;
    private Button btnEnterId;
    private Button btnRemoveTag;
    private FlowLayoutPanel searchBar;
    private Label lblQuery;
    private TextBox txtQuery;
    private Label lblYear;
    private TextBox txtYear;
    private Button btnSearch;
    private SplitContainer split;
    private DataGridView dgvCandidates;
    private BindingSource bindingSource;
    private Kolibri.Kino.WinForms.Controls.ItemDetailsControl details;
    private FlowLayoutPanel actions;
    private Button btnLink;
    private Button btnImdb;
    private Button btnOpenFolder;
    private Button btnSkip;
    private Button btnIgnore;
    private Button btnStop;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
    private ToolStripProgressBar progress;
}
