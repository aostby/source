namespace SortPics.Forms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip1 = new MenuStrip();
            filToolStripMenuItem = new ToolStripMenuItem();
            lukkToolStripMenuItem = new ToolStripMenuItem();
            verktøyToolStripMenuItem = new ToolStripMenuItem();
            finnDuplikaterToolStripMenuItem = new ToolStripMenuItem();
            grusToolStripMenuItem = new ToolStripMenuItem();
            multiMediaToolStripMenuItem = new ToolStripMenuItem();
            movieslocalToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { filToolStripMenuItem, verktøyToolStripMenuItem, multiMediaToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(5, 2, 0, 2);
            menuStrip1.Size = new Size(933, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // filToolStripMenuItem
            // 
            filToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { lukkToolStripMenuItem });
            filToolStripMenuItem.Name = "filToolStripMenuItem";
            filToolStripMenuItem.Size = new Size(31, 20);
            filToolStripMenuItem.Text = "Fil";
            // 
            // lukkToolStripMenuItem
            // 
            lukkToolStripMenuItem.Name = "lukkToolStripMenuItem";
            lukkToolStripMenuItem.Size = new Size(99, 22);
            lukkToolStripMenuItem.Text = "Lukk";
            lukkToolStripMenuItem.Click += lukkToolStripMenuItem_Click;
            // 
            // verktøyToolStripMenuItem
            // 
            verktøyToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { finnDuplikaterToolStripMenuItem });
            verktøyToolStripMenuItem.Name = "verktøyToolStripMenuItem";
            verktøyToolStripMenuItem.Size = new Size(58, 20);
            verktøyToolStripMenuItem.Text = "Verktøy";
            // 
            // finnDuplikaterToolStripMenuItem
            // 
            finnDuplikaterToolStripMenuItem.Name = "finnDuplikaterToolStripMenuItem";
            finnDuplikaterToolStripMenuItem.Size = new Size(153, 22);
            finnDuplikaterToolStripMenuItem.Text = "Finn duplikater";
            finnDuplikaterToolStripMenuItem.Click += finnDuplikaterToolStripMenuItem_Click;
            // 
            // grusToolStripMenuItem
            // 
            grusToolStripMenuItem.Name = "grusToolStripMenuItem";
            grusToolStripMenuItem.Size = new Size(180, 22);
            grusToolStripMenuItem.Text = "grus";
            // 
            // multiMediaToolStripMenuItem
            // 
            multiMediaToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { movieslocalToolStripMenuItem });
            multiMediaToolStripMenuItem.Name = "multiMediaToolStripMenuItem";
            multiMediaToolStripMenuItem.Size = new Size(80, 20);
            multiMediaToolStripMenuItem.Text = "MultiMedia";
            // 
            // movieslocalToolStripMenuItem
            // 
            movieslocalToolStripMenuItem.Name = "movieslocalToolStripMenuItem";
            movieslocalToolStripMenuItem.Size = new Size(180, 22);
            movieslocalToolStripMenuItem.Text = "Movies (local)";
            movieslocalToolStripMenuItem.Click += multiMedialocalToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(933, 519);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            IsMdiContainer = true;
            MainMenuStrip = menuStrip1;
            Margin = new Padding(4, 3, 4, 3);
            Name = "MainForm";
            Text = "Kolibri Utilities";
            WindowState = FormWindowState.Maximized;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
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
    }
}