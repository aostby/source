namespace Kolibri.SortPictures.Forms
{
   
 
        partial class SortPicsDesktopForm
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
            components = new System.ComponentModel.Container();
            textBoxSource = new TextBox();
            buttonSource = new Button();
            buttonDestination = new Button();
            textBoxDestination = new TextBox();
            button1 = new Button();
            groupBoxSettings = new GroupBox();
            groupBoxPWSetting = new GroupBox();
            radioButtonPreviewFile = new RadioButton();
            radioButtonPreviewLog = new RadioButton();
            radioButtonNoPreview = new RadioButton();
            checkBoxMTP = new CheckBox();
            groupBoxDirs = new GroupBox();
            checkBoxEmptyDirs = new CheckBox();
            checkBoxSubdirs = new CheckBox();
            groupBoxDateTree = new GroupBox();
            radioButtonByYearMonth = new RadioButton();
            radioButtonByYear = new RadioButton();
            radioButtonHerarchy = new RadioButton();
            radioButtonByDate = new RadioButton();
            groupBoxCpyOrMove = new GroupBox();
            radioButtonFlytt = new RadioButton();
            radioButtonKopier = new RadioButton();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelFilnavn = new ToolStripStatusLabel();
            groupBoxPreview = new GroupBox();
            buttonBreak = new Button();
            buttonOpenDirS = new Button();
            buttonOpenDirD = new Button();
            toolTipSortPics = new ToolTip(components);
            labelKilde = new Label();
            labelDestinasjon = new Label();
            groupBoxSettings.SuspendLayout();
            groupBoxPWSetting.SuspendLayout();
            groupBoxDirs.SuspendLayout();
            groupBoxDateTree.SuspendLayout();
            groupBoxCpyOrMove.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // textBoxSource
            // 
            textBoxSource.Location = new Point(91, 15);
            textBoxSource.Margin = new Padding(4, 3, 4, 3);
            textBoxSource.Name = "textBoxSource";
            textBoxSource.Size = new Size(647, 23);
            textBoxSource.TabIndex = 0;
            textBoxSource.TextChanged += textBox_TextChanged;
            // 
            // buttonSource
            // 
            buttonSource.Location = new Point(775, 10);
            buttonSource.Margin = new Padding(4, 3, 4, 3);
            buttonSource.Name = "buttonSource";
            buttonSource.Size = new Size(88, 27);
            buttonSource.TabIndex = 1;
            buttonSource.Text = "Let opp";
            buttonSource.UseVisualStyleBackColor = true;
            buttonSource.Click += button_Click;
            // 
            // buttonDestination
            // 
            buttonDestination.Location = new Point(775, 40);
            buttonDestination.Margin = new Padding(4, 3, 4, 3);
            buttonDestination.Name = "buttonDestination";
            buttonDestination.Size = new Size(88, 27);
            buttonDestination.TabIndex = 3;
            buttonDestination.Text = "Let opp";
            buttonDestination.UseVisualStyleBackColor = true;
            buttonDestination.Click += button_Click;
            // 
            // textBoxDestination
            // 
            textBoxDestination.Location = new Point(91, 45);
            textBoxDestination.Margin = new Padding(4, 3, 4, 3);
            textBoxDestination.Name = "textBoxDestination";
            textBoxDestination.Size = new Size(647, 23);
            textBoxDestination.TabIndex = 2;
            textBoxDestination.TextChanged += textBox_TextChanged;
            // 
            // button1
            // 
            button1.Location = new Point(774, 83);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(88, 47);
            button1.TabIndex = 4;
            button1.Text = "Utfør";
            button1.UseVisualStyleBackColor = true;
            button1.Click += buttonExecute_Click;
            // 
            // groupBoxSettings
            // 
            groupBoxSettings.Controls.Add(groupBoxPWSetting);
            groupBoxSettings.Controls.Add(checkBoxMTP);
            groupBoxSettings.Controls.Add(groupBoxDirs);
            groupBoxSettings.Controls.Add(groupBoxDateTree);
            groupBoxSettings.Controls.Add(groupBoxCpyOrMove);
            groupBoxSettings.Location = new Point(15, 76);
            groupBoxSettings.Margin = new Padding(4, 3, 4, 3);
            groupBoxSettings.Name = "groupBoxSettings";
            groupBoxSettings.Padding = new Padding(4, 3, 4, 3);
            groupBoxSettings.Size = new Size(751, 151);
            groupBoxSettings.TabIndex = 5;
            groupBoxSettings.TabStop = false;
            groupBoxSettings.Text = "Innstillinger";
            // 
            // groupBoxPWSetting
            // 
            groupBoxPWSetting.Controls.Add(radioButtonPreviewFile);
            groupBoxPWSetting.Controls.Add(radioButtonPreviewLog);
            groupBoxPWSetting.Controls.Add(radioButtonNoPreview);
            groupBoxPWSetting.Location = new Point(581, 52);
            groupBoxPWSetting.Margin = new Padding(4, 3, 4, 3);
            groupBoxPWSetting.Name = "groupBoxPWSetting";
            groupBoxPWSetting.Padding = new Padding(4, 3, 4, 3);
            groupBoxPWSetting.Size = new Size(153, 92);
            groupBoxPWSetting.TabIndex = 3;
            groupBoxPWSetting.TabStop = false;
            groupBoxPWSetting.Text = "Kun dato eller lage tre";
            // 
            // radioButtonPreviewFile
            // 
            radioButtonPreviewFile.AutoSize = true;
            radioButtonPreviewFile.Location = new Point(5, 68);
            radioButtonPreviewFile.Margin = new Padding(4, 3, 4, 3);
            radioButtonPreviewFile.Name = "radioButtonPreviewFile";
            radioButtonPreviewFile.Size = new Size(85, 19);
            radioButtonPreviewFile.TabIndex = 2;
            radioButtonPreviewFile.Text = "Preview file";
            toolTipSortPics.SetToolTip(radioButtonPreviewFile, "flytt/kopier, men vis bilder underveis.");
            radioButtonPreviewFile.UseVisualStyleBackColor = true;
            radioButtonPreviewFile.CheckedChanged += radioButtonPreview_CheckedChanged;
            // 
            // radioButtonPreviewLog
            // 
            radioButtonPreviewLog.AutoSize = true;
            radioButtonPreviewLog.Location = new Point(5, 44);
            radioButtonPreviewLog.Margin = new Padding(4, 3, 4, 3);
            radioButtonPreviewLog.Name = "radioButtonPreviewLog";
            radioButtonPreviewLog.Size = new Size(86, 19);
            radioButtonPreviewLog.TabIndex = 1;
            radioButtonPreviewLog.Text = "Preview log";
            toolTipSortPics.SetToolTip(radioButtonPreviewLog, "Flytt/kopier bildene, og lag en logg over hva som er gjort (fra/til).");
            radioButtonPreviewLog.UseVisualStyleBackColor = true;
            radioButtonPreviewLog.CheckedChanged += radioButtonPreview_CheckedChanged;
            // 
            // radioButtonNoPreview
            // 
            radioButtonNoPreview.AutoSize = true;
            radioButtonNoPreview.Checked = true;
            radioButtonNoPreview.Location = new Point(5, 20);
            radioButtonNoPreview.Margin = new Padding(4, 3, 4, 3);
            radioButtonNoPreview.Name = "radioButtonNoPreview";
            radioButtonNoPreview.Size = new Size(99, 19);
            radioButtonNoPreview.TabIndex = 0;
            radioButtonNoPreview.TabStop = true;
            radioButtonNoPreview.Text = "Ingen preview";
            toolTipSortPics.SetToolTip(radioButtonNoPreview, "Raskest, bare kopier/flytt bildene");
            radioButtonNoPreview.UseVisualStyleBackColor = true;
            radioButtonNoPreview.CheckedChanged += radioButtonPreview_CheckedChanged;
            // 
            // checkBoxMTP
            // 
            checkBoxMTP.AutoSize = true;
            checkBoxMTP.Location = new Point(14, 23);
            checkBoxMTP.Margin = new Padding(4, 3, 4, 3);
            checkBoxMTP.Name = "checkBoxMTP";
            checkBoxMTP.Size = new Size(284, 19);
            checkBoxMTP.TabIndex = 5;
            checkBoxMTP.Text = "Kilden er MTP (mobiltelefon via usb eller SD kort)";
            toolTipSortPics.SetToolTip(checkBoxMTP, "Fungerer dårlig å hente bilder rett fra mobiltlf, men prøv å sjekke av for denne om du vil");
            checkBoxMTP.UseVisualStyleBackColor = true;
            checkBoxMTP.CheckedChanged += checkBoxWPD_CheckedChanged;
            // 
            // groupBoxDirs
            // 
            groupBoxDirs.Controls.Add(checkBoxEmptyDirs);
            groupBoxDirs.Controls.Add(checkBoxSubdirs);
            groupBoxDirs.Location = new Point(7, 52);
            groupBoxDirs.Margin = new Padding(4, 3, 4, 3);
            groupBoxDirs.Name = "groupBoxDirs";
            groupBoxDirs.Padding = new Padding(4, 3, 4, 3);
            groupBoxDirs.Size = new Size(211, 92);
            groupBoxDirs.TabIndex = 4;
            groupBoxDirs.TabStop = false;
            groupBoxDirs.Text = "Mappealternativer";
            // 
            // checkBoxEmptyDirs
            // 
            checkBoxEmptyDirs.AutoSize = true;
            checkBoxEmptyDirs.Checked = true;
            checkBoxEmptyDirs.CheckState = CheckState.Checked;
            checkBoxEmptyDirs.Location = new Point(7, 50);
            checkBoxEmptyDirs.Margin = new Padding(4, 3, 4, 3);
            checkBoxEmptyDirs.Name = "checkBoxEmptyDirs";
            checkBoxEmptyDirs.Size = new Size(169, 19);
            checkBoxEmptyDirs.TabIndex = 3;
            checkBoxEmptyDirs.Text = "Fjern tomme undermapper";
            toolTipSortPics.SetToolTip(checkBoxEmptyDirs, "Dersom en flytter, rydd opp tomme mapper etterpå.");
            checkBoxEmptyDirs.UseVisualStyleBackColor = true;
            // 
            // checkBoxSubdirs
            // 
            checkBoxSubdirs.AutoSize = true;
            checkBoxSubdirs.Checked = true;
            checkBoxSubdirs.CheckState = CheckState.Checked;
            checkBoxSubdirs.Location = new Point(7, 24);
            checkBoxSubdirs.Margin = new Padding(4, 3, 4, 3);
            checkBoxSubdirs.Name = "checkBoxSubdirs";
            checkBoxSubdirs.Size = new Size(123, 19);
            checkBoxSubdirs.TabIndex = 0;
            checkBoxSubdirs.Text = "Let i undermapper";
            toolTipSortPics.SetToolTip(checkBoxSubdirs, "Søk etter bilder i undermapper");
            checkBoxSubdirs.UseVisualStyleBackColor = true;
            // 
            // groupBoxDateTree
            // 
            groupBoxDateTree.Controls.Add(radioButtonByYearMonth);
            groupBoxDateTree.Controls.Add(radioButtonByYear);
            groupBoxDateTree.Controls.Add(radioButtonHerarchy);
            groupBoxDateTree.Controls.Add(radioButtonByDate);
            groupBoxDateTree.Location = new Point(376, 52);
            groupBoxDateTree.Margin = new Padding(4, 3, 4, 3);
            groupBoxDateTree.Name = "groupBoxDateTree";
            groupBoxDateTree.Padding = new Padding(4, 3, 4, 3);
            groupBoxDateTree.Size = new Size(189, 92);
            groupBoxDateTree.TabIndex = 2;
            groupBoxDateTree.TabStop = false;
            groupBoxDateTree.Text = "Destinasjon mappestruktur";
            // 
            // radioButtonByYearMonth
            // 
            radioButtonByYearMonth.AutoSize = true;
            radioButtonByYearMonth.Location = new Point(12, 47);
            radioButtonByYearMonth.Margin = new Padding(4, 3, 4, 3);
            radioButtonByYearMonth.Name = "radioButtonByYearMonth";
            radioButtonByYearMonth.Size = new Size(85, 19);
            radioButtonByYearMonth.TabIndex = 3;
            radioButtonByYearMonth.Text = "År / måned";
            toolTipSortPics.SetToolTip(radioButtonByYearMonth, "Legg bildene inn, samlet på årstall og måned de er tatt i , i destinasjonsmappen");
            radioButtonByYearMonth.UseVisualStyleBackColor = true;
            // 
            // radioButtonByYear
            // 
            radioButtonByYear.AutoSize = true;
            radioButtonByYear.Location = new Point(12, 22);
            radioButtonByYear.Margin = new Padding(4, 3, 4, 3);
            radioButtonByYear.Name = "radioButtonByYear";
            radioButtonByYear.Size = new Size(80, 19);
            radioButtonByYear.TabIndex = 2;
            radioButtonByYear.Text = "Kun årstall";
            toolTipSortPics.SetToolTip(radioButtonByYear, "Legg bildene inn, samlet på årstall i destinasjonsmappen");
            radioButtonByYear.UseVisualStyleBackColor = true;
            // 
            // radioButtonHerarchy
            // 
            radioButtonHerarchy.AutoSize = true;
            radioButtonHerarchy.Checked = true;
            radioButtonHerarchy.Location = new Point(104, 48);
            radioButtonHerarchy.Margin = new Padding(4, 3, 4, 3);
            radioButtonHerarchy.Name = "radioButtonHerarchy";
            radioButtonHerarchy.Size = new Size(67, 19);
            radioButtonHerarchy.TabIndex = 1;
            radioButtonHerarchy.TabStop = true;
            radioButtonHerarchy.Text = "Dato tre";
            toolTipSortPics.SetToolTip(radioButtonHerarchy, "Legg bildene inn, sortert på år, måned og dato i destinasjonsmappen.");
            radioButtonHerarchy.UseVisualStyleBackColor = true;
            // 
            // radioButtonByDate
            // 
            radioButtonByDate.AutoSize = true;
            radioButtonByDate.Location = new Point(104, 24);
            radioButtonByDate.Margin = new Padding(4, 3, 4, 3);
            radioButtonByDate.Name = "radioButtonByDate";
            radioButtonByDate.Size = new Size(73, 19);
            radioButtonByDate.TabIndex = 0;
            radioButtonByDate.Text = "Kun dato";
            toolTipSortPics.SetToolTip(radioButtonByDate, "Legg bildene inn basert på dato de er tatt i destinasjonsmappen");
            radioButtonByDate.UseVisualStyleBackColor = true;
            // 
            // groupBoxCpyOrMove
            // 
            groupBoxCpyOrMove.Controls.Add(radioButtonFlytt);
            groupBoxCpyOrMove.Controls.Add(radioButtonKopier);
            groupBoxCpyOrMove.Location = new Point(225, 52);
            groupBoxCpyOrMove.Margin = new Padding(4, 3, 4, 3);
            groupBoxCpyOrMove.Name = "groupBoxCpyOrMove";
            groupBoxCpyOrMove.Padding = new Padding(4, 3, 4, 3);
            groupBoxCpyOrMove.Size = new Size(144, 92);
            groupBoxCpyOrMove.TabIndex = 1;
            groupBoxCpyOrMove.TabStop = false;
            groupBoxCpyOrMove.Text = "Kopier eller flytt filer";
            // 
            // radioButtonFlytt
            // 
            radioButtonFlytt.AutoSize = true;
            radioButtonFlytt.Checked = true;
            radioButtonFlytt.Location = new Point(8, 50);
            radioButtonFlytt.Margin = new Padding(4, 3, 4, 3);
            radioButtonFlytt.Name = "radioButtonFlytt";
            radioButtonFlytt.Size = new Size(48, 19);
            radioButtonFlytt.TabIndex = 1;
            radioButtonFlytt.TabStop = true;
            radioButtonFlytt.Text = "Flytt";
            toolTipSortPics.SetToolTip(radioButtonFlytt, "Flytt bildene til valgt destinasjonsmappe");
            radioButtonFlytt.UseVisualStyleBackColor = true;
            // 
            // radioButtonKopier
            // 
            radioButtonKopier.AutoSize = true;
            radioButtonKopier.Location = new Point(8, 23);
            radioButtonKopier.Margin = new Padding(4, 3, 4, 3);
            radioButtonKopier.Name = "radioButtonKopier";
            radioButtonKopier.Size = new Size(59, 19);
            radioButtonKopier.TabIndex = 0;
            radioButtonKopier.Text = "Kopier";
            toolTipSortPics.SetToolTip(radioButtonKopier, "Kopier bildene til valgt destinasjonsmappe (bildene blir liggende igjen i kildemappen etter utført kopiering)");
            radioButtonKopier.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelFilnavn });
            statusStrip1.Location = new Point(0, 497);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(882, 22);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelFilnavn
            // 
            toolStripStatusLabelFilnavn.Name = "toolStripStatusLabelFilnavn";
            toolStripStatusLabelFilnavn.Size = new Size(150, 17);
            toolStripStatusLabelFilnavn.Text = "toolStripStatusLabelFilnavn";
            // 
            // groupBoxPreview
            // 
            groupBoxPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxPreview.Location = new Point(15, 235);
            groupBoxPreview.Margin = new Padding(4, 3, 4, 3);
            groupBoxPreview.Name = "groupBoxPreview";
            groupBoxPreview.Padding = new Padding(4, 3, 4, 3);
            groupBoxPreview.Size = new Size(751, 245);
            groupBoxPreview.TabIndex = 7;
            groupBoxPreview.TabStop = false;
            groupBoxPreview.Text = "Preview";
            // 
            // buttonBreak
            // 
            buttonBreak.Enabled = false;
            buttonBreak.Font = new Font("Microsoft Sans Serif", 6F, FontStyle.Regular, GraphicsUnit.Point, 0);
            buttonBreak.Location = new Point(775, 137);
            buttonBreak.Margin = new Padding(4, 3, 4, 3);
            buttonBreak.Name = "buttonBreak";
            buttonBreak.Size = new Size(88, 20);
            buttonBreak.TabIndex = 8;
            buttonBreak.Text = "Avbryt";
            buttonBreak.TextAlign = ContentAlignment.TopCenter;
            buttonBreak.UseVisualStyleBackColor = true;
            buttonBreak.Click += buttonBreak_Click;
            // 
            // buttonOpenDirS
            // 
            buttonOpenDirS.Location = new Point(742, 10);
            buttonOpenDirS.Margin = new Padding(4, 3, 4, 3);
            buttonOpenDirS.Name = "buttonOpenDirS";
            buttonOpenDirS.Size = new Size(29, 27);
            buttonOpenDirS.TabIndex = 9;
            buttonOpenDirS.Text = "Let opp";
            buttonOpenDirS.UseVisualStyleBackColor = true;
            buttonOpenDirS.Click += buttonOpenDir_Click;
            // 
            // buttonOpenDirD
            // 
            buttonOpenDirD.Location = new Point(742, 40);
            buttonOpenDirD.Margin = new Padding(4, 3, 4, 3);
            buttonOpenDirD.Name = "buttonOpenDirD";
            buttonOpenDirD.Size = new Size(29, 27);
            buttonOpenDirD.TabIndex = 10;
            buttonOpenDirD.Text = "Let opp";
            buttonOpenDirD.UseVisualStyleBackColor = true;
            buttonOpenDirD.Click += buttonOpenDir_Click;
            // 
            // labelKilde
            // 
            labelKilde.AutoSize = true;
            labelKilde.Location = new Point(19, 21);
            labelKilde.Name = "labelKilde";
            labelKilde.Size = new Size(33, 15);
            labelKilde.TabIndex = 11;
            labelKilde.Text = "Kilde";
            // 
            // labelDestinasjon
            // 
            labelDestinasjon.AutoSize = true;
            labelDestinasjon.Location = new Point(19, 49);
            labelDestinasjon.Name = "labelDestinasjon";
            labelDestinasjon.Size = new Size(68, 15);
            labelDestinasjon.TabIndex = 12;
            labelDestinasjon.Text = "Destinasjon";
            // 
            // SortPicsDesktopForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 519);
            Controls.Add(labelDestinasjon);
            Controls.Add(labelKilde);
            Controls.Add(buttonOpenDirD);
            Controls.Add(buttonOpenDirS);
            Controls.Add(buttonBreak);
            Controls.Add(groupBoxPreview);
            Controls.Add(statusStrip1);
            Controls.Add(groupBoxSettings);
            Controls.Add(button1);
            Controls.Add(buttonDestination);
            Controls.Add(textBoxDestination);
            Controls.Add(buttonSource);
            Controls.Add(textBoxSource);
            Margin = new Padding(4, 3, 4, 3);
            Name = "SortPicsDesktopForm";
            Text = "SortPicsDesktopForm";
            WindowState = FormWindowState.Maximized;
            groupBoxSettings.ResumeLayout(false);
            groupBoxSettings.PerformLayout();
            groupBoxPWSetting.ResumeLayout(false);
            groupBoxPWSetting.PerformLayout();
            groupBoxDirs.ResumeLayout(false);
            groupBoxDirs.PerformLayout();
            groupBoxDateTree.ResumeLayout(false);
            groupBoxDateTree.PerformLayout();
            groupBoxCpyOrMove.ResumeLayout(false);
            groupBoxCpyOrMove.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSource;
            private System.Windows.Forms.Button buttonSource;
            private System.Windows.Forms.Button buttonDestination;
            private System.Windows.Forms.TextBox textBoxDestination;
            private System.Windows.Forms.Button button1;
            private System.Windows.Forms.GroupBox groupBoxSettings;
            private System.Windows.Forms.CheckBox checkBoxSubdirs;
            private System.Windows.Forms.GroupBox groupBoxCpyOrMove;
            private System.Windows.Forms.RadioButton radioButtonFlytt;
            private System.Windows.Forms.RadioButton radioButtonKopier;
            private System.Windows.Forms.StatusStrip statusStrip1;
            private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFilnavn;
            private System.Windows.Forms.GroupBox groupBoxDateTree;
            private System.Windows.Forms.RadioButton radioButtonHerarchy;
            private System.Windows.Forms.RadioButton radioButtonByDate;
            private System.Windows.Forms.GroupBox groupBoxDirs;
            private System.Windows.Forms.CheckBox checkBoxEmptyDirs;
            private System.Windows.Forms.CheckBox checkBoxMTP;
            private System.Windows.Forms.GroupBox groupBoxPWSetting;
            private System.Windows.Forms.RadioButton radioButtonPreviewFile;
            private System.Windows.Forms.RadioButton radioButtonPreviewLog;
            private System.Windows.Forms.RadioButton radioButtonNoPreview;
            private System.Windows.Forms.GroupBox groupBoxPreview;
            private System.Windows.Forms.Button buttonBreak;
            private System.Windows.Forms.Button buttonOpenDirS;
            private System.Windows.Forms.Button buttonOpenDirD;
            private System.Windows.Forms.RadioButton radioButtonByYear;
            private System.Windows.Forms.RadioButton radioButtonByYearMonth;
            private System.Windows.Forms.ToolTip toolTipSortPics;
        private Label labelKilde;
        private Label labelDestinasjon;
    }
    }