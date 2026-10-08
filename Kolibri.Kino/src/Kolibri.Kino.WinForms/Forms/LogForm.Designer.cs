namespace Kolibri.Kino.WinForms.Forms;

partial class LogForm
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
        filters = new FlowLayoutPanel();
        lblPeriod = new Label();
        cboPeriod = new ComboBox();
        lblLevel = new Label();
        cboLevel = new ComboBox();
        txtSearch = new TextBox();
        btnRefresh = new Button();
        split = new SplitContainer();
        grid = new DataGridView();
        txtDetails = new TextBox();
        lblNote = new Label();
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        filters.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)split).BeginInit();
        split.Panel1.SuspendLayout();
        split.Panel2.SuspendLayout();
        split.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // filters
        //
        filters.AutoSize = true;
        filters.Controls.Add(lblPeriod);
        filters.Controls.Add(cboPeriod);
        filters.Controls.Add(lblLevel);
        filters.Controls.Add(cboLevel);
        filters.Controls.Add(txtSearch);
        filters.Controls.Add(btnRefresh);
        filters.Dock = DockStyle.Top;
        filters.Location = new Point(0, 0);
        filters.Name = "filters";
        filters.Padding = new Padding(6, 6, 6, 0);
        filters.Size = new Size(884, 37);
        filters.TabIndex = 0;
        //
        // lblPeriod
        //
        lblPeriod.Anchor = AnchorStyles.Left;
        lblPeriod.AutoSize = true;
        lblPeriod.Name = "lblPeriod";
        lblPeriod.TabIndex = 0;
        lblPeriod.Text = "Show:";
        //
        // cboPeriod
        //
        cboPeriod.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPeriod.Name = "cboPeriod";
        cboPeriod.Size = new Size(110, 23);
        cboPeriod.TabIndex = 1;
        cboPeriod.SelectedIndexChanged += Filter_Changed;
        //
        // lblLevel
        //
        lblLevel.Anchor = AnchorStyles.Left;
        lblLevel.AutoSize = true;
        lblLevel.Margin = new Padding(12, 0, 3, 0);
        lblLevel.Name = "lblLevel";
        lblLevel.TabIndex = 2;
        lblLevel.Text = "Level:";
        //
        // cboLevel
        //
        cboLevel.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLevel.Name = "cboLevel";
        cboLevel.Size = new Size(170, 23);
        cboLevel.TabIndex = 3;
        cboLevel.SelectedIndexChanged += Filter_Changed;
        //
        // txtSearch
        //
        txtSearch.Margin = new Padding(12, 3, 3, 3);
        txtSearch.Name = "txtSearch";
        txtSearch.PlaceholderText = "Search text, then Enter…";
        txtSearch.Size = new Size(220, 23);
        txtSearch.TabIndex = 4;
        txtSearch.KeyDown += txtSearch_KeyDown;
        //
        // btnRefresh
        //
        btnRefresh.AutoSize = true;
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(75, 25);
        btnRefresh.TabIndex = 5;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        //
        // split
        //
        split.Dock = DockStyle.Fill;
        split.Location = new Point(0, 37);
        split.Name = "split";
        split.Orientation = Orientation.Horizontal;
        split.Panel1.Controls.Add(grid);
        split.Panel2.Controls.Add(txtDetails);
        split.Size = new Size(884, 457);
        split.SplitterDistance = 330;
        split.TabIndex = 1;
        //
        // grid
        //
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.BackgroundColor = SystemColors.Window;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        grid.Dock = DockStyle.Fill;
        grid.MultiSelect = false;
        grid.Name = "grid";
        grid.ReadOnly = true;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.TabIndex = 0;
        grid.CellFormatting += grid_CellFormatting;
        grid.SelectionChanged += grid_SelectionChanged;
        //
        // txtDetails
        //
        txtDetails.BackColor = SystemColors.Window;
        txtDetails.Dock = DockStyle.Fill;
        txtDetails.Font = new Font("Consolas", 9F);
        txtDetails.Multiline = true;
        txtDetails.Name = "txtDetails";
        txtDetails.ReadOnly = true;
        txtDetails.ScrollBars = ScrollBars.Both;
        txtDetails.TabIndex = 0;
        txtDetails.WordWrap = false;
        //
        // lblNote
        //
        lblNote.Dock = DockStyle.Bottom;
        lblNote.ForeColor = SystemColors.GrayText;
        lblNote.Name = "lblNote";
        lblNote.Padding = new Padding(9, 4, 9, 4);
        lblNote.Size = new Size(884, 40);
        lblNote.TabIndex = 2;
        //
        // statusStrip
        //
        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus });
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(884, 22);
        statusStrip.TabIndex = 3;
        //
        // lblStatus
        //
        lblStatus.Name = "lblStatus";
        lblStatus.Spring = true;
        lblStatus.Text = "Ready";
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        //
        // LogForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(884, 556);
        Controls.Add(split);
        Controls.Add(filters);
        Controls.Add(lblNote);
        Controls.Add(statusStrip);
        Name = "LogForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Log";
        Load += LogForm_Load;
        filters.ResumeLayout(false);
        filters.PerformLayout();
        split.Panel1.ResumeLayout(false);
        split.Panel2.ResumeLayout(false);
        split.Panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)split).EndInit();
        split.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grid).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private FlowLayoutPanel filters;
    private Label lblPeriod;
    private ComboBox cboPeriod;
    private Label lblLevel;
    private ComboBox cboLevel;
    private TextBox txtSearch;
    private Button btnRefresh;
    private SplitContainer split;
    private DataGridView grid;
    private TextBox txtDetails;
    private Label lblNote;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
}
