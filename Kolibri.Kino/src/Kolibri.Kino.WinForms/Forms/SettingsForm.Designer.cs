namespace Kolibri.Kino.WinForms.Forms;

partial class SettingsForm
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
        grid = new PropertyGrid();
        txtResults = new TextBox();
        buttons = new FlowLayoutPanel();
        btnCancel = new Button();
        btnSave = new Button();
        btnTest = new Button();
        buttons.SuspendLayout();
        SuspendLayout();
        //
        // grid
        //
        grid.Dock = DockStyle.Fill;
        grid.Name = "grid";
        grid.PropertySort = PropertySort.Categorized;
        grid.TabIndex = 0;
        grid.ToolbarVisible = false;
        //
        // txtResults
        //
        txtResults.BorderStyle = BorderStyle.None;
        txtResults.Dock = DockStyle.Bottom;
        txtResults.Multiline = true;
        txtResults.Name = "txtResults";
        txtResults.ReadOnly = true;
        txtResults.Size = new Size(584, 64);
        txtResults.TabIndex = 1;
        txtResults.TabStop = false;
        //
        // buttons
        //
        buttons.AutoSize = true;
        buttons.Controls.Add(btnCancel);
        buttons.Controls.Add(btnSave);
        buttons.Controls.Add(btnTest);
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
        btnCancel.Size = new Size(75, 25);
        btnCancel.TabIndex = 2;
        btnCancel.Text = "Cancel";
        //
        // btnSave
        //
        btnSave.AutoSize = true;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(75, 25);
        btnSave.TabIndex = 1;
        btnSave.Text = "Save";
        btnSave.Click += btnSave_Click;
        //
        // btnTest
        //
        btnTest.AutoSize = true;
        btnTest.Name = "btnTest";
        btnTest.Size = new Size(110, 25);
        btnTest.TabIndex = 0;
        btnTest.Text = "Test connections";
        btnTest.Click += btnTest_Click;
        //
        // SettingsForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(584, 560);
        Controls.Add(grid);
        Controls.Add(txtResults);
        Controls.Add(buttons);
        MinimizeBox = false;
        Name = "SettingsForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Settings";
        buttons.ResumeLayout(false);
        buttons.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private PropertyGrid grid;
    private TextBox txtResults;
    private FlowLayoutPanel buttons;
    private Button btnCancel;
    private Button btnSave;
    private Button btnTest;
}
