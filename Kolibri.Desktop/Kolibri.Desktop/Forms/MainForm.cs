using DapperGenericRepository.Controller;
using Kolibri.net.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Kolibri.Desktop.Forms
{
    public partial class MainForm : BaseForm
    {
        string _dbconnectionstring = string.Empty;

        public MainForm()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            _dbconnectionstring = "Server=localhost;Database=rapt;User Id = asoes; Password=OU812;"; //TODO: Create a way to make this configurable

            // Add extra menu items to the inherited MenuStrip
            var databaseMenu = new ToolStripMenuItem("Database");
            var showRapt = new ToolStripMenuItem("Show RAPT Tables", null, ShowRaptTables);
            databaseMenu.DropDownItems.Add(new ToolStripSeparator());
            var mysqlItem = new ToolStripMenuItem("Create classes for MysqlTables", null, MysqlTables_Click);
            
            databaseMenu.DropDownItems.Add(showRapt);
            databaseMenu.DropDownItems.Add(mysqlItem);

            databaseMenu.MergeIndex = 2;
            this.MainMenuStrip.Items.Insert(databaseMenu.MergeIndex, databaseMenu);  

        }

        private void ShowRaptTables(object? sender, EventArgs e)
        {
            try
            {

                this.SetNewForm(new RAPT.Forms.RAPTForm(_dbconnectionstring));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().FullName);
            }
        }

        private void MysqlTables_Click(object sender, EventArgs e)
        {
            var dlg1 = FolderUtilities.LetOppMappe((sender as ToolStripMenuItem).Text, $"Select a folder for your operation ({(sender as ToolStripMenuItem).Text}");
            if (dlg1 != null)
            {
             
                Kolibri.net.Common.Dal.Controller.MySQLController contr = new(_dbconnectionstring);
                var test = contr.GetData( contr.GetMySQLTables( ));
                var dapperContr = new ConvertDBTableToDapperClassesController(_dbconnectionstring);
                foreach (var item in dapperContr.GetTableNames())
                {
                    var jasså = dapperContr.GetDDLClassForTable(item);
                    // var nah = DataSetUtilities.DataTableToEntityClass(jasså);
                    // File.WriteAllText(Path.Combine(dlg1.FullName, item + ".cs"), nah);
                    dapperContr.GetClassForTable(item, dlg1, Assembly.GetExecutingAssembly().GetName().Name);
                }
                FileUtilities.Start(dlg1);
            }
        }
    }
}
