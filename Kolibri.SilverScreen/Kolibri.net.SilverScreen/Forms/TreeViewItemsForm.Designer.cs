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
            radioButtonExists = new RadioButton();
            radioButtonRated = new RadioButton();
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
            treeView1.Location = new Point(14, 67);
            treeView1.Margin = new Padding(3, 4, 3, 4);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(886, 516);
            treeView1.TabIndex = 0;
            treeView1.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;
            treeView1.MouseDown += treeView1_MouseDown;
            // 
            // groupBoxOrder
            // 
            groupBoxOrder.Controls.Add(radioButtonExists);
            groupBoxOrder.Controls.Add(radioButtonRated);
            groupBoxOrder.Controls.Add(checkBoxToolTip);
            groupBoxOrder.Controls.Add(radioButtonActor);
            groupBoxOrder.Controls.Add(radioButtonRating);
            groupBoxOrder.Controls.Add(radioButtonYear);
            groupBoxOrder.Controls.Add(radioButtonGenre);
            groupBoxOrder.Controls.Add(radioButtonTitle);
            groupBoxOrder.Location = new Point(14, 4);
            groupBoxOrder.Margin = new Padding(3, 4, 3, 4);
            groupBoxOrder.Name = "groupBoxOrder";
            groupBoxOrder.Padding = new Padding(3, 4, 3, 4);
            groupBoxOrder.Size = new Size(646, 55);
            groupBoxOrder.TabIndex = 1;
            groupBoxOrder.TabStop = false;
            groupBoxOrder.Text = "Order by";
            // 
            // radioButtonExists
            // 
            radioButtonExists.AutoSize = true;
            radioButtonExists.Location = new Point(414, 21);
            radioButtonExists.Margin = new Padding(3, 4, 3, 4);
            radioButtonExists.Name = "radioButtonExists";
            radioButtonExists.Size = new Size(66, 24);
            radioButtonExists.TabIndex = 7;
            radioButtonExists.Text = "Exists";
            radioButtonExists.UseVisualStyleBackColor = true;
            radioButtonExists.CheckedChanged += Radio_CheckedChanged;
            // 
            // radioButtonRated
            // 
            radioButtonRated.AutoSize = true;
            radioButtonRated.Location = new Point(351, 21);
            radioButtonRated.Margin = new Padding(3, 4, 3, 4);
            radioButtonRated.Name = "radioButtonRated";
            radioButtonRated.Size = new Size(69, 24);
            radioButtonRated.TabIndex = 6;
            radioButtonRated.Text = "Rated";
            radioButtonRated.UseVisualStyleBackColor = true;
            radioButtonRated.CheckedChanged += Radio_CheckedChanged;
            // 
            // checkBoxToolTip
            // 
            checkBoxToolTip.AutoSize = true;
            checkBoxToolTip.Location = new Point(480, 21);
            checkBoxToolTip.Margin = new Padding(3, 4, 3, 4);
            checkBoxToolTip.Name = "checkBoxToolTip";
            checkBoxToolTip.Size = new Size(88, 24);
            checkBoxToolTip.TabIndex = 5;
            checkBoxToolTip.Text = "Vis bilde";
            checkBoxToolTip.UseVisualStyleBackColor = true;
            // 
            // radioButtonActor
            // 
            radioButtonActor.AutoSize = true;
            radioButtonActor.Location = new Point(288, 21);
            radioButtonActor.Margin = new Padding(3, 4, 3, 4);
            radioButtonActor.Name = "radioButtonActor";
            radioButtonActor.Size = new Size(66, 24);
            radioButtonActor.TabIndex = 4;
            radioButtonActor.Text = "Actor";
            radioButtonActor.UseVisualStyleBackColor = true;
            radioButtonActor.CheckedChanged += Radio_CheckedChanged;
            // 
            // radioButtonRating
            // 
            radioButtonRating.AutoSize = true;
            radioButtonRating.Location = new Point(185, 21);
            radioButtonRating.Margin = new Padding(3, 4, 3, 4);
            radioButtonRating.Name = "radioButtonRating";
            radioButtonRating.Size = new Size(108, 24);
            radioButtonRating.TabIndex = 3;
            radioButtonRating.Text = "ImdbRating";
            radioButtonRating.UseVisualStyleBackColor = true;
            radioButtonRating.CheckedChanged += Radio_CheckedChanged;
            // 
            // radioButtonYear
            // 
            radioButtonYear.AutoSize = true;
            radioButtonYear.Location = new Point(127, 21);
            radioButtonYear.Margin = new Padding(3, 4, 3, 4);
            radioButtonYear.Name = "radioButtonYear";
            radioButtonYear.Size = new Size(58, 24);
            radioButtonYear.TabIndex = 2;
            radioButtonYear.Text = "Year";
            radioButtonYear.UseVisualStyleBackColor = true;
            radioButtonYear.CheckedChanged += Radio_CheckedChanged;
            // 
            // radioButtonGenre
            // 
            radioButtonGenre.AutoSize = true;
            radioButtonGenre.Location = new Point(62, 21);
            radioButtonGenre.Margin = new Padding(3, 4, 3, 4);
            radioButtonGenre.Name = "radioButtonGenre";
            radioButtonGenre.Size = new Size(69, 24);
            radioButtonGenre.TabIndex = 1;
            radioButtonGenre.Text = "Genre";
            radioButtonGenre.UseVisualStyleBackColor = true;
            radioButtonGenre.CheckedChanged += Radio_CheckedChanged;
            // 
            // radioButtonTitle
            // 
            radioButtonTitle.AutoSize = true;
            radioButtonTitle.Checked = true;
            radioButtonTitle.Location = new Point(7, 21);
            radioButtonTitle.Margin = new Padding(3, 4, 3, 4);
            radioButtonTitle.Name = "radioButtonTitle";
            radioButtonTitle.Size = new Size(59, 24);
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
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(groupBoxOrder);
            Controls.Add(treeView1);
            Margin = new Padding(3, 4, 3, 4);
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
        private RadioButton radioButtonRated;
        private RadioButton radioButtonExists;
    }
}