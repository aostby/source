using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Kolibri.net.Common.Utilities.Extensions;
namespace Kolibri.net.Common.Utilities.Controller
{
    
 
    // Class to hold a pair of duplicate files with a built-in deletion method
    public class DuplicatePair
    {
        public FileInfo FileA { get; }
        public FileInfo FileB { get; }


        public bool SameDir { get {
                var ret = false;
                return FileA.DirectoryName.Equals(FileB.DirectoryName);
            } }

        public DuplicatePair(FileInfo fileA, FileInfo fileB)
        {
            FileA = fileA;
            FileB = fileB;
        }


        public bool DeleteFileA()
        {
            try
            {
                if (FileA.Exists)
                {
                    FileA.Delete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting file: {ex.Message}");
            }
            return false;
        }



        // Deletes FileB safely and returns true if successful
        public bool DeleteFileB()
        {
            try
            {
                if (FileB.Exists)
                {
                    FileB.Delete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting file: {ex.Message}");
            }
            return false;
        }
    }

    public class ImageDuplicateFinder
    {
        // Allowed image extensions
        private readonly HashSet<string> _imageExtensions = ImageUtilities.PictureFileExt(true).ToArray().ToHashSet<string>(StringComparer.OrdinalIgnoreCase);
       

        public async Task<List<DuplicatePair>> FindDuplicatesAsync(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"The directory {folderPath} does not exist.");

            return await Task.Run(() =>
            {
                var directoryInfo = new DirectoryInfo(folderPath);

                // 1. Gather all valid image files
                var allFiles = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories)
                    .Where(f => _imageExtensions.Contains(f.Extension))
                    .ToList();
                //filter
                allFiles = allFiles.Where(cdr => !cdr.Directory.Name?.Contains("@__thumb") == true).ToList();

                // 2. File Size Check: Group files by their size
                var sizeGroups = allFiles.GroupBy(f => f.Length)
                                         .Where(g => g.Count() > 1);

                var duplicates = new List<DuplicatePair>();
              

                // 3. Hashing: Only hash files that share the exact same size
                foreach (var group in sizeGroups)
                {
                    var hashGroups = new Dictionary<string, List<FileInfo>>();

                    foreach (var file in group)
                    {
                        string hash = FileExt.GetFileHash(file.FullName);

                        if (!hashGroups.ContainsKey(hash))
                        {
                            hashGroups[hash] = new List<FileInfo>();
                        }
                        hashGroups[hash].Add(file);
                    }

                    // 4. Generate pairs from identical hashes
                    foreach (var hashGroup in hashGroups.Values)
                    {
                        if (hashGroup.Count > 1)
                        {
                            // Pair the first file with every subsequent duplicate found
                            for (int i = 1; i < hashGroup.Count; i++)
                            {
                                duplicates.Add(new DuplicatePair(hashGroup[0], hashGroup[i]));
                            }
                        }
                    }
                }

                return duplicates;
            });
        }

        //private static string ComputeHash(string filePath, HashAlgorithm algorithm)
        //{
        //    using var stream = File.OpenRead(filePath);
        //    byte[] hashBytes = algorithm.ComputeHash(stream);
        //    return Convert.ToHexString(hashBytes); // Fast hex string conversion in modern .NET
        //}
    }
}
