namespace Kolibri.Kino.WinForms.Forms;

partial class SubtitleChoiceDialog
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
        lstFiles = new ListView();
        colSize = new ColumnHeader();
        colFile = new ColumnHeader();
        buttons = new FlowLayoutPanel();
        btnCancel = new Button();
        btnUse = new Button();
        buttons.SuspendLayout();
        SuspendLayout();
        //
        // lblInfo
        //
        lblInfo.Dock = DockStyle.Top;
        lblInfo.Name = "lblInfo";
        lblInfo.Padding = new Padding(10, 10, 10, 6);
        lblInfo.Size = new Size(684, 56);
        lblInfo.TabIndex = 0;
        lblInfo.Text = "Info";
        //
        // lstFiles
        //
        lstFiles.Columns.AddRange(new ColumnHeader[] { colSize, colFile });
        lstFiles.Dock = DockStyle.Fill;
        lstFiles.FullRowSelect = true;
        lstFiles.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        lstFiles.HideSelection = false;
        lstFiles.MultiSelect = false;
        lstFiles.Name = "lstFiles";
        lstFiles.TabIndex = 1;
        lstFiles.UseCompatibleStateImageBehavior = false;
        lstFiles.View = View.Details;
        lstFiles.SelectedIndexChanged += lstFiles_SelectedIndexChanged;
        lstFiles.DoubleClick += lstFiles_DoubleClick;
        //
        // colSize
        //
        colSize.Text = "Size";
        colSize.TextAlign = HorizontalAlignment.Right;
        colSize.Width = 80;
        //
        // colFile
        //
        colFile.Text = "Subtitle";
        colFile.Width = 560;
        //
        // buttons
        //
        buttons.AutoSize = true;
        buttons.Controls.Add(btnCancel);
        buttons.Controls.Add(btnUse);
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
        btnCancel.TabIndex = 1;
        btnCancel.Text = "Cancel";
        btnCancel.UseVisualStyleBackColor = true;
        //
        // btnUse
        //
        btnUse.AutoSize = true;
        btnUse.DialogResult = DialogResult.OK;
        btnUse.Name = "btnUse";
        btnUse.Size = new Size(110, 25);
        btnUse.TabIndex = 0;
        btnUse.Text = "Use selected";
        btnUse.UseVisualStyleBackColor = true;
        //
        // SubtitleChoiceDialog
        //
        AcceptButton = btnUse;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(684, 321);
        Controls.Add(lstFiles);
        Controls.Add(lblInfo);
        Controls.Add(buttons);
        MinimizeBox = false;
        Name = "SubtitleChoiceDialog";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Choose a subtitle";
        buttons.ResumeLayout(false);
        buttons.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblInfo;
    private ListView lstFiles;
    private ColumnHeader colSize;
    private ColumnHeader colFile;
    private FlowLayoutPanel buttons;
    private Button btnCancel;
    private Button btnUse;
}
