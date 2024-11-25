using Utilities.Properties;

namespace Kolibri.Common.FormUtilities.Forms
{
    partial class BTMapViewer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ContextMenuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.FileMenuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.FileMenuSaveAsHtml = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.FileMenuPageSetup = new System.Windows.Forms.ToolStripMenuItem();
            this.FileMenuPrint = new System.Windows.Forms.ToolStripMenuItem();
            this.FileMenuPrintPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.FileMenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.SettingsMenuRegister = new System.Windows.Forms.ToolStripMenuItem();
            this.SettingsMenuUnregister = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStripCurrentMapFile = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenu.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.ContextMenuStrip = this.contextMenu;
            this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser1.Location = new System.Drawing.Point(0, 27);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(793, 369);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.WebBrowserShortcutsEnabled = false;
            this.webBrowser1.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser1_Navigating);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ContextMenuCopy});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(103, 26);
            // 
            // ContextMenuCopy
            // 
            this.ContextMenuCopy.Name = "ContextMenuCopy";
            this.ContextMenuCopy.ShortcutKeyDisplayString = "";
            this.ContextMenuCopy.Size = new System.Drawing.Size(102, 22);
            this.ContextMenuCopy.Text = "Copy";
            this.ContextMenuCopy.Click += new System.EventHandler(this.ContextMenu_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.editToolStripMenuItem,
            this.toolStripMenuItem2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(237, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenuOpen,
            this.FileMenuSaveAsHtml,
            this.toolStripSeparator1,
            this.FileMenuPageSetup,
            this.FileMenuPrint,
            this.FileMenuPrintPreview,
            this.toolStripSeparator2,
            this.FileMenuExit});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "&File";
            // 
            // FileMenuOpen
            // 
            this.FileMenuOpen.Image =  Resources.openHS;
            this.FileMenuOpen.Name = "FileMenuOpen";
            this.FileMenuOpen.Size = new System.Drawing.Size(152, 22);
            this.FileMenuOpen.Text = "&Open...";
            this.FileMenuOpen.Click += new System.EventHandler(this.FileMenuOpen_Click);
            // 
            // FileMenuSaveAsHtml
            // 
            this.FileMenuSaveAsHtml.Image =  Resources.saveHS;
            this.FileMenuSaveAsHtml.Name = "FileMenuSaveAsHtml";
            this.FileMenuSaveAsHtml.Size = new System.Drawing.Size(152, 22);
            this.FileMenuSaveAsHtml.Text = "&Save as html...";
            this.FileMenuSaveAsHtml.Click += new System.EventHandler(this.FileMenuSaveAsHtml_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // FileMenuPageSetup
            // 
            this.FileMenuPageSetup.Image =  Resources.PrintSetupHS;
            this.FileMenuPageSetup.Name = "FileMenuPageSetup";
            this.FileMenuPageSetup.Size = new System.Drawing.Size(152, 22);
            this.FileMenuPageSetup.Text = "Page Setup...";
            this.FileMenuPageSetup.Click += new System.EventHandler(this.FileMenuPageSetup_Click);
            // 
            // FileMenuPrint
            // 
            this.FileMenuPrint.Image =  Resources.PrintHS;
            this.FileMenuPrint.Name = "FileMenuPrint";
            this.FileMenuPrint.Size = new System.Drawing.Size(152, 22);
            this.FileMenuPrint.Text = "&Print...";
            this.FileMenuPrint.Click += new System.EventHandler(this.FileMenuPrint_Click);
            // 
            // FileMenuPrintPreview
            // 
            this.FileMenuPrintPreview.Image =  Resources.PrintPreviewHS;
            this.FileMenuPrintPreview.Name = "FileMenuPrintPreview";
            this.FileMenuPrintPreview.Size = new System.Drawing.Size(152, 22);
            this.FileMenuPrintPreview.Text = "Print Preview...";
            this.FileMenuPrintPreview.Click += new System.EventHandler(this.FileMenuPrintPreview_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            // 
            // FileMenuExit
            // 
            this.FileMenuExit.Name = "FileMenuExit";
            this.FileMenuExit.Size = new System.Drawing.Size(152, 22);
            this.FileMenuExit.Text = "E&xit";
            this.FileMenuExit.Click += new System.EventHandler(this.FileMenuExit_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SettingsMenuRegister,
            this.SettingsMenuUnregister});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(61, 20);
            this.toolStripMenuItem2.Text = "&Settings";
            // 
            // SettingsMenuRegister
            // 
            this.SettingsMenuRegister.Name = "SettingsMenuRegister";
            this.SettingsMenuRegister.Size = new System.Drawing.Size(244, 22);
            this.SettingsMenuRegister.Text = "&Register in shell context menu";
            this.SettingsMenuRegister.Click += new System.EventHandler(this.SettingsMenuRegister_Click);
            // 
            // SettingsMenuUnregister
            // 
            this.SettingsMenuUnregister.Name = "SettingsMenuUnregister";
            this.SettingsMenuUnregister.Size = new System.Drawing.Size(244, 22);
            this.SettingsMenuUnregister.Text = "&Unregister in shell context menu";
            this.SettingsMenuUnregister.Click += new System.EventHandler(this.SettingsMenuUnregister_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "BizTalk Mapper files|*.btm|BizTalk schema files|*.xsd";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.statusStripCurrentMapFile});
            this.statusStrip1.Location = new System.Drawing.Point(0, 399);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(793, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(96, 17);
            this.toolStripStatusLabel1.Text = "Current map file:";
            // 
            // statusStripCurrentMapFile
            // 
            this.statusStripCurrentMapFile.Name = "statusStripCurrentMapFile";
            this.statusStripCurrentMapFile.Size = new System.Drawing.Size(144, 17);
            this.statusStripCurrentMapFile.Text = "statusStripCurrentMapFile";
            // 
            // BizTalkMapViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 421);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.statusStrip1);
            //this.Icon = Resources.openHS;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BizTalkMapViewer";
            this.Text = "BizTalk Map Viewer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenu.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem FileMenuOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem FileMenuExit;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem FileMenuSaveAsHtml;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem ContextMenuCopy;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem FileMenuPageSetup;
        private System.Windows.Forms.ToolStripMenuItem FileMenuPrint;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem SettingsMenuRegister;
        private System.Windows.Forms.ToolStripMenuItem SettingsMenuUnregister;
        private System.Windows.Forms.ToolStripMenuItem FileMenuPrintPreview;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusStripCurrentMapFile;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
    }
}

