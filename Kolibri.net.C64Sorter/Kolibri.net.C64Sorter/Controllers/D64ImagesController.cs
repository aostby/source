using Kolibri.net.Common.Utilities;
using System.Text;


namespace Kolibri.net.C64Sorter.Controllers
{
    public class D64ImagesController
    {
        private DirectoryInfo _dirSource;
        private FileInfo _fileSource;

        public D64ImagesController(DirectoryInfo source) { this._dirSource = source; }

        public D64ImagesController(FileInfo source) { this._fileSource = source; } 


        public FileInfo GetSearchResults(string searchString) {
        
            string rootFolder = _dirSource.FullName;
            string outputCsv = Path.Combine(_dirSource.FullName,$"{searchString}_{FileUtilities.SafeFileName(DateTime.Now.ToString("d"))}.csv");

            var rows = new List<string>();

            // CSV Header
            rows.Add("DiskPath,Name,DiskName,Filename,FileType,Size,Content");

            foreach (string d64Path in Directory.EnumerateFiles(rootFolder, "*.d64", SearchOption.AllDirectories))
            {
                try
                {

                    var disk = new D64Reader.D64ReaderCore(File.ReadAllBytes(d64Path));

                    foreach (var entry in disk.Directory.DirectoryItems)
                    {
                        if (entry.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        {
                            string csvLine = string.Join(",",
                                EscapeCsv(d64Path),
                                EscapeCsv(Path.GetFileName(d64Path)),
                               EscapeCsv(disk.Directory.DiskName),
                               EscapeCsv(entry.Name),
                                entry.Type.ToString(),
                                entry.Blocks.ToString(),
                                disk.Directory.DirectoryItems.Count().ToString() + " items"
                             );

                            rows.Add(csvLine);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading {d64Path}: {ex.Message}");
                }
            }

            File.WriteAllLines(outputCsv, rows, Encoding.UTF8);
            Console.WriteLine($"CSV export complete: {outputCsv}");
            return new FileInfo(outputCsv);
        }


        public FileInfo GetDiskInfos()
        {

            string rootFolder = _dirSource.FullName;
            string outputCsv = Path.Combine(_dirSource.FullName, _dirSource.Name + ".csv");

            var rows = new List<string>();

            // CSV Header
            rows.Add("DiskPath,DiskName,Filename,FileType,Size,Content");

            foreach (string d64Path in Directory.EnumerateFiles(rootFolder, "*.d64", SearchOption.AllDirectories))
            {
                try
                {

                    var disk = new D64Reader.D64ReaderCore(File.ReadAllBytes(d64Path));

                    foreach (var entry in disk.Directory.DirectoryItems)
                    {
                        string csvLine = string.Join(",",
                            EscapeCsv(d64Path),
                           EscapeCsv(disk.Directory.DiskName),
                           EscapeCsv(entry.Name),
                            entry.Type.ToString(),
                            entry.Blocks.ToString(),
                            disk.Directory.DirectoryItems.Count().ToString() + " items"
                         );

                        rows.Add(csvLine);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading {d64Path}: {ex.Message}");
                }
            }

            File.WriteAllLines(outputCsv, rows, Encoding.UTF8);
            Console.WriteLine($"CSV export complete: {outputCsv}");
            return new FileInfo(outputCsv);
        }

        // --- Helpers -------------------------------------------------------------

        static string EscapeCsv(string field)
        {
            if (field == null) return "";
            field = field.Replace("\"", "\"\"");
            return $"\"{field}\"";
        }

        

        static string PetsciiToAscii(byte[] data)
        {
            // Very basic PETSCII → ASCII converter (works for most typical text)
            return string.Concat(data.Select(b =>
            {
                // Uppercase PETSCII to lowercase ASCII
                if (b >= 0x41 && b <= 0x5A) return ((char)(b + 32)).ToString();
                // Lowercase PETSCII to uppercase ASCII
                if (b >= 0x61 && b <= 0x7A) return ((char)(b - 32)).ToString();

                // Map control chars to space
                if (b < 32) return " ";

                return ((char)b).ToString();
            }));
        }
    }
 

}

