namespace Kolibri.net.SilverScreen.Forms
{
    partial class SortMultimediaDesktopForm:Form
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
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelFilnavn = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            groupBox1 = new GroupBox();
            textBoxSource = new TextBox();
            buttonExecute = new Button();
            textBoxDestination = new TextBox();
            checkBoxXCopyAndDelete = new CheckBox();
            buttonOpenDirS = new Button();
            buttonOpenDirD = new Button();
            buttonDestination = new Button();
            buttonSource = new Button();
            groupBox2 = new GroupBox();
            linkLabelMovieFolder = new LinkLabel();
            buttonMovieFolderMoveFiles = new Button();
            buttonMovieFolder = new Button();
            statusStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelFilnavn, toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 497);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(882, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelFilnavn
            // 
            toolStripStatusLabelFilnavn.Name = "toolStripStatusLabelFilnavn";
            toolStripStatusLabelFilnavn.Size = new Size(0, 17);
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textBoxSource);
            groupBox1.Controls.Add(buttonExecute);
            groupBox1.Controls.Add(textBoxDestination);
            groupBox1.Controls.Add(checkBoxXCopyAndDelete);
            groupBox1.Controls.Add(buttonOpenDirS);
            groupBox1.Controls.Add(buttonOpenDirD);
            groupBox1.Controls.Add(buttonDestination);
            groupBox1.Controls.Add(buttonSource);
            groupBox1.Location = new Point(12, 108);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(832, 140);
            groupBox1.TabIndex = 9;
            groupBox1.TabStop = false;
            groupBox1.Text = "Move movies to year, based on folder name";
            // 
            // textBoxSource
            // 
            textBoxSource.Location = new Point(25, 42);
            textBoxSource.Name = "textBoxSource";
            textBoxSource.Size = new Size(522, 23);
            textBoxSource.TabIndex = 0;
            textBoxSource.TextChanged += textBox_TextChanged;
            // 
            // buttonExecute
            // 
            buttonExecute.Location = new Point(684, 42);
            buttonExecute.Name = "buttonExecute";
            buttonExecute.Size = new Size(85, 54);
            buttonExecute.TabIndex = 8;
            buttonExecute.Text = "Utfør";
            buttonExecute.UseVisualStyleBackColor = true;
            buttonExecute.Click += buttonExecute_Click;
            // 
            // textBoxDestination
            // 
            textBoxDestination.Location = new Point(25, 73);
            textBoxDestination.Name = "textBoxDestination";
            textBoxDestination.Size = new Size(522, 23);
            textBoxDestination.TabIndex = 1;
            textBoxDestination.TextChanged += textBox_TextChanged;
            // 
            // checkBoxXCopyAndDelete
            // 
            checkBoxXCopyAndDelete.AutoSize = true;
            checkBoxXCopyAndDelete.Location = new Point(28, 105);
            checkBoxXCopyAndDelete.Name = "checkBoxXCopyAndDelete";
            checkBoxXCopyAndDelete.Size = new Size(242, 19);
            checkBoxXCopyAndDelete.TabIndex = 7;
            checkBoxXCopyAndDelete.Text = "RoboCopy and Delete from source folder";
            checkBoxXCopyAndDelete.UseVisualStyleBackColor = true;
            // 
            // buttonOpenDirS
            // 
            buttonOpenDirS.Location = new Point(559, 42);
            buttonOpenDirS.Name = "buttonOpenDirS";
            buttonOpenDirS.Size = new Size(27, 23);
            buttonOpenDirS.TabIndex = 3;
            buttonOpenDirS.Text = "Vis kildemappe";
            buttonOpenDirS.UseVisualStyleBackColor = true;
            buttonOpenDirS.Click += buttonOpenDir_Click;
            // 
            // buttonOpenDirD
            // 
            buttonOpenDirD.Location = new Point(560, 73);
            buttonOpenDirD.Name = "buttonOpenDirD";
            buttonOpenDirD.Size = new Size(27, 23);
            buttonOpenDirD.TabIndex = 6;
            buttonOpenDirD.Text = "Vis destinasjonsmappe";
            buttonOpenDirD.UseVisualStyleBackColor = true;
            buttonOpenDirD.Click += buttonOpenDir_Click;
            // 
            // buttonDestination
            // 
            buttonDestination.Location = new Point(604, 73);
            buttonDestination.Name = "buttonDestination";
            buttonDestination.Size = new Size(75, 23);
            buttonDestination.TabIndex = 4;
            buttonDestination.Text = "Let opp destinasjon";
            buttonDestination.UseVisualStyleBackColor = true;
            buttonDestination.Click += button_Click;
            // 
            // buttonSource
            // 
            buttonSource.Location = new Point(603, 42);
            buttonSource.Name = "buttonSource";
            buttonSource.Size = new Size(75, 23);
            buttonSource.TabIndex = 5;
            buttonSource.Text = "Let opp kilde";
            buttonSource.UseVisualStyleBackColor = true;
            buttonSource.Click += button_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(linkLabelMovieFolder);
            groupBox2.Controls.Add(buttonMovieFolderMoveFiles);
            groupBox2.Controls.Add(buttonMovieFolder);
            groupBox2.Location = new Point(12, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(832, 79);
            groupBox2.TabIndex = 10;
            groupBox2.TabStop = false;
            groupBox2.Text = "Large movefolder with many moviefiles - create folders and move Directories based on movie year (from foldername)";
            // 
            // linkLabelMovieFolder
            // 
            linkLabelMovieFolder.AutoSize = true;
            linkLabelMovieFolder.Location = new Point(11, 48);
            linkLabelMovieFolder.Name = "linkLabelMovieFolder";
            linkLabelMovieFolder.Size = new Size(73, 15);
            linkLabelMovieFolder.TabIndex = 10;
            linkLabelMovieFolder.TabStop = true;
            linkLabelMovieFolder.Text = "MovieFolder";
            linkLabelMovieFolder.LinkClicked += linkLabelMovieFolder_LinkClicked;
            // 
            // buttonMovieFolderMoveFiles
            // 
            buttonMovieFolderMoveFiles.Location = new Point(380, 22);
            buttonMovieFolderMoveFiles.Name = "buttonMovieFolderMoveFiles";
            buttonMovieFolderMoveFiles.Size = new Size(85, 23);
            buttonMovieFolderMoveFiles.TabIndex = 9;
            buttonMovieFolderMoveFiles.Text = "Utfør";
            buttonMovieFolderMoveFiles.UseVisualStyleBackColor = true;
            buttonMovieFolderMoveFiles.Click += buttonMovieFolderMoveFiles_Click;
            // 
            // buttonMovieFolder
            // 
            buttonMovieFolder.Location = new Point(6, 22);
            buttonMovieFolder.Name = "buttonMovieFolder";
            buttonMovieFolder.Size = new Size(368, 23);
            buttonMovieFolder.TabIndex = 6;
            buttonMovieFolder.Text = "Let opp kilde";
            buttonMovieFolder.UseVisualStyleBackColor = true;
            buttonMovieFolder.Click += buttonMovieFolder_Click;
            // 
            // SortMultimediaDesktopForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 519);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(statusStrip1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "SortMultimediaDesktopForm";
            Text = "SortMoviesDesktopForm";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelFilnavn;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private GroupBox groupBox1;
        private TextBox textBoxSource;
        private Button buttonExecute;
        private TextBox textBoxDestination;
        private CheckBox checkBoxXCopyAndDelete;
        private Button buttonOpenDirS;
        private Button buttonOpenDirD;
        private Button buttonDestination;
        private Button buttonSource;
        private GroupBox groupBox2;
        private Button buttonMovieFolderMoveFiles;
        private Button buttonMovieFolder;
        private LinkLabel linkLabelMovieFolder;
    }
}