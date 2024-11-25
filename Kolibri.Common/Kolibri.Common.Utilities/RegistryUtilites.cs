using System;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Kolibri.Common.Utilities
{
    /// <summary>
    /// http://www.codeproject.com/KB/system/modifyregistry.aspx
    /// </summary>
    public class RegistryUtilites
    {
        public enum SystemNavn
        {

            UKJENT = 0,
            #region Kolibri
            Kolibri_Tidsur = 100,
            Kolibri_AmazingCast
            #endregion
        }
        
        public enum KolibriKey
        {
            Settings,
            Application
        }

        private static string Application_LOC_IN_REGISTRY = "Software\\Kolibri\\Application\\";
        private static string Settings_LOC_IN_REGISTRY = "Software\\Kolibri\\";
        // private static string Settings_LOC_IN_REGISTRY = "Software\\Kolibri\\";

        private static bool m_showError = false;

 
        #region resources
        //http://www.codeproject.com/Questions/403305/Search-for-values-in-REGEDIT-using-Csharp
        #endregion

        /// <summary>
        /// http://www.codeproject.com/KB/system/modifyregistry.aspx
        /// </summary>
        /// 
       
            private static string PROJECT_MRU_PATH = string.Format(@"Software\Microsoft\VisualStudio\{0}\ProjectMRUList", "12.0"); //Må endres med VS versjon
            private static RegistryKey m_ProjectMRUKey;
 
 

     

            private static SystemNavn m_KolibriSystem = SystemNavn.Kolibri_AmazingCast;
            // HKEY_CURRENT_USER\Software\Kolibri\Application\Kolibri Lønn
            private static string m_subKey;
            private static RegistryKey baseRegistryKey = Registry.CurrentUser;

            public static void SetSystemNavn(SystemNavn KolibriSystem)
            {
                m_KolibriSystem = KolibriSystem;
                if (string.IsNullOrEmpty(m_subKey))
                    m_subKey = "KlientNr";
            }

            public static void SetSubKey(string subkey)
            {
                m_subKey = subkey;
            }

            public static void InitSubkeySettings(SystemNavn KolibriSystem, string subkey)
            {
                RegistryUtilites.KolibriKey typeKey = RegistryUtilites.KolibriKey.Settings;
                SetSystemNavn(KolibriSystem);
                SetSubKey(subkey);
            }

            public static void SetSystemNavn(SystemNavn KolibriSystem, string subkey)
            {
                m_KolibriSystem = KolibriSystem;
                m_subKey = subkey;
            }
            /// <summary>
            /// A property to show or hide error messages 
            /// (default = false)
            /// </summary>
            public bool ShowError
            {
                get { return m_showError; }
                set { m_showError = value; }
            }

            /// <summary>
            /// A property to set the SubKey value
            /// Eksempel: Application: Autologin,Database,EnterTilTab,KlientNr,LagreOppsett,PostPrinterDelay,PrePrinterDelay
            /// Eksempel: Settings: w_beregn_skatt\\Preferences
            /// </summary>
            public string SubKey
            {
                get { return m_subKey; }
                set { m_subKey = value; }
            }


            /// <summary>
            /// A property to set the BaseRegistryKey value.
            /// (default = Registry.LocalMachine)
            /// </summary>
            public RegistryKey BaseRegistryKey
            {
                get { return baseRegistryKey; }
                set { baseRegistryKey = value; }
            }

            /// <summary>
            /// To read a registry key.
            /// input: KeyName (string)
            /// output: value (string) 
            /// </summary>
            public static string ReadUser(KolibriKey KeyName, string UserName)
            {
                string ret = null;
                RegistryKey sk1;
                string path = string.Empty;
                switch (KeyName)
                {
                    case KolibriKey.Settings:
                        path = Settings_LOC_IN_REGISTRY + StringUtilities.FirstToUpperCamelCasing(m_KolibriSystem.ToString().Replace("_", ""));
                        break;
                    case KolibriKey.Application:
                        path = Application_LOC_IN_REGISTRY + StringUtilities.FirstToUpperCamelCasing(m_KolibriSystem.ToString()).Replace("_", "") + "\\" + UserName;
                        break;
                    default:
                        break;
                }

                // Opening the registry key
                RegistryKey rk = baseRegistryKey;

                // Open a m_subKey as read-only
                try
                {
                    switch (KeyName)
                    {
                        case KolibriKey.Settings:

                            string temp = Path.GetDirectoryName(m_subKey);
                            sk1 = rk.OpenSubKey(path + "\\" + temp);
                            ret = sk1.GetValue(Path.GetFileName(m_subKey)).ToString();
                            //  ret = (string)sk1.GetValue("window.height");
                            break;
                        case KolibriKey.Application:

                            sk1 = rk.OpenSubKey(path);
                            // If the RegistryKey exists I get its value
                            // or null is returned.
                            ret = sk1.GetValue(m_subKey).ToString();
                            break;
                        default:
                            sk1 = null;
                            break;
                    }

                }
                catch (Exception e)
                {
                    if (m_showError == true)
                        Logger.Logg(Logger.LoggType.Feil, "Reading registry " + m_subKey.ToUpper() + " " + e.Message);
                    ret = null;
                }
                return ret;
            }

            /// <summary>
            /// To read a registry key.
            /// input: KeyName (string)
            /// output: value (string) 
            /// </summary>
            public static string Read(string fullPath, string keyName)
            {
                string ret = null;
                try
                {

                    string[] pathArray = fullPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

                    RegistryKey sk1;
                    string path = fullPath;

                    // Opening the registry key
                    RegistryKey rk = baseRegistryKey;

                    if (pathArray[0].ToUpper().Contains("HKEY_LOCAL_MACHINE"))
                        rk = Registry.LocalMachine;

                    Array.Copy(pathArray, 1, pathArray, 0, pathArray.Length - 1);
                    pathArray[pathArray.Length - 1] = "";

                    path = string.Join("\\", pathArray).Trim();
                    if (path.EndsWith("\\"))
                        path = path.Substring(0, path.Length - 1);

                    sk1 = rk.OpenSubKey(path);
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    ret = sk1.GetValue(keyName).ToString();
                }
                catch (Exception e)
                {
                    if (m_showError == true)
                        Logger.Logg(Logger.LoggType.Feil, "Reading registry " + keyName.ToUpper() + " " + e.Message);
                    ret = null;
                }
                return ret;
            }

            public static string Read(KolibriKey type, string keyName)
            {
                string ret = null;
                try
                {
                    string path = string.Empty;

                    RegistryKey rk = baseRegistryKey;
                    RegistryKey sk1;
                    string temp;
                    switch (type)
                    {
                        case KolibriKey.Settings:
                            path = Settings_LOC_IN_REGISTRY + StringUtilities.FirstToUpperCamelCasing(m_KolibriSystem.ToString().Replace("_", ""));
                            temp = Path.GetDirectoryName(m_subKey);
                            //      sk1 = rk.CreateSubKey(path + "\\" + temp);
                            //      sk1.GetValue(keyName);

                            path = path + "\\" + temp;
                            sk1 = rk.OpenSubKey(path);
                            // If the RegistryKey exists I get its value
                            // or null is returned.
                            ret = sk1.GetValue(keyName).ToString();


                            break;
                        case KolibriKey.Application:
                            path = Application_LOC_IN_REGISTRY + StringUtilities.FirstToUpperCamelCasing(m_KolibriSystem.ToString()).Replace("_", "");
                            sk1 = rk.OpenSubKey(path);
                            ret = sk1.GetValue(m_subKey).ToString();
                            //sk1 = rk.CreateSubKey(m_subKey);
                            //    temp = Path.GetDirectoryName(m_subKey);
                            //    sk1 = rk.CreateSubKey(path + "\\" + temp);
                            //    sk1.SetValue(KeyName.ToUpper(), Value);
                            break;
                        default:
                            sk1 = null;
                            break;
                    }
                    // return true;
                    // Setting
                    // RegistryKey rk = baseRegistryKey;

                    //RegistryKey sk1 = rk.CreateSubKey(m_subKey);
                    // Save the value
                    //sk1.SetValue(KeyName.ToUpper(), Value);

                }
                catch (Exception e)
                {
                    //if (m_showError == true)                    Logger.Logg(Logger.LoggType.Feil, "Writing registry " + KeyName.ToUpper() + " " + e.Message);

                }
                return ret;
            }


            /// <summary>
            /// To write into a registry key.
            /// input: KeyName (string) , Value (object)
            /// output: true or false 
            /// </summary>
            public static bool Write(KolibriKey type, string KeyName, object Value)
            {
                try
                {
                    string path = string.Empty;

                    RegistryKey rk = baseRegistryKey;
                    RegistryKey sk1;
                    string temp;
                    switch (type)
                    {
                        case KolibriKey.Settings:
                            path = Settings_LOC_IN_REGISTRY + StringUtilities.FirstToUpperCamelCasing(m_KolibriSystem.ToString().Replace("_", ""));
                            temp = Path.GetDirectoryName(m_subKey);
                            // sk1 = rk.CreateSubKey(m_subKey);
                            sk1 = rk.CreateSubKey(path + "\\" + temp);
                            sk1.SetValue(KeyName.ToUpper(), Value);
                            break;
                        case KolibriKey.Application:
                            path = Application_LOC_IN_REGISTRY + StringUtilities.FirstToUpperCamelCasing(m_KolibriSystem.ToString()).Replace("_", "");
                            //sk1 = rk.CreateSubKey(m_subKey);
                            temp = Path.GetDirectoryName(m_subKey);
                            sk1 = rk.CreateSubKey(path + "\\" + temp);
                            sk1.SetValue(KeyName.ToUpper(), Value);
                            break;
                        default:
                            sk1 = null;
                            break;
                    }
                    // return true;
                    // Setting
                    // RegistryKey rk = baseRegistryKey;

                    //RegistryKey sk1 = rk.CreateSubKey(m_subKey);
                    // Save the value
                    //sk1.SetValue(KeyName.ToUpper(), Value);

                    return true;
                }
                catch (Exception e)
                {
                    if (m_showError == true)
                        Logger.Logg(Logger.LoggType.Feil, "Writing registry " + KeyName.ToUpper() + " " + e.Message);
                    return false;
                }
            }

            /// <summary>
            /// To delete a registry key.
            /// input: KeyName (string)
            /// output: true or false 
            /// </summary>
            public bool DeleteKey(string KeyName)
            {
                try
                {
                    // Setting
                    RegistryKey rk = baseRegistryKey;
                    RegistryKey sk1 = rk.CreateSubKey(m_subKey);
                    // If the RegistrySubKey doesn't exists -> (true)
                    if (sk1 == null)
                        return true;
                    else
                        sk1.DeleteValue(KeyName);

                    return true;
                }
                catch (Exception e)
                {
                    if (m_showError == true)
                        Logger.Logg(Logger.LoggType.Feil, "Deleting SubKey " + m_subKey + " " + e.Message);
                    return false;
                }
            }

            /// <summary>
            /// To delete a sub key and any child.
            /// input: void
            /// output: true or false 
            /// </summary>
            public bool DeleteSubKeyTree()
            {
                try
                {
                    // Setting
                    RegistryKey rk = baseRegistryKey;
                    RegistryKey sk1 = rk.OpenSubKey(m_subKey);
                    // If the RegistryKey exists, I delete it
                    if (sk1 != null)
                        rk.DeleteSubKeyTree(m_subKey);

                    return true;
                }
                catch (Exception e)
                {
                    if (m_showError == true)
                        Logger.Logg(Logger.LoggType.Feil, "Deleting SubKey " + m_subKey + " " + e.Message);
                    return false;
                }
            }

            /// <summary>
            /// Retrive the count of subkeys at the current key.
            /// input: void
            /// output: number of subkeys
            /// </summary>
            public int SubKeyCount()
            {
                try
                {
                    // Setting
                    RegistryKey rk = baseRegistryKey;
                    RegistryKey sk1 = rk.OpenSubKey(m_subKey);
                    // If the RegistryKey exists...
                    if (sk1 != null)
                        return sk1.SubKeyCount;
                    else
                        return 0;
                }
                catch (Exception e)
                {
                    if (m_showError == true)
                        Logger.Logg(Logger.LoggType.Feil, "Retriving subkeys of " + m_subKey + " " + e.Message);
                    return 0;
                }
            }

            /// <summary>
            /// Retrive the count of values in the key.
            /// input: void
            /// output: number of keys
            /// </summary>
            public int ValueCount()
            {
                try
                {
                    // Setting
                    RegistryKey rk = baseRegistryKey;
                    RegistryKey sk1 = rk.OpenSubKey(m_subKey);
                    // If the RegistryKey exists...
                    if (sk1 != null)
                        return sk1.ValueCount;
                    else
                        return 0;
                }
                catch (Exception e)
                {
                    if (m_showError == true)
                        Logger.Logg(Logger.LoggType.Feil, "Retriving keys of " + m_subKey + " " + e.Message);
                    return 0;
                }
            }

            /// <summary>
            /// method for retrieving the users default web browser
            /// </summary>
            /// <returns></returns>
            public static string GetSystemDefaultBrowser()
            {
                const string userChoice = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice";
                using (RegistryKey userChoiceKey = Registry.CurrentUser.OpenSubKey(userChoice))
                {
                    if (userChoiceKey != null)
                    {
                        object progIdValue = userChoiceKey.GetValue("Progid");
                        if (progIdValue != null)
                        {
                            string progId = progIdValue.ToString();
                            const string exeSuffix = ".exe";
                            string progIdPath = progId + @"\shell\open\command";
                            using (RegistryKey pathKey = Registry.ClassesRoot.OpenSubKey(progIdPath))
                            {
                                if (pathKey != null)
                                {
                                    string path = pathKey.GetValue(null).ToString().ToLower().Replace("\"", "");
                                    if (!path.EndsWith(exeSuffix))
                                    {
                                        path = path.Substring(0, path.LastIndexOf(exeSuffix, StringComparison.Ordinal) + exeSuffix.Length);
                                    }
                                    return path;
                                }
                            }
                        }
                    }
                }

                return null;
            }

            /// <summary>
            /// Gets the working directory for logged on/current user 
            /// </summary>
            /// <returns>DirectoryInfo with last used directory if exist, else null</returns>
            public static DirectoryInfo WorkingDirectory()
            {
                DirectoryInfo ret = null;
                try
                {
                    RegistryUtilites.InitSubkeySettings(RegistryUtilites.SystemNavn.UKJENT, "WORKINGFOLDER");
                    string dirpath = RegistryUtilites.Read(RegistryUtilites.KolibriKey.Application,
                        System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1]);
                    ret = new DirectoryInfo(dirpath);
                    if (!ret.Exists)
                        throw new FileNotFoundException("Directory does not exist");
                }
                catch (Exception)
                {
                    ret = null;
                }
                return ret;
            }

            /// <summary>
            /// Set the last used directory for current user
            /// </summary>
            /// <param name="setWorkingDirectory">The working directory to be set</param>
            /// <returns>boolean result for registrywrite  success</returns>
            public static bool WorkingDirectory(DirectoryInfo setWorkingDirectory)
            {
                RegistryUtilites.InitSubkeySettings(RegistryUtilites.SystemNavn.UKJENT, "WORKINGFOLDER");
                return RegistryUtilites.Write(RegistryUtilites.KolibriKey.Application, "WorkingFolder", setWorkingDirectory.FullName);

            }

            /// <summary>
            /// Henter en liste over sist brukte prosjekter i visual studio. 12.0 er standard, og benyttes om null sendes som parameter
            /// </summary>
            /// <param name="version">f.eks 12.0 for 2013
            /// Which Version of Visual Studio do I Have?
            /// VS 2012 Version Version ID 11.0
            /// VS 2013 Version Version ID 12.0
            /// VS 2015 Version Version ID 14.0
            /// </param>
            /// <returns></returns>
            public static List<string> GetMRUList(string version)
            {
                List<string> ret = new List<string>();
                List<string> versions = RegistryUtilites.GetVisualStudioVersions();
                double tall = 12.0;
                try
                {
                    if (version != null && version.Length > 0 && Double.TryParse(version, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out tall))
                    {
                        PROJECT_MRU_PATH = string.Format(@"Software\Microsoft\VisualStudio\{0}\ProjectMRUList", version.Replace(",", ".")); //Må endres med VS versjon
                    }
                    else if (versions != null && versions.Count > 0)
                    {
                        PROJECT_MRU_PATH = string.Format(@"Software\Microsoft\VisualStudio\{0}\ProjectMRUList", versions[versions.Count - 1]); //Må endres med VS versjon
                    }
                    else throw new NotFiniteNumberException("No version of Visual Studio found.");
                }
                catch (Exception)
                {
                    PROJECT_MRU_PATH = string.Format(@"Software\Microsoft\VisualStudio\{0}\ProjectMRUList", "12.0");
                }

                // Open a writable RegKey
                m_ProjectMRUKey = Registry.CurrentUser.OpenSubKey(PROJECT_MRU_PATH, true);
                foreach (string key in m_ProjectMRUKey.GetValueNames())
                {
                    var item = m_ProjectMRUKey.GetValue(key);
                    string temp = item.ToString().Split('|')[0];
                    ret.Add(temp);
                }
                return ret;
            }

            public static List<string> GetVisualStudioVersions()
            {
                List<string> ret = new List<string>();
                try
                {
                    var registry = Registry.ClassesRoot;
                    var subKeyNames = registry.GetSubKeyNames();
                    var regex = new Regex(@"^VisualStudio\.edmx\.(\d+)\.(\d+)$");
                    foreach (var subKeyName in subKeyNames)
                    {
                        var match = regex.Match(subKeyName);
                        if (match.Success)
                        {
                            string tmp = string.Format("{0}.{1}", match.Groups[1].Value, match.Groups[2].Value);
                            ret.Add(tmp);
                        }
                    }
                }
                catch (Exception)
                { }
                ret.Sort();
                return ret;
            }
            /// <summary>
            /// List 45Plus dotnet versions
            /// </summary>
            /// <returns></returns>
            public static string GetDOTNETVersionFromRegistry()
            {
                string ret = string.Empty;
                const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

                using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
                {
                    if (ndpKey != null && ndpKey.GetValue("Release") != null)
                    {
                        ret = ($".NET Framework Version: {CheckFor45PlusVersion((int)ndpKey.GetValue("Release"))}");
                    }
                    else
                    {
                        ret = (".NET Framework Version 4.5 or later is not detected.");
                    }
                }
                return ret;
            }

            // Checking the version using >= enables forward compatibility.
            private static string CheckFor45PlusVersion(int releaseKey)
            {
                if (releaseKey >= 528049)
                    return "4.8 or later";
                if (releaseKey >= 461808)
                    return "4.7.2";
                if (releaseKey >= 461308)
                    return "4.7.1";
                if (releaseKey >= 460798)
                    return "4.7";
                if (releaseKey >= 394802)
                    return "4.6.2";
                if (releaseKey >= 394254)
                    return "4.6.1";
                if (releaseKey >= 393295)
                    return "4.6";
                if (releaseKey >= 379893)
                    return "4.5.2";
                if (releaseKey >= 378675)
                    return "4.5.1";
                if (releaseKey >= 378389)
                    return "4.5";
                // This code should never execute. A non-null release key should mean
                // that 4.5 or later is installed.
                return "No 4.5 or later version detected";
            }






        }
 
}