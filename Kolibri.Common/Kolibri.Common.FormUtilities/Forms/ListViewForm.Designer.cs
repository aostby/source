
namespace Kolibri.Common.FormUtilities.Forms
{
    public partial class ListViewForm
    {
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblCurrentPath = new System.Windows.Forms.Label();
            this.ilLarge = new System.Windows.Forms.ImageList(this.components);
            this.ilSmall = new System.Windows.Forms.ImageList(this.components);
            this.lwFilesAndFolders = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lblCurrentPath
            // 
            this.lblCurrentPath.Location = new System.Drawing.Point(16, 8);
            this.lblCurrentPath.Name = "lblCurrentPath";
            this.lblCurrentPath.Size = new System.Drawing.Size(528, 16);
            this.lblCurrentPath.TabIndex = 3;
            // 
            // ilLarge
            // 
            this.ilLarge.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ilLarge.ImageSize = new System.Drawing.Size(32, 32);
            this.ilLarge.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ilSmall
            // 
            this.ilSmall.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ilSmall.ImageSize = new System.Drawing.Size(16, 16);
            this.ilSmall.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // lwFilesAndFolders
            // 
            this.lwFilesAndFolders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lwFilesAndFolders.LargeImageList = this.ilLarge;
            this.lwFilesAndFolders.Location = new System.Drawing.Point(16, 32);
            this.lwFilesAndFolders.MultiSelect = false;
            this.lwFilesAndFolders.Name = "lwFilesAndFolders";
            this.lwFilesAndFolders.Size = new System.Drawing.Size(400, 249);
            this.lwFilesAndFolders.SmallImageList = this.ilSmall;
            this.lwFilesAndFolders.TabIndex = 0;
            this.lwFilesAndFolders.UseCompatibleStateImageBehavior = false;
            this.lwFilesAndFolders.View = System.Windows.Forms.View.List;
            this.lwFilesAndFolders.SelectedIndexChanged += new System.EventHandler(this.lwFilesAndFolders_ItemActivate);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(431, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Enabled = false;
            this.richTextBox1.Location = new System.Drawing.Point(433, 56);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(107, 225);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            // 
            // ListViewForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(552, 293);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCurrentPath);
            this.Controls.Add(this.lwFilesAndFolders);
            this.Name = "ListViewForm";
            this.Text = "ListView";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBox1;

    }
}