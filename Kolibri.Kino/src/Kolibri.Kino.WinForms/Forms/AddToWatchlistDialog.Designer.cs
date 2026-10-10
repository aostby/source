namespace Kolibri.Kino.WinForms.Forms;

partial class AddToWatchlistDialog
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
        lblMovie = new Label();
        cboList = new ComboBox();
        lblWhere = new Label();
        chkKino = new CheckBox();
        chkPlex = new CheckBox();
        btnAdd = new Button();
        btnCancel = new Button();
        SuspendLayout();
        //
        // lblMovie
        //
        lblMovie.AutoEllipsis = true;
        lblMovie.Location = new Point(12, 12);
        lblMovie.Name = "lblMovie";
        lblMovie.Size = new Size(400, 20);
        lblMovie.TabIndex = 0;
        lblMovie.Text = "Add to:";
        //
        // cboList
        //
        cboList.FormattingEnabled = true;
        cboList.Location = new Point(12, 36);
        cboList.Name = "cboList";
        cboList.Size = new Size(400, 23);
        cboList.TabIndex = 1;
        cboList.TextChanged += cboList_TextChanged;
        //
        // lblWhere
        //
        lblWhere.ForeColor = SystemColors.GrayText;
        lblWhere.Location = new Point(12, 64);
        lblWhere.Name = "lblWhere";
        lblWhere.Size = new Size(400, 34);
        lblWhere.TabIndex = 2;
        //
        // chkKino
        //
        chkKino.AutoSize = true;
        chkKino.Checked = true;
        chkKino.CheckState = CheckState.Checked;
        chkKino.Location = new Point(12, 102);
        chkKino.Name = "chkKino";
        chkKino.TabIndex = 3;
        chkKino.Text = "Kino watchlist";
        chkKino.UseVisualStyleBackColor = true;
        chkKino.CheckedChanged += Target_CheckedChanged;
        //
        // chkPlex
        //
        chkPlex.AutoSize = true;
        chkPlex.Location = new Point(200, 102);
        chkPlex.Name = "chkPlex";
        chkPlex.TabIndex = 4;
        chkPlex.Text = "Plex playlist";
        chkPlex.UseVisualStyleBackColor = true;
        chkPlex.CheckedChanged += Target_CheckedChanged;
        //
        // btnAdd
        //
        btnAdd.Location = new Point(256, 136);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(75, 25);
        btnAdd.TabIndex = 5;
        btnAdd.Text = "Add";
        btnAdd.Click += btnAdd_Click;
        //
        // btnCancel
        //
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(337, 136);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(75, 25);
        btnCancel.TabIndex = 6;
        btnCancel.Text = "Cancel";
        //
        // AddToWatchlistDialog
        //
        AcceptButton = btnAdd;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(424, 173);
        Controls.Add(lblMovie);
        Controls.Add(cboList);
        Controls.Add(lblWhere);
        Controls.Add(chkKino);
        Controls.Add(chkPlex);
        Controls.Add(btnAdd);
        Controls.Add(btnCancel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AddToWatchlistDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Add to watchlist";
        Load += AddToWatchlistDialog_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblMovie;
    private ComboBox cboList;
    private Label lblWhere;
    private CheckBox chkKino;
    private CheckBox chkPlex;
    private Button btnAdd;
    private Button btnCancel;
}
