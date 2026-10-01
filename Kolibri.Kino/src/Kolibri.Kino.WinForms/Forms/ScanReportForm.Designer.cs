namespace Kolibri.Kino.WinForms.Forms;

partial class ScanReportForm
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
        lblSummary = new Label();
        toolbar = new FlowLayoutPanel();
        lblShow = new Label();
        cboShow = new ComboBox();
        btnOpenFolder = new Button();
        btnFindMovie = new Button();
        dgvEntries = new DataGridView();
        bindingSource = new BindingSource(components);
        toolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvEntries).BeginInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource).BeginInit();
        SuspendLayout();
        // 
        // lblSummary
        // 
        lblSummary.AutoSize = true;
        lblSummary.Dock = DockStyle.Top;
        lblSummary.Location = new Point(0, 0);
        lblSummary.MaximumSize = new Size(1100, 0);
        lblSummary.Name = "lblSummary";
        lblSummary.Padding = new Padding(9, 9, 9, 3);
        lblSummary.Size = new Size(76, 27);
        lblSummary.TabIndex = 0;
        lblSummary.Text = "Summary";
        // 
        // toolbar
        // 
        toolbar.AutoSize = true;
        toolbar.Controls.Add(lblShow);
        toolbar.Controls.Add(cboShow);
        toolbar.Controls.Add(btnOpenFolder);
        toolbar.Controls.Add(btnFindMovie);
        toolbar.Dock = DockStyle.Top;
        toolbar.Location = new Point(0, 27);
        toolbar.Name = "toolbar";
        toolbar.Padding = new Padding(6, 3, 6, 6);
        toolbar.Size = new Size(1184, 39);
        toolbar.TabIndex = 1;
        // 
        // lblShow
        // 
        lblShow.AutoSize = true;
        lblShow.Location = new Point(9, 10);
        lblShow.Margin = new Padding(3, 7, 3, 0);
        lblShow.Name = "lblShow";
        lblShow.Size = new Size(39, 15);
        lblShow.TabIndex = 0;
        lblShow.Text = "Show:";
        // 
        // cboShow
        // 
        cboShow.DropDownStyle = ComboBoxStyle.DropDownList;
        cboShow.Location = new Point(54, 6);
        cboShow.Name = "cboShow";
        cboShow.Size = new Size(200, 23);
        cboShow.TabIndex = 1;
        cboShow.SelectedIndexChanged += cboShow_SelectedIndexChanged;
        // 
        // btnOpenFolder
        // 
        btnOpenFolder.AutoSize = true;
        btnOpenFolder.Location = new Point(281, 5);
        btnOpenFolder.Margin = new Padding(24, 2, 3, 3);
        btnOpenFolder.Name = "btnOpenFolder";
        btnOpenFolder.Size = new Size(90, 25);
        btnOpenFolder.TabIndex = 2;
        btnOpenFolder.Text = "Open folder";
        btnOpenFolder.Click += btnOpenFolder_Click;
        // 
        // btnFindMovie
        // 
        btnFindMovie.AutoSize = true;
        btnFindMovie.Enabled = false;
        btnFindMovie.Location = new Point(377, 5);
        btnFindMovie.Margin = new Padding(3, 2, 3, 3);
        btnFindMovie.Name = "btnFindMovie";
        btnFindMovie.Size = new Size(90, 25);
        btnFindMovie.TabIndex = 3;
        btnFindMovie.Text = "Find movie…";
        btnFindMovie.Click += btnFindMovie_Click;
        // 
        // dgvEntries
        // 
        dgvEntries.AllowUserToAddRows = false;
        dgvEntries.AllowUserToDeleteRows = false;
        dgvEntries.AutoGenerateColumns = false;
        dgvEntries.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvEntries.DataSource = bindingSource;
        dgvEntries.Dock = DockStyle.Fill;
        dgvEntries.Location = new Point(0, 66);
        dgvEntries.MultiSelect = false;
        dgvEntries.Name = "dgvEntries";
        dgvEntries.ReadOnly = true;
        dgvEntries.RowHeadersVisible = false;
        dgvEntries.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvEntries.Size = new Size(1184, 534);
        dgvEntries.TabIndex = 2;
        dgvEntries.CellDoubleClick += dgvEntries_CellDoubleClick;
        dgvEntries.SelectionChanged += dgvEntries_SelectionChanged;
        // 
        // ScanReportForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1184, 600);
        Controls.Add(dgvEntries);
        Controls.Add(toolbar);
        Controls.Add(lblSummary);
        Name = "ScanReportForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Scan report";
        toolbar.ResumeLayout(false);
        toolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvEntries).EndInit();
        ((System.ComponentModel.ISupportInitialize)bindingSource).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblSummary;
    private FlowLayoutPanel toolbar;
    private Label lblShow;
    private ComboBox cboShow;
    private Button btnOpenFolder;
    private Button btnFindMovie;
    private DataGridView dgvEntries;
    private BindingSource bindingSource;
}
