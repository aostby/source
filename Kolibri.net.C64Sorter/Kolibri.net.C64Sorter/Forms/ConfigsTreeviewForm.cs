using FastColoredTextBoxNS;
using javax.xml.crypto;
using Kolibri.net.C64Sorter.Controllers;
using Kolibri.net.C64Sorter.Entities;
using Kolibri.net.Common.Images;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Dynamic;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Kolibri.net.C64Sorter.Forms
{
    public partial class ConfigsTreeviewForm : Form
    {
        // Store credentials securely, not in global variables in a real app

        private UE2LogOn _ue2logon = null;
        private UltmateEliteClient _client = null;
        private Dictionary<string, Dictionary<string, string>> CategoriesNodeDic = new Dictionary<string, Dictionary<string, string>>();
        private List<Sections> CategoriesSectiosList = new List<Sections>();

        public ConfigsTreeviewForm(UE2LogOn ue2logon)
        {
            InitializeComponent();
            _ue2logon = ue2logon;
            Init();
        }


        // Initial load (e.g., in Form_Load event)
        private async void Init()
        {
            fastColoredTextBox1.Text = $"Double click on the grid line -  often twice..\r\nInitializing takes time....\r\nRight Click to change a value, if you have initialized the current category.\r\nafter changing a value, double clik again on the category";
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
            try
            {
                toolStripButton1.Image = (Image)Common.Images.Icons.IconFromExtensionShell(".txt", Icons.SystemIconSize.Small).ToBitmap(); // imageListIcons.Images["default"];
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
                //fastColoredTextBox1.Text = result;

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
            Dictionary<string, string> dictionary = null;
            if (e.Node.Name != "Categories")
            {
                try
                {
                    toolStripStatusLabelStatus.Tag = e.Node.Name;
                    if (CategoriesNodeDic.ContainsKey(e.Node.Name))
                    {
                        dictionary = CategoriesNodeDic[e.Node.Name];
                    }
                    else
                    {
                        dictionary = await CreateDictionaryForNode(e.Node.Name);
                    }
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = dictionary.ToDataTable();
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
        }

        private async Task<Dictionary<string, string>> CreateDictionaryForNode(string nodeName)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            var url = ApiUrls.GetConfigCategory(_ue2logon.Hostname, HttpUtility.UrlEncode(nodeName));
            HttpClient client = new HttpClient();
            string jsonString = await client.GetStringAsync(url);

            //    e.Node.Tag = url;

            //// Note:Json convertor needs a json with one node as root
            jsonString = $"{{ \"rootNode\": {{{jsonString.Trim().TrimStart('{').TrimEnd('}')}}} }}";

            var xd = JsonConvert.DeserializeXmlNode(jsonString.Replace("DMA Load Mimics ID:", "DMA Load Mimics ID"));

            //// DataSet is able to read from XML and return a proper DataSet
            var result = new DataSet();
            XmlReadMode mode = XmlReadMode.Auto;
            result.ReadXml(new XmlNodeReader(xd), mode);

            DataRow row = result.Tables[0].Rows[0];

            dictionary = row.Table.Columns
                 .Cast<DataColumn>()
                 .ToDictionary(col => col.ColumnName, col => row[col.ColumnName].ToString());
            CategoriesNodeDic[nodeName] = dictionary;
            return dictionary;
        }
        private async void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {


            var lang = fastColoredTextBox1.Language;
            try
            {
                fastColoredTextBox1.Language = FastColoredTextBoxNS.Language.JSON;
                // Get the clicked row and cell
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                var category = toolStripStatusLabelStatus.Tag.ToString();
                var section = row.Cells["Key"].Value.ToString();
                var currentValue = row.Cells["Value"].Value.ToString();


                //  string text = $"Row Index: {e.RowIndex}, Column Index: {e.ColumnIndex}\nKey: {row.Cells["Key"].Value.ToString()} \nValue: {row.Cells["Value"].Value.ToString()}";
                SetStatusLabel($"{category}: Right click to change [{section}] value (Current: [{currentValue}])");

                var url = ApiUrls.GetConfigCategorySection(_client.ClientName, category, section);
                url = ApiUrls.ToLocalPath(url);
                var json = await _client.Get(url);

                if (!CategoriesSectiosList.Any(p => p.SectionName == section))
                {
                    CategoriesSectiosList.Add(new Sections() { CatagoryName = category, SectionName = section, AllowedValues = GetAllowedValues(section, currentValue, json) });
                }
                fastColoredTextBox1.Text = json.JsonPrettyPrint();



            }
            catch (Exception ex)
            {
                fastColoredTextBox1.Language = lang;

                SetStatusLabel($"Error occured: {ex.GetType().Name} -  {ex.Message.Replace(@"\r", " ")}");
            }
        }

        private List<string> GetAllowedValues(string section, string? currentValue, string jsonString)
        {
            List<string> allStrings = new List<string>();
            List<string> ret = new List<string>() { currentValue };
            {
                try
                {


                    dynamic dynamicObject = JsonConvert.DeserializeObject(jsonString);

                    JObject obj = JObject.Parse(jsonString);
                    var firstProperty = obj.Properties().First();
                    var firstKey = firstProperty.First().First();
                    var nestedValue = obj[firstProperty.Name][section]["values"].ToArray();
                    ret = nestedValue.Select(x => x.ToString()).ToList();
                }
                catch (Exception) { }

            }
            return ret;
        }

        private async void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {

                    DataGridViewRow row = dataGridView1.SelectedRows[0];
                    var key = row.Cells["Key"].Value.ToString();
                    var change = CategoriesSectiosList.Find(x => x.SectionName == key);
                    if (change == null)
                    {
                        string text = $"Change for {key} not possibible, double click to initalize the setting {key} first";
                        SetStatusLabel(text);
                        return;
                    }
                    var currentValue = row.Cells["Value"].Value.ToString();
                    var res = DialogResult.Cancel;

                    string sValue = string.Empty;
                    object oValue = null;

                    if (change.AllowedValues.Count == 1)
                    {
                        sValue = currentValue;
                        res = Kolibri.net.Common.FormUtilities.Forms.InputDialogs.InputBox(change.CatagoryName, $"Set a value for {change.SectionName}", change.AllowedValues.ToArray(), ref sValue);
                    }
                    else
                    {
                        res = Kolibri.net.Common.FormUtilities.Forms.InputDialogs.ChooseListBox(change.CatagoryName, $"Set a value for {change.SectionName}", change.AllowedValues, ref oValue, false);
                    }
                    if (res == DialogResult.OK)
                    {
                        var value = $"{sValue}".Trim();
                        if (string.IsNullOrWhiteSpace(value))
                            value = (oValue as ListViewItem).Text.Trim();

                        var url = ApiUrls.UpdateConfigCategorySectionValue(_client.ClientName, change.CatagoryName, change.SectionName, HttpUtility.UrlEncode(value));
                        ApiUrls.ToLocalPath(url);
                        var happened = await _client.PutUrl(url);
                        if (happened)
                        {
                            string text = $"New value for {change.CatagoryName} - {change.SectionName} set to {value} (was {currentValue})";
                            SetStatusLabel(text);
                            if (CategoriesSectiosList.Any(p => p.SectionName == change.SectionName))
                            {
                                CategoriesSectiosList.Remove(change);
                                CategoriesNodeDic.Remove(change.CatagoryName);
                                fastColoredTextBox1.Language = Language.Custom;
                                fastColoredTextBox1.Text = $"New value for {change.CatagoryName} - {change.SectionName} set to {value} (was {currentValue})";
                                Thread.Sleep(200);
                                var dictionary = await CreateDictionaryForNode(change.CatagoryName);
                                dataGridView1.DataSource = null;
                                dataGridView1.DataSource = dictionary.ToDataTable();
                                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                                dataGridView1.MultiSelect = false;
                                dataGridView1.ReadOnly = true;
                                dataGridView1.Refresh();


                            }
                        }
                        else
                        {
                            throw new Exception($"Could not complete request: New value for {change.CatagoryName} - {change.SectionName} was not carried out. Still {currentValue}. ");
                        }
                    }
                    else
                    {
                        SetStatusLabel($"New value for {change.CatagoryName} - {change.SectionName} was not carried out. Still {currentValue}. Operation dialog: {res})");

                    }
                }
            }
            catch (Exception ex)
            {
                SetStatusLabel($"Error occured: {ex.GetType().Name} -  {ex.Message.Replace(@"\r", " ")} - Exception: {ex.Message}");
            }
        }



        private async void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                var items = await _client.GetConfigs();

                var sysInfo = await _client.GetSystemInformationAsync();
                SetStatusLabel($"Getting current config for {sysInfo.Product} - Host: {sysInfo.Hostname}, Firmware version: {sysInfo.FirmwareVersion}");

                // Add directories first, then files
                foreach (var item in items.categories)
                {
                await    CreateDictionaryForNode(item);
                }
                //CategoriesNodeDic[nodeName] = dictionary;

                var sb = new StringBuilder();
                foreach (var section in CategoriesNodeDic)
                {
                    sb.AppendLine($"[{section.Key}]");

                    foreach (var kvp in section.Value)
                    {
                        sb.AppendLine($"{kvp.Key}={kvp.Value}");
                    }

                    sb.AppendLine(); // blank line between sections
                }
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}-v{sysInfo.FirmwareVersion}.cfg";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    FileInfo info = new FileInfo(sfd.FileName);
                    FileUtilities.WriteStringToFile(sb.ToString(), info.FullName);
                    FileUtilities.OpenFolderHighlightFile(info);
                }
                else SetStatusLabel("Operation save System config cancelled.");
            }
            catch (Exception ex)
            {

                SetStatusLabel($"{ex.GetType().Name} - {ex.Message}");
            }
        }
    }
}
