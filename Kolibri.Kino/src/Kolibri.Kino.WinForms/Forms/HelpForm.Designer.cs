namespace Kolibri.Kino.WinForms.Forms;

partial class HelpForm
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
        browser = new WebBrowser();
        SuspendLayout();
        //
        // browser
        //
        browser.AllowWebBrowserDrop = false;
        browser.Dock = DockStyle.Fill;
        browser.IsWebBrowserContextMenuEnabled = false;
        browser.Location = new Point(0, 0);
        browser.MinimumSize = new Size(20, 20);
        browser.Name = "browser";
        browser.ScriptErrorsSuppressed = true;
        browser.Size = new Size(784, 661);
        browser.TabIndex = 0;
        browser.WebBrowserShortcutsEnabled = false;
        browser.DocumentCompleted += browser_DocumentCompleted;
        browser.Navigating += browser_Navigating;
        //
        // HelpForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(784, 661);
        Controls.Add(browser);
        Name = "HelpForm";
        Text = "Kolibri.Kino help";
        ResumeLayout(false);
    }

    #endregion

    private WebBrowser browser;
}
