namespace File_Organizer
{
    partial class ExtensionOrganizer
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExtensionOrganizer));
            btn_select_folder = new Button();
            buttonExtensionSort = new Button();
            panel1 = new Panel();
            panel2 = new Panel();
            buttonFlattenC64files = new Button();
            buttonAlphabetizeSort = new Button();
            toolStrip1 = new ToolStrip();
            toolStripStatusLabel1 = new ToolStripLabel();
            fileCount = new Label();
            label3 = new Label();
            currentFolder = new Label();
            label1 = new Label();
            panel2.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btn_select_folder
            // 
            btn_select_folder.BackColor = Color.Transparent;
            btn_select_folder.FlatAppearance.BorderColor = Color.White;
            btn_select_folder.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 192, 192);
            btn_select_folder.FlatStyle = FlatStyle.Flat;
            btn_select_folder.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btn_select_folder.ForeColor = SystemColors.ControlLightLight;
            btn_select_folder.Location = new Point(289, 181);
            btn_select_folder.Name = "btn_select_folder";
            btn_select_folder.Size = new Size(273, 50);
            btn_select_folder.TabIndex = 0;
            btn_select_folder.Text = "Select Folder";
            btn_select_folder.UseVisualStyleBackColor = false;
            btn_select_folder.Click += SelectFolder_Click;
            // 
            // buttonExtensionSort
            // 
            buttonExtensionSort.BackColor = Color.DeepSkyBlue;
            buttonExtensionSort.FlatAppearance.BorderColor = SystemColors.ControlLightLight;
            buttonExtensionSort.FlatStyle = FlatStyle.Flat;
            buttonExtensionSort.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonExtensionSort.ForeColor = SystemColors.ControlLightLight;
            buttonExtensionSort.Location = new Point(196, 237);
            buttonExtensionSort.Name = "buttonExtensionSort";
            buttonExtensionSort.Size = new Size(450, 44);
            buttonExtensionSort.TabIndex = 1;
            buttonExtensionSort.Text = "Organize files by C64 extensions";
            buttonExtensionSort.UseVisualStyleBackColor = false;
            buttonExtensionSort.Click += Organize_Click;
            // 
            // panel1
            // 
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 100);
            panel1.TabIndex = 2;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.AutoSize = true;
            panel2.BackColor = Color.LightSkyBlue;
            panel2.BackgroundImage = (Image)resources.GetObject("panel2.BackgroundImage");
            panel2.BackgroundImageLayout = ImageLayout.Zoom;
            panel2.Controls.Add(buttonFlattenC64files);
            panel2.Controls.Add(buttonAlphabetizeSort);
            panel2.Controls.Add(toolStrip1);
            panel2.Controls.Add(fileCount);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(currentFolder);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(buttonExtensionSort);
            panel2.Controls.Add(btn_select_folder);
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(1281, 478);
            panel2.TabIndex = 3;
            // 
            // buttonFlattenC64files
            // 
            buttonFlattenC64files.BackColor = Color.DeepSkyBlue;
            buttonFlattenC64files.FlatAppearance.BorderColor = SystemColors.ControlLightLight;
            buttonFlattenC64files.FlatStyle = FlatStyle.Flat;
            buttonFlattenC64files.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonFlattenC64files.ForeColor = SystemColors.ControlLightLight;
            buttonFlattenC64files.Location = new Point(196, 337);
            buttonFlattenC64files.Name = "buttonFlattenC64files";
            buttonFlattenC64files.Size = new Size(450, 44);
            buttonFlattenC64files.TabIndex = 8;
            buttonFlattenC64files.Text = "Flatten C64 files to current folder";
            buttonFlattenC64files.UseVisualStyleBackColor = false;
            buttonFlattenC64files.Click += Organize_Click;
            // 
            // buttonAlphabetizeSort
            // 
            buttonAlphabetizeSort.BackColor = Color.DeepSkyBlue;
            buttonAlphabetizeSort.FlatAppearance.BorderColor = SystemColors.ControlLightLight;
            buttonAlphabetizeSort.FlatStyle = FlatStyle.Flat;
            buttonAlphabetizeSort.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonAlphabetizeSort.ForeColor = SystemColors.ControlLightLight;
            buttonAlphabetizeSort.Location = new Point(196, 287);
            buttonAlphabetizeSort.Name = "buttonAlphabetizeSort";
            buttonAlphabetizeSort.Size = new Size(450, 44);
            buttonAlphabetizeSort.TabIndex = 7;
            buttonAlphabetizeSort.Text = "Alphabetize current folder by filenames";
            buttonAlphabetizeSort.UseVisualStyleBackColor = false;
            buttonAlphabetizeSort.Click += Organize_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.Bottom;
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            toolStrip1.Location = new Point(0, 453);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1281, 25);
            toolStrip1.Stretch = true;
            toolStrip1.TabIndex = 6;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(60, 22);
            toolStripStatusLabel1.Text = "Welcome!";
            // 
            // fileCount
            // 
            fileCount.AutoSize = true;
            fileCount.Cursor = Cursors.IBeam;
            fileCount.FlatStyle = FlatStyle.Flat;
            fileCount.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            fileCount.ForeColor = SystemColors.ControlLightLight;
            fileCount.Location = new Point(12, 170);
            fileCount.MinimumSize = new Size(200, 20);
            fileCount.Name = "fileCount";
            fileCount.Size = new Size(200, 24);
            fileCount.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = SystemColors.ControlLightLight;
            label3.Cursor = Cursors.IBeam;
            label3.FlatStyle = FlatStyle.Flat;
            label3.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.BlueViolet;
            label3.Location = new Point(12, 134);
            label3.Name = "label3";
            label3.Size = new Size(128, 24);
            label3.TabIndex = 4;
            label3.Text = "Files Found:";
            // 
            // currentFolder
            // 
            currentFolder.AutoSize = true;
            currentFolder.Cursor = Cursors.IBeam;
            currentFolder.FlatStyle = FlatStyle.Flat;
            currentFolder.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            currentFolder.ForeColor = SystemColors.ControlLightLight;
            currentFolder.Location = new Point(12, 86);
            currentFolder.MinimumSize = new Size(200, 20);
            currentFolder.Name = "currentFolder";
            currentFolder.Size = new Size(200, 24);
            currentFolder.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ControlLightLight;
            label1.Cursor = Cursors.IBeam;
            label1.FlatStyle = FlatStyle.Flat;
            label1.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.BlueViolet;
            label1.Location = new Point(12, 50);
            label1.Name = "label1";
            label1.Size = new Size(152, 24);
            label1.TabIndex = 2;
            label1.Text = "Current Folder:";
            label1.Click += label1_Click;
            // 
            // ExtensionOrganizer
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1280, 478);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ExtensionOrganizer";
            Text = "Extension Organizer (Organize by extension, then alphabet if folders, or aplphabetize current folder)";
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_select_folder;
        private System.Windows.Forms.Button buttonExtensionSort;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label currentFolder;
        private System.Windows.Forms.Label fileCount;
        private System.Windows.Forms.Label label3;
        private ToolStrip toolStrip1;
        private ToolStripLabel toolStripStatusLabel1;
        private Button buttonAlphabetizeSort;
        private Button buttonFlattenC64files;
    }
}

