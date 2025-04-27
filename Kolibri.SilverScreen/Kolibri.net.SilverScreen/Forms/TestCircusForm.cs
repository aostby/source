using OMDbApiNet.Model;
using Sylvan.Data.Csv;
using System.Data;
using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Dal.Controller;
using System.Text;


namespace Kolibri.net.SilverScreen.Forms
{
    public partial class TestCircusForm : Form
    {
        private UserSettings _userSettings;

        public TestCircusForm(UserSettings userSettings = null)
        {
            InitializeComponent();
            _userSettings = userSettings;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            int numberOfThreads = Environment.ProcessorCount;
            string filePath = @"C:\Users\asoes\source\repos\Kolibri.SilverScreen\Kolibri.SilverScreen\bin\Debug\net8.0-windows\IMDbDataFiles\title.episode.tsv";

            var options = new CsvDataReaderOptions
            {
                Delimiter = '\t', // Use tab delimiter
                HasHeaders = true // Skip header automatically
            };

            using var reader = CsvDataReader.Create(filePath, options);

            // Process in parallel
            Parallel.ForEach(reader.Cast<IDataRecord>(), record =>
            {
                var episode = new Episode
                {
                    //     Id = record.GetString(0),
                    Title = record.GetString(1),
                    //   Season = record.IsDBNull(2) ? (int?)null : record.GetInt32(2),
                    EpisodeNumber = record.GetString(3) //record.IsDBNull(3) ? (int?)null : record.GetInt32(3)
                };

                // Process each episode
                Console.WriteLine($"Read Episode: {episode.Title}");

            });
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            //var sdk = new PlexAPI(ip: "192.168.1.2",accessToken:_userSettings.XPlexToken);
            //var res = await sdk.Server.GetServerCapabilitiesAsync();




            //   MessageBox.Show(res.ToString());
        }

        private void buttonExecuteChange_Click(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text)) throw new Exception("Cannot change from nothing");

                using (LiteDBController controller = new(new FileInfo(_userSettings.LiteDBFilePath), false, false))
                {
                    var list = controller.FindAllFileItems();
                    foreach (var fItem in list)
                    {
                        if (fItem.FullName.Contains(textBox1.Text, StringComparison.OrdinalIgnoreCase))
                        {
                            fItem.FullName = fItem.FullName.Replace(textBox1.Text, textBox2.Text);
                            controller.Upsert(fItem);
                        }
                        var txt = $"{fItem.ItemFileInfo.Exists} - {fItem.FullName}";
                        builder.AppendLine(txt);
                    }
                }
                Kolibri.net.Common.FormUtilities.Forms.OutputDialogs.ShowRichTextBoxDialog("Report filepaths",builder.ToString(), this.Size);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
    }
}
