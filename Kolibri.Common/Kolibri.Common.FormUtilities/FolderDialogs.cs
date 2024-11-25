
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 
using System.Runtime.InteropServices;
using Kolibri.Common.FormUtilities.Controller;
using Kolibri.Common.Utilities;

namespace Kolibri.Common.FormUtilities
{
    public class FolderDialogs
    {
        /// <summary>
        /// Opens Windows explorer, and if present, given file is highlighted
        /// </summary>
        /// <param name="info"></param>
        public static void OpenExplorerMarkFile(FileInfo info)
        {
            if (info.Exists)
            {   // combine the arguments together
                // it doesn't matter if there is a space after ','
                string argument = @"/select, " + info.FullName;
                Process.Start("explorer.exe", argument);
            }
            else
                Process.Start(info.Directory.FullName);
        }


        /// <summary>
        /// uses FolderBrowserDialogEx
        /// </summary>
        /// <returns></returns>
        public static DirectoryInfo LetOppMappeEX(string description, DirectoryInfo initialdirectory = null)
        {

            DirectoryInfo ret = null;

            var dlg1 = new FolderBrowserDialogEx();
            dlg1.Description = description;
            dlg1.ShowNewFolderButton = true;
            dlg1.ShowEditBox = true;
            dlg1.NewStyle = true;
            //    dlg1.SelectedPath = ret.FullName;

            dlg1.ShowFullPathInEditBox = true;
            dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            dlg1.ShowBothFilesAndFolders = true;

            if (initialdirectory != null && Directory.Exists(initialdirectory.FullName))
            {
                dlg1.SelectedPath = initialdirectory.FullName;
                
            }

            // Show the FolderBrowserDialog.
            DialogResult result = dlg1.ShowDialog( );
            if (result == DialogResult.OK)
            {
                ret = new DirectoryInfo(dlg1.SelectedPath);
            }
            return ret;
        }

        /// <summary>
        /// Uses Vista style FolderSelectDialog
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static DirectoryInfo LetOppMappe()
        {
            DirectoryInfo ret = RegistryUtilites.WorkingDirectory(); ;

            //  return LetOppMappe( "Select folder"); 

            FolderSelectDialog fsd = new FolderSelectDialog();

            fsd.Title = "Select folder";
            if (ret != null && ret.Exists)
                fsd.InitialDirectory = ret.FullName;
            else
                fsd.InitialDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.MyComputer);

            // Show the FolderBrowserDialog.
            if (fsd.ShowDialog(IntPtr.Zero)) //if (fsd.ShowDialog(this.Handle))
            {
                ret = new DirectoryInfo(fsd.FileName);
            }

            else ret = null;
            return ret;
        }

        public static DirectoryInfo LetOppMappe(DirectoryInfo initialdirectory = null) {
            return LetOppMappe(null, initialdirectory);
        }

        /// <summary>
        /// Uses the standard System.Windows.Forms.FolderBrowserDialog
        /// </summary>
        /// <param name="initialdirectory"></param>
        /// <returns></returns>
        public static DirectoryInfo LetOppMappe(string description=null, DirectoryInfo initialdirectory=null)
        { 
            DirectoryInfo ret = null;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (initialdirectory!=null&& Directory.Exists(initialdirectory.FullName))
                dialog.SelectedPath = initialdirectory.FullName;
            if (!string.IsNullOrWhiteSpace(description))
                dialog.Description = description;

            if (dialog.ShowDialog(true) == DialogResult.OK)
            {
                ret = new DirectoryInfo(dialog.SelectedPath);
            }
            return ret;
        } 




        /// <summary>
        /// Class containing methods to retrieve specific file system paths.
        /// http://stackoverflow.com/questions/10667012/getting-downloads-folder-in-c
        /// </summary>
        public static class KnownFolders
        {
            private static string[] _knownFolderGuids = new string[]
            {
        "{56784854-C6CB-462B-8169-88E350ACB882}", // Contacts
        "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", // Desktop
        "{FDD39AD0-238F-46AF-ADB4-6C85480369C7}", // Documents
        "{374DE290-123F-4565-9164-39C4925E467B}", // Downloads
        "{1777F761-68AD-4D8A-87BD-30B759FA33DD}", // Favorites
        "{BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968}", // Links
        "{4BD8D571-6D19-48D3-BE97-422220080E43}", // Music
        "{33E28130-4E1E-4676-835A-98395C3BC3BB}", // Pictures
        "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}", // SavedGames
        "{7D1D3A04-DEBB-4115-95CF-2F29DA2920DA}", // SavedSearches
        "{18989B1D-99B5-455B-841C-AB7C74E4DDFC}", // Videos
            };

            /// <summary>
            /// Gets the current path to the specified known folder as currently configured. This does
            /// not require the folder to be existent.
            /// </summary>
            /// <param name="knownFolder">The known folder which current path will be returned.</param>
            /// <returns>The default path of the known folder.</returns>
            /// <exception cref="System.Runtime.InteropServices.ExternalException">Thrown if the path
            ///     could not be retrieved.</exception>
            public static string GetPath(KnownFolder knownFolder)
            {
                return GetPath(knownFolder, false);
            }

            /// <summary>
            /// Gets the current path to the specified known folder as currently configured. This does
            /// not require the folder to be existent.
            /// </summary>
            /// <param name="knownFolder">The known folder which current path will be returned.</param>
            /// <param name="defaultUser">Specifies if the paths of the default user (user profile
            ///     template) will be used. This requires administrative rights.</param>
            /// <returns>The default path of the known folder.</returns>
            /// <exception cref="System.Runtime.InteropServices.ExternalException">Thrown if the path
            ///     could not be retrieved.</exception>
            public static string GetPath(KnownFolder knownFolder, bool defaultUser)
            {
                return GetPath(knownFolder, KnownFolderFlags.DontVerify, defaultUser);
            }

            private static string GetPath(KnownFolder knownFolder, KnownFolderFlags flags,
                bool defaultUser)
            {
                IntPtr outPath;
                int result = SHGetKnownFolderPath(new Guid(_knownFolderGuids[(int)knownFolder]),
                    (uint)flags, new IntPtr(defaultUser ? -1 : 0), out outPath);
                if (result >= 0)
                {
                    return Marshal.PtrToStringUni(outPath);
                }
                else
                {
                    throw new ExternalException("Unable to retrieve the known folder path. It may not "
                        + "be available on this system.", result);
                }
            }

            [DllImport("Shell32.dll")]
            private static extern int SHGetKnownFolderPath(
                [MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken,
                out IntPtr ppszPath);

            [Flags]
            private enum KnownFolderFlags : uint
            {
                SimpleIDList = 0x00000100,
                NotParentRelative = 0x00000200,
                DefaultPath = 0x00000400,
                Init = 0x00000800,
                NoAlias = 0x00001000,
                DontUnexpand = 0x00002000,
                DontVerify = 0x00004000,
                Create = 0x00008000,
                NoAppcontainerRedirection = 0x00010000,
                AliasOnly = 0x80000000
            }
        }

        /// <summary>
        /// Standard folders registered with the system. These folders are installed with Windows Vista
        /// and later operating systems, and a computer will have only folders appropriate to it
        /// installed.
        /// </summary>
        public enum KnownFolder
        {
            Contacts,
            Desktop,
            Documents,
            Downloads,
            Favorites,
            Links,
            Music,
            Pictures,
            SavedGames,
            SavedSearches,
            Videos
        } 
    }

    internal static class FolderBrowserDialogExtension
    {
        public static DialogResult ShowDialog(this FolderBrowserDialog dialog, bool scrollIntoView)
        {
            return ShowDialog(dialog, null, scrollIntoView);
        }

        public static DialogResult ShowDialog(this FolderBrowserDialog dialog, IWin32Window owner, bool scrollIntoView)
        {
            if (scrollIntoView)
            {
                SendKeys.Send("{TAB}{TAB}{RIGHT}");
            }

            return dialog.ShowDialog(owner);
        }
    }

}
