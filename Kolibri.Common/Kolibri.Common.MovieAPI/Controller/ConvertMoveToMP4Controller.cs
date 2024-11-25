using Kolibri.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading;

namespace Kolibri.Common.MovieAPI.Controller
{
    public class ConvertMoveToMP4Controller
    {
        private List<FileInfo> infos;
        public ConvertMoveToMP4Controller(DirectoryInfo info)
        {
            var searchStr = "*." + string.Join("|*.", FileUtilities.MoviesFileExt());
            var strlist = FileUtilities.GetFiles(info, searchStr, SearchOption.TopDirectoryOnly);
            infos = strlist.Select(a => new FileInfo(a)).ToList();
        }

        public void ExecuteConversion()
        {
            foreach (var info in infos) { CreateCommand(info); }
        }
        private void CreateCommand(FileInfo info)
        {
            string source = info.FullName;
            string destination = Path.ChangeExtension(info.FullName, ".mp4");
            string command = $@"ffmpeg -i ""{source}"" -strict -2 ""{destination}""";
            try
            {
                using (PowerShell PowerShellInstance = PowerShell.Create())
                {
                    // this script has a sleep in it to simulate a long running script
                    PowerShellInstance.AddScript(command);
                    
                    // begin invoke execution on the pipeline
                    IAsyncResult result = PowerShellInstance.BeginInvoke();

                    // do something else until execution has completed.
                    // this could be sleep/wait, or perhaps some other work
                    while (result.IsCompleted == false)
                    {
                        Console.WriteLine("Waiting for pipeline to finish...");
                        Thread.Sleep(1000);

                        // might want to place a timeout here...
                    }

                    Console.WriteLine("Finished!");
                }

            }
            catch (Exception ex)
            {
             
            }
   



        }
    }
}
