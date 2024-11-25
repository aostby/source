using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.Common.PlugIn.Controller
{
    public class DownloadPluginsController
    {
        public static string GetPluginsPath()
        {
            var executableLocation = Assembly.GetEntryAssembly().Location;
            var path = Path.Combine(Path.GetDirectoryName(executableLocation), "Plugins");
            return path;
        }

        public static void DownloadFromFolders(string sourcePath, string destinationPath = null)
        {
            string destPath = destinationPath;
            if (string.IsNullOrEmpty(destPath))
            {
                destPath = GetPluginsPath();
            }
            CopyFolderToFolder(new DirectoryInfo(sourcePath), new DirectoryInfo(destPath));
        }
        public static void CopyFolderToFolder(DirectoryInfo sourcePath, DirectoryInfo targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath.FullName, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath.FullName, targetPath.FullName));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath.FullName, "*.*", SearchOption.AllDirectories))
            {
                string destPath = newPath.Replace(sourcePath.FullName, targetPath.FullName);
                if(File.Exists(destPath)) { File.Delete(destPath); }
                File.Copy(newPath,destPath, true);
            }
        }
    }
}
