namespace Kolibri.SortPictures.Forms
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
            verktøyToolStripMenuItem = new ToolStripMenuItem();
            fjernTommeMapperToolStripMenuItem = new ToolStripMenuItem();
            listExtensionsToolStripMenuItem = new ToolStripMenuItem();
            lagIconFilerToolStripMenuItem = new ToolStripMenuItem();
            bilderToolStripMenuItem = new ToolStripMenuItem();
            sorterBilderToolStripMenuItem = new ToolStripMenuItem();
            reduserBildestørrelseToolStripMenuItem = new ToolStripMenuItem();
            hjelpToolStripMenuItem = new ToolStripMenuItem();
            brukermanualToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { filToolStripMenuItem, verktøyToolStripMenuItem, bilderToolStripMenuItem, hjelpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1107, 24);
            menuStrip1.TabIndex = 0;
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
            verktøyToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { fjernTommeMapperToolStripMenuItem, listExtensionsToolStripMenuItem, lagIconFilerToolStripMenuItem });
            verktøyToolStripMenuItem.Name = "verktøyToolStripMenuItem";
            verktøyToolStripMenuItem.Size = new Size(58, 20);
            verktøyToolStripMenuItem.Text = "Verktøy";
            // 
            // fjernTommeMapperToolStripMenuItem
            // 
            fjernTommeMapperToolStripMenuItem.Name = "fjernTommeMapperToolStripMenuItem";
            fjernTommeMapperToolStripMenuItem.Size = new Size(186, 22);
            fjernTommeMapperToolStripMenuItem.Text = "Fjern tomme mapper";
            fjernTommeMapperToolStripMenuItem.Click += MappeUtilsToolStripMenuItem_Click;
            // 
            // listExtensionsToolStripMenuItem
            // 
            listExtensionsToolStripMenuItem.Name = "listExtensionsToolStripMenuItem";
            listExtensionsToolStripMenuItem.Size = new Size(186, 22);
            listExtensionsToolStripMenuItem.Text = "List extensions";
            listExtensionsToolStripMenuItem.Click += MappeUtilsToolStripMenuItem_Click;
            // 
            // lagIconFilerToolStripMenuItem
            // 
            lagIconFilerToolStripMenuItem.Name = "lagIconFilerToolStripMenuItem";
            lagIconFilerToolStripMenuItem.Size = new Size(186, 22);
            lagIconFilerToolStripMenuItem.Text = "Lag icon filer";
            lagIconFilerToolStripMenuItem.Click += lagIconFilerToolStripMenuItem_Click;
            // 
            // bilderToolStripMenuItem
            // 
            bilderToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { sorterBilderToolStripMenuItem, reduserBildestørrelseToolStripMenuItem });
            bilderToolStripMenuItem.Name = "bilderToolStripMenuItem";
            bilderToolStripMenuItem.Size = new Size(49, 20);
            bilderToolStripMenuItem.Text = "Bilder";
            // 
            // sorterBilderToolStripMenuItem
            // 
            sorterBilderToolStripMenuItem.Name = "sorterBilderToolStripMenuItem";
            sorterBilderToolStripMenuItem.Size = new Size(189, 22);
            sorterBilderToolStripMenuItem.Text = "Sorter bilder";
            sorterBilderToolStripMenuItem.Click += MDIChildNew_Click;
            // 
            // reduserBildestørrelseToolStripMenuItem
            // 
            reduserBildestørrelseToolStripMenuItem.Name = "reduserBildestørrelseToolStripMenuItem";
            reduserBildestørrelseToolStripMenuItem.Size = new Size(189, 22);
            reduserBildestørrelseToolStripMenuItem.Text = "Reduser bildestørrelse";
            reduserBildestørrelseToolStripMenuItem.Click += MDIChildNew_Click;
            // 
            // hjelpToolStripMenuItem
            // 
            hjelpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { brukermanualToolStripMenuItem });
            hjelpToolStripMenuItem.Name = "hjelpToolStripMenuItem";
            hjelpToolStripMenuItem.Size = new Size(47, 20);
            hjelpToolStripMenuItem.Text = "Hjelp";
            // 
            // brukermanualToolStripMenuItem
            // 
            brukermanualToolStripMenuItem.Name = "brukermanualToolStripMenuItem";
            brukermanualToolStripMenuItem.Size = new Size(180, 22);
            brukermanualToolStripMenuItem.Text = "Brukermanual";
            brukermanualToolStripMenuItem.Click += brukermanualToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1107, 661);
            Controls.Add(menuStrip1);
            IsMdiContainer = true;
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "MainForm";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem filToolStripMenuItem;
        private ToolStripMenuItem verktøyToolStripMenuItem;
        private ToolStripMenuItem bilderToolStripMenuItem;
        private ToolStripMenuItem sorterBilderToolStripMenuItem;
        private ToolStripMenuItem lukkToolStripMenuItem;
        private ToolStripMenuItem fjernTommeMapperToolStripMenuItem;
        private ToolStripMenuItem reduserBildestørrelseToolStripMenuItem;
        private ToolStripMenuItem lagIconFilerToolStripMenuItem;
        private ToolStripMenuItem listExtensionsToolStripMenuItem;
        private ToolStripMenuItem hjelpToolStripMenuItem;
        private ToolStripMenuItem brukermanualToolStripMenuItem;
    }
}