namespace Kolibri.Kino.WinForms.Forms;

partial class MissingEpisodesForm
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
        btnRun = new Button();
        chkRefreshOngoing = new CheckBox();
        chkShowComplete = new CheckBox();
        dgvSeries = new DataGridView();
        bindingSource = new BindingSource(components);
        statusStrip = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();
        progress = new ToolStripProgressBar();
        btnCancel = new ToolStripButton();
        toolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvSeries).BeginInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource).BeginInit();
        statusStrip.SuspendLayout();
        SuspendLayout();
        //
        // toolbar
        //
        toolbar.AutoSize = true;
        toolbar.Controls.Add(btnRun);
        toolbar.Controls.Add(chkRefreshOngoing);
        toolbar.Controls.Add(chkShowComplete);
        toolbar.Dock = DockStyle.Top;
        toolbar.Location = new Point(0, 0);
        toolbar.Name = "toolbar";
        toolbar.Padding = new Padding(6);
        toolbar.Size = new Size(1184, 43);
        toolbar.TabIndex = 0;
        //
        // btnRun
        //
        btnRun.AutoSize = true;
        btnRun.Margin = new Padding(3, 6, 3, 3);
        btnRun.Name = "btnRun";
        btnRun.Size = new Size(90, 25);
        btnRun.TabIndex = 0;
        btnRun.Text = "Make report";
        btnRun.Click += btnRun_Click;
        //
        // chkRefreshOngoing
        //
        chkRefreshOngoing.AutoSize = true;
        chkRefreshOngoing.Margin = new Padding(24, 10, 3, 3);
        chkRefreshOngoing.Name = "chkRefreshOngoing";
        chkRefreshOngoing.TabIndex = 1;
        chkRefreshOngoing.Text = "Fetch episode lists of ongoing series again";
        //
        // chkShowComplete
        //
        chkShowComplete.AutoSize = true;
        chkShowComplete.Margin = new Padding(24, 10, 3, 3);
        chkShowComplete.Name = "chkShowComplete";
        chkShowComplete.TabIndex = 2;
        chkShowComplete.Text = "Show complete series too";
        chkShowComplete.CheckedChanged += chkShowComplete_CheckedChanged;
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
        dgvSeries.TabIndex = 1;
        dgvSeries.CellFormatting += dgvSeries_CellFormatting;
        dgvSeries.CellDoubleClick += dgvSeries_CellDoubleClick;
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
        progress.Name = "progress";
        progress.Size = new Size(160, 16);
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
        // MissingEpisodesForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1184, 661);
        Controls.Add(dgvSeries);
        Controls.Add(statusStrip);
        Controls.Add(toolbar);
        Name = "MissingEpisodesForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Missing episodes";
        Load += MissingEpisodesForm_Load;
        toolbar.ResumeLayout(false);
        toolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvSeries).EndInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource).EndInit();
        statusStrip.ResumeLayout(false);
        statusStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private FlowLayoutPanel toolbar;
    private Button btnRun;
    private CheckBox chkRefreshOngoing;
    private CheckBox chkShowComplete;
    private DataGridView dgvSeries;
    private BindingSource bindingSource;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblStatus;
    private ToolStripProgressBar progress;
    private ToolStripButton btnCancel;
}
