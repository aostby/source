namespace Kolibri.Common.FormUtilities.Forms
{
    partial class ValidateXMLForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValidateXMLForm));
            this.buttonSendSOAP = new System.Windows.Forms.Button();
            this.richTextBox1 = new FastColoredTextBoxNS.FastColoredTextBox();
            this.buttonValidate = new System.Windows.Forms.Button();
            this.buttonBeautify = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.buttonMySchema = new System.Windows.Forms.Button();
            this.buttonMyStylesheet = new System.Windows.Forms.Button();
            this.buttonTransform = new System.Windows.Forms.Button();
            this.comboBoxSchema = new System.Windows.Forms.ComboBox();
            this.comboBoxStyleSheet = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileListStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.beautifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeEmptyAndIllegalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linarizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.linksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.samsvarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.seraptaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fHIRValidatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelXML = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.richTextBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSendSOAP
            // 
            this.buttonSendSOAP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSendSOAP.Location = new System.Drawing.Point(677, 511);
            this.buttonSendSOAP.Name = "buttonSendSOAP";
            this.buttonSendSOAP.Size = new System.Drawing.Size(151, 23);
            this.buttonSendSOAP.TabIndex = 0;
            this.buttonSendSOAP.Text = "Send Content as SOAP";
            this.buttonSendSOAP.UseVisualStyleBackColor = true;
            this.buttonSendSOAP.Click += new System.EventHandler(this.buttonSendSOAP_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
                  /* //TODO  this.richTextBox1.AutoCompleteBracketsList = new char[] {
            '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};*/
            //TODO     this.richTextBox1.AutoScrollMinSize = new System.Drawing.Size(27, 14);
            //TODO      this.richTextBox1.BackBrush = null;
            //TODO     this.richTextBox1.CharHeight = 14;
            //TODO     this.richTextBox1.CharWidth = 8;
            this.richTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
            //TODO      this.richTextBox1.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            //TODO      this.richTextBox1.IsReplaceMode = false;
            this.richTextBox1.Location = new System.Drawing.Point(12, 27);
            this.richTextBox1.Name = "richTextBox1";
            //TODO       this.richTextBox1.Paddings = new System.Windows.Forms.Padding(0);
            this.richTextBox1.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            //TODO        this.richTextBox1.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("richTextBox1.ServiceColors")));
            this.richTextBox1.Size = new System.Drawing.Size(831, 453);
            this.richTextBox1.TabIndex = 1;
            //TODO       this.richTextBox1.Zoom = 100;
            this.richTextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.richTextBox1_KeyDown);
            // 
            // buttonValidate
            // 
            this.buttonValidate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonValidate.Location = new System.Drawing.Point(570, 480);
            this.buttonValidate.Name = "buttonValidate";
            this.buttonValidate.Size = new System.Drawing.Size(98, 23);
            this.buttonValidate.TabIndex = 4;
            this.buttonValidate.Text = "Validate";
            this.toolTip1.SetToolTip(this.buttonValidate, "CTRL + SHIFT + ALT + V");
            this.buttonValidate.UseVisualStyleBackColor = true;
            this.buttonValidate.Click += new System.EventHandler(this.buttonValidate_Click);
            // 
            // buttonBeautify
            // 
            this.buttonBeautify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonBeautify.Location = new System.Drawing.Point(677, 482);
            this.buttonBeautify.Name = "buttonBeautify";
            this.buttonBeautify.Size = new System.Drawing.Size(151, 23);
            this.buttonBeautify.TabIndex = 6;
            this.buttonBeautify.Text = "Beautify";
            this.toolTip1.SetToolTip(this.buttonBeautify, "(CTRL + SHIFT + ALT + B)");
            this.buttonBeautify.UseVisualStyleBackColor = true;
            this.buttonBeautify.Click += new System.EventHandler(this.buttonBeautify_Click);
            // 
            // buttonMySchema
            // 
            this.buttonMySchema.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMySchema.Location = new System.Drawing.Point(28, 480);
            this.buttonMySchema.Name = "buttonMySchema";
            this.buttonMySchema.Size = new System.Drawing.Size(97, 23);
            this.buttonMySchema.TabIndex = 12;
            this.buttonMySchema.Text = "My Schema >";
            this.buttonMySchema.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip1.SetToolTip(this.buttonMySchema, "My Schema\r\nLast inn ditt eget skjema\r\n");
            this.buttonMySchema.UseVisualStyleBackColor = true;
            this.buttonMySchema.Click += new System.EventHandler(this.buttonMySchema_Click);
            // 
            // buttonMyStylesheet
            // 
            this.buttonMyStylesheet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMyStylesheet.Location = new System.Drawing.Point(27, 510);
            this.buttonMyStylesheet.Name = "buttonMyStylesheet";
            this.buttonMyStylesheet.Size = new System.Drawing.Size(98, 23);
            this.buttonMyStylesheet.TabIndex = 13;
            this.buttonMyStylesheet.Text = "My Stylesheet >";
            this.buttonMyStylesheet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip1.SetToolTip(this.buttonMyStylesheet, "My Stylesheet\r\nLast inn ditt eget stilark\r\n");
            this.buttonMyStylesheet.UseVisualStyleBackColor = true;
            this.buttonMyStylesheet.Click += new System.EventHandler(this.buttonMyStylesheet_Click);
            // 
            // buttonTransform
            // 
            this.buttonTransform.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTransform.Location = new System.Drawing.Point(570, 510);
            this.buttonTransform.Name = "buttonTransform";
            this.buttonTransform.Size = new System.Drawing.Size(98, 23);
            this.buttonTransform.TabIndex = 9;
            this.buttonTransform.Text = "Transform";
            this.toolTip1.SetToolTip(this.buttonTransform, "CTRL + SHIFT + ALT + T");
            this.buttonTransform.UseVisualStyleBackColor = true;
            this.buttonTransform.Click += new System.EventHandler(this.buttonTransform_Click);
            // 
            // comboBoxSchema
            // 
            this.comboBoxSchema.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxSchema.FormattingEnabled = true;
            this.comboBoxSchema.Location = new System.Drawing.Point(133, 482);
            this.comboBoxSchema.Name = "comboBoxSchema";
            this.comboBoxSchema.Size = new System.Drawing.Size(429, 21);
            this.comboBoxSchema.TabIndex = 8;
            this.comboBoxSchema.SelectedIndexChanged += new System.EventHandler(this.comboBoxSchema_SelectedIndexChanged);
            // 
            // comboBoxStyleSheet
            // 
            this.comboBoxStyleSheet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxStyleSheet.FormattingEnabled = true;
            this.comboBoxStyleSheet.Location = new System.Drawing.Point(133, 511);
            this.comboBoxStyleSheet.Name = "comboBoxStyleSheet";
            this.comboBoxStyleSheet.Size = new System.Drawing.Size(429, 21);
            this.comboBoxStyleSheet.TabIndex = 10;
            this.comboBoxStyleSheet.SelectedIndexChanged += new System.EventHandler(this.comboBoxStyleSheet_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.applicationToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(855, 24);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.fileListStripMenuItem,
            this.toolStripSeparator1,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.ToolTipText = "CTRL + O";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // fileListStripMenuItem
            // 
            this.fileListStripMenuItem.Name = "fileListStripMenuItem";
            this.fileListStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.fileListStripMenuItem.Text = "Filelist";
            this.fileListStripMenuItem.ToolTipText = "CTRL + SHIFT + ALT + L";
            this.fileListStripMenuItem.Click += new System.EventHandler(this.buttonFilelist_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(104, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.ToolTipText = "(CTRL + S)";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.beautifyToolStripMenuItem,
            this.removeEmptyAndIllegalToolStripMenuItem,
            this.linarizeToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // beautifyToolStripMenuItem
            // 
            this.beautifyToolStripMenuItem.Name = "beautifyToolStripMenuItem";
            this.beautifyToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.beautifyToolStripMenuItem.Text = "Beautify";
            this.beautifyToolStripMenuItem.ToolTipText = "(CTRL + SHIFT + ALT + B)";
            this.beautifyToolStripMenuItem.Click += new System.EventHandler(this.buttonBeautify_Click);
            // 
            // removeEmptyAndIllegalToolStripMenuItem
            // 
            this.removeEmptyAndIllegalToolStripMenuItem.Name = "removeEmptyAndIllegalToolStripMenuItem";
            this.removeEmptyAndIllegalToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.removeEmptyAndIllegalToolStripMenuItem.Text = "Remove Empty and Illegal";
            this.removeEmptyAndIllegalToolStripMenuItem.ToolTipText = "Removes all empty elements. \r\nCTRL + SHIFT + ALT + I\r\nBeautifies.\r\nRemoves elemet" +
    "s with the following values:\r\n\"0.0\"\r\n\"0000-00-00\"\r\n\"000000\"";
            this.removeEmptyAndIllegalToolStripMenuItem.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // linarizeToolStripMenuItem
            // 
            this.linarizeToolStripMenuItem.Name = "linarizeToolStripMenuItem";
            this.linarizeToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.linarizeToolStripMenuItem.Text = "Linarize";
            this.linarizeToolStripMenuItem.ToolTipText = "(CTRL + SHIFT + ALT + L)";
            this.linarizeToolStripMenuItem.Click += new System.EventHandler(this.linarizeToolStripMenuItem_Click);
            // 
            // applicationToolStripMenuItem
            // 
            this.applicationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.linksToolStripMenuItem});
            this.applicationToolStripMenuItem.Name = "applicationToolStripMenuItem";
            this.applicationToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.applicationToolStripMenuItem.Text = "Application";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(107, 22);
            this.toolStripMenuItem2.Tag = "\\\\helsemn.no\\system$\\Fagomraader\\Biztalk\\ESB-POD\\Hemit_Leveranser2020\\Tools\\XMLVa" +
    "lidator\\XMLValidator_HowTo.txt";
            this.toolStripMenuItem2.Text = "About";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.ApplicationToolStripMenuItem_Click);
            // 
            // linksToolStripMenuItem
            // 
            this.linksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.samsvarToolStripMenuItem,
            this.seraptaToolStripMenuItem,
            this.fHIRValidatorToolStripMenuItem});
            this.linksToolStripMenuItem.Name = "linksToolStripMenuItem";
            this.linksToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.linksToolStripMenuItem.Text = "Links";
            // 
            // samsvarToolStripMenuItem
            // 
            this.samsvarToolStripMenuItem.Name = "samsvarToolStripMenuItem";
            this.samsvarToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.samsvarToolStripMenuItem.Tag = "https://samsvar.nhn.no/validering/xml";
            this.samsvarToolStripMenuItem.Text = "Samsvar";
            this.samsvarToolStripMenuItem.Click += new System.EventHandler(this.ApplicationToolStripMenuItem_Click);
            // 
            // seraptaToolStripMenuItem
            // 
            this.seraptaToolStripMenuItem.Name = "seraptaToolStripMenuItem";
            this.seraptaToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.seraptaToolStripMenuItem.Tag = "https://git.sarepta.ehelse.no/publisert/standarder";
            this.seraptaToolStripMenuItem.Text = "Serapta";
            this.seraptaToolStripMenuItem.Click += new System.EventHandler(this.ApplicationToolStripMenuItem_Click);
            // 
            // fHIRValidatorToolStripMenuItem
            // 
            this.fHIRValidatorToolStripMenuItem.Name = "fHIRValidatorToolStripMenuItem";
            this.fHIRValidatorToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.fHIRValidatorToolStripMenuItem.Tag = "https://inferno.healthit.gov/validator/";
            this.fHIRValidatorToolStripMenuItem.Text = "FHIR validator";
            this.fHIRValidatorToolStripMenuItem.Click += new System.EventHandler(this.ApplicationToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelXML});
            this.statusStrip1.Location = new System.Drawing.Point(0, 544);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(855, 22);
            this.statusStrip1.TabIndex = 16;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelXML
            // 
            this.toolStripStatusLabelXML.Name = "toolStripStatusLabelXML";
            this.toolStripStatusLabelXML.Size = new System.Drawing.Size(31, 17);
            this.toolStripStatusLabelXML.Text = "XML";
            // 
            // RBDXMLForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 566);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.buttonMyStylesheet);
            this.Controls.Add(this.buttonMySchema);
            this.Controls.Add(this.comboBoxStyleSheet);
            this.Controls.Add(this.buttonTransform);
            this.Controls.Add(this.comboBoxSchema);
            this.Controls.Add(this.buttonBeautify);
            this.Controls.Add(this.buttonValidate);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.buttonSendSOAP);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RBDXMLForm";
            this.Text = "XML validator and transform";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RBDXMLForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.richTextBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSendSOAP;
        private  FastColoredTextBoxNS.FastColoredTextBox  richTextBox1;
        private System.Windows.Forms.Button buttonValidate;
        private System.Windows.Forms.Button buttonBeautify;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox comboBoxSchema;
        private System.Windows.Forms.Button buttonTransform;
        private System.Windows.Forms.ComboBox comboBoxStyleSheet;
        private System.Windows.Forms.Button buttonMySchema;
        private System.Windows.Forms.Button buttonMyStylesheet;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileListStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem applicationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem linksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem samsvarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem seraptaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fHIRValidatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem beautifyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeEmptyAndIllegalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem linarizeToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelXML;
    }
}

