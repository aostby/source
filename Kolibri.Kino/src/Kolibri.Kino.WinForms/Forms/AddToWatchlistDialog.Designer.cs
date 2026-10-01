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
        btnAdd = new Button();
        btnCancel = new Button();
        SuspendLayout();
        //
        // lblMovie
        //
        lblMovie.AutoEllipsis = true;
        lblMovie.Location = new Point(12, 12);
        lblMovie.Name = "lblMovie";
        lblMovie.Size = new Size(360, 20);
        lblMovie.TabIndex = 0;
        lblMovie.Text = "Add to watchlist:";
        //
        // cboList
        //
        cboList.Location = new Point(12, 36);
        cboList.Name = "cboList";
        cboList.Size = new Size(360, 23);
        cboList.TabIndex = 1;
        //
        // btnAdd
        //
        btnAdd.Location = new Point(216, 72);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(75, 25);
        btnAdd.TabIndex = 2;
        btnAdd.Text = "Add";
        btnAdd.Click += btnAdd_Click;
        //
        // btnCancel
        //
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(297, 72);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(75, 25);
        btnCancel.TabIndex = 3;
        btnCancel.Text = "Cancel";
        //
        // AddToWatchlistDialog
        //
        AcceptButton = btnAdd;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(384, 109);
        Controls.Add(lblMovie);
        Controls.Add(cboList);
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
    }

    #endregion

    private Label lblMovie;
    private ComboBox cboList;
    private Button btnAdd;
    private Button btnCancel;
}
