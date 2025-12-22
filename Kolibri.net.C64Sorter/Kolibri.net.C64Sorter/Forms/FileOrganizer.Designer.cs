namespace File_Organizer
{
    partial class FileOrganizer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileOrganizer));
            btn_select_folder = new Button();
            button1 = new Button();
            panel1 = new Panel();
            panel2 = new Panel();
            fileCount = new Label();
            label3 = new Label();
            currentFolder = new Label();
            label1 = new Label();
            panel2.SuspendLayout();
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
            btn_select_folder.Location = new Point(305, 296);
            btn_select_folder.Name = "btn_select_folder";
            btn_select_folder.Size = new Size(226, 50);
            btn_select_folder.TabIndex = 0;
            btn_select_folder.Text = "Select Folder";
            btn_select_folder.UseVisualStyleBackColor = false;
            btn_select_folder.Click += Button1_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.Magenta;
            button1.FlatAppearance.BorderColor = SystemColors.ControlLightLight;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Microsoft Sans Serif", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = SystemColors.ControlLightLight;
            button1.Location = new Point(305, 365);
            button1.Name = "button1";
            button1.Size = new Size(226, 44);
            button1.TabIndex = 1;
            button1.Text = "Organize!";
            button1.UseVisualStyleBackColor = false;
            button1.Click += Button1_Click_1;
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
            panel2.BackColor = Color.PaleGoldenrod;
            panel2.BackgroundImage = (Image)resources.GetObject("panel2.BackgroundImage");
            panel2.BackgroundImageLayout = ImageLayout.Zoom;
            panel2.Controls.Add(fileCount);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(currentFolder);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(button1);
            panel2.Controls.Add(btn_select_folder);
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(801, 519);
            panel2.TabIndex = 3;
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
            fileCount.Click += FileCount_Click;
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
            label3.Click += Label3_Click;
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
            currentFolder.Click += CurrentFolder_Click;
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
            // 
            // FileOrganizer
            // 
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 519);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FileOrganizer";
            Text = "File Organizer (Orgainize by file category)";
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_select_folder;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label currentFolder;
        private System.Windows.Forms.Label fileCount;
        private System.Windows.Forms.Label label3;
    }
}

