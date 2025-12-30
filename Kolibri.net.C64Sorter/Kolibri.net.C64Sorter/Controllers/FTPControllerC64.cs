using Kolibri.net.C64Sorter.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.net.C64Sorter.Controllers
{
    internal class FTPControllerC64
    {

        public static List<FtpItemDetail> GetDirectoryListing(string ftpPath, string username, string password)
        {
            List<FtpItemDetail> details = new List<FtpItemDetail>();
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = new NetworkCredential(username, password);

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // Simple parsing logic (might need adjustment based on server OS)
                        // Example line: "drwxr-xr-x 2 user group 4096 Jan 1 2024 directoryName"
                        // or "-rw-r--r-- 1 user group 1024 Jan 1 2024 fileName.txt"
                        string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 8)
                        {
                            string[] itemNames = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Select((value, index) => new { value, index }).Where(i => i.index > 7).Select(i => i.value).ToArray();
                            bool isDirectory = parts[0].StartsWith("d");
                            string name = string.Join(" ", itemNames);  // parts[8];
                            if (name != "." && name != "..")
                            {
                                details.Add(new FtpItemDetail
                                {
                                    Name = name,
                                    FullPath = ftpPath.TrimEnd('/') + "/" + name,
                                    IsDirectory = isDirectory
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., connection issues, permissions)
                Console.WriteLine($"Error listing directory {ftpPath}: {ex.Message}");
            }
            return details;
        }

        public static FileInfo DownloadFileFTP(UE2LogOn logon, DirectoryInfo destinationPath, string ftpfilepath)
        {
            FileInfo ret = null;
            string inputfilepath = Path.Combine(UltmateEliteClient.ResourcesPath.FullName,  Path.GetFileName(ftpfilepath)); // @"C:\Temp\FileName.exe";
           // string ftphost = "xxx.xx.x.xxx";
            //string ftpfilepath = "/Updater/Dir1/FileName.exe";

            string ftpfullpath = "ftp://" + logon.Hostname + ftpfilepath;

            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential(logon.Username,logon.Password);
                byte[] fileData = request.DownloadData(ftpfullpath);

                using (FileStream file = File.Create(inputfilepath))
                {
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                }
             //   MessageBox.Show("Download Complete");
                ret= new FileInfo(inputfilepath); 
            }
            return ret;
        }

    }
}
