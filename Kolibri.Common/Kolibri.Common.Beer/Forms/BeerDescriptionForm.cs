using iTextSharp.text;
using Kolibri.Common.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Kolibri.Common.Beer.Forms
{
    public partial class BeerDescriptionForm : Form
    {
        private FileInfo _info = null;
        private dynamic _dynamic = null;

        public BeerDescriptionForm(FileInfo info)
        {
            _info = info;
            InitializeComponent();
            Init(_info);
        }

        private void Init(FileInfo info)
        {
            string json = System.IO.File.ReadAllText(info.FullName);
            _dynamic = JsonConvert.DeserializeObject(json);

            List<string> names = new List<string>();

            foreach (dynamic dynItem in _dynamic.Children())
            {
                string name = dynItem.Name;

                if (!string.IsNullOrEmpty(name))
                    names.Add(name);
            }
            names.Sort();

            comboBox1.DataSource = names;
            this.Text += $" - {Path.GetFileNameWithoutExtension(info.Name)}";
            hopsDescriptionsToolStripMenuItem.Enabled = this.Text.Contains("Hops");

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.Visible)
                {
                    GetDynamicObject(comboBox1.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void GetDynamicObject(string text)
        {
            labelStatus.Text = "Status:";
            foreach (dynamic dynItem in _dynamic.Children())
            {
                string name = dynItem.Name;
                
                if (name.Equals(text))
                {
                    labelStatus.Text = name;
                    richTextBox1.Text = $"{dynItem.Description}";
                    if (string.IsNullOrEmpty(richTextBox1.Text)) { labelStatus.Text = "No description found!"; }
                    DataTable dt = DataSetUtilities.DynamicObjectToDataTable(dynItem);

                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;   

                    return;
                }
            }
        }

        private void allWithoutDescriptionToolStripMenuItem_Click(object sender, EventArgs e)
        {List<string> list = new List<string>();
            foreach (dynamic dynItem in _dynamic.Children())
            {
                string name = dynItem.Name;

                string description = $"{dynItem.Description}";

                if (string.IsNullOrEmpty(description))
                {
                    list.Add(name);
                }
            }
            list.Sort();
           
                richTextBox1.Text = string.Join(Environment.NewLine, list);    
            
        }

        private void hopsDescriptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = string.Empty;
            comboBox1.SelectedIndex = -1;    
            try
            {

                {
                    List<string> list = new List<string>();
                    foreach (dynamic dynItem in _dynamic.Children())
                    {
                        string name = dynItem.Name;

                        string description = $"{dynItem.Description}";

                        if (string.IsNullOrEmpty(description))
                        {
                            list.Add(name);
                        }
                    }



                    var info = new FileInfo(Path.Combine(_info.Directory.FullName, "hopsdescription.json"));
                    dynamic dyn = FileUtilities.FileInfoJsonAsDynamicObject(info);

                    var dict = JsonConvert.DeserializeObject<DataTable>(FileUtilities.ReadTextFile(info.FullName));

                    foreach (var name in list)
                    {

                        foreach (DataRow row in dict.Rows)
                        {
                            var dicName = row[0].ToString();
                            if (dicName.ToLower().IndexOf(name.ToLower().Split(' ').FirstOrDefault()) >= 1)
                            {
                                richTextBox1.Text += $"{dicName} resembles {name} -  Description:{Environment.NewLine}{row[1]}{Environment.NewLine}";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);


            }
        }

        private void buttonUpdateDescription_Click(object sender, EventArgs e)
        {
            try
            {
                string description = richTextBox1.Text;
                if(string.IsNullOrEmpty(description)) { return; }
                var dynamic = FindByName(comboBox1.Text);
                dynamic.Description = richTextBox1.Text;
                buttonSave.Enabled = true;
            }
            catch (Exception ex)
            {buttonSave.Enabled = false;
            }
        }
        private dynamic FindByName(string name) {
           
            foreach (dynamic dynItem in _dynamic.Children())
            {
                string dynName = $"{dynItem.Name}";

                if (dynName.Equals(name))
                {
                    return dynItem;
                }
            }
            throw new Exception($"The name {name} was not found in list!");
        }
        private List<string> FindBUrl(string webadress)
        {
            List<string> list = new List<string>(); 
            foreach (dynamic dynItem in _dynamic.Children())
            {
                string url = $"{dynItem.Url}";
                string name = dynItem.Name ;    

                if (url.ToLower().TrimEnd('/').Equals(webadress.ToLower().TrimEnd('/')))
                {
                  list.Add(name);
                }
            }
            return list;
       
        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.InitialDirectory = _info.DirectoryName;
                sfd.FileName = _info.Name;
                if (sfd.ShowDialog() == DialogResult.OK) {
                    Controller.BeerMetaDataController.PrettyWrite(_dynamic, sfd.FileName);
                   FileUtilities.OpenFolderHighlightFile(new FileInfo(sfd.FileName));   
                    buttonSave.Enabled = false;  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void hopsFindAllWeirdosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string noUrl = "https://www.hopslist.com/hops/";
            var liste =FindBUrl(noUrl);
            liste.Sort();

            richTextBox1.Text = string.Join(Environment.NewLine, liste);
        }
    }
}
