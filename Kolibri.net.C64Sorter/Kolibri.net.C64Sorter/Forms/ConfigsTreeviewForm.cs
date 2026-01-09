

using com.sun.org.apache.bcel.@internal.generic;
using com.sun.rowset.@internal;
using com.sun.security.ntlm;
using com.sun.tools.@internal.ws.wsdl.document.http;
using Kolibri.net.C64Sorter.Controllers;
using Kolibri.net.C64Sorter.Entities;
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Kolibri.net.C64Sorter.Forms
{
    public partial class ConfigsTreeviewForm : Form
    {
        // Store credentials securely, not in global variables in a real app

        private UE2LogOn _ue2logon = null;
        private UltmateEliteClient _client = null;

        public ConfigsTreeviewForm(UE2LogOn ue2logon)
        {
            InitializeComponent();
            _ue2logon = ue2logon;
            Init();
        }


        // Initial load (e.g., in Form_Load event)
        private async void Init()
        {
            treeView1.Nodes.Clear();
            _client = new UltmateEliteClient(_ue2logon.Hostname);

            // TreeNode rootNode = CreateDirectoryNode(_FtpRootUrl, _FtpRootUrl);
            TreeNode rootNode = CreateCategoriesNode("Categories");
            treeView1.Nodes.Add(rootNode);
            treeView1.ImageList = imageListIcons;
            this.treeView1.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.Text = $"Host:{(await _client.GetSystemInformationAsync()).Hostname} User: {Environment.UserName}";
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
            try
            {

                string result = Regex.Replace(statusText, @"\r\n?|\n", Environment.NewLine);
                result = result.Replace(Environment.NewLine, " ");
                toolStripStatusLabelStatus.Text = result;
            }
            catch (Exception) { }
        }


        // Function to create a node and add a placeholder child for dynamic loading
        private TreeNode CreateCategoriesNode(string categories)
        {
            TreeNode directoryNode = new TreeNode(categories);
            directoryNode.Name = categories;
            // Set the icons for the normal and selected states
            directoryNode.ImageKey = "default";
            directoryNode.SelectedImageKey = "default";
            directoryNode.Tag = categories; // Store the full FTP path in the Tag

            // Add a dummy node to allow the parent node to be expandable
            if (categories.Equals("Categories"))
                directoryNode.Nodes.Add("Loading...");
            return directoryNode;
        }

        private async void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode expandingNode = e.Node;

            // Check if the node has the dummy child node
            if (expandingNode.Nodes.Count == 1 && expandingNode.Nodes[0].Text == "Loading..." && expandingNode.Name.Equals("Categories"))
            {
                expandingNode.Nodes.Clear(); // Clear the dummy node

                //    string currentPath = (expandingNode.Tag as FtpItemDetail).FullPath;
                var items = await _client.GetConfigs();

                // Add directories first, then files
                foreach (var item in items.categories)
                {
                    //   expandingNode.Nodes.Add(CreateDirectoryNode(item.FullPath, item.Name));
                    expandingNode.Nodes.Add(CreateCategoriesNode(item));
                }
            }

        }

        private async void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            SetStatusLabel($"{e.Node.Name}: Showing values");

            if (e.Node.Name != "Categories")
            {
                try
                {
                    var url = ApiUrls.GetConfigCategory(_ue2logon.Hostname, HttpUtility.UrlEncode(e.Node.Name));
                    HttpClient client = new HttpClient();
                    string jsonString = await client.GetStringAsync(url);

                    e.Node.Tag = url;

                    //// Note:Json convertor needs a json with one node as root
                    jsonString = $"{{ \"rootNode\": {{{jsonString.Trim().TrimStart('{').TrimEnd('}')}}} }}";

                    var xd = JsonConvert.DeserializeXmlNode(jsonString.Replace("DMA Load Mimics ID:", "DMA Load Mimics ID"));

                    //// DataSet is able to read from XML and return a proper DataSet
                    var result = new DataSet();
                    XmlReadMode mode = XmlReadMode.Auto;
                    result.ReadXml(new XmlNodeReader(xd), mode);

                    DataRow row = result.Tables[0].Rows[0];

                    Dictionary<string, object> dictionary = row.Table.Columns
                        .Cast<DataColumn>()
                        .ToDictionary(col => col.ColumnName, col => row[col.ColumnName]);


                    // Example using a DataTable
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = dictionary.ToDataTable();
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.MultiSelect = false;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.Refresh();
                
                }
                catch (Exception ex)
                {
                    dataGridView1.DataSource = null;
                    SetStatusLabel($"{e.Node.Name}: {ex.Message}");
                }

            }


            //    string url = string.Empty;
            //    TreeNode clickedNode = e.Node;
            //    try
            //    {
            //        // Perform your desired action with the clicked node
            //        if (clickedNode != null)
            //        {
            //            var test = e.Node.ImageKey;
            //            SetStatusLabel($"You double-clicked: {clickedNode.Text} - {test}");

            //            if (clickedNode.Tag != null && !(clickedNode.Tag as FtpItemDetail).IsDirectory)
            //            {
            //                UltmateEliteClient client = new UltmateEliteClient(_hostname);
            //                if (test.Contains("prg", StringComparison.OrdinalIgnoreCase))
            //                {
            //                    Uri tst = new Uri((clickedNode.Tag as FtpItemDetail).FullPath);
            //                    url = ApiUrls.LoadProgramOnDevice(_hostname, tst.LocalPath);
            //                    url = url.Replace($"http://{_hostname}/", string.Empty);
            //                    client.PutUrl(url);
            //                    client.SendCommand("RUN");
            //                    return;
            //                }
            //                else if (test.Contains("crt", StringComparison.OrdinalIgnoreCase))
            //                {
            //                    Uri tst = new Uri((clickedNode.Tag as FtpItemDetail).FullPath);
            //                    url = ApiUrls.RunCartridgeOnDeviceUri(tst.LocalPath);
            //                    url = url.Replace($"http://{_hostname}/", string.Empty);
            //                    client.PutUrl(url);
            //                    client.SendCommand("RUN");
            //                    return;
            //                }
            //                else if (test.Contains("sid", StringComparison.OrdinalIgnoreCase) || test.Contains("mod", StringComparison.OrdinalIgnoreCase))
            //                {
            //                    Uri tst = new Uri((clickedNode.Tag as FtpItemDetail).FullPath);
            //                    url = ApiUrls.SidPlayOnDevice(_hostname, tst.LocalPath, 0);
            //                    client.PutUrl(url);
            //                    client.SendCommand("RUN");
            //                    return;
            //                }

            //                DiskImageType type = (DiskImageType)Enum.Parse(typeof(DiskImageType), test.ToUpper());
            //                url = ApiUrls.MountImageOnDevice(_hostname, "a", clickedNode.FullPath.Replace(_FtpRootUrl, string.Empty).Replace("\\", "/"), type, DiskMode.ReadWrite);
            //                url = url.Replace($"http://{_hostname}/", string.Empty);
            //                url = $"v1/drives/a:mount?image={HttpUtility.UrlEncode(clickedNode.FullPath.Replace(_FtpRootUrl, string.Empty).Replace("\\", "/"))}";

            //                await client.MachineReset();
            //                await client.MachineReboot();
            //                Thread.Sleep(3000);
            //                var jall = await client.PutUrl(url, true);

            //            }
            //        }
            //    }
            //    catch (HttpRequestException hex)
            //    {
            //        MessageBox.Show(hex.Message, hex.GetType().Name);
            //        SetStatusLabel($"Mounting failed: {hex.Message}");
            //    }
            //    catch (AggregateException aex)
            //    {
            //        SetStatusLabel(aex.Message);
            //    }
            //    catch (Exception ex)
            //    {
            //        SetStatusLabel(ex.Message);
            //        try
            //        {
            //            Uri tst = new Uri((clickedNode.Tag as FtpItemDetail).FullPath);
            //            url = tst.LocalPath;

            //            if (!tst.LocalPath.Contains(".exe", StringComparison.OrdinalIgnoreCase))
            //            {
            //                DirectoryInfo tmp = new DirectoryInfo(Path.Combine(Kolibri.net.C64Sorter.Controllers.UltmateEliteClient.ResourcesPath.FullName, "TEMP"));
            //                if (!tmp.Exists) tmp.Create();
            //                var test = FTPControllerC64.DownloadFileFTP(
            //                    new UE2LogOn() { Hostname = _hostname, Username = _FtpUser, Password = _FtpPass },
            //                    tmp,
            //                    url);
            //                FileUtilities.Start(test);
            //            }
            //        }
            //        catch (Exception ftpex)
            //        {
            //            SetStatusLabel(ftpex.Message);
            //        }
            //    }

        }

      

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {


                // Get the clicked row and cell
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                string text = $"Row Index: {e.RowIndex}, Column Index: {e.ColumnIndex}\nKey: {row.Cells["Key"].Value.ToString()} \nValue: {row.Cells["Value"].Value.ToString()}";
                // Example: Display the row index and cell value
                //MessageBox.Show(text);


                throw new NotImplementedException(text);

            }
            catch (Exception ex)
            {
                
                    SetStatusLabel($"Error occured: {ex.GetType().Name} -  {ex.Message.Replace(@"\r", " ")}");
            }
        }
    }
}
