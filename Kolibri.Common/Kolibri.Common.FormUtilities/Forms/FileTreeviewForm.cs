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

namespace Kolibri.Common.FormUtilities.Forms
{
    //http://stackoverflow.com/questions/16315042/how-to-display-directories-in-a-treeview
    public partial class FileTreeviewForm : Form
      {
     
        private DirectoryInfo m_directoryInfo;
        public static ImageList m_imgList;
        private Panel panel;
        public FileTreeviewForm(DirectoryInfo dir, Panel displayPanel)
        {
            m_imgList = new ImageList();
            panel = displayPanel;
            m_directoryInfo = dir;
            InitializeComponent();
            PopulateTreeView();
        }

        private void PopulateTreeView()
        {
            if (m_directoryInfo.Exists)
            {
                try
                {
                    treeView.ImageList = m_imgList;
                    treeView.Nodes.Add(LoadDirectory(m_directoryInfo));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private TreeNode LoadDirectory(DirectoryInfo di)
        {
            if (!di.Exists)
                return null;

            TreeNode output = new TreeNode(di.Name, 0, 0);
            output.Name = di.Name;
            output.Text = di.Name;
            output.Tag = di;

            string folderUt = "folder";

            if (m_imgList.Images.IndexOfKey(folderUt) < 0)
            {
                try
                {
                    m_imgList.Images.Add(folderUt,  Icons.GetFolderIcon());
                }
                catch (Exception) { }
            }

            if (m_imgList.Images.IndexOfKey(folderUt) >= 0) output.ImageIndex = m_imgList.Images.IndexOfKey(folderUt);
            foreach (var subDir in di.GetDirectories())
            {
                try
                {
                    output.Nodes.Add(LoadDirectory(subDir));
                }
                catch (IOException ex) { }
                catch { }
            }

            foreach (var file in di.GetFiles())
            {
                if (file.Exists)
                {
                    try
                    {
                        TreeNode filenode = new TreeNode() { Name = file.Name, Tag = file, Text = file.Name };
                        output.Nodes.Add(filenode);
                        try
                        {
                            if (m_imgList.Images.IndexOfKey(file.Extension) < 0 && !String.IsNullOrEmpty(file.Extension))
                            {
                                m_imgList.Images.Add(file.Extension, ImageUtilities.GetIconFromFile(file));
                            }
                        }
                        catch (Exception)
                        {
                            try { m_imgList.Images.Add(file.Extension, SystemIcons.Application); }
                            catch (Exception) { }
                        }

                        try
                        {
                            if (!String.IsNullOrEmpty(file.Extension) && m_imgList.Images.IndexOfKey(file.Extension) > 0)
                            {
                                filenode.ImageIndex = m_imgList.Images.IndexOfKey(file.Extension);
                                filenode.SelectedImageIndex = filenode.ImageIndex;
                            }
                        }
                        catch (Exception) { }
                    }
                    catch (Exception)
                    {
                        output.Nodes.Add(file.Name);
                    }

                }
            }

            return output;
        }

        private TreeNode LoadDirectory_fail(DirectoryInfo di)
        {
            if (!di.Exists)
                return null;

            TreeNode output = new TreeNode(di.Name, 0, 0);
            output.Tag = di;

            foreach (var subDir in di.GetDirectories())
            {
                try
                {
                    output.Nodes.Add(LoadDirectory(subDir));
                }
                catch (IOException ex)
                {
                    //handle error
                }
                catch { }
            }

            foreach (var file in di.GetFiles())
            {
                if (file.Exists)
                {
                    TreeNode filenode = new TreeNode() { Name = file.Name, Tag = file };
                    output.Nodes.Add(filenode);
                }
            }

            return output;
        }

        private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            DirectoryInfo dir = e.Node.Tag as DirectoryInfo;
            FileInfo info =    e.Node.Tag as FileInfo;
            Form form= null;
            try
            {
                #region Directory
                if (dir != null && dir.Exists)
                {
                    panel.Controls.Clear();
                    form = new ListViewForm(dir);
                }
                #endregion
                #region File - selector for options
                if (info != null && info.Exists)
                {
                    panel.Controls.Clear();
                    if (false) { }
                    else
                    {
                        string ext = info.Extension.ToUpper().TrimStart('.');
                        switch (ext)
                        {
                            case "SLN":
                                FileInfo sln = new FileInfo(@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe");
                                if (sln.Exists)
                                {
                                    //FileUtilities.Start(sln, info);
                                    Process proc = new Process();
                                    proc.StartInfo.FileName = sln.FullName;
                                    proc.StartInfo.UseShellExecute = true;
                                    proc.StartInfo.Verb = "runas";
                                    proc.StartInfo.Arguments = info.FullName;
                                    proc.Start();

                                    //Få kjørt som administrator 
                                    //"runas /user:administrator C:\data\mybatchfile.bat/

                                }
                                else
                                {
                                    //Process.Start(info.FullName); 
                                   // List <string> liste = Utilities.FileUtilities.GetVS_SLN_dependencies(info.FullName);
                                }
                                break;
                            case "BTM": form = new Forms.BTMapViewer(info); break;
                            
                            case "CS": form = FormUtilities.Controller.OutputFormController.RichTexBoxForm(info.Name, File.ReadAllText(info.FullName), FastColoredTextBoxNS.Language.CSharp, this.Size); break;
                            case "CSS": form =FormUtilities.Controller.OutputFormController.RichTexBoxForm(info.Name, File.ReadAllText(info.FullName), FastColoredTextBoxNS.Language.PHP, this.Size); break;
                            #region transform
                            case "XSLT":
                            case "XSL":
                            case "XML": form = new Forms.TransformForm(info.Directory, ext); break; //System.Collections.DictionaryEntry item               
                            #endregion
                            #region xsd
                            case "XSD":
                                panel.Controls.Add(FormUtilities.Controller.ControlController.GetTransformedHTML(info, "xs3p")); break;
                            #endregion //xsd

                            case "RTF":
                                string command = string.Format(@"""{0}"" ""{1}""", @"%ProgramFiles%\Windows NT\Accessories\wordpad.exe", info.FullName);
                                FileUtilities.Start(command);
                                break;
                            default:
                                //Bare slutt om en tror det er en farlig filtype, ellers kjør på...
                                if (FileUtilities.IsDangerousFileExtension(ext))
                                { throw new Exception("File considered not to be opened by this app. Use Right mouse button, or open the file in Explorer", new Exception(string.Format("Files with extension {0} are considered unopenable by this application", ext.ToUpper()))); }

                                else
                                {  if(MessageBox.Show("Open file " + info.Name, "Unknown Event", MessageBoxButtons.OKCancel, MessageBoxIcon.Question                                    )==DialogResult.OK)
                                    Process.Start(info.FullName); 
                                }
                                return;
                                break;
                        }
                    }
                }
                #endregion

                if (form != null)
                {
                    form.TopLevel = false;
                    form.AutoScroll = true;
                    form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    form.Dock = DockStyle.Fill;
                    panel.Controls.Add(form);
                    form.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

  

        private void treeView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Right))
            {
                try
                {
                    FileInfo info = (sender as TreeView).SelectedNode.Tag as FileInfo;
                    if (info != null && info.FullName != null)
                    {
                        if (info.Extension.Equals(".sln", StringComparison.InvariantCultureIgnoreCase))
                        {
                            Uri url = new Uri("http://wiki.hemit.helsemn.no/Grupper/Integrasjonsdrift/3_Applikasjoner/BizTalk_Server_2013/BizTalk_Applikasjoner/Andre_-_BizTalk_Applikasjoner/" +
                            Path.GetFileNameWithoutExtension(info.FullName));

                            MenuItem item = new MenuItem("Open WIKI", new EventHandler(OpenEvent_click));
                            item.Tag = url;
                            ContextMenu cm = new ContextMenu();
                            cm.MenuItems.Add(item);
                            (sender as TreeView).SelectedNode.ContextMenu = cm;

                        }
                        else
                        {
                            FileUtilities.ShowOpenWithDialog(info.FullName);
                        }
                    }
                    if (info == null)
                    {
                        DirectoryInfo di = (sender as TreeView).SelectedNode.Tag as DirectoryInfo;

                        if (di != null && di.FullName != null)
                        {
                            ContextMenu cm = new ContextMenu();

                            MenuItem item = new MenuItem("Open path", new EventHandler(OpenEvent_click));
                            item.Tag = (sender as TreeView).SelectedNode.Tag;
                              cm.MenuItems.Add(item);
                            if (item.Tag as DirectoryInfo != null) {
                                  item = new MenuItem("Set as startup path", new EventHandler(OpenEvent_click));
                               item.Tag = (sender as TreeView).SelectedNode.Tag;
                                cm.MenuItems.Add(item);
                            }
                          
                          
                            (sender as TreeView).SelectedNode.ContextMenu = cm;
                        }
                    }

                }
                catch (Exception) { }
            }
        }

        private void OpenEvent_click(object sender, EventArgs e)
        {
            try
            {
                if (sender as MenuItem != null)
                {
                    if ((((sender as MenuItem).Tag) as DirectoryInfo) != null)
                    {
                        if((sender as MenuItem).Text.Contains("startup"))
                        {
                                RegistryUtilites.WorkingDirectory((sender as MenuItem).Tag as DirectoryInfo);
                                MessageBox.Show("Changes will be effective on next startup\r\nIf you want to change the path immediately, please use the 'File' menu",   "New startup path set to " + ((sender as MenuItem).Tag as DirectoryInfo).Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                            Process.Start((((sender as MenuItem).Tag) as DirectoryInfo).FullName);
                    }
                    else if ((((sender as MenuItem).Tag) as FileInfo) != null)
                        Process.Start((((sender as MenuItem).Tag) as FileInfo).FullName);

                    else
                        Process.Start(string.Format("{0}", (sender as MenuItem).Tag));
                }  
            }
            catch (Exception)
            { }
        }      
    }
}