using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Kolibri.net.Common.Utilities
{
    public class AmigaUtilities
    {
        static readonly Regex DiskRegex = new Regex(@"^(?<title>.+?)\s*\(Disk\s*(?<disk>\d+)\s*of\s*(?<total>\d+)\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex VariantRegex = new Regex(@"\[(.*?)\]", RegexOptions.Compiled);

        public static List<string> AmigaCommonFileExt(bool withPunctuation = false)
        {
            var ret = new List<string>() { "ROM", "ZIP","ADF","8SVX","SVX",
                                                "ARC","ARJ","DMS","EXE","GUIDE","IFF","ILBM",
                                                "LHA","LZH","LZW","LZX","PIT","TXT","ZOO"  };

            if (withPunctuation)
                ret = ret.Select(r => string.Concat('.', r)).ToList();

            return ret.Distinct().ToList();
        }
        public static string ExtractVariant(string fileName)
        {
            var matches = VariantRegex.Matches(fileName);

            foreach (Match m in matches)
            {
                var value = m.Groups[1].Value;

                // Prefer "cr XYZ"
                if (value.StartsWith("cr ", StringComparison.OrdinalIgnoreCase))
                    return value.ToLower();
            }

            // fallback: first tag if no "cr"
            if (matches.Count > 0)
                return matches[0].Groups[1].Value.ToLower();

            return "unknown";
        }

        private static string CleanTitle(string title)
        {
            // Normalize whitespace
            title = Regex.Replace(title, @"\s+", " ");

            // Optional: remove trailing spaces and weird chars
            title = title.Trim();

            return title;
        }
        public static AamigaFile ParseAdf(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);

            // Remove trailing GUID garbage (_xxxxx)
            fileName = Regex.Replace(fileName, @"_[a-f0-9\-]{8,}$", "", RegexOptions.IgnoreCase);

            // Extract variant (cr group)
            var variant = ExtractVariant(fileName);

            // Remove ALL [tags] to get clean title
            var title = Regex.Replace(fileName, @"\[[^\]]+\]", "").Trim();

            return new AamigaFile
            {
                FilePath = filePath,
                Title = CleanTitle(title),
                Disk = 1,
                TotalDisks = 1,
                Variant = variant
            };
        }

        public static AamigaFile ParseAdf_old(string filePath)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var match = DiskRegex.Match(fileName);

            if (!match.Success)
                return null;

            return new AamigaFile
            {
                FilePath = filePath,
                Title = CleanTitle(match.Groups["title"].Value),
                Disk = int.Parse(match.Groups["disk"].Value),
                TotalDisks = int.Parse(match.Groups["total"].Value),
                Variant = ExtractVariant(fileName)
            };
        }
        private static void ExtractZipRecursive(string zipPath, string extractRoot)
        {
            string tempExtractPath = Path.Combine(
                extractRoot,
                Path.GetFileNameWithoutExtension(zipPath));

            Directory.CreateDirectory(tempExtractPath);

            System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, tempExtractPath, true);

            // Find nested zips
            var nestedZips = Directory.GetFiles(tempExtractPath, "*.zip", SearchOption.AllDirectories);

            foreach (var nested in nestedZips)
            {
                ExtractZipRecursive(nested, Path.GetDirectoryName(nested));
            }
        }
        private static List<string> GetAllAdfFiles(string rootFolder)
        {
            return Directory
                .GetFiles(rootFolder, "*.adf", SearchOption.AllDirectories)
                .ToList();
        }
        private static List<IGrouping<(string Title, string Variant), AamigaFile>> GroupFiles_new(List<AamigaFile> files)
        {
            return files
                .Where(f => f != null)
                .GroupBy(f => (f.Title, f.Variant))
                .ToList();
        }


        private static List<IGrouping<(string Title, int Total, string Variant), AamigaFile>> GroupFiles(List<AamigaFile> files)
        {
            return files
                .Where(f => f != null)
                .GroupBy(f => (f.Title, f.TotalDisks, f.Variant))
                .ToList();
        }
        public static void OrganizeGroups(IEnumerable<IGrouping<(string Title, int Total, string Variant), AamigaFile>> groups, string outputRoot)
        {
            foreach (var group in groups)
            {
                //string folderName = $"{group.Key.Title} [{group.Key.Variant}]";
                string folderName = $"{group.Key.Title} [{group.Key.Variant}]";
                folderName = MakeSafeFolderName(folderName);

                string folderPath = Path.Combine(outputRoot, folderName);
                Directory.CreateDirectory(folderPath);

                foreach (var file in group.OrderBy(f => f.Disk))
                {
                    string destPath = Path.Combine(folderPath, file.FileName);
                    destPath = EnsureUniquePath(destPath);

                    File.Move(file.FilePath, destPath);
                }
            }
        }
        private static string MakeSafeFolderName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            return name;
        }

        private static string EnsureUniquePath(string path)
        {
            if (!File.Exists(path))
                return path;

            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            int i = 1;
            while (true)
            {
                string newPath = Path.Combine(dir, $"{name} ({i}){ext}");
                if (!File.Exists(newPath))
                    return newPath;

                i++;
            }
        }

        public static void ProcessArchiveFolder(string zipFolder, string outputFolder)
        {
            string workingFolder = Directory.CreateTempSubdirectory(Path.GetRandomFileName()).FullName;

            // 1. Extract all zips
            var zipFiles = Directory.GetFiles(zipFolder, "*.zip", SearchOption.AllDirectories);

            foreach (var zip in zipFiles)
            {
                ExtractZipRecursive(zip, workingFolder);
            }

            // 2. Find all ADF files
            var adfPaths = GetAllAdfFiles(workingFolder);

            // 3. Parse
            var parsed = adfPaths
                .Select(ParseAdf)
                .Where(x => x != null)
                .ToList();

            ;

            // DEBUG
            var failed = parsed.Count(x => x == null);
            Console.WriteLine($"Failed parses: {failed}");

            // 4. Group
            var groups = GroupFiles(parsed);
            if (groups != null && groups.Count >= 1)
            {

                // 5. Organize
                OrganizeGroups(groups, outputFolder);
            }
            try
            {
                FileUtilities.DeleteEmptyDirs(new DirectoryInfo(workingFolder));

            }
            catch (Exception ex)
            {
            }

        }
    }

    public class AamigaFile
    {

        public string FilePath { get; set; }
        public string FileName => Path.GetFileName(FilePath);

        public string Title { get; set; }
        public int Disk { get; set; }
        public int TotalDisks { get; set; }

        public string Variant { get; set; } // NEW
    }
}

