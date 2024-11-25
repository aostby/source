using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Collections;
using System.IO;
using System.Net;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Runtime.CompilerServices;
using Kolibri.Common.Utilities;

namespace Kolibri.Common.FormUtilities
{
    public class InputDialogs
    {
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            return InputBox(title, promptText, null, ref value);
        }
        public static DialogResult InputRichTextBox(string title, string promptText, ref string value)
        {

            Label label = new Label();
            label.AutoSize = true;
            label.Text = promptText;

            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.None;// DialogResult.OK;  //http://stackoverflow.com/questions/10330882/prevent-showdialog-from-returning-when-ok-button-is-clicked
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonOk.Click += new EventHandler(OKbuttonClick);

            FastColoredTextBoxNS.FastColoredTextBox textBox = new FastColoredTextBoxNS.FastColoredTextBox();
            textBox.Multiline = true;
            textBox.Enabled = true;
            textBox.Text = value;
            //TODO:            //http://stackoverflow.com/questions/10878977/how-to-not-close-a-form-when-user-presses-enter-key-inside-a-text-box
            //Endte opp  med enkelt å sette acceptreturn, men en må trukke ok to ganger
            textBox.AcceptsReturn = true;


            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            textBox.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom);
            buttonOk.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);
            buttonCancel.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right);

            Form form = new Form();
            form.Text = title;
            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.Sizable;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            //Øk størrelsen
            form.ClientSize = new Size(600, 400);

            textBox.Focus();
            DialogResult dialogResult = form.ShowDialog();

            value = textBox.Text;
            return dialogResult;
        }

        private static void OKbuttonClick(object sender, EventArgs e)
        {
            if ((sender as Button).Focused)
            {
                (sender as Button).DialogResult = DialogResult.OK;

                ((sender as Button).Parent as Form).DialogResult = (sender as Button).DialogResult;
            }
            else
            { }
        }

        public static DialogResult InputBox(string title, string promptText, string[] arr, ref string value)
        {
            Form form = new Form();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            //  TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            ComboBox comboboxArr = new ComboBox();

            comboboxArr.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboboxArr.AutoCompleteSource = AutoCompleteSource.ListItems;
            // comboboxArr.Focus();

            //Hvis arr = null
            TextBox textBox = new TextBox();
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    comboboxArr.Items.Add(arr[i]);
                    comboboxArr.SelectedIndex = 0;
                }
            }
            else
            {
                textBox.Text = value;

            }

            form.Text = title;
            label.Text = promptText;
            // textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            if (arr != null)
                comboboxArr.SetBounds(12, 36, 372, 20);
            else
                textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            if (arr != null)
                comboboxArr.Anchor = comboboxArr.Anchor | AnchorStyles.Right;
            else
                textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            if (arr != null)
                form.Controls.AddRange(new Control[] { label, comboboxArr, buttonOk, buttonCancel });
            else
                form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            if (arr != null)
                value = comboboxArr.Text;
            else
                value = textBox.Text;
            return dialogResult;
        }

        /// <summary>
        /// Metode som lar deg velge fra en dropdownliste
        /// </summary>
        /// <param name="title">Tittel på dialogboksen</param>
        /// <param name="promptText">Beskrivelse av hva som skal velges</param>
        /// <param name="arr">datatabelll som det skal velges en linje fra</param>
        /// <param name="valuemember">kolonnen  som inneholder verdien</param>
        /// <param name="displaymember">kolonnen som inneholder beskrivelsen av verdien</param>
        /// <param name="value">verdien (datarow) som velges</param>
        /// <returns></returns>
        public static DialogResult InputBox(string title, string promptText, DataTable arr, string valuemember, string displaymember, ref object value)
        {
            Form form = new Form();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            //  TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            ComboBox comboboxArr = new ComboBox();

            comboboxArr.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboboxArr.AutoCompleteSource = AutoCompleteSource.ListItems;
            // comboboxArr.Focus();

            //Hvis arr = null
            TextBox textBox = new TextBox();
            if (arr != null)
            {
                comboboxArr.DataSource = arr;
                comboboxArr.ValueMember = valuemember;
                comboboxArr.DisplayMember = displaymember;
            }

            form.Text = title;
            label.Text = promptText;
            // textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            if (arr != null)
                comboboxArr.SetBounds(12, 36, 372, 20);
            else
                textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            if (arr != null)
                comboboxArr.Anchor = comboboxArr.Anchor | AnchorStyles.Right;
            else
                textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            if (arr != null)
                form.Controls.AddRange(new Control[] { label, comboboxArr, buttonOk, buttonCancel });
            else
                form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            form.Width += 100;
            DialogResult dialogResult = form.ShowDialog();
            if (arr != null)
                value = comboboxArr.SelectedItem;
            else
                value = textBox.Text;
            return dialogResult;
        }

        public static DialogResult ChooseListBox(string title, string promptText, List<string> arr, ref object value, bool multiSelect = false)
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("StringValue"));
            foreach (var item in arr)
            {
                DataRow row = table.NewRow();
                row[0] = item;
                table.Rows.Add(row);
            }
            return ChooseListBox(title, promptText, table, ref value, multiSelect);
        }

        public static DialogResult ChooseListBox(string title, string promptText, DataTable arr, ref object value, bool multiSelect = false, string imageLinkColumn = null)
        { 

            Form form = new Form();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            //  TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();
            ListView listviewArr = new ListView();
            // Get the table from the data set
            DataTable dtable = arr;


            /// Set the view to show details.
            listviewArr.View = View.Details;

            // Allow the user to edit item text.
            listviewArr.LabelEdit = false;

            // Allow the user to rearrange columns.
            listviewArr.AllowColumnReorder = false;

            // Select the item and subitems when selection is made.
            listviewArr.FullRowSelect = true;
            listviewArr.MultiSelect = multiSelect;


            // Display grid lines.
            listviewArr.GridLines = true;

            // Sort the items in the list in ascending order.
            //listviewArr.Sorting = SortOrder.Ascending;

            for (int i = 0; i < arr.Columns.Count; i++)
            {
                listviewArr.Columns.Add(arr.Columns[i].ColumnName);//"Title", 300, HorizontalAlignment.Left);
            }

            // Clear the ListView control
            listviewArr.Items.Clear();

            // Display items in the ListView control
            for (int i = 0; i < dtable.Rows.Count; i++)
            {
                DataRow row = dtable.Rows[i];

                ListViewItem item = new ListViewItem(row[0].ToString());
                for (int j = 1; j < arr.Columns.Count; j++)
                {
                    item.SubItems.Add(row[j].ToString());
                    if (dtable.Columns[j].ColumnName.Equals(imageLinkColumn))
                        item.Tag = row[j].ToString();
                }
                listviewArr.Items.Add(item);
            }

            listviewArr.Items[0].Selected = true;
            listviewArr.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            if(!string.IsNullOrEmpty(imageLinkColumn))
                listviewArr.SelectedIndexChanged += ListviewArr_SelectedIndexChanged;

            form.Text = title;
            label.Text = promptText;
            // textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);

            listviewArr.SetBounds(12, 36, 372, 20 * 12);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;

            listviewArr.Anchor = listviewArr.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            if (arr != null)
                form.Controls.AddRange(new Control[] { label, listviewArr, buttonOk, buttonCancel });

            form.ClientSize = new Size(Math.Max(400, label.Right + 10), form.ClientSize.Height * 3);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;
            form.Width += 100;

            try
            {
                if (value != null)
                {
                    var item = listviewArr.FindItemWithText(value.ToString());
                    if (item != null)
                    {
                        item.Selected = true;
                        listviewArr.EnsureVisible(item.Index);
                    }
                }
            }
            catch (Exception)
            { }

            DialogResult dialogResult = form.ShowDialog();
            if (!multiSelect)
                value = listviewArr.SelectedItems[0];
            else
                value = listviewArr.SelectedItems;

            return dialogResult;
        }

        private static void ListviewArr_SelectedIndexChanged(object sender, EventArgs e)
        { 
            try
            {
                ListView view = sender as ListView;
                if (!view.Visible) return;
                ListViewItem value = view.SelectedItems[0];
            
                string imageUrl = $"{value.Tag}";
                if (imageUrl != null)
                {
                    try
                    {
                        Image img = ImageUtilities.GetImageFromUrl(imageUrl);
                        FormUtilities.SplashScreen.Splash(String.Empty, 1000, img);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            { 
            }
        }

        protected static void tbPassword_TextChanged(object sender, EventArgs e)
        {
            //Dette virker, men fratar textbox fokus, så det fukker up.
            //if (sender.GetType().Equals(typeof(MaskedTextBox)))
            //{
            //    try
            //    {
            //        Control tempsender = (sender as Control);

            //    Control[] btnarr = tempsender.Parent.Controls.Find("btnOK", true);


            //    if(btnarr!=null && btnarr.Length >=1)
            //        (btnarr[0] as Button).Focus();

            //    }
            //    catch (Exception)
            //    {

            //        throw;
            //    }
            //}


        }

        public static DialogResult PasswordDialog(string title, string promptText, ref string username, ref string password)
        {

            return GenericLogOnValueDialog(title, promptText, ref username, ref password, "Brukernavn", "Passord");
        }

        public static DialogResult GenericLogOnValueDialog(string title, string promptText, ref string value1, ref string value2, string label1 = "Bruker id", string label2 = "Passord")
        {
            TextBox tbEnterpriseSystemId;
            MaskedTextBox tbPassword;
            Label lbEnterpriseSystemId, lbPassword;
            Button btnOk, btnCancel;
            tbEnterpriseSystemId = new System.Windows.Forms.TextBox();
            tbPassword = new System.Windows.Forms.MaskedTextBox();
            lbEnterpriseSystemId = new System.Windows.Forms.Label();
            lbPassword = new System.Windows.Forms.Label();
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            // SuspendLayout();

            // tbEnterpriseSystemId
            tbEnterpriseSystemId.Location = new System.Drawing.Point(139, 12);
            tbEnterpriseSystemId.Name = "tbEnterpriseSystemId";
            tbEnterpriseSystemId.Size = new System.Drawing.Size(155, 20);


            // tbPassword
            tbPassword.Location = new System.Drawing.Point(139, 38);
            tbPassword.Name = "tbPassword";
            tbPassword.PasswordChar = '*';
            tbPassword.Size = new System.Drawing.Size(155, 20);
            tbPassword.TabIndex = 1;
            tbPassword.TextChanged += new System.EventHandler(tbPassword_TextChanged);

            // lbEnterpriseSystemId
            lbEnterpriseSystemId.AutoSize = true;
            lbEnterpriseSystemId.Location = new System.Drawing.Point(32, 12);
            lbEnterpriseSystemId.Name = "lbEnterpriseSystemId";
            lbEnterpriseSystemId.Size = new System.Drawing.Size(53, 13);
            lbEnterpriseSystemId.TabIndex = 2;
            lbEnterpriseSystemId.Text = label1;

            // lbPassword
            lbPassword.AutoSize = true;
            lbPassword.Location = new System.Drawing.Point(32, 38);
            lbPassword.Name = "lbPassword";
            lbPassword.Size = new System.Drawing.Size(45, 13);
            lbPassword.Text = label2;


            // btnOk
            btnOk.Location = new System.Drawing.Point(179, 78);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(75, 23);
            btnOk.Text = "Ok";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.DialogResult = DialogResult.OK;

            // btnCancel
            btnCancel.Location = new System.Drawing.Point(53, 78);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.Text = "Avbryt";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.DialogResult = DialogResult.Cancel;
            //  btnCancel.Click += new System.EventHandler(btnCancel_Click);

            // AltInnPwdDialog
            Form form = new Form();
            form.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            form.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            form.ClientSize = new System.Drawing.Size(306, 113);
            form.Controls.Add(lbPassword);
            form.Controls.Add(lbEnterpriseSystemId);
            form.Controls.Add(tbEnterpriseSystemId);
            form.Controls.Add(tbPassword);
            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.Name = "AltInnPwdDialog";
            form.ShowIcon = false;
            form.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            form.Text = "AltInn - Autentisering";
            form.ResumeLayout(false);
            tbEnterpriseSystemId.TabIndex = 0;
            tbPassword.TabIndex = 1;
            btnOk.TabIndex = 2;
            btnCancel.TabIndex = 3;
            form.PerformLayout();

            if (!string.IsNullOrEmpty(value1))
                tbEnterpriseSystemId.Text = value1;
            if (!string.IsNullOrEmpty(value2))
                tbPassword.Text = value2;

            if (string.IsNullOrEmpty(title))
                form.Text = $"Vennligst oppgi {label1}";
            else
                form.Text = title;

            DialogResult dialogResult = form.ShowDialog();
            value1 = tbEnterpriseSystemId.Text; //textBoxUsername.Text;
            value2 = tbPassword.Text;// textBoxPassWord.Text;
            return dialogResult;
        }
        public static DialogResult Generic2ValuesDialog(string title, string promptText, ref string value1, ref string value2, string label1 = "Bruker id", string label2 = "Passord")
        {
            TextBox tbEnterpriseSystemId;
            TextBox tbPassword;
            Label lbEnterpriseSystemId, lbPassword;
            Button btnOk, btnCancel;
            tbEnterpriseSystemId = new System.Windows.Forms.TextBox();
            tbPassword = new System.Windows.Forms.TextBox();
            lbEnterpriseSystemId = new System.Windows.Forms.Label();
            lbPassword = new System.Windows.Forms.Label();
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            // SuspendLayout();

            // tbEnterpriseSystemId
            tbEnterpriseSystemId.Location = new System.Drawing.Point(139, 12);
            tbEnterpriseSystemId.Name = "tbEnterpriseSystemId";
            tbEnterpriseSystemId.Size = new System.Drawing.Size(155, 20);


            // tbPassword
            tbPassword.Location = new System.Drawing.Point(139, 38);
            tbPassword.Name = "tbPassword";

            tbPassword.Size = new System.Drawing.Size(155, 20);
            tbPassword.TabIndex = 1;
            tbPassword.TextChanged += new System.EventHandler(tbPassword_TextChanged);

            // lbEnterpriseSystemId
            lbEnterpriseSystemId.AutoSize = true;
            lbEnterpriseSystemId.Location = new System.Drawing.Point(32, 12);
            lbEnterpriseSystemId.Name = "lbEnterpriseSystemId";
            lbEnterpriseSystemId.Size = new System.Drawing.Size(53, 13);
            lbEnterpriseSystemId.TabIndex = 2;
            lbEnterpriseSystemId.Text = label1;

            // lbPassword
            lbPassword.AutoSize = true;
            lbPassword.Location = new System.Drawing.Point(32, 38);
            lbPassword.Name = "lbPassword";
            lbPassword.Size = new System.Drawing.Size(45, 13);
            lbPassword.Text = label2;


            // btnOk
            btnOk.Location = new System.Drawing.Point(179, 78);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(75, 23);
            btnOk.Text = "Ok";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.DialogResult = DialogResult.OK;

            // btnCancel
            btnCancel.Location = new System.Drawing.Point(53, 78);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.Text = "Avbryt";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.DialogResult = DialogResult.Cancel;
            //  btnCancel.Click += new System.EventHandler(btnCancel_Click);

            // AltInnPwdDialog
            Form form = new Form();
            form.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            form.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            form.ClientSize = new System.Drawing.Size(306, 113);
            form.Controls.Add(lbPassword);
            form.Controls.Add(lbEnterpriseSystemId);
            form.Controls.Add(tbEnterpriseSystemId);
            form.Controls.Add(tbPassword);
            form.Controls.Add(btnOk);
            form.Controls.Add(btnCancel);
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.Name = "AltInnPwdDialog";
            form.ShowIcon = false;
            form.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            form.Text = "AltInn - Autentisering";
            form.ResumeLayout(false);
            tbEnterpriseSystemId.TabIndex = 0;
            tbPassword.TabIndex = 1;
            btnOk.TabIndex = 2;
            btnCancel.TabIndex = 3;

            form.PerformLayout();
            btnOk.Focus();

            if (!string.IsNullOrEmpty(value1))
                tbEnterpriseSystemId.Text = value1;
            if (!string.IsNullOrEmpty(value2))
                tbPassword.Text = value2;

            if (string.IsNullOrEmpty(title))
                form.Text = $"Vennligst oppgi {label1}";
            else
                form.Text = title;

            DialogResult dialogResult = form.ShowDialog();
            value1 = tbEnterpriseSystemId.Text;
            value2 = tbPassword.Text;


            return dialogResult;
        }

        protected static void listView_ItemActivate(object sender, EventArgs e)
        {
            foreach (ListViewItem item in (sender as ListView).SelectedItems)
            {
                //Windows.WindowsApp.SetKlientInfo((int)item.Tag, item.SubItems[1].Text);
                // DialogResult = DialogResult.OK;
            }
        }

        protected static void listView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //   AcceptButton.Enabled = listView.SelectedItems.Count > 0;
        }
    }
}