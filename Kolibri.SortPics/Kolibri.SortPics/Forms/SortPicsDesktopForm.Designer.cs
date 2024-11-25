namespace SortPics.Forms
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
            this.components = new System.ComponentModel.Container();
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.buttonSource = new System.Windows.Forms.Button();
            this.buttonDestination = new System.Windows.Forms.Button();
            this.textBoxDestination = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.groupBoxPWSetting = new System.Windows.Forms.GroupBox();
            this.radioButtonPreviewFile = new System.Windows.Forms.RadioButton();
            this.radioButtonPreviewLog = new System.Windows.Forms.RadioButton();
            this.radioButtonNoPreview = new System.Windows.Forms.RadioButton();
            this.checkBoxMTP = new System.Windows.Forms.CheckBox();
            this.groupBoxDirs = new System.Windows.Forms.GroupBox();
            this.checkBoxEmptyDirs = new System.Windows.Forms.CheckBox();
            this.checkBoxSubdirs = new System.Windows.Forms.CheckBox();
            this.groupBoxDateTree = new System.Windows.Forms.GroupBox();
            this.radioButtonByYearMonth = new System.Windows.Forms.RadioButton();
            this.radioButtonByYear = new System.Windows.Forms.RadioButton();
            this.radioButtonHerarchy = new System.Windows.Forms.RadioButton();
            this.radioButtonByDate = new System.Windows.Forms.RadioButton();
            this.groupBoxCpyOrMove = new System.Windows.Forms.GroupBox();
            this.radioButtonFlytt = new System.Windows.Forms.RadioButton();
            this.radioButtonKopier = new System.Windows.Forms.RadioButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelFilnavn = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBoxPreview = new System.Windows.Forms.GroupBox();
            this.buttonBreak = new System.Windows.Forms.Button();
            this.buttonOpenDirS = new System.Windows.Forms.Button();
            this.buttonOpenDirD = new System.Windows.Forms.Button();
            this.toolTipSortPics = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxSettings.SuspendLayout();
            this.groupBoxPWSetting.SuspendLayout();
            this.groupBoxDirs.SuspendLayout();
            this.groupBoxDateTree.SuspendLayout();
            this.groupBoxCpyOrMove.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxSource
            // 
            this.textBoxSource.Location = new System.Drawing.Point(13, 13);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.Size = new System.Drawing.Size(620, 20);
            this.textBoxSource.TabIndex = 0;
            this.textBoxSource.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // buttonSource
            // 
            this.buttonSource.Location = new System.Drawing.Point(664, 9);
            this.buttonSource.Name = "buttonSource";
            this.buttonSource.Size = new System.Drawing.Size(75, 23);
            this.buttonSource.TabIndex = 1;
            this.buttonSource.Text = "Let opp";
            this.buttonSource.UseVisualStyleBackColor = true;
            this.buttonSource.Click += new System.EventHandler(this.button_Click);
            // 
            // buttonDestination
            // 
            this.buttonDestination.Location = new System.Drawing.Point(664, 35);
            this.buttonDestination.Name = "buttonDestination";
            this.buttonDestination.Size = new System.Drawing.Size(75, 23);
            this.buttonDestination.TabIndex = 3;
            this.buttonDestination.Text = "Let opp";
            this.buttonDestination.UseVisualStyleBackColor = true;
            this.buttonDestination.Click += new System.EventHandler(this.button_Click);
            // 
            // textBoxDestination
            // 
            this.textBoxDestination.Location = new System.Drawing.Point(13, 39);
            this.textBoxDestination.Name = "textBoxDestination";
            this.textBoxDestination.Size = new System.Drawing.Size(620, 20);
            this.textBoxDestination.TabIndex = 2;
            this.textBoxDestination.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(663, 72);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 41);
            this.button1.TabIndex = 4;
            this.button1.Text = "Utfør";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonExecute_Click);
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.groupBoxPWSetting);
            this.groupBoxSettings.Controls.Add(this.checkBoxMTP);
            this.groupBoxSettings.Controls.Add(this.groupBoxDirs);
            this.groupBoxSettings.Controls.Add(this.groupBoxDateTree);
            this.groupBoxSettings.Controls.Add(this.groupBoxCpyOrMove);
            this.groupBoxSettings.Location = new System.Drawing.Point(13, 66);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(644, 131);
            this.groupBoxSettings.TabIndex = 5;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Innstillinger";
            // 
            // groupBoxPWSetting
            // 
            this.groupBoxPWSetting.Controls.Add(this.radioButtonPreviewFile);
            this.groupBoxPWSetting.Controls.Add(this.radioButtonPreviewLog);
            this.groupBoxPWSetting.Controls.Add(this.radioButtonNoPreview);
            this.groupBoxPWSetting.Location = new System.Drawing.Point(498, 45);
            this.groupBoxPWSetting.Name = "groupBoxPWSetting";
            this.groupBoxPWSetting.Size = new System.Drawing.Size(131, 80);
            this.groupBoxPWSetting.TabIndex = 3;
            this.groupBoxPWSetting.TabStop = false;
            this.groupBoxPWSetting.Text = "Kun dato eller lage tre";
            // 
            // radioButtonPreviewFile
            // 
            this.radioButtonPreviewFile.AutoSize = true;
            this.radioButtonPreviewFile.Location = new System.Drawing.Point(4, 59);
            this.radioButtonPreviewFile.Name = "radioButtonPreviewFile";
            this.radioButtonPreviewFile.Size = new System.Drawing.Size(79, 17);
            this.radioButtonPreviewFile.TabIndex = 2;
            this.radioButtonPreviewFile.Text = "Preview file";
            this.toolTipSortPics.SetToolTip(this.radioButtonPreviewFile, "flytt/kopier, men vis bilder underveis.");
            this.radioButtonPreviewFile.UseVisualStyleBackColor = true;
            this.radioButtonPreviewFile.CheckedChanged += new System.EventHandler(this.radioButtonPreview_CheckedChanged);
            // 
            // radioButtonPreviewLog
            // 
            this.radioButtonPreviewLog.AutoSize = true;
            this.radioButtonPreviewLog.Location = new System.Drawing.Point(4, 38);
            this.radioButtonPreviewLog.Name = "radioButtonPreviewLog";
            this.radioButtonPreviewLog.Size = new System.Drawing.Size(80, 17);
            this.radioButtonPreviewLog.TabIndex = 1;
            this.radioButtonPreviewLog.Text = "Preview log";
            this.toolTipSortPics.SetToolTip(this.radioButtonPreviewLog, "Flytt/kopier bildene, og lag en logg over hva som er gjort (fra/til).");
            this.radioButtonPreviewLog.UseVisualStyleBackColor = true;
            this.radioButtonPreviewLog.CheckedChanged += new System.EventHandler(this.radioButtonPreview_CheckedChanged);
            // 
            // radioButtonNoPreview
            // 
            this.radioButtonNoPreview.AutoSize = true;
            this.radioButtonNoPreview.Checked = true;
            this.radioButtonNoPreview.Location = new System.Drawing.Point(4, 17);
            this.radioButtonNoPreview.Name = "radioButtonNoPreview";
            this.radioButtonNoPreview.Size = new System.Drawing.Size(92, 17);
            this.radioButtonNoPreview.TabIndex = 0;
            this.radioButtonNoPreview.TabStop = true;
            this.radioButtonNoPreview.Text = "Ingen preview";
            this.toolTipSortPics.SetToolTip(this.radioButtonNoPreview, "Raskest, bare kopier/flytt bildene");
            this.radioButtonNoPreview.UseVisualStyleBackColor = true;
            this.radioButtonNoPreview.CheckedChanged += new System.EventHandler(this.radioButtonPreview_CheckedChanged);
            // 
            // checkBoxMTP
            // 
            this.checkBoxMTP.AutoSize = true;
            this.checkBoxMTP.Location = new System.Drawing.Point(12, 20);
            this.checkBoxMTP.Name = "checkBoxMTP";
            this.checkBoxMTP.Size = new System.Drawing.Size(256, 17);
            this.checkBoxMTP.TabIndex = 5;
            this.checkBoxMTP.Text = "Kilden er MTP (mobiltelefon via usb eller SD kort)";
            this.toolTipSortPics.SetToolTip(this.checkBoxMTP, "Fungerer dårlig å hente bilder rett fra mobiltlf, men prøv å sjekke av for denne " +
        "om du vil");
            this.checkBoxMTP.UseVisualStyleBackColor = true;
            this.checkBoxMTP.CheckedChanged += new System.EventHandler(this.checkBoxWPD_CheckedChanged);
            // 
            // groupBoxDirs
            // 
            this.groupBoxDirs.Controls.Add(this.checkBoxEmptyDirs);
            this.groupBoxDirs.Controls.Add(this.checkBoxSubdirs);
            this.groupBoxDirs.Location = new System.Drawing.Point(6, 45);
            this.groupBoxDirs.Name = "groupBoxDirs";
            this.groupBoxDirs.Size = new System.Drawing.Size(181, 80);
            this.groupBoxDirs.TabIndex = 4;
            this.groupBoxDirs.TabStop = false;
            this.groupBoxDirs.Text = "Mappealternativer";
            // 
            // checkBoxEmptyDirs
            // 
            this.checkBoxEmptyDirs.AutoSize = true;
            this.checkBoxEmptyDirs.Checked = true;
            this.checkBoxEmptyDirs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEmptyDirs.Location = new System.Drawing.Point(6, 43);
            this.checkBoxEmptyDirs.Name = "checkBoxEmptyDirs";
            this.checkBoxEmptyDirs.Size = new System.Drawing.Size(148, 17);
            this.checkBoxEmptyDirs.TabIndex = 3;
            this.checkBoxEmptyDirs.Text = "Fjern tomme undermapper";
            this.toolTipSortPics.SetToolTip(this.checkBoxEmptyDirs, "Dersom en flytter, rydd opp tomme mapper etterpå.");
            this.checkBoxEmptyDirs.UseVisualStyleBackColor = true;
            // 
            // checkBoxSubdirs
            // 
            this.checkBoxSubdirs.AutoSize = true;
            this.checkBoxSubdirs.Checked = true;
            this.checkBoxSubdirs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSubdirs.Location = new System.Drawing.Point(6, 21);
            this.checkBoxSubdirs.Name = "checkBoxSubdirs";
            this.checkBoxSubdirs.Size = new System.Drawing.Size(111, 17);
            this.checkBoxSubdirs.TabIndex = 0;
            this.checkBoxSubdirs.Text = "Let i undermapper";
            this.toolTipSortPics.SetToolTip(this.checkBoxSubdirs, "Søk etter bilder i undermapper");
            this.checkBoxSubdirs.UseVisualStyleBackColor = true;
            // 
            // groupBoxDateTree
            // 
            this.groupBoxDateTree.Controls.Add(this.radioButtonByYearMonth);
            this.groupBoxDateTree.Controls.Add(this.radioButtonByYear);
            this.groupBoxDateTree.Controls.Add(this.radioButtonHerarchy);
            this.groupBoxDateTree.Controls.Add(this.radioButtonByDate);
            this.groupBoxDateTree.Location = new System.Drawing.Point(322, 45);
            this.groupBoxDateTree.Name = "groupBoxDateTree";
            this.groupBoxDateTree.Size = new System.Drawing.Size(162, 80);
            this.groupBoxDateTree.TabIndex = 2;
            this.groupBoxDateTree.TabStop = false;
            this.groupBoxDateTree.Text = "Destinasjon mappestruktur";
            // 
            // radioButtonByYearMonth
            // 
            this.radioButtonByYearMonth.AutoSize = true;
            this.radioButtonByYearMonth.Location = new System.Drawing.Point(10, 41);
            this.radioButtonByYearMonth.Name = "radioButtonByYearMonth";
            this.radioButtonByYearMonth.Size = new System.Drawing.Size(78, 17);
            this.radioButtonByYearMonth.TabIndex = 3;
            this.radioButtonByYearMonth.Text = "År / måned";
            this.toolTipSortPics.SetToolTip(this.radioButtonByYearMonth, "Legg bildene inn, samlet på årstall og måned de er tatt i , i destinasjonsmappen");
            this.radioButtonByYearMonth.UseVisualStyleBackColor = true;
            // 
            // radioButtonByYear
            // 
            this.radioButtonByYear.AutoSize = true;
            this.radioButtonByYear.Location = new System.Drawing.Point(10, 19);
            this.radioButtonByYear.Name = "radioButtonByYear";
            this.radioButtonByYear.Size = new System.Drawing.Size(74, 17);
            this.radioButtonByYear.TabIndex = 2;
            this.radioButtonByYear.Text = "Kun årstall";
            this.toolTipSortPics.SetToolTip(this.radioButtonByYear, "Legg bildene inn, samlet på årstall i destinasjonsmappen");
            this.radioButtonByYear.UseVisualStyleBackColor = true;
            // 
            // radioButtonHerarchy
            // 
            this.radioButtonHerarchy.AutoSize = true;
            this.radioButtonHerarchy.Checked = true;
            this.radioButtonHerarchy.Location = new System.Drawing.Point(89, 42);
            this.radioButtonHerarchy.Name = "radioButtonHerarchy";
            this.radioButtonHerarchy.Size = new System.Drawing.Size(63, 17);
            this.radioButtonHerarchy.TabIndex = 1;
            this.radioButtonHerarchy.TabStop = true;
            this.radioButtonHerarchy.Text = "Dato tre";
            this.toolTipSortPics.SetToolTip(this.radioButtonHerarchy, "Legg bildene inn, sortert på år, måned og dato i destinasjonsmappen.");
            this.radioButtonHerarchy.UseVisualStyleBackColor = true;
            // 
            // radioButtonByDate
            // 
            this.radioButtonByDate.AutoSize = true;
            this.radioButtonByDate.Location = new System.Drawing.Point(89, 21);
            this.radioButtonByDate.Name = "radioButtonByDate";
            this.radioButtonByDate.Size = new System.Drawing.Size(68, 17);
            this.radioButtonByDate.TabIndex = 0;
            this.radioButtonByDate.Text = "Kun dato";
            this.toolTipSortPics.SetToolTip(this.radioButtonByDate, "Legg bildene inn basert på dato de er tatt i destinasjonsmappen");
            this.radioButtonByDate.UseVisualStyleBackColor = true;
            // 
            // groupBoxCpyOrMove
            // 
            this.groupBoxCpyOrMove.Controls.Add(this.radioButtonFlytt);
            this.groupBoxCpyOrMove.Controls.Add(this.radioButtonKopier);
            this.groupBoxCpyOrMove.Location = new System.Drawing.Point(193, 45);
            this.groupBoxCpyOrMove.Name = "groupBoxCpyOrMove";
            this.groupBoxCpyOrMove.Size = new System.Drawing.Size(123, 80);
            this.groupBoxCpyOrMove.TabIndex = 1;
            this.groupBoxCpyOrMove.TabStop = false;
            this.groupBoxCpyOrMove.Text = "Kopier eller flytt filer";
            // 
            // radioButtonFlytt
            // 
            this.radioButtonFlytt.AutoSize = true;
            this.radioButtonFlytt.Checked = true;
            this.radioButtonFlytt.Location = new System.Drawing.Point(7, 43);
            this.radioButtonFlytt.Name = "radioButtonFlytt";
            this.radioButtonFlytt.Size = new System.Drawing.Size(44, 17);
            this.radioButtonFlytt.TabIndex = 1;
            this.radioButtonFlytt.TabStop = true;
            this.radioButtonFlytt.Text = "Flytt";
            this.toolTipSortPics.SetToolTip(this.radioButtonFlytt, "Flytt bildene til valgt destinasjonsmappe");
            this.radioButtonFlytt.UseVisualStyleBackColor = true;
            // 
            // radioButtonKopier
            // 
            this.radioButtonKopier.AutoSize = true;
            this.radioButtonKopier.Location = new System.Drawing.Point(7, 20);
            this.radioButtonKopier.Name = "radioButtonKopier";
            this.radioButtonKopier.Size = new System.Drawing.Size(55, 17);
            this.radioButtonKopier.TabIndex = 0;
            this.radioButtonKopier.Text = "Kopier";
            this.toolTipSortPics.SetToolTip(this.radioButtonKopier, "Kopier bildene til valgt destinasjonsmappe (bildene blir liggende igjen i kildema" +
        "ppen etter utført kopiering)");
            this.radioButtonKopier.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelFilnavn});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(756, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelFilnavn
            // 
            this.toolStripStatusLabelFilnavn.Name = "toolStripStatusLabelFilnavn";
            this.toolStripStatusLabelFilnavn.Size = new System.Drawing.Size(150, 17);
            this.toolStripStatusLabelFilnavn.Text = "toolStripStatusLabelFilnavn";
            // 
            // groupBoxPreview
            // 
            this.groupBoxPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxPreview.Location = new System.Drawing.Point(13, 204);
            this.groupBoxPreview.Name = "groupBoxPreview";
            this.groupBoxPreview.Size = new System.Drawing.Size(644, 212);
            this.groupBoxPreview.TabIndex = 7;
            this.groupBoxPreview.TabStop = false;
            this.groupBoxPreview.Text = "Preview";
            // 
            // buttonBreak
            // 
            this.buttonBreak.Enabled = false;
            this.buttonBreak.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBreak.Location = new System.Drawing.Point(664, 119);
            this.buttonBreak.Name = "buttonBreak";
            this.buttonBreak.Size = new System.Drawing.Size(75, 17);
            this.buttonBreak.TabIndex = 8;
            this.buttonBreak.Text = "Avbryt";
            this.buttonBreak.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonBreak.UseVisualStyleBackColor = true;
            this.buttonBreak.Click += new System.EventHandler(this.buttonBreak_Click);
            // 
            // buttonOpenDirS
            // 
            this.buttonOpenDirS.Location = new System.Drawing.Point(636, 9);
            this.buttonOpenDirS.Name = "buttonOpenDirS";
            this.buttonOpenDirS.Size = new System.Drawing.Size(25, 23);
            this.buttonOpenDirS.TabIndex = 9;
            this.buttonOpenDirS.Text = "Let opp";
            this.buttonOpenDirS.UseVisualStyleBackColor = true;
            this.buttonOpenDirS.Click += new System.EventHandler(this.buttonOpenDir_Click);
            // 
            // buttonOpenDirD
            // 
            this.buttonOpenDirD.Location = new System.Drawing.Point(636, 35);
            this.buttonOpenDirD.Name = "buttonOpenDirD";
            this.buttonOpenDirD.Size = new System.Drawing.Size(25, 23);
            this.buttonOpenDirD.TabIndex = 10;
            this.buttonOpenDirD.Text = "Let opp";
            this.buttonOpenDirD.UseVisualStyleBackColor = true;
            this.buttonOpenDirD.Click += new System.EventHandler(this.buttonOpenDir_Click);
            // 
            // SortPicsDesktopForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 450);
            this.Controls.Add(this.buttonOpenDirD);
            this.Controls.Add(this.buttonOpenDirS);
            this.Controls.Add(this.buttonBreak);
            this.Controls.Add(this.groupBoxPreview);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonDestination);
            this.Controls.Add(this.textBoxDestination);
            this.Controls.Add(this.buttonSource);
            this.Controls.Add(this.textBoxSource);
            this.Name = "SortPicsDesktopForm";
            this.Text = "SortPicsDesktopForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.groupBoxPWSetting.ResumeLayout(false);
            this.groupBoxPWSetting.PerformLayout();
            this.groupBoxDirs.ResumeLayout(false);
            this.groupBoxDirs.PerformLayout();
            this.groupBoxDateTree.ResumeLayout(false);
            this.groupBoxDateTree.PerformLayout();
            this.groupBoxCpyOrMove.ResumeLayout(false);
            this.groupBoxCpyOrMove.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}