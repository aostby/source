namespace Kolibri.net.SilverScreen.Forms
{
    partial class TreeViewItemsForm
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
            treeView1 = new TreeView();
            groupBoxOrder = new GroupBox();
            checkBoxToolTip = new CheckBox();
            radioButtonActor = new RadioButton();
            radioButtonRating = new RadioButton();
            radioButtonYear = new RadioButton();
            radioButtonGenre = new RadioButton();
            radioButtonTitle = new RadioButton();
            imageListIcons = new ImageList(components);
            groupBoxOrder.SuspendLayout();
            SuspendLayout();
            // 
            // treeView1
            // 
            treeView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treeView1.Location = new Point(12, 50);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(776, 388);
            treeView1.TabIndex = 0;
            treeView1.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;
            treeView1.MouseDown += treeView1_MouseDown;
            // 
            // groupBoxOrder
            // 
            groupBoxOrder.Controls.Add(checkBoxToolTip);
            groupBoxOrder.Controls.Add(radioButtonActor);
            groupBoxOrder.Controls.Add(radioButtonRating);
            groupBoxOrder.Controls.Add(radioButtonYear);
            groupBoxOrder.Controls.Add(radioButtonGenre);
            groupBoxOrder.Controls.Add(radioButtonTitle);
            groupBoxOrder.Location = new Point(12, 3);
            groupBoxOrder.Name = "groupBoxOrder";
            groupBoxOrder.Size = new Size(447, 41);
            groupBoxOrder.TabIndex = 1;
            groupBoxOrder.TabStop = false;
            groupBoxOrder.Text = "Order by";
            // 
            // checkBoxToolTip
            // 
            checkBoxToolTip.AutoSize = true;
            checkBoxToolTip.Location = new Point(353, 17);
            checkBoxToolTip.Name = "checkBoxToolTip";
            checkBoxToolTip.Size = new Size(70, 19);
            checkBoxToolTip.TabIndex = 5;
            checkBoxToolTip.Text = "Vis bilde";
            checkBoxToolTip.UseVisualStyleBackColor = true;
            // 
            // radioButtonActor
            // 
            radioButtonActor.AutoSize = true;
            radioButtonActor.Location = new Point(289, 16);
            radioButtonActor.Name = "radioButtonActor";
            radioButtonActor.Size = new Size(54, 19);
            radioButtonActor.TabIndex = 4;
            radioButtonActor.Text = "Actor";
            radioButtonActor.UseVisualStyleBackColor = true;
            radioButtonActor.CheckedChanged += Radio_CheckedChanged;
            // 
            // radioButtonRating
            // 
            radioButtonRating.AutoSize = true;
            radioButtonRating.Location = new Point(196, 16);
            radioButtonRating.Name = "radioButtonRating";
            radioButtonRating.Size = new Size(87, 19);
            radioButtonRating.TabIndex = 3;
            radioButtonRating.Text = "ImdbRating";
            radioButtonRating.UseVisualStyleBackColor = true;
            radioButtonRating.CheckedChanged += Radio_CheckedChanged;
            // 
            // radioButtonYear
            // 
            radioButtonYear.AutoSize = true;
            radioButtonYear.Location = new Point(136, 16);
            radioButtonYear.Name = "radioButtonYear";
            radioButtonYear.Size = new Size(47, 19);
            radioButtonYear.TabIndex = 2;
            radioButtonYear.Text = "Year";
            radioButtonYear.UseVisualStyleBackColor = true;
            radioButtonYear.CheckedChanged += Radio_CheckedChanged;
            // 
            // radioButtonGenre
            // 
            radioButtonGenre.AutoSize = true;
            radioButtonGenre.Location = new Point(67, 16);
            radioButtonGenre.Name = "radioButtonGenre";
            radioButtonGenre.Size = new Size(56, 19);
            radioButtonGenre.TabIndex = 1;
            radioButtonGenre.Text = "Genre";
            radioButtonGenre.UseVisualStyleBackColor = true;
            radioButtonGenre.CheckedChanged += Radio_CheckedChanged;
            // 
            // radioButtonTitle
            // 
            radioButtonTitle.AutoSize = true;
            radioButtonTitle.Checked = true;
            radioButtonTitle.Location = new Point(6, 16);
            radioButtonTitle.Name = "radioButtonTitle";
            radioButtonTitle.Size = new Size(48, 19);
            radioButtonTitle.TabIndex = 0;
            radioButtonTitle.TabStop = true;
            radioButtonTitle.Text = "Title";
            radioButtonTitle.UseVisualStyleBackColor = true;
            radioButtonTitle.CheckedChanged += Radio_CheckedChanged;
            // 
            // imageListIcons
            // 
            imageListIcons.ColorDepth = ColorDepth.Depth32Bit;
            imageListIcons.ImageSize = new Size(16, 16);
            imageListIcons.TransparentColor = Color.Transparent;
            // 
            // TreeViewItemsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBoxOrder);
            Controls.Add(treeView1);
            Name = "TreeViewItemsForm";
            Text = "TreeViewItemsForm";
            groupBoxOrder.ResumeLayout(false);
            groupBoxOrder.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TreeView treeView1;
        private GroupBox groupBoxOrder;
        private RadioButton radioButtonYear;
        private RadioButton radioButtonGenre;
        private RadioButton radioButtonTitle;
        private RadioButton radioButtonRating;
        private ImageList imageListIcons;
        private RadioButton radioButtonActor;
        private CheckBox checkBoxToolTip;
    }
}