namespace Kolibri.SilverScreen.Forms
{
    partial class MainForm
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
            menuStrip1 = new MenuStrip();
            filToolStripMenuItem = new ToolStripMenuItem();
            lukkToolStripMenuItem = new ToolStripMenuItem();
            liteDBFilepathToolStripMenuItem = new ToolStripMenuItem();
            innstillingerToolStripMenuItem = new ToolStripMenuItem();
            verktøyToolStripMenuItem = new ToolStripMenuItem();
            finnDuplikaterToolStripMenuItem = new ToolStripMenuItem();
            flyttMapperToolStripMenuItem = new ToolStripMenuItem();
            flyttFilmerToolStripMenuItem = new ToolStripMenuItem();
            bakgrunnOppdateringToolStripMenuItem = new ToolStripMenuItem();
            filmerToolStripMenuItem = new ToolStripMenuItem();
            iMDbDataFilesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            testCircusToolStripMenuItem = new ToolStripMenuItem();
            removeEmptyDirectoriesToolStripMenuItem = new ToolStripMenuItem();
            multiMediaToolStripMenuItem = new ToolStripMenuItem();
            movieslocalToolStripMenuItem = new ToolStripMenuItem();
            serieslocalToolStripMenuItem = new ToolStripMenuItem();
            searchToolStripMenuItem = new ToolStripMenuItem();
            genreSearchToolStripMenuItem = new ToolStripMenuItem();
            windowsToolStripMenuItem = new ToolStripMenuItem();
            cascadeWindowsToolStripMenuItem = new ToolStripMenuItem();
            tileVerticalToolStripMenuItem = new ToolStripMenuItem();
            tileHorizontalToolStripMenuItem = new ToolStripMenuItem();
            arrangeIconsToolStripMenuItem = new ToolStripMenuItem();
            closeAllToolStripMenuItem = new ToolStripMenuItem();
            grusToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelStatus = new ToolStripStatusLabel();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripSeparator3 = new ToolStripSeparator();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { filToolStripMenuItem, verktøyToolStripMenuItem, multiMediaToolStripMenuItem, windowsToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.MdiWindowListItem = windowsToolStripMenuItem;
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(5, 2, 0, 2);
            menuStrip1.Size = new Size(933, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // filToolStripMenuItem
            // 
            filToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { liteDBFilepathToolStripMenuItem, innstillingerToolStripMenuItem, toolStripSeparator3, lukkToolStripMenuItem });
            filToolStripMenuItem.Name = "filToolStripMenuItem";
            filToolStripMenuItem.Size = new Size(31, 20);
            filToolStripMenuItem.Text = "Fil";
            // 
            // lukkToolStripMenuItem
            // 
            lukkToolStripMenuItem.Name = "lukkToolStripMenuItem";
            lukkToolStripMenuItem.Size = new Size(151, 22);
            lukkToolStripMenuItem.Text = "Lukk";
            lukkToolStripMenuItem.Click += lukkToolStripMenuItem_Click;
            // 
            // liteDBFilepathToolStripMenuItem
            // 
            liteDBFilepathToolStripMenuItem.Name = "liteDBFilepathToolStripMenuItem";
            liteDBFilepathToolStripMenuItem.Size = new Size(151, 22);
            liteDBFilepathToolStripMenuItem.Text = "LiteDB filepath";
            liteDBFilepathToolStripMenuItem.Click += liteDBFilepathToolStripMenuItem_Click;
            // 
            // innstillingerToolStripMenuItem
            // 
            innstillingerToolStripMenuItem.Name = "innstillingerToolStripMenuItem";
            innstillingerToolStripMenuItem.Size = new Size(151, 22);
            innstillingerToolStripMenuItem.Text = "Innstillinger";
            innstillingerToolStripMenuItem.Click += innstillingerToolStripMenuItem_Click;
            // 
            // verktøyToolStripMenuItem
            // 
            verktøyToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { removeEmptyDirectoriesToolStripMenuItem, finnDuplikaterToolStripMenuItem, toolStripSeparator2, flyttMapperToolStripMenuItem, bakgrunnOppdateringToolStripMenuItem, toolStripSeparator1, testCircusToolStripMenuItem });
            verktøyToolStripMenuItem.Name = "verktøyToolStripMenuItem";
            verktøyToolStripMenuItem.Size = new Size(58, 20);
            verktøyToolStripMenuItem.Text = "Verktøy";
            // 
            // finnDuplikaterToolStripMenuItem
            // 
            finnDuplikaterToolStripMenuItem.Name = "finnDuplikaterToolStripMenuItem";
            finnDuplikaterToolStripMenuItem.Size = new Size(193, 22);
            finnDuplikaterToolStripMenuItem.Text = "Finn duplikater";
            finnDuplikaterToolStripMenuItem.Click += finnDuplikaterToolStripMenuItem_Click;
            // 
            // flyttMapperToolStripMenuItem
            // 
            flyttMapperToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { flyttFilmerToolStripMenuItem });
            flyttMapperToolStripMenuItem.Name = "flyttMapperToolStripMenuItem";
            flyttMapperToolStripMenuItem.Size = new Size(193, 22);
            flyttMapperToolStripMenuItem.Text = "Flytt mapper";
            // 
            // flyttFilmerToolStripMenuItem
            // 
            flyttFilmerToolStripMenuItem.Name = "flyttFilmerToolStripMenuItem";
            flyttFilmerToolStripMenuItem.Size = new Size(180, 22);
            flyttFilmerToolStripMenuItem.Text = "Flytt filmer";
            flyttFilmerToolStripMenuItem.Click += multiMedialocalToolStripMenuItem_Click;
            // 
            // bakgrunnOppdateringToolStripMenuItem
            // 
            bakgrunnOppdateringToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { filmerToolStripMenuItem, iMDbDataFilesToolStripMenuItem });
            bakgrunnOppdateringToolStripMenuItem.Name = "bakgrunnOppdateringToolStripMenuItem";
            bakgrunnOppdateringToolStripMenuItem.Size = new Size(193, 22);
            bakgrunnOppdateringToolStripMenuItem.Text = "Bakgrunn oppdatering";
            // 
            // filmerToolStripMenuItem
            // 
            filmerToolStripMenuItem.Name = "filmerToolStripMenuItem";
            filmerToolStripMenuItem.Size = new Size(150, 22);
            filmerToolStripMenuItem.Text = "Filmer";
            filmerToolStripMenuItem.Click += filmerToolStripMenuItem_Click;
            // 
            // iMDbDataFilesToolStripMenuItem
            // 
            iMDbDataFilesToolStripMenuItem.Name = "iMDbDataFilesToolStripMenuItem";
            iMDbDataFilesToolStripMenuItem.Size = new Size(150, 22);
            iMDbDataFilesToolStripMenuItem.Text = "IMDbDataFiles";
            iMDbDataFilesToolStripMenuItem.Click += multiMedialocalToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(190, 6);
            // 
            // testCircusToolStripMenuItem
            // 
            testCircusToolStripMenuItem.Name = "testCircusToolStripMenuItem";
            testCircusToolStripMenuItem.Size = new Size(193, 22);
            testCircusToolStripMenuItem.Text = "Test Circus";
            testCircusToolStripMenuItem.Click += multiMedialocalToolStripMenuItem_Click;
            // 
            // removeEmptyDirectoriesToolStripMenuItem
            // 
            removeEmptyDirectoriesToolStripMenuItem.Name = "removeEmptyDirectoriesToolStripMenuItem";
            removeEmptyDirectoriesToolStripMenuItem.Size = new Size(193, 22);
            removeEmptyDirectoriesToolStripMenuItem.Text = "Slett tomme mapper";
            removeEmptyDirectoriesToolStripMenuItem.Click += removeEmptyDirectoriesToolStripMenuItem_Click;
            // 
            // multiMediaToolStripMenuItem
            // 
            multiMediaToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { movieslocalToolStripMenuItem, serieslocalToolStripMenuItem, searchToolStripMenuItem, genreSearchToolStripMenuItem });
            multiMediaToolStripMenuItem.Name = "multiMediaToolStripMenuItem";
            multiMediaToolStripMenuItem.Size = new Size(80, 20);
            multiMediaToolStripMenuItem.Text = "MultiMedia";
            // 
            // movieslocalToolStripMenuItem
            // 
            movieslocalToolStripMenuItem.Name = "movieslocalToolStripMenuItem";
            movieslocalToolStripMenuItem.Size = new Size(148, 22);
            movieslocalToolStripMenuItem.Text = "Movies (local)";
            movieslocalToolStripMenuItem.Click += multiMedialocalToolStripMenuItem_Click;
            // 
            // serieslocalToolStripMenuItem
            // 
            serieslocalToolStripMenuItem.Name = "serieslocalToolStripMenuItem";
            serieslocalToolStripMenuItem.Size = new Size(148, 22);
            serieslocalToolStripMenuItem.Text = "Series (local)";
            serieslocalToolStripMenuItem.Click += multiMedialocalToolStripMenuItem_Click;
            // 
            // searchToolStripMenuItem
            // 
            searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            searchToolStripMenuItem.Size = new Size(148, 22);
            searchToolStripMenuItem.Text = "Search...";
            searchToolStripMenuItem.Click += multiMedialocalToolStripMenuItem_Click;
            // 
            // genreSearchToolStripMenuItem
            // 
            genreSearchToolStripMenuItem.Name = "genreSearchToolStripMenuItem";
            genreSearchToolStripMenuItem.Size = new Size(148, 22);
            genreSearchToolStripMenuItem.Text = "Genre search";
            genreSearchToolStripMenuItem.Click += multiMedialocalToolStripMenuItem_Click;
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
            // grusToolStripMenuItem
            // 
            grusToolStripMenuItem.Name = "grusToolStripMenuItem";
            grusToolStripMenuItem.Size = new Size(180, 22);
            grusToolStripMenuItem.Text = "grus";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelStatus });
            statusStrip1.Location = new Point(0, 497);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(933, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelStatus
            // 
            toolStripStatusLabelStatus.Name = "toolStripStatusLabelStatus";
            toolStripStatusLabelStatus.Size = new Size(39, 17);
            toolStripStatusLabelStatus.Text = "Status";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(190, 6);
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(148, 6);
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(933, 519);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            IsMdiContainer = true;
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "Kolibri SilverScreen";
            WindowState = FormWindowState.Maximized;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem filToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lukkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem grusToolStripMenuItem;
        private ToolStripMenuItem verktøyToolStripMenuItem;
        private ToolStripMenuItem finnDuplikaterToolStripMenuItem;
        private ToolStripMenuItem multiMediaToolStripMenuItem;
        private ToolStripMenuItem movieslocalToolStripMenuItem;
        private ToolStripMenuItem liteDBFilepathToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripMenuItem serieslocalToolStripMenuItem;
        private ToolStripStatusLabel toolStripStatusLabelStatus;
        private ToolStripMenuItem innstillingerToolStripMenuItem;
        private ToolStripMenuItem flyttMapperToolStripMenuItem;
        private ToolStripMenuItem flyttFilmerToolStripMenuItem;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripMenuItem genreSearchToolStripMenuItem;
        private ToolStripMenuItem bakgrunnOppdateringToolStripMenuItem;
        private ToolStripMenuItem filmerToolStripMenuItem;
        private ToolStripMenuItem windowsToolStripMenuItem;
        private ToolStripMenuItem cascadeWindowsToolStripMenuItem;
        private ToolStripMenuItem closeAllToolStripMenuItem;
        private ToolStripMenuItem tileVerticalToolStripMenuItem;
        private ToolStripMenuItem tileHorizontalToolStripMenuItem;
        private ToolStripMenuItem arrangeIconsToolStripMenuItem;
        private ToolStripMenuItem iMDbDataFilesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem testCircusToolStripMenuItem;
        private ToolStripMenuItem removeEmptyDirectoriesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator2;
    }
}