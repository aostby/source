namespace Kolibri.Kino.WinForms.Forms;

partial class ManualLookupForm
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
        lblFile = new Label();
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
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        progress = new ToolStripProgressBar();
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
        // lblFile
        //
        lblFile.AutoEllipsis = true;
        lblFile.Dock = DockStyle.Top;
        lblFile.ForeColor = SystemColors.GrayText;
        lblFile.Name = "lblFile";
        lblFile.Padding = new Padding(9, 9, 9, 0);
        lblFile.Size = new Size(1084, 27);
        lblFile.TabIndex = 0;
        lblFile.Text = "File";
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
        searchBar.TabIndex = 1;
        //
        // lblQuery
        //
        lblQuery.AutoSize = true;
        lblQuery.Margin = new Padding(3, 10, 3, 0);
        lblQuery.Name = "lblQuery";
        lblQuery.TabIndex = 0;
        lblQuery.Text = "Title or IMDb id:";
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
        actions.Controls.Add(btnLink);
        actions.Controls.Add(btnImdb);
        actions.Dock = DockStyle.Bottom;
        actions.Name = "actions";
        actions.Padding = new Padding(6);
        actions.TabIndex = 3;
        //
        // btnLink
        //
        btnLink.AutoSize = true;
        btnLink.Name = "btnLink";
        btnLink.Size = new Size(120, 25);
        btnLink.TabIndex = 0;
        btnLink.Text = "Link to this file";
        btnLink.Click += btnLink_Click;
        //
        // btnImdb
        //
        btnImdb.AutoSize = true;
        btnImdb.Name = "btnImdb";
        btnImdb.Size = new Size(100, 25);
        btnImdb.TabIndex = 1;
        btnImdb.Text = "Open on IMDb";
        btnImdb.Click += btnImdb_Click;
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
        // ManualLookupForm
        //
        AcceptButton = btnSearch;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1084, 620);
        Controls.Add(split);
        Controls.Add(actions);
        Controls.Add(statusStrip);
        Controls.Add(searchBar);
        Controls.Add(lblFile);
        Name = "ManualLookupForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Find movie";
        Load += ManualLookupForm_Load;
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

    private Label lblFile;
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
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
    private ToolStripProgressBar progress;
}
