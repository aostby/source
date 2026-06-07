using DapperGenericRepository.Controller;
using Kolibri.net.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Kolibri.Desktop.Forms
{
    public partial class MainForm : BaseForm
    {
        public MainForm()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {       // Add extra menu items to the inherited MenuStrip
            var helpMenu = new ToolStripMenuItem("Database");
            var aboutItem = new ToolStripMenuItem("MysqlTables", null, MysqlTables_Click);

            helpMenu.DropDownItems.Add(aboutItem);
            this.MainMenuStrip.Items.Add(helpMenu);



        }
        private void MysqlTables_Click(object sender, EventArgs e)
        {
                    var dlg1 = FolderUtilities.LetOppMappe((sender as ToolStripMenuItem).Text, $"Select a folder for your operation ({(sender as ToolStripMenuItem).Text}");
            if (dlg1 != null)
            {
                string dbconnectionstring = "Server=localhost;Database=rapt;User Id = asoes; Password=OU812;";
                Kolibri.net.Common.Dal.Controller.MySQLController contr = new(dbconnectionstring);
                var test = contr.GetTableMySQL(dbconnectionstring);
                var jall = new ConvertDBTableToDapperClassesController(dbconnectionstring);
                foreach (var item in jall.GetTableNames())
                {
                    var jasså = jall.GetClassForTable(item);
                    var nah = DataSetUtilities.DataTableToEntityClass(jasså);
                    File.WriteAllText(Path.Combine(dlg1.FullName, item+".cs"), nah);
                }
                FileUtilities.Start(dlg1);
            }
        }
    }
}
