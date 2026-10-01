namespace Kolibri.Kino.WinForms.Forms;

partial class WatchlistsForm
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
        lblList = new Label();
        cboLists = new ComboBox();
        btnNewList = new Button();
        txtFilter = new TextBox();
        rbAll = new RadioButton();
        rbNotWatched = new RadioButton();
        rbWatched = new RadioButton();
        btnWatched = new Button();
        btnRemove = new Button();
        btnImdb = new Button();
        btnCopy = new Button();
        btnExport = new Button();
        btnPlex = new Button();
        split = new SplitContainer();
        dgvItems = new DataGridView();
        bindingSource = new BindingSource(components);
        details = new Kolibri.Kino.WinForms.Controls.ItemDetailsControl();
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        progress = new ToolStripProgressBar();
        toolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)split).BeginInit();
        split.Panel1.SuspendLayout();
        split.Panel2.SuspendLayout();
        split.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvItems).BeginInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // toolbar
        //
        toolbar.AutoSize = true;
        toolbar.Controls.Add(lblList);
        toolbar.Controls.Add(cboLists);
        toolbar.Controls.Add(btnNewList);
        toolbar.Controls.Add(txtFilter);
        toolbar.Controls.Add(rbAll);
        toolbar.Controls.Add(rbNotWatched);
        toolbar.Controls.Add(rbWatched);
        toolbar.Controls.Add(btnWatched);
        toolbar.Controls.Add(btnRemove);
        toolbar.Controls.Add(btnImdb);
        toolbar.Controls.Add(btnCopy);
        toolbar.Controls.Add(btnExport);
        toolbar.Controls.Add(btnPlex);
        toolbar.Dock = DockStyle.Top;
        toolbar.Name = "toolbar";
        toolbar.Padding = new Padding(6);
        toolbar.TabIndex = 0;
        //
        // lblList
        //
        lblList.AutoSize = true;
        lblList.Margin = new Padding(3, 10, 3, 0);
        lblList.Name = "lblList";
        lblList.TabIndex = 0;
        lblList.Text = "Watchlist:";
        //
        // cboLists
        //
        cboLists.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLists.Margin = new Padding(3, 7, 3, 3);
        cboLists.Name = "cboLists";
        cboLists.Size = new Size(200, 23);
        cboLists.TabIndex = 1;
        cboLists.SelectedIndexChanged += cboLists_SelectedIndexChanged;
        //
        // btnNewList
        //
        btnNewList.AutoSize = true;
        btnNewList.Margin = new Padding(3, 6, 3, 3);
        btnNewList.Name = "btnNewList";
        btnNewList.Size = new Size(75, 25);
        btnNewList.TabIndex = 2;
        btnNewList.Text = "New list…";
        btnNewList.Click += btnNewList_Click;
        //
        // txtFilter
        //
        txtFilter.Margin = new Padding(24, 7, 3, 3);
        txtFilter.Name = "txtFilter";
        txtFilter.PlaceholderText = "Filter title…";
        txtFilter.Size = new Size(180, 23);
        txtFilter.TabIndex = 3;
        txtFilter.TextChanged += Filter_Changed;
        //
        // rbAll
        //
        rbAll.AutoSize = true;
        rbAll.Checked = true;
        rbAll.Margin = new Padding(12, 9, 3, 3);
        rbAll.Name = "rbAll";
        rbAll.TabIndex = 4;
        rbAll.TabStop = true;
        rbAll.Text = "All";
        rbAll.CheckedChanged += Filter_Changed;
        //
        // rbNotWatched
        //
        rbNotWatched.AutoSize = true;
        rbNotWatched.Margin = new Padding(3, 9, 3, 3);
        rbNotWatched.Name = "rbNotWatched";
        rbNotWatched.TabIndex = 5;
        rbNotWatched.Text = "Not watched";
        rbNotWatched.CheckedChanged += Filter_Changed;
        //
        // rbWatched
        //
        rbWatched.AutoSize = true;
        rbWatched.Margin = new Padding(3, 9, 3, 3);
        rbWatched.Name = "rbWatched";
        rbWatched.TabIndex = 6;
        rbWatched.Text = "Watched";
        rbWatched.CheckedChanged += Filter_Changed;
        //
        // btnWatched
        //
        btnWatched.AutoSize = true;
        btnWatched.Margin = new Padding(24, 6, 3, 3);
        btnWatched.Name = "btnWatched";
        btnWatched.Size = new Size(110, 25);
        btnWatched.TabIndex = 7;
        btnWatched.Text = "Mark watched";
        btnWatched.Click += btnWatched_Click;
        //
        // btnRemove
        //
        btnRemove.AutoSize = true;
        btnRemove.Margin = new Padding(3, 6, 3, 3);
        btnRemove.Name = "btnRemove";
        btnRemove.Size = new Size(75, 25);
        btnRemove.TabIndex = 8;
        btnRemove.Text = "Remove…";
        btnRemove.Click += btnRemove_Click;
        //
        // btnImdb
        //
        btnImdb.AutoSize = true;
        btnImdb.Margin = new Padding(3, 6, 3, 3);
        btnImdb.Name = "btnImdb";
        btnImdb.Size = new Size(95, 25);
        btnImdb.TabIndex = 9;
        btnImdb.Text = "Open on IMDb";
        btnImdb.Click += btnImdb_Click;
        //
        // btnCopy
        //
        btnCopy.AutoSize = true;
        btnCopy.Margin = new Padding(24, 6, 3, 3);
        btnCopy.Name = "btnCopy";
        btnCopy.Size = new Size(75, 25);
        btnCopy.TabIndex = 10;
        btnCopy.Text = "Copy";
        btnCopy.Click += btnCopy_Click;
        //
        // btnExport
        //
        btnExport.AutoSize = true;
        btnExport.Margin = new Padding(3, 6, 3, 3);
        btnExport.Name = "btnExport";
        btnExport.Size = new Size(90, 25);
        btnExport.TabIndex = 11;
        btnExport.Text = "Export CSV…";
        btnExport.Click += btnExport_Click;
        //
        // btnPlex
        //
        btnPlex.AutoSize = true;
        btnPlex.Margin = new Padding(3, 6, 3, 3);
        btnPlex.Name = "btnPlex";
        btnPlex.Size = new Size(130, 25);
        btnPlex.TabIndex = 12;
        btnPlex.Text = "Copy to Plex playlist";
        btnPlex.Click += btnPlex_Click;
        //
        // split
        //
        split.Dock = DockStyle.Fill;
        split.Name = "split";
        //
        // split.Panel1
        //
        split.Panel1.Controls.Add(dgvItems);
        //
        // split.Panel2
        //
        split.Panel2.Controls.Add(details);
        split.Size = new Size(1184, 596);
        split.SplitterDistance = 600;
        split.TabIndex = 1;
        //
        // dgvItems
        //
        dgvItems.AllowUserToAddRows = false;
        dgvItems.AllowUserToDeleteRows = false;
        dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvItems.DataSource = bindingSource;
        dgvItems.Dock = DockStyle.Fill;
        dgvItems.MultiSelect = false;
        dgvItems.Name = "dgvItems";
        dgvItems.ReadOnly = true;
        dgvItems.RowHeadersVisible = false;
        dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvItems.TabIndex = 0;
        dgvItems.CellDoubleClick += dgvItems_CellDoubleClick;
        dgvItems.CellFormatting += dgvItems_CellFormatting;
        dgvItems.SelectionChanged += dgvItems_SelectionChanged;
        //
        // details
        //
        details.Dock = DockStyle.Fill;
        details.Name = "details";
        details.TabIndex = 0;
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
        // WatchlistsForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1184, 661);
        Controls.Add(split);
        Controls.Add(statusStrip);
        Controls.Add(toolbar);
        Name = "WatchlistsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Watchlists";
        Load += WatchlistsForm_Load;
        toolbar.ResumeLayout(false);
        toolbar.PerformLayout();
        split.Panel1.ResumeLayout(false);
        split.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)split).EndInit();
        split.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvItems).EndInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private FlowLayoutPanel toolbar;
    private Label lblList;
    private ComboBox cboLists;
    private Button btnNewList;
    private TextBox txtFilter;
    private RadioButton rbAll;
    private RadioButton rbNotWatched;
    private RadioButton rbWatched;
    private Button btnWatched;
    private Button btnRemove;
    private Button btnImdb;
    private Button btnCopy;
    private Button btnExport;
    private Button btnPlex;
    private SplitContainer split;
    private DataGridView dgvItems;
    private BindingSource bindingSource;
    private Kolibri.Kino.WinForms.Controls.ItemDetailsControl details;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
    private ToolStripProgressBar progress;
}
