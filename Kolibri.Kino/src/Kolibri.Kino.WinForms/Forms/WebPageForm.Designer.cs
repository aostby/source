namespace Kolibri.Kino.WinForms.Forms;

partial class WebPageForm
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
        bar = new TableLayoutPanel();
        btnBack = new Button();
        btnForward = new Button();
        txtAddress = new TextBox();
        btnUseId = new Button();
        btnBrowser = new Button();
        browser = new Microsoft.Web.WebView2.WinForms.WebView2();
        bar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)browser).BeginInit();
        SuspendLayout();
        //
        // bar
        //
        bar.AutoSize = true;
        bar.ColumnCount = 5;
        bar.ColumnStyles.Add(new ColumnStyle());
        bar.ColumnStyles.Add(new ColumnStyle());
        bar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        bar.ColumnStyles.Add(new ColumnStyle());
        bar.ColumnStyles.Add(new ColumnStyle());
        bar.Controls.Add(btnBack, 0, 0);
        bar.Controls.Add(btnForward, 1, 0);
        bar.Controls.Add(txtAddress, 2, 0);
        bar.Controls.Add(btnUseId, 3, 0);
        bar.Controls.Add(btnBrowser, 4, 0);
        bar.Dock = DockStyle.Top;
        bar.Name = "bar";
        bar.Padding = new Padding(4);
        bar.RowCount = 1;
        bar.RowStyles.Add(new RowStyle());
        bar.Size = new Size(1084, 39);
        bar.TabIndex = 0;
        //
        // btnBack
        //
        btnBack.AutoSize = true;
        btnBack.Enabled = false;
        btnBack.Name = "btnBack";
        btnBack.Size = new Size(30, 25);
        btnBack.TabIndex = 0;
        btnBack.Text = "◀";
        btnBack.UseVisualStyleBackColor = true;
        btnBack.Click += btnBack_Click;
        //
        // btnForward
        //
        btnForward.AutoSize = true;
        btnForward.Enabled = false;
        btnForward.Name = "btnForward";
        btnForward.Size = new Size(30, 25);
        btnForward.TabIndex = 1;
        btnForward.Text = "▶";
        btnForward.UseVisualStyleBackColor = true;
        btnForward.Click += btnForward_Click;
        //
        // txtAddress
        //
        txtAddress.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        txtAddress.BackColor = SystemColors.Window;
        txtAddress.Name = "txtAddress";
        txtAddress.ReadOnly = true;
        txtAddress.TabIndex = 2;
        //
        // btnUseId
        //
        btnUseId.AutoSize = true;
        btnUseId.Enabled = false;
        btnUseId.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnUseId.Name = "btnUseId";
        btnUseId.Size = new Size(110, 25);
        btnUseId.TabIndex = 3;
        btnUseId.Text = "Use this movie";
        btnUseId.UseVisualStyleBackColor = true;
        btnUseId.Click += btnUseId_Click;
        //
        // btnBrowser
        //
        btnBrowser.AutoSize = true;
        btnBrowser.Name = "btnBrowser";
        btnBrowser.Size = new Size(120, 25);
        btnBrowser.TabIndex = 4;
        btnBrowser.Text = "Open in browser";
        btnBrowser.UseVisualStyleBackColor = true;
        btnBrowser.Click += btnBrowser_Click;
        //
        // browser
        //
        browser.AllowExternalDrop = false;
        browser.CreationProperties = null;
        browser.DefaultBackgroundColor = Color.White;
        browser.Dock = DockStyle.Fill;
        browser.Name = "browser";
        browser.TabIndex = 1;
        browser.ZoomFactor = 1D;
        browser.SourceChanged += browser_SourceChanged;
        //
        // WebPageForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1084, 761);
        Controls.Add(browser);
        Controls.Add(bar);
        Name = "WebPageForm";
        Text = "Web page";
        Load += WebPageForm_Load;
        bar.ResumeLayout(false);
        bar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)browser).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TableLayoutPanel bar;
    private Button btnBack;
    private Button btnForward;
    private TextBox txtAddress;
    private Button btnUseId;
    private Button btnBrowser;
    private Microsoft.Web.WebView2.WinForms.WebView2 browser;
}
