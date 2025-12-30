

namespace Kolibri.net.C64Sorter
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemHostname = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripMenuItemRemoveEmptyDirs = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripMenuItem1 = new ToolStripMenuItem();
            pNGFilesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            toolStripMenuItemFTPClient = new ToolStripMenuItem();
            toolStripSeparator7 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            organizeFilesToolStripMenuItem = new ToolStripMenuItem();
            singleFileCategorizerToolStripMenuItem = new ToolStripMenuItem();
            extensionOrganizerToolStripMenuItem = new ToolStripMenuItem();
            showContentsToolStripMenuItem = new ToolStripMenuItem();
            d64FoldersToolStripMenuItem = new ToolStripMenuItem();
            searchToolStripMenuItem = new ToolStripMenuItem();
            browseLocalFilesToolStripMenuItem = new ToolStripMenuItem();
            ultimateEliteIIToolStripMenuItem = new ToolStripMenuItem();
            machineToolStripMenuItem = new ToolStripMenuItem();
            resetToolStripMenuItem = new ToolStripMenuItem();
            rebootToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            pauseToolStripMenuItem = new ToolStripMenuItem();
            resumeToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            powerOffToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            volumeToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItemRun = new ToolStripMenuItem();
            toolStripMenuItemFTPTreeView = new ToolStripMenuItem();
            toolStripSeparator8 = new ToolStripSeparator();
            pRGToolStripMenuItem = new ToolStripMenuItem();
            d64ToolStripMenuItem = new ToolStripMenuItem();
            sIDToolStripMenuItem = new ToolStripMenuItem();
            allFilesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator6 = new ToolStripSeparator();
            aboutToolStripMenuItem1 = new ToolStripMenuItem();
            windowsToolStripMenuItem = new ToolStripMenuItem();
            cascadeWindowsToolStripMenuItem = new ToolStripMenuItem();
            tileVerticalToolStripMenuItem = new ToolStripMenuItem();
            tileHorizontalToolStripMenuItem = new ToolStripMenuItem();
            arrangeIconsToolStripMenuItem = new ToolStripMenuItem();
            closeAllToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            aboutC64SorterToolStripMenuItem = new ToolStripMenuItem();
            linksToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            toolStripStatusLabelStatus = new ToolStripLabel();
            toolStripSeparator9 = new ToolStripSeparator();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, organizeFilesToolStripMenuItem, ultimateEliteIIToolStripMenuItem, windowsToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.MdiWindowListItem = windowsToolStripMenuItem;
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(5, 2, 0, 2);
            menuStrip1.Size = new Size(1032, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItemHostname, toolStripSeparator2, toolStripMenuItemRemoveEmptyDirs, toolStripSeparator1, toolStripMenuItem1, toolStripSeparator3, toolStripMenuItemFTPClient, toolStripSeparator7, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // toolStripMenuItemHostname
            // 
            toolStripMenuItemHostname.Name = "toolStripMenuItemHostname";
            toolStripMenuItemHostname.Size = new Size(213, 22);
            toolStripMenuItemHostname.Text = "Set C64U Hostname (IP)";
            toolStripMenuItemHostname.Click += toolStripMenuItemHostname_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(210, 6);
            // 
            // toolStripMenuItemRemoveEmptyDirs
            // 
            toolStripMenuItemRemoveEmptyDirs.Name = "toolStripMenuItemRemoveEmptyDirs";
            toolStripMenuItemRemoveEmptyDirs.Size = new Size(213, 22);
            toolStripMenuItemRemoveEmptyDirs.Text = "Remove Empty Directories";
            toolStripMenuItemRemoveEmptyDirs.Click += toolStripMenuItemRemoveEmptyDirs_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(210, 6);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { pNGFilesToolStripMenuItem });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(213, 22);
            toolStripMenuItem1.Text = "Print...";
            // 
            // pNGFilesToolStripMenuItem
            // 
            pNGFilesToolStripMenuItem.Name = "pNGFilesToolStripMenuItem";
            pNGFilesToolStripMenuItem.Size = new Size(122, 22);
            pNGFilesToolStripMenuItem.Text = "PNG files";
            pNGFilesToolStripMenuItem.Click += PrintFilesToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(210, 6);
            // 
            // toolStripMenuItemFTPClient
            // 
            toolStripMenuItemFTPClient.Name = "toolStripMenuItemFTPClient";
            toolStripMenuItemFTPClient.Size = new Size(213, 22);
            toolStripMenuItemFTPClient.Text = "FTP Client";
            toolStripMenuItemFTPClient.Click += toolStripMenuItemFTP_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(210, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(213, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // organizeFilesToolStripMenuItem
            // 
            organizeFilesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { singleFileCategorizerToolStripMenuItem, extensionOrganizerToolStripMenuItem, showContentsToolStripMenuItem, browseLocalFilesToolStripMenuItem });
            organizeFilesToolStripMenuItem.Name = "organizeFilesToolStripMenuItem";
            organizeFilesToolStripMenuItem.Size = new Size(92, 20);
            organizeFilesToolStripMenuItem.Text = "Organize Files";
            // 
            // singleFileCategorizerToolStripMenuItem
            // 
            singleFileCategorizerToolStripMenuItem.Name = "singleFileCategorizerToolStripMenuItem";
            singleFileCategorizerToolStripMenuItem.Size = new Size(212, 22);
            singleFileCategorizerToolStripMenuItem.Text = "Single file categorizer (PC)";
            singleFileCategorizerToolStripMenuItem.ToolTipText = "Groups common PC files in a folder (top level) into different categories.";
            singleFileCategorizerToolStripMenuItem.Click += organizeFilesToolStripMenuItem_Click;
            // 
            // extensionOrganizerToolStripMenuItem
            // 
            extensionOrganizerToolStripMenuItem.Name = "extensionOrganizerToolStripMenuItem";
            extensionOrganizerToolStripMenuItem.Size = new Size(212, 22);
            extensionOrganizerToolStripMenuItem.Tag = "Organize by extension, alphabetize or flatten a directory - used for single files.";
            extensionOrganizerToolStripMenuItem.Text = "Extension Organizer (C64)";
            extensionOrganizerToolStripMenuItem.Click += organizeFilesToolStripMenuItem_Click;
            // 
            // showContentsToolStripMenuItem
            // 
            showContentsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { d64FoldersToolStripMenuItem, searchToolStripMenuItem });
            showContentsToolStripMenuItem.Name = "showContentsToolStripMenuItem";
            showContentsToolStripMenuItem.Size = new Size(212, 22);
            showContentsToolStripMenuItem.Tag = "D64 content viewers";
            showContentsToolStripMenuItem.Text = "Show contents";
            // 
            // d64FoldersToolStripMenuItem
            // 
            d64FoldersToolStripMenuItem.Name = "d64FoldersToolStripMenuItem";
            d64FoldersToolStripMenuItem.Size = new Size(179, 22);
            d64FoldersToolStripMenuItem.Tag = "List all C64 files and their .d64 properties.";
            d64FoldersToolStripMenuItem.Text = "D64 Folders";
            d64FoldersToolStripMenuItem.ToolTipText = "Opens a folder, reads all content within .d64 files\\r\\nand saves a CSV file with the details of those fies to be viewed later.";
            d64FoldersToolStripMenuItem.Click += d64FoldersToolStripMenuItem_Click;
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new Size(179, 22);
            searchToolStripMenuItem.Text = "Search (in .d64 files)";
            searchToolStripMenuItem.Click += searchToolStripMenuItem_Click;
            // 
            // browseLocalFilesToolStripMenuItem
            // 
            browseLocalFilesToolStripMenuItem.Name = "browseLocalFilesToolStripMenuItem";
            browseLocalFilesToolStripMenuItem.Size = new Size(212, 22);
            browseLocalFilesToolStripMenuItem.Text = "Browse local files";
            browseLocalFilesToolStripMenuItem.Click += browseLocalFilesToolStripMenuItem_Click;
            // 
            // ultimateEliteIIToolStripMenuItem
            // 
            ultimateEliteIIToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { machineToolStripMenuItem, toolStripMenuItem3, toolStripMenuItemRun, toolStripSeparator6, aboutToolStripMenuItem1 });
            ultimateEliteIIToolStripMenuItem.Name = "ultimateEliteIIToolStripMenuItem";
            ultimateEliteIIToolStripMenuItem.Size = new Size(98, 20);
            ultimateEliteIIToolStripMenuItem.Text = "Ultimate Elite II";
            // 
            // machineToolStripMenuItem
            // 
            machineToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { resetToolStripMenuItem, rebootToolStripMenuItem, toolStripSeparator4, pauseToolStripMenuItem, resumeToolStripMenuItem, toolStripSeparator5, powerOffToolStripMenuItem });
            machineToolStripMenuItem.Name = "machineToolStripMenuItem";
            machineToolStripMenuItem.Size = new Size(180, 22);
            machineToolStripMenuItem.Text = "Machine";
            // 
            // resetToolStripMenuItem
            // 
            resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            resetToolStripMenuItem.Size = new Size(125, 22);
            resetToolStripMenuItem.Text = "Reset";
            resetToolStripMenuItem.Click += machineToolStripMenuItem_Click;
            // 
            // rebootToolStripMenuItem
            // 
            rebootToolStripMenuItem.Name = "rebootToolStripMenuItem";
            rebootToolStripMenuItem.Size = new Size(125, 22);
            rebootToolStripMenuItem.Text = "Reboot";
            rebootToolStripMenuItem.Click += machineToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(122, 6);
            // 
            // pauseToolStripMenuItem
            // 
            pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            pauseToolStripMenuItem.Size = new Size(125, 22);
            pauseToolStripMenuItem.Text = "Pause";
            pauseToolStripMenuItem.Click += machineToolStripMenuItem_Click;
            // 
            // resumeToolStripMenuItem
            // 
            resumeToolStripMenuItem.Name = "resumeToolStripMenuItem";
            resumeToolStripMenuItem.Size = new Size(125, 22);
            resumeToolStripMenuItem.Text = "Resume";
            resumeToolStripMenuItem.Click += machineToolStripMenuItem_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(122, 6);
            // 
            // powerOffToolStripMenuItem
            // 
            powerOffToolStripMenuItem.Name = "powerOffToolStripMenuItem";
            powerOffToolStripMenuItem.Size = new Size(125, 22);
            powerOffToolStripMenuItem.Text = "Power off";
            powerOffToolStripMenuItem.Click += machineToolStripMenuItem_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.DropDownItems.AddRange(new ToolStripItem[] { volumeToolStripMenuItem });
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(180, 22);
            toolStripMenuItem3.Text = "Configuration";
            // 
            // volumeToolStripMenuItem
            // 
            volumeToolStripMenuItem.Name = "volumeToolStripMenuItem";
            volumeToolStripMenuItem.Size = new Size(180, 22);
            volumeToolStripMenuItem.Text = "Volume";
            volumeToolStripMenuItem.Click += ConfigurationToolStripMenuItem_Click;
            // 
            // toolStripMenuItemRun
            // 
            toolStripMenuItemRun.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItemFTPTreeView, toolStripSeparator8, allFilesToolStripMenuItem, toolStripSeparator9, pRGToolStripMenuItem, d64ToolStripMenuItem, sIDToolStripMenuItem });
            toolStripMenuItemRun.Name = "toolStripMenuItemRun";
            toolStripMenuItemRun.Size = new Size(180, 22);
            toolStripMenuItemRun.Text = "Run...";
            // 
            // toolStripMenuItemFTPTreeView
            // 
            toolStripMenuItemFTPTreeView.Name = "toolStripMenuItemFTPTreeView";
            toolStripMenuItemFTPTreeView.Size = new Size(216, 22);
            toolStripMenuItemFTPTreeView.Text = "FTP TreeView";
            toolStripMenuItemFTPTreeView.ToolTipText = "View and run external files";
            toolStripMenuItemFTPTreeView.Click += toolStripMenuItemFTP_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(213, 6);
            // 
            // pRGToolStripMenuItem
            // 
            pRGToolStripMenuItem.Name = "pRGToolStripMenuItem";
            pRGToolStripMenuItem.Size = new Size(216, 22);
            pRGToolStripMenuItem.Text = "PRG or CRT";
            pRGToolStripMenuItem.ToolTipText = "Run local files externally";
            pRGToolStripMenuItem.Click += runnersMenuItem_Click;
            // 
            // d64ToolStripMenuItem
            // 
            d64ToolStripMenuItem.Name = "d64ToolStripMenuItem";
            d64ToolStripMenuItem.Size = new Size(216, 22);
            d64ToolStripMenuItem.Text = "D64 or g64, d71, g71 or d81";
            d64ToolStripMenuItem.ToolTipText = "Run local files externally";
            d64ToolStripMenuItem.Click += runnersMenuItem_Click;
            // 
            // sIDToolStripMenuItem
            // 
            sIDToolStripMenuItem.Name = "sIDToolStripMenuItem";
            sIDToolStripMenuItem.Size = new Size(216, 22);
            sIDToolStripMenuItem.Text = "SID or MOD";
            sIDToolStripMenuItem.ToolTipText = "Run local files externally";
            sIDToolStripMenuItem.Click += runnersMenuItem_Click;
            // 
            // allFilesToolStripMenuItem
            // 
            allFilesToolStripMenuItem.Name = "allFilesToolStripMenuItem";
            allFilesToolStripMenuItem.Size = new Size(216, 22);
            allFilesToolStripMenuItem.Text = "All files";
            allFilesToolStripMenuItem.ToolTipText = "Run all types of local files externally";
            allFilesToolStripMenuItem.Click += runnersMenuItem_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(177, 6);
            // 
            // aboutToolStripMenuItem1
            // 
            aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            aboutToolStripMenuItem1.Size = new Size(180, 22);
            aboutToolStripMenuItem1.Text = "About";
            aboutToolStripMenuItem1.Click += aboutToolStripMenuItem1_Click;
            // 
            // windowsToolStripMenuItem
            // 
            windowsToolStripMenuItem.AutoToolTip = true;
            windowsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cascadeWindowsToolStripMenuItem, tileVerticalToolStripMenuItem, tileHorizontalToolStripMenuItem, arrangeIconsToolStripMenuItem, closeAllToolStripMenuItem });
            windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
            windowsToolStripMenuItem.Size = new Size(63, 20);
            windowsToolStripMenuItem.Text = "&Window";
            // 
            // cascadeWindowsToolStripMenuItem
            // 
            cascadeWindowsToolStripMenuItem.Name = "cascadeWindowsToolStripMenuItem";
            cascadeWindowsToolStripMenuItem.Size = new Size(168, 22);
            cascadeWindowsToolStripMenuItem.Text = "Cascade windows";
            cascadeWindowsToolStripMenuItem.Click += windowsToolStripMenuItem_Click;
            // 
            // tileVerticalToolStripMenuItem
            // 
            tileVerticalToolStripMenuItem.Name = "tileVerticalToolStripMenuItem";
            tileVerticalToolStripMenuItem.Size = new Size(168, 22);
            tileVerticalToolStripMenuItem.Text = "Tile Vertical";
            tileVerticalToolStripMenuItem.Click += windowsToolStripMenuItem_Click;
            // 
            // tileHorizontalToolStripMenuItem
            // 
            tileHorizontalToolStripMenuItem.Name = "tileHorizontalToolStripMenuItem";
            tileHorizontalToolStripMenuItem.Size = new Size(168, 22);
            tileHorizontalToolStripMenuItem.Text = "Tile Horizontal";
            tileHorizontalToolStripMenuItem.Click += windowsToolStripMenuItem_Click;
            // 
            // arrangeIconsToolStripMenuItem
            // 
            arrangeIconsToolStripMenuItem.Name = "arrangeIconsToolStripMenuItem";
            arrangeIconsToolStripMenuItem.Size = new Size(168, 22);
            arrangeIconsToolStripMenuItem.Text = "Arrange Icons";
            arrangeIconsToolStripMenuItem.Click += windowsToolStripMenuItem_Click;
            // 
            // closeAllToolStripMenuItem
            // 
            closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            closeAllToolStripMenuItem.Size = new Size(168, 22);
            closeAllToolStripMenuItem.Text = "Close All";
            closeAllToolStripMenuItem.Click += windowsToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutC64SorterToolStripMenuItem, linksToolStripMenuItem });
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(52, 20);
            aboutToolStripMenuItem.Text = "About";
            // 
            // aboutC64SorterToolStripMenuItem
            // 
            aboutC64SorterToolStripMenuItem.Name = "aboutC64SorterToolStripMenuItem";
            aboutC64SorterToolStripMenuItem.Size = new Size(161, 22);
            aboutC64SorterToolStripMenuItem.Text = "About C64Sorter";
            aboutC64SorterToolStripMenuItem.Click += aboutC64SorterToolStripMenuItem_Click;
            // 
            // linksToolStripMenuItem
            // 
            linksToolStripMenuItem.Name = "linksToolStripMenuItem";
            linksToolStripMenuItem.Size = new Size(161, 22);
            linksToolStripMenuItem.Text = "Links";
            linksToolStripMenuItem.Click += linksToolStripMenuItem_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelStatus });
            toolStrip1.Location = new Point(0, 632);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1032, 25);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripStatusLabelStatus
            // 
            toolStripStatusLabelStatus.Name = "toolStripStatusLabelStatus";
            toolStripStatusLabelStatus.Size = new Size(57, 22);
            toolStripStatusLabelStatus.Text = "Welcome";
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(213, 6);
            // 
            // MainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1032, 657);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            IsMdiContainer = true;
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "Kolibri.net.C64Sorter";
            DragDrop += MainForm_DragDrop;
            DragEnter += MainForm_DragEnter;
            DragOver += MainForm_DragOver;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }



        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripMenuItem organizeFilesToolStripMenuItem;
        private ToolStripMenuItem singleFileCategorizerToolStripMenuItem;
        private ToolStripLabel toolStripStatusLabelStatus;
        private ToolStripMenuItem windowsToolStripMenuItem;
        private ToolStripMenuItem cascadeWindowsToolStripMenuItem;
        private ToolStripMenuItem closeAllToolStripMenuItem;
        private ToolStripMenuItem tileVerticalToolStripMenuItem;
        private ToolStripMenuItem tileHorizontalToolStripMenuItem;
        private ToolStripMenuItem arrangeIconsToolStripMenuItem;
        private ToolStripMenuItem extensionOrganizerToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemRemoveEmptyDirs;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem showContentsToolStripMenuItem;
        private ToolStripMenuItem d64FoldersToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem pNGFilesToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemFTPClient;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItemHostname;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem aboutC64SorterToolStripMenuItem;
        private ToolStripMenuItem browseLocalFilesToolStripMenuItem;
        private ToolStripMenuItem ultimateEliteIIToolStripMenuItem;
        private ToolStripMenuItem machineToolStripMenuItem;
        private ToolStripMenuItem resetToolStripMenuItem;
        private ToolStripMenuItem rebootToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem pauseToolStripMenuItem;
        private ToolStripMenuItem resumeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem powerOffToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItemRun;
        private ToolStripMenuItem pRGToolStripMenuItem;
        private ToolStripMenuItem d64ToolStripMenuItem;
        private ToolStripMenuItem sIDToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem linksToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem volumeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem toolStripMenuItemFTPTreeView;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem allFilesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
    }
}
