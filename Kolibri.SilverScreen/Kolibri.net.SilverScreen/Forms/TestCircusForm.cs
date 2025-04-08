using OMDbApiNet.Model;
using Sylvan.Data.Csv;
using System.Data;
using System.Text.RegularExpressions;

namespace Kolibri.net.SilverScreen.Forms
{
    public partial class TestCircusForm : Form
    {
  
        public TestCircusForm()
        {
            InitializeComponent();
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
    }
}
