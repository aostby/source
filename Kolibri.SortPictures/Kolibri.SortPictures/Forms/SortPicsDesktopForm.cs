using System.Diagnostics;
     using Kolibri.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 
using System.Reflection;
namespace Kolibri.SortPictures.Forms
{
  
        public partial class SortPicsDesktopForm : Form
        {
            private static RichTextBox m_log = new RichTextBox();
            private static PictureBox m_picbox;
            private bool m_break;

            public SortPicsDesktopForm()
            {
                InitializeComponent();
                Init();
            }

            private void Init()
            {
                toolStripStatusLabelFilnavn.Text = string.Empty;
                textBoxSource.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                textBoxDestination.Text = Environment.CurrentDirectory;
                if (string.IsNullOrEmpty(textBoxDestination.Text))
                    textBoxDestination.Text = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures);


                this.StartPosition = FormStartPosition.CenterParent;
                //Lets make it nasty (some forms aren't rendered properly otherwise)
                this.WindowState = FormWindowState.Normal;
                this.WindowState = FormWindowState.Maximized;
                this.StartPosition = FormStartPosition.CenterParent;
                this.WindowState = FormWindowState.Normal;
                #region foldericon
                try
                {
                    // You're looking for a 16x16 opened folder icon : Icon icon16 = IconUtils.GetShellIcon(SHSTOCKICONID.SIID_FOLDEROPEN, SHGSI.SHGSI_ICON Or SHGSI.SHGSI_SMALLICON);
                    // You're looking for a 32x32 default state folder icon :Icon icon32 = IconUtils.GetShellIcon(SHSTOCKICONID.SIID_FOLDER, SHGSI.SHGSI_ICON Or SHGSI.SHGSI_LARGEICON);
                    Bitmap image = Icons.GetSystemIcon(Icons.SHSTOCKICONID.SIID_FOLDEROPEN, Icons.SHGSI.SHGSI_ICON | Icons.SHGSI.SHGSI_SMALLICON).ToBitmap();

                    if (image != null)
                    {
                        image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        buttonOpenDirS.Text = string.Empty;
                        buttonOpenDirD.Text = string.Empty;

                        buttonOpenDirS.BackgroundImage = image;
                        buttonOpenDirS.BackgroundImageLayout = ImageLayout.Stretch;
                        buttonOpenDirD.BackgroundImage = image;
                        buttonOpenDirD.BackgroundImageLayout = ImageLayout.Stretch;
                    }


                }
                catch (Exception) { }
                #endregion
            }

            private void button_Click(object sender, EventArgs e)
            {
                string text = null;
                if (sender.Equals(buttonSource))
                    text = textBoxSource.Text;
                else
                    text = textBoxDestination.Text;

                DirectoryInfo folder = null;
                if (checkBoxMTP.Checked)
                {
                    try
                    {
                        //CommonOpenFileDialog cofd = new CommonOpenFileDialog();
                        //cofd.IsFolderPicker = true;
                        //cofd.ShowDialog();
                        //folder = new DirectoryInfo(cofd.FileName);

                        var dlg1 = new FolderUtilities.FolderBrowserDialogEx();
                        dlg1.Description = "Select a folder to extract to:";
                        dlg1.ShowNewFolderButton = true;
                        dlg1.ShowEditBox = true;
                        //dlg1.NewStyle = false;
                        dlg1.SelectedPath = textBoxSource.Text;
                        dlg1.ShowFullPathInEditBox = true;
                        dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;

                        // Show the FolderBrowserDialog.
                        DialogResult result = dlg1.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            textBoxSource.Text = dlg1.SelectedPath;
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }

                }
                else
                {
                    folder = FileUtilities.LetOppMappe(text);////.Replace(@"\\", @"\"));
                }
                if (folder != null && folder.Exists)
                {
                    if (sender.Equals(buttonSource))
                    {
                        textBoxSource.Text = folder.FullName.ToString();
                        if (string.IsNullOrWhiteSpace(textBoxDestination.Text))
                        {
                            textBoxDestination.Text = new DirectoryInfo(textBoxSource.Text).Parent.FullName;
                        }
                    }
                    else
                    {
                        textBoxDestination.Text = folder.FullName.ToString();
                    }
                }

            }

            private void textBox_TextChanged(object sender, EventArgs e)
            {
                try
                {
                    var box = sender as TextBox;
                    DirectoryInfo info = new DirectoryInfo(box.Text);
                    if (info.Exists)
                        box.BackColor = Color.White;
                    else
                        box.BackColor = Color.LightSalmon;

                    if (textBoxSource.Text.Equals(textBoxDestination.Text))
                        textBoxDestination.BackColor = Color.LightYellow;
                }
                catch (Exception)
                { }

            }

            private void buttonExecute_Click(object sender, EventArgs e)
            {
            string stdtxt = groupBoxPreview.Text;
            m_break = false;
                buttonBreak.Enabled = !m_break;
                FileInfo info;
                DirectoryInfo dinfo = new DirectoryInfo(textBoxSource.Text);
                if (!dinfo.Exists)
                {//    throw new FileNotFoundException(dinfo.FullName);
                    MessageBox.Show("Kildefilsti ble ikke funnet!", "Kan ikke utføre handling");
                }

                var allowedExtensions = new[] { ".jpg", ".tiff", ".bmp", ".png" };

                var liste = Directory.EnumerateFiles(dinfo.FullName, "*.*", checkBoxSubdirs.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(s => allowedExtensions.Any(s.ToLower().EndsWith));

                var files = liste.Select(file => new FileInfo(file)).ToArray();

                DirectoryInfo dest = new DirectoryInfo(textBoxDestination.Text);
                foreach (FileInfo imgFile in files)
                {
                    if (m_break) break;
                    var date = ImageUtilities.DateTaken(imgFile.FullName);
                    DirectoryInfo dirDest = null;
                    if (radioButtonByYear.Checked)
                    {
                        dirDest = new DirectoryInfo(Path.Combine(dest.FullName, date.ToString("yyyy")));
                    }
                    else if (radioButtonByYearMonth.Checked)
                    {
                        dirDest = new DirectoryInfo(Path.Combine(dest.FullName, date.ToString("yyyy-MM")));
                    }
                    else if (radioButtonByDate.Checked)
                    {
                        dirDest = new DirectoryInfo(Path.Combine(dest.FullName, date.ToString("yyyy-MM-dd")));
                    }
                    else if (radioButtonHerarchy.Checked)
                    {
                        dirDest = new DirectoryInfo(Path.Combine(dest.FullName, $"{date.Year}", $"{date.Year}-{date.Month.ToString().PadLeft(2, '0')}", date.ToString("yyyy-MM-dd")));
                    }
                    if (!dirDest.Exists) dirDest.Create();
                    string filename = imgFile.Name;
                    info = new FileInfo(Path.Combine(dirDest.FullName, imgFile.Name));
                    try
                    {
                        string text = string.Empty;

                        if (radioButtonPreviewFile.Checked)
                        {
                        try
                        {
                            Image image;
                            using (FileStream stream = new FileStream(imgFile.FullName, FileMode.Open, FileAccess.Read))
                            {
                                image = Image.FromStream(stream);
                            }
                            m_picbox.Image = image;
                            groupBoxPreview.Text = $"{stdtxt} ({imgFile.FullName})";

                            Application.DoEvents();
                        }
                        catch (Exception)
                        {
                        }
                        }

                        if (radioButtonKopier.Checked)
                        {
                            System.IO.File.Copy(imgFile.FullName, info.FullName);

                            text = $"Kopierer filen {info.Name} fra {imgFile.DirectoryName} til {info.DirectoryName}";
                        }
                        else if (radioButtonFlytt.Checked)
                        {
                            try
                            {
                                text = $"Flytter filen {info.Name} fra {imgFile.DirectoryName} til {info.DirectoryName}";
                                if (File.Exists(info.FullName)) { File.Delete(info.FullName); }
                                System.IO.File.Move(imgFile.FullName, info.FullName);

                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                        }

                        this.toolStripStatusLabelFilnavn.Text = text;
                        m_log.AppendText(text + Environment.NewLine);
                        this.Refresh();
                        try { m_log.ScrollToCaret(); } catch (Exception) { }
                    }
                    catch (Exception ex)
                    {
                        string msg = $"{info} {ex.Message}".Trim();
                        m_log.AppendText($"{msg}\r\n");
                        this.toolStripStatusLabelFilnavn.Text = msg;
                    }
                    Application.DoEvents();
                }

                if (checkBoxEmptyDirs.Checked)
                    FileUtilities.DeleteEmptyDirs(dinfo);
                Thread.Sleep(100);
                try
                {
                    //Process.Start($"{dest.FullName}");
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = dest.FullName,
                        UseShellExecute = true
                    });
                }
                 catch (Exception) { }
              
                this.toolStripStatusLabelFilnavn.Text = $"Fullført! {files.Count()} files";
                m_log.AppendText($"{this.toolStripStatusLabelFilnavn.Text}\r\n");
                if (radioButtonPreviewLog.Checked) { Application.DoEvents(); this.Refresh(); }

                try
                {
                    m_log.ScrollToCaret();
                }
                catch (Exception)
                { }

                buttonBreak.Enabled = !m_break;
            groupBoxPreview.Text = stdtxt;
            }

            private void checkBoxWPD_CheckedChanged(object sender, EventArgs e)
            {
                if (checkBoxMTP.Checked)
                {

                    //if (MessageBox.Show("Windows Portable Devices lar seg ikke åpne via OpenFileDialog, dermed må du kopiere filene til en harddisk først. Beklager.\r\nVil du åpne windows explorer så du kan kopiere filene selv?", "Kan ikke åpne WPD filer via USB i denne versjonen", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    //    Process.Start("explorer.exe");
                    //checkBoxMTP.Checked = false;
                }
            }

            private void radioButtonPreview_CheckedChanged(object sender, EventArgs e)
            {
                try
                {
                    var radioButton = sender as RadioButton;

                    if (radioButton == radioButtonNoPreview && radioButton.Checked == true)
                    {
                        groupBoxPreview.Controls.Clear();
                    }
                    if (radioButton == radioButtonPreviewLog && radioButton.Checked == true)
                    {
                        groupBoxPreview.Controls.Clear();
                        if (m_log == null) m_log = new RichTextBox();
                        groupBoxPreview.Controls.Add(m_log);
                        m_log.Dock = DockStyle.Fill;
                        m_log.Visible = true;
                    }
                    if (radioButton == radioButtonPreviewFile && radioButton.Checked == true)
                    {
                        groupBoxPreview.Controls.Clear();
                        groupBoxPreview.Controls.Clear();
                        m_picbox = new PictureBox();
                        m_picbox.Image = m_picbox.InitialImage;

                        groupBoxPreview.Controls.Add(m_picbox);
                        m_picbox.Dock = DockStyle.Fill;
                        m_picbox.SizeMode = PictureBoxSizeMode.StretchImage;
                        m_picbox.SizeMode = PictureBoxSizeMode.Zoom;
                        m_picbox.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    this.toolStripStatusLabelFilnavn.Text = ex.Message;
                }

            }

            private void buttonBreak_Click(object sender, EventArgs e)
            {
                m_break = !m_break;
            }

            private void buttonOpenDir_Click(object sender, EventArgs e)
            {
                try
                {
                    TextBox textBox = null;
                    if (sender.Equals(buttonOpenDirS)) { textBox = textBoxSource; }
                    else if (sender.Equals(buttonOpenDirD)) { textBox = textBoxDestination; }

                    Process.Start(textBox.Text);
                }
                catch (Exception ex)
                {
                    this.toolStripStatusLabelFilnavn.Text = ex.Message;
                }
            }
        }
    }