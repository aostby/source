namespace Kolibri.Kino.WinForms.Forms;

partial class UsageReportForm
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
        lblShow = new Label();
        cboPeriod = new ComboBox();
        lblYear = new Label();
        cboYear = new ComboBox();
        lblMonth = new Label();
        cboMonth = new ComboBox();
        btnRefresh = new Button();
        lblToday = new Label();
        grid = new DataGridView();
        lblNote = new Label();
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        filters.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // filters
        //
        filters.AutoSize = true;
        filters.Controls.Add(lblShow);
        filters.Controls.Add(cboPeriod);
        filters.Controls.Add(lblYear);
        filters.Controls.Add(cboYear);
        filters.Controls.Add(lblMonth);
        filters.Controls.Add(cboMonth);
        filters.Controls.Add(btnRefresh);
        filters.Dock = DockStyle.Top;
        filters.Location = new Point(0, 0);
        filters.Name = "filters";
        filters.Padding = new Padding(6, 6, 6, 0);
        filters.Size = new Size(784, 37);
        filters.TabIndex = 0;
        //
        // lblShow
        //
        lblShow.Anchor = AnchorStyles.Left;
        lblShow.AutoSize = true;
        lblShow.Location = new Point(9, 13);
        lblShow.Name = "lblShow";
        lblShow.Size = new Size(39, 15);
        lblShow.TabIndex = 0;
        lblShow.Text = "Show:";
        //
        // cboPeriod
        //
        cboPeriod.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPeriod.Location = new Point(54, 9);
        cboPeriod.Name = "cboPeriod";
        cboPeriod.Size = new Size(120, 23);
        cboPeriod.TabIndex = 1;
        cboPeriod.SelectedIndexChanged += Filter_Changed;
        //
        // lblYear
        //
        lblYear.Anchor = AnchorStyles.Left;
        lblYear.AutoSize = true;
        lblYear.Location = new Point(189, 13);
        lblYear.Margin = new Padding(12, 0, 3, 0);
        lblYear.Name = "lblYear";
        lblYear.Size = new Size(32, 15);
        lblYear.TabIndex = 2;
        lblYear.Text = "Year:";
        //
        // cboYear
        //
        cboYear.DropDownStyle = ComboBoxStyle.DropDownList;
        cboYear.Location = new Point(227, 9);
        cboYear.Name = "cboYear";
        cboYear.Size = new Size(70, 23);
        cboYear.TabIndex = 3;
        cboYear.SelectedIndexChanged += Filter_Changed;
        //
        // lblMonth
        //
        lblMonth.Anchor = AnchorStyles.Left;
        lblMonth.AutoSize = true;
        lblMonth.Location = new Point(312, 13);
        lblMonth.Margin = new Padding(12, 0, 3, 0);
        lblMonth.Name = "lblMonth";
        lblMonth.Size = new Size(46, 15);
        lblMonth.TabIndex = 4;
        lblMonth.Text = "Month:";
        //
        // cboMonth
        //
        cboMonth.DropDownStyle = ComboBoxStyle.DropDownList;
        cboMonth.Location = new Point(364, 9);
        cboMonth.Name = "cboMonth";
        cboMonth.Size = new Size(110, 23);
        cboMonth.TabIndex = 5;
        cboMonth.SelectedIndexChanged += Filter_Changed;
        //
        // btnRefresh
        //
        btnRefresh.AutoSize = true;
        btnRefresh.Location = new Point(489, 9);
        btnRefresh.Margin = new Padding(12, 3, 3, 3);
        btnRefresh.Name = "btnRefresh";
        btnRefresh.Size = new Size(75, 25);
        btnRefresh.TabIndex = 6;
        btnRefresh.Text = "Refresh";
        btnRefresh.UseVisualStyleBackColor = true;
        btnRefresh.Click += btnRefresh_Click;
        //
        // lblToday
        //
        lblToday.AutoSize = false;
        lblToday.Dock = DockStyle.Top;
        lblToday.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblToday.Location = new Point(0, 37);
        lblToday.Name = "lblToday";
        lblToday.Padding = new Padding(9, 4, 6, 4);
        lblToday.Size = new Size(784, 26);
        lblToday.TabIndex = 1;
        lblToday.Text = "Today:";
        //
        // grid
        //
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.BackgroundColor = SystemColors.Window;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        grid.Dock = DockStyle.Fill;
        grid.Location = new Point(0, 63);
        grid.Name = "grid";
        grid.ReadOnly = true;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.Size = new Size(784, 417);
        grid.TabIndex = 2;
        grid.CellFormatting += grid_CellFormatting;
        //
        // lblNote
        //
        lblNote.Dock = DockStyle.Bottom;
        lblNote.ForeColor = SystemColors.GrayText;
        lblNote.Location = new Point(0, 480);
        lblNote.Name = "lblNote";
        lblNote.Padding = new Padding(9, 4, 9, 4);
        lblNote.Size = new Size(784, 44);
        lblNote.TabIndex = 3;
        //
        // statusStrip
        //
        statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus });
        statusStrip.Location = new Point(0, 524);
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(784, 22);
        statusStrip.TabIndex = 4;
        //
        // lblStatus
        //
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(769, 17);
        lblStatus.Spring = true;
        lblStatus.Text = "Ready";
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        //
        // UsageReportForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(784, 546);
        Controls.Add(grid);
        Controls.Add(lblToday);
        Controls.Add(filters);
        Controls.Add(lblNote);
        Controls.Add(statusStrip);
        Name = "UsageReportForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "API usage";
        Load += UsageReportForm_Load;
        filters.ResumeLayout(false);
        filters.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grid).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private FlowLayoutPanel filters;
    private Label lblShow;
    private ComboBox cboPeriod;
    private Label lblYear;
    private ComboBox cboYear;
    private Label lblMonth;
    private ComboBox cboMonth;
    private Button btnRefresh;
    private Label lblToday;
    private DataGridView grid;
    private Label lblNote;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
}
