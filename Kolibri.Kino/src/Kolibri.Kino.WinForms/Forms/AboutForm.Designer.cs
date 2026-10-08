namespace Kolibri.Kino.WinForms.Forms;

partial class AboutForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        picIcon = new PictureBox();
        lblProduct = new Label();
        lblVersion = new Label();
        lblDescription = new Label();
        txtInfo = new TextBox();
        lnkTmdb = new LinkLabel();
        btnOk = new Button();
        btnUsage = new Button();
        btnLog = new Button();
        ((System.ComponentModel.ISupportInitialize)picIcon).BeginInit();
        SuspendLayout();
        //
        // picIcon
        //
        picIcon.Location = new Point(16, 16);
        picIcon.Name = "picIcon";
        picIcon.Size = new Size(64, 64);
        picIcon.SizeMode = PictureBoxSizeMode.CenterImage;
        picIcon.TabIndex = 0;
        picIcon.TabStop = false;
        //
        // lblProduct
        //
        lblProduct.AutoSize = true;
        lblProduct.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblProduct.Location = new Point(96, 16);
        lblProduct.Name = "lblProduct";
        lblProduct.Size = new Size(152, 30);
        lblProduct.TabIndex = 1;
        lblProduct.Text = "Kolibri.Kino";
        //
        // lblVersion
        //
        lblVersion.AutoSize = true;
        lblVersion.Location = new Point(99, 50);
        lblVersion.Name = "lblVersion";
        lblVersion.Size = new Size(72, 15);
        lblVersion.TabIndex = 2;
        lblVersion.Text = "Version 1.0.0";
        //
        // lblDescription
        //
        lblDescription.Location = new Point(16, 92);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(520, 36);
        lblDescription.TabIndex = 3;
        lblDescription.Text = "Your movie and series library: find titles, link them to your files, keep watchlists and copy them to Plex. " +
            "A rewrite of Kolibri.SilverScreen, using the same database.";
        //
        // txtInfo
        //
        txtInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtInfo.BackColor = SystemColors.Window;
        txtInfo.Font = new Font("Consolas", 9F);
        txtInfo.Location = new Point(16, 134);
        txtInfo.Multiline = true;
        txtInfo.Name = "txtInfo";
        txtInfo.ReadOnly = true;
        txtInfo.ScrollBars = ScrollBars.Horizontal;
        txtInfo.Size = new Size(520, 96);
        txtInfo.TabIndex = 4;
        txtInfo.WordWrap = false;
        //
        // lnkTmdb
        //
        lnkTmdb.Location = new Point(16, 240);
        lnkTmdb.Name = "lnkTmdb";
        lnkTmdb.Size = new Size(520, 36);
        lnkTmdb.TabIndex = 5;
        lnkTmdb.Text = "Movie and series data from OMDb and TMDb. This product uses the TMDB API but is not endorsed or certified by TMDB. " +
            "Icon from iconpacks.net.";
        lnkTmdb.LinkClicked += lnkTmdb_LinkClicked;
        //
        // btnOk
        //
        btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnOk.DialogResult = DialogResult.OK;
        btnOk.Location = new Point(461, 286);
        btnOk.Name = "btnOk";
        btnOk.Size = new Size(75, 25);
        btnOk.TabIndex = 6;
        btnOk.Text = "OK";
        btnOk.UseVisualStyleBackColor = true;
        //
        // btnUsage
        //
        btnUsage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnUsage.AutoSize = true;
        btnUsage.Location = new Point(16, 286);
        btnUsage.Name = "btnUsage";
        btnUsage.Size = new Size(110, 25);
        btnUsage.TabIndex = 7;
        btnUsage.Text = "API usage report…";
        btnUsage.UseVisualStyleBackColor = true;
        btnUsage.Click += btnUsage_Click;
        //
        // btnLog
        //
        btnLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnLog.AutoSize = true;
        btnLog.Location = new Point(140, 286);
        btnLog.Name = "btnLog";
        btnLog.Size = new Size(75, 25);
        btnLog.TabIndex = 8;
        btnLog.Text = "Log…";
        btnLog.UseVisualStyleBackColor = true;
        btnLog.Click += btnLog_Click;
        //
        // AboutForm
        //
        AcceptButton = btnOk;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnOk;
        ClientSize = new Size(552, 325);
        Controls.Add(btnOk);
        Controls.Add(btnUsage);
        Controls.Add(btnLog);
        Controls.Add(lnkTmdb);
        Controls.Add(txtInfo);
        Controls.Add(lblDescription);
        Controls.Add(lblVersion);
        Controls.Add(lblProduct);
        Controls.Add(picIcon);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AboutForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "About Kolibri.Kino";
        ((System.ComponentModel.ISupportInitialize)picIcon).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private PictureBox picIcon;
    private Label lblProduct;
    private Label lblVersion;
    private Label lblDescription;
    private TextBox txtInfo;
    private LinkLabel lnkTmdb;
    private Button btnOk;
    private Button btnUsage;
    private Button btnLog;
}
