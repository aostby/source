

using Kolibri.net.C64Sorter.Controllers;
using Kolibri.net.C64Sorter.Entities;
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Kolibri.net.C64Sorter.Forms
{
    public partial class FTPTreeviewForm : Form
    {
        // Store credentials securely, not in global variables in a real app
        private string _FtpUser = "anonymous";
        private string _FtpPass = string.Empty;
        private string _FtpRootUrl = string.Empty;
        private string _hostname = string.Empty;
        public FTPTreeviewForm(string hostname)
        {
            InitializeComponent();
            _hostname = hostname;
            _FtpRootUrl = $"ftp://{hostname}";
            Init();
        }


        // Initial load (e.g., in Form_Load event)
        private void Init()
        {
            treeView1.Nodes.Clear();
            var tmp = new FtpItemDetail() { Name = _FtpRootUrl, FullPath = _FtpRootUrl, IsDirectory = true };

            // TreeNode rootNode = CreateDirectoryNode(_FtpRootUrl, _FtpRootUrl);
            TreeNode rootNode = CreateDirectoryNode(tmp);
            treeView1.Nodes.Add(rootNode);
            treeView1.ImageList = imageListIcons;
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.Text = $"Host:{_FtpRootUrl} User: {_FtpUser}";
            SetStatusLabel(this.Text);


            try
            {
                var defaultIcon = Icons.GetFolderIcon();
                imageListIcons.Images.Add("default", defaultIcon);
                imageListIcons.Images.Add(nameof(MessageBoxIcon.Question).ToLower(), SystemIcons.Question);  
                var fileList = FileUtilities.GetFiles(new DirectoryInfo(@".\Resources\"), "*.ico", false);
                foreach (var file in fileList)
                {
                    try
                    {
                        var icon = new Icon(file.FullName);
                        imageListIcons.Images.Add(Path.GetFileNameWithoutExtension(file.Name), icon);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception)
            { }
        }

        public void SetStatusLabel(string statusText)
        {
            try { toolStripStatusLabelStatus.Text = statusText; }
            catch (Exception) { }
        }


        // Function to create a node and add a placeholder child for dynamic loading
        private TreeNode CreateDirectoryNode(FtpItemDetail detail)
        {
            TreeNode directoryNode = new TreeNode(detail.Name);
            // Set the icons for the normal and selected states
            directoryNode.ImageKey = "default";
            directoryNode.SelectedImageKey = "default";
            directoryNode.Tag = detail; // Store the full FTP path in the Tag

            // Add a dummy node to allow the parent node to be expandable
            directoryNode.Nodes.Add("Loading...");
            return directoryNode;
        }

        //private TreeNode CreateDirectoryNode(string path, string name)
        //{
        //    TreeNode directoryNode = new TreeNode(name);
        //    // Set the icons for the normal and selected states
        //    directoryNode.ImageKey = "default";
        //    directoryNode.SelectedImageKey = "default";
        //    directoryNode.Tag = path; // Store the full FTP path in the Tag

        //    // Add a dummy node to allow the parent node to be expandable
        //    directoryNode.Nodes.Add("Loading...");
        //    return directoryNode;
        //}

        // Handle the BeforeExpand event of the TreeView
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode expandingNode = e.Node;

            // Check if the node has the dummy child node
            if (expandingNode.Nodes.Count == 1 && expandingNode.Nodes[0].Text == "Loading...")
            {
                expandingNode.Nodes.Clear(); // Clear the dummy node

                string currentPath = ( expandingNode.Tag as FtpItemDetail).FullPath;
                var items = Controllers.FTPControllerC64.GetDirectoryListing(currentPath, _FtpUser, _FtpPass);

                // Add directories first, then files
                foreach (var item in items.Where(i => i.IsDirectory))
                {
                    //   expandingNode.Nodes.Add(CreateDirectoryNode(item.FullPath, item.Name));
                    expandingNode.Nodes.Add(CreateDirectoryNode(item));
                }
                foreach (var item in items.Where(i => !i.IsDirectory))
                {
                    TreeNode fileNode = new TreeNode(item.Name);
                    fileNode.Tag = item;// item.FullPath;
                    // Optional: assign a different image index for files
                    var key = Path.GetExtension(item.FullPath).TrimStart('.').ToLower();
                    if (imageListIcons.Images.ContainsKey(key))
                    {
                        fileNode.ImageKey = key;
                        fileNode.SelectedImageKey = fileNode.ImageKey;
                    }
                    else
                    {
                        key =nameof(MessageBoxIcon.Question).ToLower();
                        fileNode.ImageKey = key;
                        fileNode.SelectedImageKey = fileNode.ImageKey;
                    }
                    expandingNode.Nodes.Add(fileNode);
                }
            }
        }

        private async void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string url = string.Empty;
            TreeNode clickedNode = e.Node;
            try
            {
                // Perform your desired action with the clicked node
                if (clickedNode != null)
                {
                    var test = e.Node.ImageKey;
                    SetStatusLabel($"You double-clicked: {clickedNode.Text} - {test}");

                    if (clickedNode.Tag != null && !(clickedNode.Tag as FtpItemDetail).IsDirectory)
                    {
                        UltmateEliteClient client = new UltmateEliteClient(_hostname);
                        if (test.Contains("prg", StringComparison.OrdinalIgnoreCase))
                        {
                            url = ApiUrls.LoadProgramOnDevice(_hostname, clickedNode.FullPath.Replace(_FtpRootUrl, string.Empty).Replace("\\", "/"));
                            url = url.Replace($"http://{_hostname}/", string.Empty);
                            client.PutUrl(url);
                            client.sendCommand("RUN");
                            return;
                        }
                        else if (test.Contains("crt", StringComparison.OrdinalIgnoreCase))
                        {
                            url = ApiUrls.RunCartridgeOnDeviceUri(clickedNode.FullPath.Replace(_FtpRootUrl, string.Empty).Replace("\\", "/"));
                            url = url.Replace($"http://{_hostname}/", string.Empty);
                            client.PutUrl(url);
                            client.sendCommand("RUN");
                            return;
                        }
                        else if (test.Contains("sid", StringComparison.OrdinalIgnoreCase) || test.Contains("mod", StringComparison.OrdinalIgnoreCase))
                        {
                            Uri tst = new Uri((clickedNode.Tag as FtpItemDetail).FullPath);
                            url = ApiUrls.SidPlayOnDevice(_hostname, tst.LocalPath, 0);
                            client.PutUrl(url);
                            client.sendCommand("RUN");
                            return;
                        }

                        DiskImageType type = (DiskImageType)Enum.Parse(typeof(DiskImageType), test.ToUpper());
                        url = ApiUrls.MountImageOnDevice(_hostname, "a", clickedNode.FullPath.Replace(_FtpRootUrl, string.Empty).Replace("\\", "/"), type, DiskMode.ReadWrite);
                        url = url.Replace($"http://{_hostname}/", string.Empty);
                        url = $"v1/drives/a:mount?image={HttpUtility.UrlEncode(clickedNode.FullPath.Replace(_FtpRootUrl, string.Empty).Replace("\\", "/"))}";

                        // Example: Open a new form based on the node data
                        // Form2 detailForm = new Form2(clickedNode.Tag);
                        // detailForm.Show();


                        await client.MachineReset();
                        await client.MachineReboot();
                        Thread.Sleep(3000);
                        var jall = await client.PutUrl(url, true);

                    }
                }
            }
            catch (HttpRequestException hex)
            {
                MessageBox.Show(hex.Message, hex.GetType().Name);
                SetStatusLabel($"Mounting failed: {hex.Message}");
            }
            catch (AggregateException aex)
            {
                SetStatusLabel(aex.Message);
            }
            catch (Exception ex)
            {
                SetStatusLabel(ex.Message);
                try
                {
                    Uri tst = new Uri((clickedNode.Tag as FtpItemDetail).FullPath);
                    url = tst.LocalPath;

                    if (!tst.LocalPath.Contains(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        DirectoryInfo tmp = new DirectoryInfo(Path.Combine(Kolibri.net.C64Sorter.Controllers.UltmateEliteClient.ResourcesPath.FullName, "TEMP"));
                        if (!tmp.Exists) tmp.Create();
                        var test = FTPControllerC64.DownloadFileFTP(
                            new UE2LogOn() { Hostname = _hostname, Username = _FtpUser, Password = _FtpPass },
                            tmp,
                            url);
                        FileUtilities.Start(test);
                    }
                }
                catch (Exception ftpex)
                {
                    SetStatusLabel(ftpex.Message);
                }
            }

        }
    }
}