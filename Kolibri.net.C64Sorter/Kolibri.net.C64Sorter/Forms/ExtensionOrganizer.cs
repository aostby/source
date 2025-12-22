using Kolibri.net.Common.Utilities;
using System.Data;

namespace File_Organizer
{
    public partial class ExtensionOrganizer : Form
    {
        string sSelectedFolder;
        string[] files;
        DirectoryInfo dir;

        public ExtensionOrganizer()
        {
            InitializeComponent();
            string userName = Environment.UserName;
            sSelectedFolder = @"C:\Users\" + userName + @"\Downloads";

            files = Directory.GetFiles(sSelectedFolder);
            fileCount.Text = files.Length.ToString();
            currentFolder.Text = sSelectedFolder;
            currentFolder.Tag = new DirectoryInfo(sSelectedFolder);
        }
        private void SetLabelText(string message)
        {
            //try
            //{
            //    toolStripStatusLabel1.Text = s;
            //}
            //catch (Exception ex)
            //{
            //}
            try
            {
                Task.Delay(1).GetAwaiter().GetResult();
                if (InvokeRequired)
                    Invoke(new MethodInvoker(
                        delegate { SetLabelText(message); }
                    ));
                else
                {
                    toolStripStatusLabel1.Text = message;
                    Thread.Sleep(5);
                }
            }
            catch (Exception ex)
            { }
        }
        private void SelectFolder_Click(object sender, EventArgs e)
        {
            sSelectedFolder = getPath();
            currentFolder.Text = sSelectedFolder;
            currentFolder.Tag = new DirectoryInfo(sSelectedFolder);
            files = Directory.GetFiles(sSelectedFolder, "*.*", SearchOption.AllDirectories);
            fileCount.Text = files.Length.ToString();
        }

        private void Organize_Click(object sender, EventArgs e)
        {
            try
            {
                DirectoryInfo destination = null;
                DirectoryInfo path = (currentFolder as Label).Tag as DirectoryInfo;
                SearchOption level = SearchOption.AllDirectories;
                string mode = "E";//   A = Alfabetisk (kun bokstavsortering   E = Ekstensjon + Alfabetisk (filtype først) 

                if (sender.Equals(buttonExtensionSort)) { mode = "E"; destination = new DirectoryInfo(Path.Combine(path.FullName )); }
                else if (sender.Equals(buttonAlphabetizeSort)) { mode = "A"; destination = new DirectoryInfo(Path.Combine(path.FullName )); }
                else if (sender.Equals(buttonFlattenC64files)) {   destination = new DirectoryInfo(Path.Combine(path.FullName )); }

                var fList = FileUtilities.GetFiles(path, C64Utilities.C64CommonFileExt(true), level);
                if (fList.Count == 0)
                    SetLabelText($"No C64 extensions found in this path \\{path.Name}");

                var fiList = fList.Select(x => new FileInfo(x)).ToArray();

                if (sender.Equals(buttonFlattenC64files))
                    FlattenToCurrentFolder(fiList, destination);
                else
                {
                    SortFilesRecursively(fiList, destination, mode);
                    CleanEmptyFolders(path.FullName);

                    //    if (AllFilesInSameFolder(fiList))
                    //        MoveFilesToExtensions(fiList, destination);
                    //    else
                    //        MoveGameFoldersToExtensions(fiList, destination);
                }
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
        private void FlattenToCurrentFolder(FileInfo[] c64Files, DirectoryInfo destinationPath)
        {
            if (!destinationPath.Parent.Exists) destinationPath.Create();
            FileInfo current = null;
            var count = 0;
            foreach (FileInfo file in c64Files)
            {
                Application.DoEvents();
                count++;
                current = file;
                try
                {
                    string ext = file.Extension.Replace(".", string.Empty).ToUpper();

                    string sourceDirectory = file.Directory.FullName;// Replace with your source folder path 
                    FileInfo destination = new FileInfo(Path.Combine(destinationPath.FullName, file.Name));


                    // Check if the source directory exists
                    if (file.Exists && !destination.Exists)
                    {
                        FileUtilities.Move(file, destination, true);
                        SetLabelText($"{count}/{c64Files.Length} Folder '{sourceDirectory}' and its contents moved to '{destination.FullName}'.");
                    }
                    else
                    {
                        SetLabelText($"{count}/{c64Files.Length} Source directory '{sourceDirectory}' does not exist.");
                    }
                }
                catch (IOException ex)
                {
                    SetLabelText($"{count}/{c64Files.Length} An error occurred during the move operation: {ex.Message}");
                }
                catch (Exception ex)
                { string text = $"{count}/{c64Files.Length} An unexpected error occurred: {ex.Message}";
                    SetLabelText(text);
                    MessageBox.Show(text, ex.GetType().Name);
                    break;
                } 
            }
            try
            {
                Thread.Sleep(5);
                FileUtilities.DeleteEmptyDirs(destinationPath.Parent);
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
            //MessageBox.Show("Process complete");
            SetLabelText($"Process complete {DateTime.Now.ToShortTimeString()}");
            if (current != null && current.Exists) FileUtilities.OpenFolderHighlightFile(current);

        }

        private void SortFilesRecursively(FileInfo[] c64Files, DirectoryInfo destinationPath,  string mode)
        {
            var files = c64Files;

            foreach (var file in files)
            {
                try
                {
                    string fileName = file.Name;
                    string extension = file.Extension.TrimStart('.').ToUpper();

                    // Hopp over filer uten filtype eller loggfilen selv
                    if (string.IsNullOrWhiteSpace(extension) ||
                        fileName.Equals("sort_log.txt", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Finn første bokstav i filnavn
                    char firstLetter = char.ToUpper(fileName[0]);
                    if (!char.IsLetter(firstLetter))
                        firstLetter = '#';

                    // Bygg destinasjonsmappe basert på modus
                    string targetFolder;

                    if (mode == "A")
                    {
                        // Kun alfabetisk
                        targetFolder = Path.Combine(destinationPath.FullName, firstLetter.ToString());
                    }
                    else
                    {
                        // Ekstensjon + alfabetisk
                        string extFolder = Path.Combine(destinationPath.FullName, extension);
                        targetFolder = Path.Combine(extFolder, firstLetter.ToString());
                    }

                    Directory.CreateDirectory(targetFolder);

                    string destPath = Path.Combine(targetFolder, fileName);

                    // Hopp over hvis allerede riktig
                    if (string.Equals(file.FullName, destPath, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Unngå navnekonflikt
                    if (File.Exists(destPath))
                    {
                        string newName = $"{Path.GetFileNameWithoutExtension(file.FullName)}_{Guid.NewGuid()}{Path.GetExtension(file.FullName)}";
                        destPath = Path.Combine(targetFolder, newName);
                    }

                    File.Move(file.FullName, destPath);

                    string msg = $"Flyttet: {fileName} → {destPath}";
                    Console.WriteLine(msg);
                    SetLabelText(msg);
                }
                catch (Exception ex)
                {
                    string err = $"⚠️ Feil ved flytting av {file}: {ex.Message}";
                    Console.WriteLine(err);
                    SetLabelText(err);
                }
            }
        }

        private void CleanEmptyFolders(string rootPath )
        {
            foreach (var dir in Directory.GetDirectories(rootPath))
            {
                CleanEmptyFolders(dir );

                if (Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
                {
                    try
                    {
                        Directory.Delete(dir);
                        string msg = $"🗑️  Slettet tom mappe: {dir}";
                        Console.WriteLine(msg);
                        SetLabelText(msg);
                    }
                    catch (Exception ex)
                    {
                        SetLabelText($"⚠️  Kunne ikke slette {dir}: {ex.Message}");
                    }
                }
            }
        }




        private void MoveGameFoldersToExtensions(FileInfo[] c64Files, DirectoryInfo destinationPath )
        {
            FileInfo current = null;
            var count = 0;
            foreach (FileInfo file in c64Files)
            {
                count++;
                Application.DoEvents();
                Thread.Sleep(3);
                current = file;
                try
                {
                    string ext = file.Extension.Replace(".", string.Empty).ToUpper();   //Kolibri.net.Common.Utilities.MovieUtilites.GetYear(file.DirectoryName);
                    DirectoryInfo destination = new DirectoryInfo(Path.Combine(destinationPath.FullName, ext.ToString(), file.Name.Substring(0, 1).ToUpper(), file.Directory.Name));
                    //if (level == SearchOption.TopDirectoryOnly)
                    //    destination = new DirectoryInfo(Path.Combine(destinationPath.FullName, file.Name.Substring(0, 1).ToUpper()));
                    string sourceDirectory = file.Directory.FullName;// Replace with your source folder path
                    DirectoryInfo destinationDirectory = new DirectoryInfo(destination.FullName); // Replace with your destination folder path
                   

                    try
                    {
                        // Check if the source directory exists
                        if (Directory.Exists(sourceDirectory))
                        {
                            if (!destinationDirectory.Parent.Exists) destinationDirectory.Parent.Create();
                            Directory.Move(sourceDirectory, destinationDirectory.FullName);// Move the directory and its contents

                            Application.DoEvents();
                            SetLabelText($"{count}/{c64Files.Length} Folder '{sourceDirectory}' and its contents moved to '{destinationDirectory}'.");

                        }
                        else
                        {
                            SetLabelText($"{count}/{c64Files.Length} Source directory '{sourceDirectory}' does not exist.");
                        }
                    }
                    catch (IOException ex)
                    {
                        SetLabelText($"{count}/{c64Files.Length} An error occurred during the move operation: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        SetLabelText($"{count}/{c64Files.Length} An unexpected error occurred: {ex.Message}");
                    }

                }
                catch (Exception ex) { }
            }
            try
            {
                FileUtilities.DeleteEmptyDirs(destinationPath.Parent);
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
            //MessageBox.Show("Process complete");
            SetLabelText($"Process complete {DateTime.Now.ToShortTimeString()}");
            if (current != null && current.Exists) FileUtilities.OpenFolderHighlightFile(current);

        }
        private void MoveFilesToExtensions(FileInfo[] c64Files, DirectoryInfo destinationPath)
        {
            FileInfo current = null;
            var count= 0;   
            foreach (FileInfo file in c64Files)
            {
                Application.DoEvents();
                count++;
                current = file;

                try
                {
                    string ext = file.Extension.Replace(".", string.Empty).ToUpper();   //Kolibri.net.Common.Utilities.MovieUtilites.GetYear(file.DirectoryName);
                    DirectoryInfo destination = new DirectoryInfo(Path.Combine(destinationPath.FullName, ext.ToString(), file.Name.Substring(0, 1).ToUpper() ));
                    /*if (level == SearchOption.TopDirectoryOnly)
                        destination = new DirectoryInfo(Path.Combine(destinationPath.FullName, file.Name.Substring(0, 1).ToUpper()));*/
                //    string sourceDirectory = file.Directory.FullName;// Replace with your source folder path
                    DirectoryInfo destinationDirectory = new DirectoryInfo(destination.FullName); // Replace with your destination folder path
                    if (!destinationDirectory.Parent.Exists) destinationDirectory.Parent.Create();

                    try
                    {
                        // Check if the source directory exists
                        if (file.Exists)
                        {
                            FileUtilities.Move(file, new FileInfo(Path.Combine(destinationDirectory.FullName, file.Name)), true);
                            SetLabelText($"{count}/{c64Files.Length} Folder '{file.Directory}' and its contents moved to '{destinationDirectory}'.");
                        }
                        else
                        {
                            SetLabelText($"{count}/{c64Files.Length} Source directory '{file.Directory}' does not exist.");
                        }
                    }
                    catch (IOException ex)
                    {
                        SetLabelText($"{count}/{c64Files.Length} An error occurred during the move operation: {ex.Message}");
                        Thread.Sleep(100);
                        break;
                    }
                    catch (Exception ex)
                    {
                        SetLabelText($"{count}/{c64Files.Length} An unexpected error occurred: {ex.Message}");
                    }

                }
                catch (Exception ex) { }
            }
            try
            {
                FileUtilities.DeleteEmptyDirs(destinationPath.Parent);
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
            //MessageBox.Show("Process complete");
            SetLabelText($"Process complete {DateTime.Now.ToShortTimeString()}");
            if (current != null && current.Exists) FileUtilities.OpenFolderHighlightFile(current);

        }


        public string getPath()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            // fbd.Description = "Custom Description";

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                dir = new DirectoryInfo(fbd.SelectedPath);
                return fbd.SelectedPath;
            }
            else
            {
                string downloads = "";
                string userName = Environment.UserName;
                downloads = @"C:\Users\" + userName + @"\Downloads";
                return downloads;
            }
        }
        public static bool AllFilesInSameFolder(FileInfo[] files)
        {
            if (files == null || files.Length <= 1)
            {
                // If there are no files or only one file, they are considered to be in "the same folder".
                return true;
            }

            // Get the directory name of the first file.
            string firstDirectory = files[0].DirectoryName;

            // Use LINQ's All() method to check if all other files have the same directory name.
            return files.All(file => file.DirectoryName == firstDirectory);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            try
            {
                FileUtilities.Start((currentFolder as Label).Tag as DirectoryInfo);
            }
            catch (Exception ex)
            {
                SetLabelText(ex.Message);
            }
        }
    }
}
