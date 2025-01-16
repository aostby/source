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
            textBoxSource = new TextBox();
            textBoxDestination = new TextBox();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelFilnavn = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            buttonOpenDirS = new Button();
            buttonDestination = new Button();
            buttonSource = new Button();
            buttonOpenDirD = new Button();
            checkBoxXCopyAndDelete = new CheckBox();
            buttonExecute = new Button();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // textBoxSource
            // 
            textBoxSource.Location = new Point(12, 12);
            textBoxSource.Name = "textBoxSource";
            textBoxSource.Size = new Size(522, 23);
            textBoxSource.TabIndex = 0;
            textBoxSource.TextChanged += textBox_TextChanged;
            // 
            // textBoxDestination
            // 
            textBoxDestination.Location = new Point(12, 43);
            textBoxDestination.Name = "textBoxDestination";
            textBoxDestination.Size = new Size(522, 23);
            textBoxDestination.TabIndex = 1;
            textBoxDestination.TextChanged += textBox_TextChanged;
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
            // buttonOpenDirS
            // 
            buttonOpenDirS.Location = new Point(546, 12);
            buttonOpenDirS.Name = "buttonOpenDirS";
            buttonOpenDirS.Size = new Size(27, 23);
            buttonOpenDirS.TabIndex = 3;
            buttonOpenDirS.Text = "Vis kildemappe";
            buttonOpenDirS.UseVisualStyleBackColor = true;
            buttonOpenDirS.Click += buttonOpenDir_Click;
            // 
            // buttonDestination
            // 
            buttonDestination.Location = new Point(591, 43);
            buttonDestination.Name = "buttonDestination";
            buttonDestination.Size = new Size(75, 23);
            buttonDestination.TabIndex = 4;
            buttonDestination.Text = "Let opp destinasjon";
            buttonDestination.UseVisualStyleBackColor = true;
            buttonDestination.Click += button_Click;
            // 
            // buttonSource
            // 
            buttonSource.Location = new Point(590, 12);
            buttonSource.Name = "buttonSource";
            buttonSource.Size = new Size(75, 23);
            buttonSource.TabIndex = 5;
            buttonSource.Text = "Let opp kilde";
            buttonSource.UseVisualStyleBackColor = true;
            buttonSource.Click += button_Click;
            // 
            // buttonOpenDirD
            // 
            buttonOpenDirD.Location = new Point(547, 43);
            buttonOpenDirD.Name = "buttonOpenDirD";
            buttonOpenDirD.Size = new Size(27, 23);
            buttonOpenDirD.TabIndex = 6;
            buttonOpenDirD.Text = "Vis destinasjonsmappe";
            buttonOpenDirD.UseVisualStyleBackColor = true;
            buttonOpenDirD.Click += buttonOpenDir_Click;
            // 
            // checkBoxXCopyAndDelete
            // 
            checkBoxXCopyAndDelete.AutoSize = true;
            checkBoxXCopyAndDelete.Location = new Point(15, 75);
            checkBoxXCopyAndDelete.Name = "checkBoxXCopyAndDelete";
            checkBoxXCopyAndDelete.Size = new Size(242, 19);
            checkBoxXCopyAndDelete.TabIndex = 7;
            checkBoxXCopyAndDelete.Text = "RoboCopy and Delete from source folder";
            checkBoxXCopyAndDelete.UseVisualStyleBackColor = true;
            // 
            // buttonExecute
            // 
            buttonExecute.Location = new Point(671, 12);
            buttonExecute.Name = "buttonExecute";
            buttonExecute.Size = new Size(85, 54);
            buttonExecute.TabIndex = 8;
            buttonExecute.Text = "Utfør";
            buttonExecute.UseVisualStyleBackColor = true;
            buttonExecute.Click += buttonExecute_Click;
            // 
            // SortMultimediaDesktopForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 519);
            Controls.Add(buttonExecute);
            Controls.Add(checkBoxXCopyAndDelete);
            Controls.Add(buttonOpenDirD);
            Controls.Add(buttonSource);
            Controls.Add(buttonDestination);
            Controls.Add(buttonOpenDirS);
            Controls.Add(statusStrip1);
            Controls.Add(textBoxDestination);
            Controls.Add(textBoxSource);
            Margin = new Padding(4, 3, 4, 3);
            Name = "SortMultimediaDesktopForm";
            Text = "SortMoviesDesktopForm";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxSource;
        private TextBox textBoxDestination;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabelFilnavn;
        private Button buttonOpenDirS;
        private Button buttonDestination;
        private Button buttonSource;
        private Button buttonOpenDirD;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private CheckBox checkBoxXCopyAndDelete;
        private Button buttonExecute;
    }
}