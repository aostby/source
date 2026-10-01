namespace Kolibri.Kino.WinForms.Forms;

partial class CleanupConfirmForm
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
        lblInfo = new Label();
        lstFiles = new ListBox();
        buttons = new FlowLayoutPanel();
        btnCancel = new Button();
        btnDelete = new Button();
        buttons.SuspendLayout();
        SuspendLayout();
        //
        // lblInfo
        //
        lblInfo.AutoSize = true;
        lblInfo.Dock = DockStyle.Top;
        lblInfo.MaximumSize = new Size(880, 0);
        lblInfo.Name = "lblInfo";
        lblInfo.Padding = new Padding(9);
        lblInfo.TabIndex = 0;
        lblInfo.Text = "Info";
        //
        // lstFiles
        //
        lstFiles.Dock = DockStyle.Fill;
        lstFiles.HorizontalScrollbar = true;
        lstFiles.IntegralHeight = false;
        lstFiles.Name = "lstFiles";
        lstFiles.SelectionMode = SelectionMode.None;
        lstFiles.TabIndex = 1;
        //
        // buttons
        //
        buttons.AutoSize = true;
        buttons.Controls.Add(btnCancel);
        buttons.Controls.Add(btnDelete);
        buttons.Dock = DockStyle.Bottom;
        buttons.FlowDirection = FlowDirection.RightToLeft;
        buttons.Name = "buttons";
        buttons.Padding = new Padding(6);
        buttons.TabIndex = 2;
        //
        // btnCancel
        //
        btnCancel.AutoSize = true;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(90, 25);
        btnCancel.TabIndex = 0;
        btnCancel.Text = "Keep files";
        //
        // btnDelete
        //
        btnDelete.AutoSize = true;
        btnDelete.DialogResult = DialogResult.OK;
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(120, 25);
        btnDelete.TabIndex = 1;
        btnDelete.Text = "Delete";
        //
        // CleanupConfirmForm
        //
        AcceptButton = btnCancel;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(900, 520);
        Controls.Add(lstFiles);
        Controls.Add(buttons);
        Controls.Add(lblInfo);
        MinimizeBox = false;
        Name = "CleanupConfirmForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Clean up leftover files";
        buttons.ResumeLayout(false);
        buttons.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblInfo;
    private ListBox lstFiles;
    private FlowLayoutPanel buttons;
    private Button btnCancel;
    private Button btnDelete;
}
