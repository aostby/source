using DapperGenericRepository.Controller;
using FastColoredTextBoxNS;
using Kolibri.Desktop.RAPT.Model;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Kolibri.Desktop.RAPT.Forms
{
    public partial class RAPTForm : Form
    {
        private string _dbconnectionstring;
        private ConvertDBTableToDapperClassesController _dappercontr;

        public RAPTForm(string dbconnectionstring)
        {
            InitializeComponent();
            _dbconnectionstring = dbconnectionstring;
            Init();
        }

        private void Init(object? sender=null, EventArgs e=null)
        {
            _dappercontr = new ConvertDBTableToDapperClassesController(_dbconnectionstring);
            var list = this.Controls.OfType<GroupBox>().ToList();

            //Nullstill alle gruppebokser
            foreach (var item in list)
            {
                foreach (var child in item.Controls.OfType<FlowLayoutPanel>().ToList())
                {
                    child.Controls.Clear();
                    child.Enabled = true;
                }
            }

            try
            {
                foreach (string tableName in _dappercontr.GetTableNames().Where(s => s.Contains("rapt", StringComparison.OrdinalIgnoreCase)))
                {
                    Button button = new Button();
                    button.Text = tableName;
                    button.Name = $"button{button.Text}";
                    button.Click += buttonShowTableContent_Click;
                    //  SetToolTipForButton(button);
                    button.Width = flowLayoutPanelTables.Width - 10;
                    flowLayoutPanelTables.Controls.Add(button);
                }
                Button clear = new Button();
                clear.Text = "Clear";
                clear.Name = $"button{clear.Text}";
                clear.Click += Init;
                //  SetToolTipForButton(button);
                clear.Width = flowLayoutPanelTables.Width - 10;
                flowLayoutPanelTables.Controls.Add(clear);
            }
            catch (Exception ex)
            {
            }
        }

        private void buttonShowTableContent_Click(object? sender, EventArgs e)
        {
            string name = (sender as Button).Text;
            Button tableButton = new Button();
            if (!flowLayoutPanelDetails.Controls.ContainsKey($"button_{name.ToLower()}"))
            {

                tableButton.Text = $"Show {name}";
                tableButton.Name = $"button_{name.ToLower()}";
                tableButton.Click += button_Details_Click;
                tableButton.Width = flowLayoutPanelDetails.Width - 10;
                flowLayoutPanelDetails.Controls.Add(tableButton);
            }

        }

        private async void button_Details_Click(object? sender, EventArgs e)
        {
            DataSet ds = null;

            string name = (sender as Button).Name.ToLower().Replace("button_", string.Empty);
            switch (name)
            {
                case "rapt_hydrometer":
                    var rhydro =   _dappercontr.GetDataToClassFromTable<RAPT.Model.RaptHydrometer>(name);
                    rhydro = rhydro.OrderByDescending(x => x.CreatedOn);
                    ds = DataSetUtilities.AutoGenererDataSet<RAPT.Model.RaptHydrometer>(rhydro.ToList());
                    CreateHydrometerGroup(rhydro);
                    break;

                case "rapt_profiles":
              var       rprof =   _dappercontr.GetDataToClassFromTable<RAPT.Model.RaptProfiles>(name);
                    rprof = rprof.OrderByDescending(x=> x.CreatedOn);
                    ds = DataSetUtilities.AutoGenererDataSet<RAPT.Model.RaptProfiles>(rprof.ToList());
                    break;

                case "rapt_posts":
                    var rposts =   _dappercontr.GetDataToClassFromTable<RAPT.Model.RaptPosts>(name);
                    rposts = rposts.OrderByDescending(x => x.CreatedOn);
                    ds = DataSetUtilities.AutoGenererDataSet<RAPT.Model.RaptPosts>(rposts.ToList());
                    break;
                case "rapt_token":
                    var rtoken =   _dappercontr.GetDataToClassFromTable<RAPT.Model.RaptToken>(name);
                    rtoken = rtoken.OrderByDescending(x => x.CreatedOn);
                    ds = DataSetUtilities.AutoGenererDataSet<RAPT.Model.RaptToken>(rtoken.ToList());
                    break;
                default:
                    break;
            }
            if (ds == null && name.StartsWith("hydrodetails_"))
            {
                name = name.Split("_").LastOrDefault();
                var thydro = "rapt_hydrometer";
                var tprof = "rapt_profiles";


                var rhydro = _dappercontr.GetDataToClassFromTable<RAPT.Model.RaptHydrometer>(thydro).Where(x => x.Name.Equals(name.ToLower(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                var rprof = _dappercontr.GetDataToClassFromTable<RAPT.Model.RaptProfiles>(tprof).Where(x => x.Id.Equals(rhydro.Id.ToLower(), StringComparison.OrdinalIgnoreCase));
                ds = DataSetUtilities.AutoGenererDataSet<RAPT.Model.RaptProfiles>(rprof.ToList());  
            }

            if (ds != null)
            {

                ds.DataSetName = name;
                fastColoredTextBoxResult.Clear();
                fastColoredTextBoxResult.Language = FastColoredTextBoxNS.Language.XML;
                fastColoredTextBoxResult.Text = ds.ToXDocument().ToString();
            }

        }

        private void CreateHydrometerGroup<T>(IEnumerable<T> item)
        {
            try
            {
                string name = $"groupBox_{item.GetType().Name}";

                GroupBox box = new GroupBox() { Name = name, Text = name };
                if (!flowLayoutPanelDetails.Controls.ContainsKey(box.Name))
                {FlowLayoutPanel boxPanel = new FlowLayoutPanel();  
                 
                    foreach (IRaptInterface en in item)
                    { 
                        Button tableButton = new Button(); 

                        tableButton.Text = $"Details {en.Name}";
                        tableButton.Name = $"button_HydroDetails_{en.Name.ToLower()}";
                        tableButton.Click += button_Details_Click;
                        tableButton.Width = flowLayoutPanelDetails.Width - 10;
                        boxPanel.Controls.Add(tableButton);
                    }
                    boxPanel.Height = item.Count()*36;
                    box.Height= boxPanel.Height + 1;   
                    box.Controls.Add(boxPanel);
                    box.BackColor = Color.LightYellow;
                    flowLayoutPanelDetails.Controls.Add(box);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}