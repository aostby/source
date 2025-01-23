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
using static Kolibri.net.SilverScreen.Controls.Constants;
using Kolibri.net.Common.Dal.Controller;
using static com.sun.tools.javac.util.Name;
using OMDbApiNet.Model;

namespace Kolibri.net.SilverScreen.OMDBForms
{
    public partial class OMDBSearchForSeriesForm : Form
    {
        

        public DirectoryInfo Source { get; private set; }
        private UserSettings _settings;
        private LiteDBController _liteDB;
        private MultiMediaSearchController _contr;

        //private DirectoryInfo[] _directoryInfos;

        public OMDBSearchForSeriesForm(DirectoryInfo source, UserSettings settings)
        {
            _settings = settings;

            InitializeComponent();
            Source = source;
            Init();
        }



        private void Init()
        {
            this.Text = $"{this.Text} - {Source.FullName}";
            SetStatusLabelText(Source.FullName);
            //_directoryInfos = Source.GetDirectories();
            //if (_directoryInfos.Length <= 0)
            //    _directoryInfos = new List<DirectoryInfo>() { Source }.ToArray();

            SetStatusLabelText($"{Source.FullName} - {Source.GetDirectories().Length} subfolders found.");

            _liteDB = new LiteDBController(new FileInfo(_settings.LiteDBFilePath));

            try
            {
                var path = Kolibri.net.Common.Utilities.FileUtilities.GetFiles(Source, MovieUtilites.MoviesCommonFileExt(true), SearchOption.AllDirectories).FirstOrDefault();
                string title = $"{MovieUtilites.GetMovieTitle(path)}";
             string year =    $"{MovieUtilites.GetYear(Source.Name)}".Trim();
                textBoxManual.Text = $"{title}".Trim();
            }
            catch (Exception)
            {
                textBoxManual.Text = "Tulsa King";
            }
           
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

        private void OnProgressUpdated(object sender, string progress)
        {
            try
            {
                SetStatusLabelText(progress);
            }
            catch (Exception)
            { }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {this.Cursor = Cursors.WaitCursor;  
            _contr = new MultiMediaSearchController(_settings);
            _contr.ProgressUpdated += OnProgressUpdated;

            StringBuilder Log = new StringBuilder();
            try
            {
                List<DataTable> list = new List<DataTable>();

                SetStatusLabelText($"{Source.Name} - Searching for details (Start: {DateTime.Now.ToShortTimeString()})");

                try
                {
                    DataTable table = null; List<Episode> epList = new List<Episode>();
                    if (radioButtonLocal.Checked)
                    {
                        var test = _contr.SearchForSeriesAsync(Source);
                        epList = test.Result.SelectMany(x => x.EpisodeList).OrderBy(x => x.Title).ToList();
                        table = DataSetUtilities.AutoGenererDataSet<Episode>(epList).Tables[0];

                    }
                    else { table = _contr.SearchForSeriesEpisodes(Source); }
                    table.TableName = DataSetUtilities.LegalSheetName(Source.Name);
                    list.Add(table);
                }
                catch (Exception ex)
                {
                    string error = $"{Source.Name} - {ex.Message}";
                    Log.AppendLine($"{error}{Environment.NewLine}{"*".PadRight(72, '*')}");
                    SetStatusLabelText(error);
                    
                    Application.DoEvents();
                    Thread.Sleep(20);
                }

                DataTable resultTable = null;
                foreach (DataTable table in list)
                {
                    if (resultTable == null)
                    {
                        resultTable = table;
                    }
                    else
                    {
                        resultTable.Merge(table);
                    }

                }
                //ta bort duplikate rader
                var tmp = new DataView(resultTable, null, $"{resultTable.Columns[0].ColumnName} ASC, SeasonNumber ASC, EpisodeNumber ASC", DataViewRowState.CurrentRows).ToTable(true, DataSetUtilities.ColumnNames(resultTable));

                resultTable = new DataView(tmp).ToTable(true, DataSetUtilities.ColumnNames(resultTable));
                dataGridView1.DataSource = resultTable;

                //TODO - finn feilen med dette
                //Kolibri.net.SilverScreen.Controls.DataGrivViewControls dgwController = new Controls.DataGrivViewControls(MultimediaType.Series, _liteDB);
                //dataGridView1 = new DataGridView();
                //dataGridView1 = dgwController.GetSeasonEpisodeDataGridView(resultTable);
                Log.AppendLine(_contr.CurrentLog.ToString());  
                if (Log.Length >= 72)
                {
                    OutputDialogs.ShowRichTextBoxDialog($"{nameof(Log)} - {DateTime.Now.ToShortTimeString()}", Log.ToString(), this.Size);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
            this.Cursor = Cursors.Default;
        }
       
        private void buttonManual_Click(object sender, EventArgs e)
        {

            try
            {
                IMDBForms.MovieForm form = new IMDBForms.MovieForm(_settings, new FileInfo(textBoxManual.Text));
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
    }
}
