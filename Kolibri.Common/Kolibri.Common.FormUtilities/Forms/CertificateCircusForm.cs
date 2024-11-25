using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Principal;
using System.Collections;
using Kolibri.Common.Utilities.Constants;
using Kolibri.Common.Utilities;

namespace Kolibri.Common.FormUtilities.Forms
{
    public partial class CertificateCircusForm : Form
    {
        public static string m_batchText = string.Empty;

        public CertificateCircusForm()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            this.Text = "FindPrivateKey - Certificate Circus " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            List<string> liste = new List<string>();
            liste.AddRange(Enum.GetNames(typeof(X509FindType)));
            comboBox1.DataSource = liste;
            comboBox1.SelectedIndex = liste.IndexOf("FindByThumbprint");

            FileInfo info = new FileInfo(Path.Combine( Constants.ToolsPath.FullName, @"FindPrivateKey\FindPrivateKey.exe"));
            if (info.Exists)
                textBoxFindPrivateKeyEXE.Text = info.FullName;
            else
            {
                MessageBox.Show(info.FullName, "File not found");
                buttonFindBy.Enabled = false;
            }


            InitUserCredentials(); InitGridView();
        }

        private void InitUserCredentials()
        {
            try
            {
                RichTextBox tbx = new RichTextBox();

                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                var test = principal.IsInRole(WindowsBuiltInRole.Administrator);
                tbx.AppendText(identity.Name + Environment.NewLine);
                tbx.AppendText(test ? "admin" : "not admin" + Environment.NewLine);

                tbx.Dock = DockStyle.Fill;
                groupBoxCredentials.Controls.Add(tbx);
            }
            catch (Exception)
            {
                groupBoxCredentials.Visible = false;
            }

        }

        private void InitGridView()
        {
            //X509Certificate2UI.DisplayCertificate(cert);

            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);

                var certCollection = store.Certificates;

                ArrayList arrayList = new ArrayList();
                foreach (X509Certificate2 signingCert in certCollection) { arrayList.Add(signingCert); }
                //dataGridView1.DataSource = Utilities.DataSetUtilities.AutoGenererDataSet(arrayList).Tables[0];
                DataSet ds = DataSetUtilities.AutoGenererTypedDataSet(arrayList, true);
                DataTable table = ds.Tables[0];
                table = new DataView(table).ToTable(false, "FriendlyName", "Thumbprint", "Subject");
                dataGridViewCert.AutoResizeColumns();

                dataGridViewCert.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                dataGridViewCert.DataSource = table;
                /*   foreach (X509Certificate2 signingCert in certCollection) { arrayList.Add(signingCert); }
                DataSet ds = Utilities.DataSetUtilities.AutoGenererDataSet(arrayList);
                DataTable table = ds.Tables[0];
               
                dataGridView1.DataSource = table;*/

            }
            finally
            {
                store.Close();
            }
        }

        private void buttonListCertificates_Click(object sender, EventArgs e)
        {
            try
            {
                var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                var selectedCertificate = X509Certificate2UI.SelectFromCollection(
                    store.Certificates,
                    "Store:" + store.Name,
                    "Location: " + store.Location,
                    X509SelectionFlag.SingleSelection);

                if (selectedCertificate.Count > 0)
                {

                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf("FindByThumbprint");
                    textBoxValue.Text = (selectedCertificate[0] as X509Certificate2).Thumbprint;
                    buttonFindBy_Click(null, null);
                }
            }
            catch (Exception)
            { }

        }

        public static X509Certificate2 GetCertificate(X509FindType type, string value)
        {
            X509Certificate2 ret = null;
            try
            {
                if (type.Equals(X509FindType.FindByThumbprint))
                    value = Regex.Replace(value, @"[^\da-zA-z]", string.Empty).ToUpper();

                var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

                try
                {
                    store.Open(OpenFlags.ReadOnly);

                    var certCollection = store.Certificates;
                    var signingCert = certCollection.Find(type, value, false);
                    if (signingCert.Count == 0)
                    {
                        throw new FileNotFoundException(string.Format("Cert with {1}: '{0}' not found in local machine cert store.", value, type));
                    }

                    ret = signingCert[0];
                }
                finally
                {
                    store.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
            return ret;
        }

        private void buttonFindBy_Click(object sender, EventArgs e)
        {
            buttonFindBy.Enabled = false;
            try
            {
                textBoxPrivateKey.Clear();
                richTextBox1.Clear();
                richTextBox1.Update();
                richTextBox1.Parent.Update();

                m_batchText = string.Empty;
                X509FindType type = (X509FindType)Enum.Parse(typeof(X509FindType), comboBox1.Text);

                X509Certificate2 cert = GetCertificate(type, textBoxValue.Text);
                this.Text = string.Format("Name: {0}", cert.FriendlyName);
                richTextBox1.Text = "Please wait...";
                richTextBox1.Update();
                richTextBox1.Parent.Update();
                CreateBatch(cert);
                richTextBox1.Text += m_batchText;

                if (richTextBox1.Text.Contains("FindPrivateKey failed for the following reason:"))
                { textBoxPrivateKey.BackColor = Color.Salmon; return; }

                string searchtext = "Private key file name:";
                searchtext = m_batchText.Substring(m_batchText.IndexOf(searchtext) + searchtext.Length);
                searchtext = searchtext.TrimStart();
                searchtext = searchtext.Substring(0, searchtext.IndexOf(Environment.NewLine));
                textBoxPrivateKey.Text = searchtext;
            }
            catch (Exception)
            { richTextBox1.Clear(); textBoxPrivateKey.Text = string.Empty; }
            buttonCreateCACLSbatch.Enabled = !string.IsNullOrEmpty(textBoxPrivateKey.Text);
            buttonFindBy.Enabled = true;
        }

        private void CreateBatch(X509Certificate2 cert)
        {
            try
            {
                FileInfo batch = new FileInfo(Path.Combine(Path.GetTempPath(), "batch.bat"));

                if (!File.Exists(textBoxFindPrivateKeyEXE.Text))
                    throw new FileNotFoundException(textBoxFindPrivateKeyEXE.Text);

                string batchScript = string.Format(@"set thumbprint={0}
echo Searching for thumbprint (value):  %thumbprint%  
FindPrivateKey My LocalMachine -t  %thumbprint%
exit", cert.Thumbprint.ToUpper().Replace(" ", string.Empty));
                FileInfo filename = new FileInfo(Path.Combine(Path.GetTempPath(), Path.GetFileName(textBoxFindPrivateKeyEXE.Text)));
                File.Copy(textBoxFindPrivateKeyEXE.Text, filename.FullName, true);


                string temp = string.Format(@"cd ""{0}"" {1}", filename.Directory.FullName, Environment.NewLine) + batchScript;

                File.WriteAllText(batch.FullName, temp);
                RunScript(batch);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        public void RunScript(FileInfo batch)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(string.Format(@"""{0}""", batch.FullName));
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;

            Process process = Process.Start(startInfo);
            process.OutputDataReceived += (sender, e) => m_batchText += (e.Data) + Environment.NewLine;
            process.BeginOutputReadLine();
            process.WaitForExit();

            // We may not have received all the events yet!
            Thread.Sleep(1000);
        }

        #region RunScript_old01
        private void RunScript_old01(string batchScript)
        {
            if (!File.Exists(textBoxFindPrivateKeyEXE.Text))
                throw new FileNotFoundException(textBoxFindPrivateKeyEXE.Text);

            FileInfo filename = new FileInfo(Path.Combine(Path.GetTempPath(), Path.GetFileName(textBoxFindPrivateKeyEXE.Text)));
            File.Copy(textBoxFindPrivateKeyEXE.Text, filename.FullName, true);

            FileInfo batch = new FileInfo(Path.Combine(Path.GetTempPath(), "batch.bat"));

            File.WriteAllText(batch.FullName,
                string.Format(@"cd ""{0}"" {1}", filename.Directory.FullName, Environment.NewLine) + batchScript);



            ProcessStartInfo info = new ProcessStartInfo();
            info.Arguments = string.Format(" {0}", batch.FullName);
            info.WindowStyle = ProcessWindowStyle.Normal;
            info.CreateNoWindow = true;
            info.FileName = "cmd.exe";
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            using (Process process = Process.Start(info))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    richTextBox1.Text += result;
                }
            }
        }
        #endregion

        private void textBoxFindPrivateKeyEXE_TextChanged(object sender, EventArgs e)
        {
            try
            {
                FileInfo info = new FileInfo(textBoxFindPrivateKeyEXE.Text);
                textBoxFindPrivateKeyEXE.BackColor = info.Exists ? Color.Wheat : Color.Red;
            }
            catch (Exception)
            {
                textBoxFindPrivateKeyEXE.BackColor = Color.Red;
            }
        }

        private void buttonCreateCACLSbatch_Click(object sender, EventArgs e)
        {
            string clipText = string.Format(@"cacls.exe ""C:\Documents and Settings\All Users\Application Data\Microsoft\Crypto\RSA\MachineKeys\{0}"" /E /G ""NETWORK SERVICE"":R", textBoxPrivateKey.Text);
            Clipboard.SetText(clipText);
            MessageBox.Show(clipText, "Script placed on clipboard");
        }

        private void buttonDisplayCert_Click(object sender, EventArgs e)
        {
            try
            {
                X509FindType type = (X509FindType)Enum.Parse(typeof(X509FindType), comboBox1.Text);

                X509Certificate2 cert = GetCertificate(type, textBoxValue.Text);
                X509Certificate2UI.DisplayCertificate(cert);

            }
            catch (Exception)
            { }

        }

        private void buttonOpenLocation_Click_1(object sender, EventArgs e)
        {
            FileInfo info = new FileInfo(string.Format(@"C:\Documents and Settings\All Users\Application Data\Microsoft\Crypto\RSA\MachineKeys\{0}", textBoxPrivateKey.Text));
            if (info.Exists)
                FileUtilities.OpenFolderHighlightFile(info);
            else
                Process.Start(info.Directory.FullName);
        }

        private void buttonOpenCertMgr_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("certmgr.msc");////"CertMgr");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void textBoxPrivateKey_TextChanged(object sender, EventArgs e)
        {

            try
            {
                FileInfo info = new FileInfo(string.Format(@"C:\Documents and Settings\All Users\Application Data\Microsoft\Crypto\RSA\MachineKeys\{0}", textBoxPrivateKey.Text));
                if (info.Exists)
                    textBoxPrivateKey.BackColor = Color.LightGreen;
                else
                    textBoxPrivateKey.BackColor = Color.White;
          

            }
            catch (Exception)
            { }
        } 

        private void dataGridViewCert_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataRow row = ((sender as DataGridView).Rows[e.RowIndex].DataBoundItem as DataRowView).Row;
                textBoxValue.Text = string.Format("{0}", row["Thumbprint"]);
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf("FindByThumbprint");
                buttonDisplayCert_Click(null, null);
            }
            catch (Exception)
            { }
          
        }
    }
}