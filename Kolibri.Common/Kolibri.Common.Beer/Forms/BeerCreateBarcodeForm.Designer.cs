namespace Kolibri.Common.Beer.Forms
{
    partial class BeerCreateBarcodeForm
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
            this.comboBoxName = new System.Windows.Forms.ComboBox();
            this.pictureBoxCode = new System.Windows.Forms.PictureBox();
            this.labelName = new System.Windows.Forms.Label();
            this.groupBoxAppearance = new System.Windows.Forms.GroupBox();
            this.labelTrim = new System.Windows.Forms.Label();
            this.linkLabelBackColor = new System.Windows.Forms.LinkLabel();
            this.linkLabelForeColor = new System.Windows.Forms.LinkLabel();
            this.radioButtonLogo = new System.Windows.Forms.RadioButton();
            this.radioButtonStyled = new System.Windows.Forms.RadioButton();
            this.radioButtonPlain = new System.Windows.Forms.RadioButton();
            this.groupBoxFile = new System.Windows.Forms.GroupBox();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.comboBoxFile = new System.Windows.Forms.ComboBox();
            this.comboBoxDirectory = new System.Windows.Forms.ComboBox();
            this.buttonSourcePath = new System.Windows.Forms.Button();
            this.linkLabelUrl = new System.Windows.Forms.LinkLabel();
            this.groupBoxAction = new System.Windows.Forms.GroupBox();
            this.buttonUploadPDF = new System.Windows.Forms.Button();
            this.buttonUploadJPG = new System.Windows.Forms.Button();
            this.buttonPrint = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.linkLabelCodeExample = new System.Windows.Forms.LinkLabel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBoxTypeCode = new System.Windows.Forms.GroupBox();
            this.checkBoxVertical = new System.Windows.Forms.CheckBox();
            this.radioButtonPDF417 = new System.Windows.Forms.RadioButton();
            this.radioButtonQR = new System.Windows.Forms.RadioButton();
            this.labelDate = new System.Windows.Forms.Label();
            this.buttonOpenXMLPath = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBoxHeader = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCode)).BeginInit();
            this.groupBoxAppearance.SuspendLayout();
            this.groupBoxFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.groupBoxAction.SuspendLayout();
            this.groupBoxTypeCode.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxName
            // 
            this.comboBoxName.FormattingEnabled = true;
            this.comboBoxName.Location = new System.Drawing.Point(118, 9);
            this.comboBoxName.Name = "comboBoxName";
            this.comboBoxName.Size = new System.Drawing.Size(200, 21);
            this.comboBoxName.TabIndex = 0;
            this.comboBoxName.SelectedIndexChanged += new System.EventHandler(this.comboBoxName_SelectedIndexChanged);
            // 
            // pictureBoxCode
            // 
            this.pictureBoxCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxCode.Location = new System.Drawing.Point(331, 35);
            this.pictureBoxCode.Name = "pictureBoxCode";
            this.pictureBoxCode.Size = new System.Drawing.Size(690, 544);
            this.pictureBoxCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCode.TabIndex = 1;
            this.pictureBoxCode.TabStop = false;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(17, 12);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(95, 13);
            this.labelName.TabIndex = 2;
            this.labelName.Text = "Brew name and url";
            // 
            // groupBoxAppearance
            // 
            this.groupBoxAppearance.Controls.Add(this.labelTrim);
            this.groupBoxAppearance.Controls.Add(this.linkLabelBackColor);
            this.groupBoxAppearance.Controls.Add(this.linkLabelForeColor);
            this.groupBoxAppearance.Controls.Add(this.radioButtonLogo);
            this.groupBoxAppearance.Controls.Add(this.radioButtonStyled);
            this.groupBoxAppearance.Controls.Add(this.radioButtonPlain);
            this.groupBoxAppearance.Location = new System.Drawing.Point(135, 415);
            this.groupBoxAppearance.Name = "groupBoxAppearance";
            this.groupBoxAppearance.Size = new System.Drawing.Size(174, 86);
            this.groupBoxAppearance.TabIndex = 4;
            this.groupBoxAppearance.TabStop = false;
            this.groupBoxAppearance.Text = "Apperance";
            // 
            // labelTrim
            // 
            this.labelTrim.Location = new System.Drawing.Point(85, 50);
            this.labelTrim.Name = "labelTrim";
            this.labelTrim.Size = new System.Drawing.Size(88, 34);
            this.labelTrim.TabIndex = 25;
            this.labelTrim.Text = "Hvit bakgrunn trimmer barkode bildet";
            // 
            // linkLabelBackColor
            // 
            this.linkLabelBackColor.AutoSize = true;
            this.linkLabelBackColor.Location = new System.Drawing.Point(89, 29);
            this.linkLabelBackColor.Name = "linkLabelBackColor";
            this.linkLabelBackColor.Size = new System.Drawing.Size(54, 13);
            this.linkLabelBackColor.TabIndex = 24;
            this.linkLabelBackColor.TabStop = true;
            this.linkLabelBackColor.Text = "backcolor";
            this.linkLabelBackColor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelChooseColor_LinkClicked);
            // 
            // linkLabelForeColor
            // 
            this.linkLabelForeColor.AutoSize = true;
            this.linkLabelForeColor.Location = new System.Drawing.Point(89, 16);
            this.linkLabelForeColor.Name = "linkLabelForeColor";
            this.linkLabelForeColor.Size = new System.Drawing.Size(48, 13);
            this.linkLabelForeColor.TabIndex = 23;
            this.linkLabelForeColor.TabStop = true;
            this.linkLabelForeColor.Text = "forecolor";
            this.linkLabelForeColor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelChooseColor_LinkClicked);
            // 
            // radioButtonLogo
            // 
            this.radioButtonLogo.AutoSize = true;
            this.radioButtonLogo.Checked = true;
            this.radioButtonLogo.Location = new System.Drawing.Point(19, 66);
            this.radioButtonLogo.Name = "radioButtonLogo";
            this.radioButtonLogo.Size = new System.Drawing.Size(49, 17);
            this.radioButtonLogo.TabIndex = 2;
            this.radioButtonLogo.TabStop = true;
            this.radioButtonLogo.Text = "Logo";
            this.radioButtonLogo.UseVisualStyleBackColor = true;
            this.radioButtonLogo.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonStyled
            // 
            this.radioButtonStyled.AutoSize = true;
            this.radioButtonStyled.Location = new System.Drawing.Point(19, 42);
            this.radioButtonStyled.Name = "radioButtonStyled";
            this.radioButtonStyled.Size = new System.Drawing.Size(54, 17);
            this.radioButtonStyled.TabIndex = 1;
            this.radioButtonStyled.Text = "Styled";
            this.radioButtonStyled.UseVisualStyleBackColor = true;
            this.radioButtonStyled.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonPlain
            // 
            this.radioButtonPlain.AutoSize = true;
            this.radioButtonPlain.Location = new System.Drawing.Point(19, 19);
            this.radioButtonPlain.Name = "radioButtonPlain";
            this.radioButtonPlain.Size = new System.Drawing.Size(48, 17);
            this.radioButtonPlain.TabIndex = 0;
            this.radioButtonPlain.Text = "Plain";
            this.radioButtonPlain.UseVisualStyleBackColor = true;
            this.radioButtonPlain.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // groupBoxFile
            // 
            this.groupBoxFile.Controls.Add(this.checkBoxHeader);
            this.groupBoxFile.Controls.Add(this.pictureBoxPreview);
            this.groupBoxFile.Controls.Add(this.comboBoxFile);
            this.groupBoxFile.Controls.Add(this.comboBoxDirectory);
            this.groupBoxFile.Controls.Add(this.buttonSourcePath);
            this.groupBoxFile.Location = new System.Drawing.Point(12, 155);
            this.groupBoxFile.Name = "groupBoxFile";
            this.groupBoxFile.Size = new System.Drawing.Size(306, 242);
            this.groupBoxFile.TabIndex = 5;
            this.groupBoxFile.TabStop = false;
            this.groupBoxFile.Text = "Source folder";
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Location = new System.Drawing.Point(6, 101);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(291, 135);
            this.pictureBoxPreview.TabIndex = 23;
            this.pictureBoxPreview.TabStop = false;
            // 
            // comboBoxFile
            // 
            this.comboBoxFile.FormattingEnabled = true;
            this.comboBoxFile.Location = new System.Drawing.Point(7, 74);
            this.comboBoxFile.Name = "comboBoxFile";
            this.comboBoxFile.Size = new System.Drawing.Size(292, 21);
            this.comboBoxFile.TabIndex = 22;
            this.comboBoxFile.SelectedIndexChanged += new System.EventHandler(this.comboBoxFile_SelectedIndexChanged);
            // 
            // comboBoxDirectory
            // 
            this.comboBoxDirectory.FormattingEnabled = true;
            this.comboBoxDirectory.Location = new System.Drawing.Point(8, 47);
            this.comboBoxDirectory.Name = "comboBoxDirectory";
            this.comboBoxDirectory.Size = new System.Drawing.Size(292, 21);
            this.comboBoxDirectory.TabIndex = 21;
            this.comboBoxDirectory.SelectedIndexChanged += new System.EventHandler(this.comboBoxDirectory_SelectedIndexChanged);
            // 
            // buttonSourcePath
            // 
            this.buttonSourcePath.Location = new System.Drawing.Point(6, 18);
            this.buttonSourcePath.Name = "buttonSourcePath";
            this.buttonSourcePath.Size = new System.Drawing.Size(291, 23);
            this.buttonSourcePath.TabIndex = 20;
            this.buttonSourcePath.Text = "Images Source path";
            this.buttonSourcePath.UseVisualStyleBackColor = true;
            this.buttonSourcePath.Click += new System.EventHandler(this.buttonSourcePath_Click);
            // 
            // linkLabelUrl
            // 
            this.linkLabelUrl.AutoSize = true;
            this.linkLabelUrl.Location = new System.Drawing.Point(324, 9);
            this.linkLabelUrl.Name = "linkLabelUrl";
            this.linkLabelUrl.Size = new System.Drawing.Size(55, 13);
            this.linkLabelUrl.TabIndex = 22;
            this.linkLabelUrl.TabStop = true;
            this.linkLabelUrl.Text = "linkLabel2";
            this.linkLabelUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelOpenTagUrl_LinkClicked);
            // 
            // groupBoxAction
            // 
            this.groupBoxAction.Controls.Add(this.buttonUploadPDF);
            this.groupBoxAction.Controls.Add(this.buttonUploadJPG);
            this.groupBoxAction.Controls.Add(this.buttonPrint);
            this.groupBoxAction.Controls.Add(this.buttonSave);
            this.groupBoxAction.Location = new System.Drawing.Point(21, 507);
            this.groupBoxAction.Name = "groupBoxAction";
            this.groupBoxAction.Size = new System.Drawing.Size(288, 83);
            this.groupBoxAction.TabIndex = 23;
            this.groupBoxAction.TabStop = false;
            this.groupBoxAction.Text = "Action";
            // 
            // buttonUploadPDF
            // 
            this.buttonUploadPDF.Location = new System.Drawing.Point(133, 48);
            this.buttonUploadPDF.Name = "buttonUploadPDF";
            this.buttonUploadPDF.Size = new System.Drawing.Size(121, 23);
            this.buttonUploadPDF.TabIndex = 12;
            this.buttonUploadPDF.Text = "Upload PDF";
            this.buttonUploadPDF.UseVisualStyleBackColor = true;
            this.buttonUploadPDF.Click += new System.EventHandler(this.buttonUploadPDF_Click);
            // 
            // buttonUploadJPG
            // 
            this.buttonUploadJPG.Location = new System.Drawing.Point(133, 19);
            this.buttonUploadJPG.Name = "buttonUploadJPG";
            this.buttonUploadJPG.Size = new System.Drawing.Size(121, 23);
            this.buttonUploadJPG.TabIndex = 11;
            this.buttonUploadJPG.Text = "Upload JPG";
            this.buttonUploadJPG.UseVisualStyleBackColor = true;
            this.buttonUploadJPG.Click += new System.EventHandler(this.buttonUpload_Click);
            // 
            // buttonPrint
            // 
            this.buttonPrint.Location = new System.Drawing.Point(6, 48);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(121, 23);
            this.buttonPrint.TabIndex = 10;
            this.buttonPrint.Text = "Print";
            this.buttonPrint.UseVisualStyleBackColor = true;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(6, 19);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(121, 23);
            this.buttonSave.TabIndex = 9;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // linkLabelCodeExample
            // 
            this.linkLabelCodeExample.AutoSize = true;
            this.linkLabelCodeExample.Location = new System.Drawing.Point(18, 394);
            this.linkLabelCodeExample.Name = "linkLabelCodeExample";
            this.linkLabelCodeExample.Size = new System.Drawing.Size(80, 13);
            this.linkLabelCodeExample.TabIndex = 8;
            this.linkLabelCodeExample.TabStop = true;
            this.linkLabelCodeExample.Tag = "https://ironsoftware.com/csharp/barcode/examples/barcode-styling-and-annotation/";
            this.linkLabelCodeExample.Text = "Code Examples";
            this.linkLabelCodeExample.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelOpenTagUrl_LinkClicked);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBox1.Location = new System.Drawing.Point(12, 56);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(306, 93);
            this.richTextBox1.TabIndex = 24;
            this.richTextBox1.Text = "";
            this.richTextBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.richTextBox1_MouseDoubleClick);
            // 
            // groupBoxTypeCode
            // 
            this.groupBoxTypeCode.Controls.Add(this.checkBoxVertical);
            this.groupBoxTypeCode.Controls.Add(this.radioButtonPDF417);
            this.groupBoxTypeCode.Controls.Add(this.radioButtonQR);
            this.groupBoxTypeCode.Location = new System.Drawing.Point(20, 415);
            this.groupBoxTypeCode.Name = "groupBoxTypeCode";
            this.groupBoxTypeCode.Size = new System.Drawing.Size(109, 86);
            this.groupBoxTypeCode.TabIndex = 25;
            this.groupBoxTypeCode.TabStop = false;
            this.groupBoxTypeCode.Text = "Type barcode";
            // 
            // checkBoxVertical
            // 
            this.checkBoxVertical.AutoSize = true;
            this.checkBoxVertical.Checked = true;
            this.checkBoxVertical.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxVertical.Location = new System.Drawing.Point(35, 63);
            this.checkBoxVertical.Name = "checkBoxVertical";
            this.checkBoxVertical.Size = new System.Drawing.Size(61, 17);
            this.checkBoxVertical.TabIndex = 2;
            this.checkBoxVertical.Text = "Vertical";
            this.checkBoxVertical.UseVisualStyleBackColor = true;
            // 
            // radioButtonPDF417
            // 
            this.radioButtonPDF417.AutoSize = true;
            this.radioButtonPDF417.Checked = true;
            this.radioButtonPDF417.Location = new System.Drawing.Point(20, 42);
            this.radioButtonPDF417.Name = "radioButtonPDF417";
            this.radioButtonPDF417.Size = new System.Drawing.Size(64, 17);
            this.radioButtonPDF417.TabIndex = 1;
            this.radioButtonPDF417.TabStop = true;
            this.radioButtonPDF417.Text = "PDF417";
            this.radioButtonPDF417.UseVisualStyleBackColor = true;
            this.radioButtonPDF417.CheckedChanged += new System.EventHandler(this.radioButtonTypeCode_CheckedChanged);
            // 
            // radioButtonQR
            // 
            this.radioButtonQR.AutoSize = true;
            this.radioButtonQR.Location = new System.Drawing.Point(20, 19);
            this.radioButtonQR.Name = "radioButtonQR";
            this.radioButtonQR.Size = new System.Drawing.Size(41, 17);
            this.radioButtonQR.TabIndex = 0;
            this.radioButtonQR.Text = "QR";
            this.radioButtonQR.UseVisualStyleBackColor = true;
            this.radioButtonQR.CheckedChanged += new System.EventHandler(this.radioButtonTypeCode_CheckedChanged);
            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.Location = new System.Drawing.Point(12, 33);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(35, 13);
            this.labelDate.TabIndex = 26;
            this.labelDate.Text = "label1";
            // 
            // buttonOpenXMLPath
            // 
            this.buttonOpenXMLPath.Location = new System.Drawing.Point(154, 33);
            this.buttonOpenXMLPath.Name = "buttonOpenXMLPath";
            this.buttonOpenXMLPath.Size = new System.Drawing.Size(93, 22);
            this.buttonOpenXMLPath.TabIndex = 27;
            this.buttonOpenXMLPath.Text = "Open Xmlpath";
            this.buttonOpenXMLPath.UseVisualStyleBackColor = true;
            this.buttonOpenXMLPath.Click += new System.EventHandler(this.buttonOpenXMLPath_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(253, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(65, 22);
            this.button1.TabIndex = 28;
            this.button1.Text = "Get latest";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonGetLatest_Click);
            // 
            // checkBoxHeader
            // 
            this.checkBoxHeader.AutoSize = true;
            this.checkBoxHeader.Location = new System.Drawing.Point(211, 218);
            this.checkBoxHeader.Name = "checkBoxHeader";
            this.checkBoxHeader.Size = new System.Drawing.Size(88, 17);
            this.checkBoxHeader.TabIndex = 24;
            this.checkBoxHeader.Text = "Sett inn tekst";
            this.checkBoxHeader.UseVisualStyleBackColor = true;
            // 
            // BeerCreateBarcodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1033, 591);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonOpenXMLPath);
            this.Controls.Add(this.labelDate);
            this.Controls.Add(this.groupBoxTypeCode);
            this.Controls.Add(this.linkLabelCodeExample);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.groupBoxAction);
            this.Controls.Add(this.linkLabelUrl);
            this.Controls.Add(this.groupBoxFile);
            this.Controls.Add(this.groupBoxAppearance);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.pictureBoxCode);
            this.Controls.Add(this.comboBoxName);
            this.Name = "BeerCreateBarcodeForm";
            this.Text = "BeerCreateBarcodeForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCode)).EndInit();
            this.groupBoxAppearance.ResumeLayout(false);
            this.groupBoxAppearance.PerformLayout();
            this.groupBoxFile.ResumeLayout(false);
            this.groupBoxFile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.groupBoxAction.ResumeLayout(false);
            this.groupBoxTypeCode.ResumeLayout(false);
            this.groupBoxTypeCode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxName;
        private System.Windows.Forms.PictureBox pictureBoxCode;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.GroupBox groupBoxAppearance;
        private System.Windows.Forms.RadioButton radioButtonStyled;
        private System.Windows.Forms.RadioButton radioButtonPlain;
        private System.Windows.Forms.RadioButton radioButtonLogo;
        private System.Windows.Forms.GroupBox groupBoxFile;
        private System.Windows.Forms.Button buttonSourcePath;
        private System.Windows.Forms.ComboBox comboBoxDirectory;
        private System.Windows.Forms.ComboBox comboBoxFile;
        private System.Windows.Forms.LinkLabel linkLabelUrl;
        private System.Windows.Forms.GroupBox groupBoxAction;
        private System.Windows.Forms.Button buttonPrint;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.LinkLabel linkLabelCodeExample;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.GroupBox groupBoxTypeCode;
        private System.Windows.Forms.RadioButton radioButtonPDF417;
        private System.Windows.Forms.RadioButton radioButtonQR;
        private System.Windows.Forms.CheckBox checkBoxVertical;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Button buttonOpenXMLPath;
        private System.Windows.Forms.LinkLabel linkLabelBackColor;
        private System.Windows.Forms.LinkLabel linkLabelForeColor;
        private System.Windows.Forms.Label labelTrim;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonUploadJPG;
        private System.Windows.Forms.Button buttonUploadPDF;
        private System.Windows.Forms.CheckBox checkBoxHeader;
    }
}