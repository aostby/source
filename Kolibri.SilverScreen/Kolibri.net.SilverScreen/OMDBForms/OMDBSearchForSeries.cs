using Kolibri.net.Common.Dal.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Kolibri.net.Common.FormUtilities.Forms;
using OMDbApiNet;
using Kolibri.net.SilverScreen.Controller;
using Kolibri.net.Common.Utilities;

namespace Kolibri.net.SilverScreen.OMDBForms
{
    public partial class OMDBSearchForSeries : Form
    {
        public DirectoryInfo Source { get; private set; }
        private UserSettings _settings;

        private DirectoryInfo[] _directoryInfos;

        public OMDBSearchForSeries(DirectoryInfo source, UserSettings settings)
        {
            _settings = settings;

            InitializeComponent();
            Source = source;
            Init();
        }



        private void Init()
        {
            SetStatusLabelText(Source.FullName);
            _directoryInfos = Source.GetDirectories();
            if (_directoryInfos.Length <= 0)
                _directoryInfos = new List<DirectoryInfo>() { Source }.ToArray();

            SetStatusLabelText($"{Source.FullName} - {_directoryInfos.Length} found.");

        }

        private void SetStatusLabelText(string message)
        {
            try
            {
                if (InvokeRequired)
                    Invoke(new MethodInvoker(
                        delegate { SetStatusLabelText(message); }
                    ));
                else
                {
                    toolStripStatusLabelStatus.Text = message;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            StringBuilder errorLog = new StringBuilder();
            try
            {
                List<DataTable> list = new List<DataTable>();   

                MultiMediaSearchController  contr = new MultiMediaSearchController(_settings);

                foreach (DirectoryInfo directoryInfo in _directoryInfos)
                {
                    if (directoryInfo.Name.Equals("Subs", StringComparison.OrdinalIgnoreCase)) continue;

                    SetStatusLabelText($"{directoryInfo.Name} - Searching for details");
                    Thread.Sleep(10);
                    Application.DoEvents();

                    try
                    {
                        DataTable table = contr.SearchForSeries(directoryInfo);
                        table.TableName = DataSetUtilities.LegalSheetName(directoryInfo.Name);
                        list.Add(table);
                    }
                    catch (Exception ex)
                    { string error = $"{directoryInfo.Name} - {ex.Message}";
                        errorLog.AppendLine($"{error}{Environment.NewLine}{"*".PadRight(72, '*')}");
                        SetStatusLabelText(error);
                        Application.DoEvents();
                        Thread.Sleep(5000);
                    }
                }
                DataTable resultTable = null;
                foreach (DataTable table in list) {
                    if (resultTable == null)
                    {
                        resultTable = table;
                    }
                    else
                    {
                        resultTable.Merge(table);
                    }

                }
                dataGridView1.DataSource = resultTable;
                if (errorLog.Length >= 72)
                {
                    OutputDialogs.ShowRichTextBoxDialog(nameof(errorLog), errorLog.ToString(), this.Size);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
    }
}
