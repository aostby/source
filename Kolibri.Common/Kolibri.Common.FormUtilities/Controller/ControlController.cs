using Kolibri.Common.Utilities;
using Kolibri.Common.Utilities.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.Common.FormUtilities.Controller
{
  public  class ControlController
    {
        public static WebBrowser GetTransformedHTML(FileInfo info, string resource)
        {
            FileInfo newInfo ;
             newInfo = new FileInfo(Path.Combine(Environment.SpecialFolder.LocalApplicationData.ToString(), "TestBench", resource + ".xsl"));
            File.WriteAllText(newInfo.FullName,  ResourceController.GetResouceObject("xs3p").ToString());
            FileInfo temp = FileUtilities.GetTempFile("html");
            File.WriteAllText(temp.FullName, XSLTTransform.TransformFiles(newInfo, info));
            temp.Refresh();
            WebBrowser browser = new WebBrowser();
            browser.Navigate(temp.FullName);
            browser.Dock = DockStyle.Fill;
            return browser;
        }

        public static WebBrowser GetWebBrower(Uri url)
        {
            try
            {

                if (! FormUtilities.Controller.WBEmulator.IsBrowserEmulationSet())
                {
                   FormUtilities.Controller.WBEmulator.SetBrowserEmulationVersion();
                }
            }
            catch (Exception)
            { }
             
            WebBrowser browser = new WebBrowser();
            browser.Navigate(url);
            browser.Dock = DockStyle.Fill;
            browser.ScriptErrorsSuppressed = true;
            return browser;
        }


        public static ToolStripMenuItem GetToolStripMenuItemFromFile(FileInfo info)
        {   try
                {
                    return XMLUtilities.SerializationHelper<ToolStripMenuItem>.DeserializeObject(File.ReadAllText(info.FullName));
                }
                catch (Exception)
                { return null; }
            
        }

        public static bool WriteToolStripMenuItemFromFile(FileInfo info, ToolStripMenuItem toolsToolStripMenuItem)
        {
            try
            {
                if (!info.Directory.Exists) info.Directory.Create();
                File.WriteAllText(info.FullName,  XMLUtilities.SerializationHelper<ToolStripMenuItem>.SerializeObject(toolsToolStripMenuItem));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static ToolStripMenuItem RecursiveAddMenuItem(DirectoryInfo directory, List<DirectoryInfo> currentFolders = null)
        {
            if (currentFolders == null)
                currentFolders = new List<DirectoryInfo>();

            currentFolders.Add(directory);

            ToolStripMenuItem item = new ToolStripMenuItem(directory.Name);

            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                if (!currentFolders.Any(x => x.FullName == dir.FullName))//check to see if we already processed this folder (i.e. a unwanted shortcut)
                {
                    item.Tag = dir;
                    item.DropDownItems.Add(RecursiveAddMenuItem(dir, currentFolders));
                }
            }

            return item;
        }

        public static string GetGroupName(string appName)
        {
            var arr = appName.Split('.');
            if (arr.Length >= 3) arr = arr.Take(3).Select(i => i.ToString()).ToArray();
            string groupName = string.Join(".", arr);
            return groupName;
        }

        /// <summary>
        /// Recursively get SubMenu Items. Includes Separators.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<ToolStripItem> FindMenuItemByName(ToolStripItem item)
        {
            if (item is ToolStripMenuItem)
            {
                foreach (ToolStripItem tsi in (item as ToolStripMenuItem).DropDownItems)
                {
                    if (tsi is ToolStripMenuItem)
                    {
                        if ((tsi as ToolStripMenuItem).HasDropDownItems)
                        {
                            foreach (ToolStripItem subItem in FindMenuItemByName((tsi as ToolStripMenuItem)))
                                yield return subItem;
                        }
                        yield return (tsi as ToolStripMenuItem);
                    }
                    else if (tsi is ToolStripSeparator)
                    {
                        yield return (tsi as ToolStripSeparator);
                    }
                }
            }
            else if (item is ToolStripSeparator)
            {
                yield return (item as ToolStripSeparator);
            }
        }


        public static ToolStripMenuItem FindMenuItemByName(string tekst, ToolStripMenuItem solutions)
        {
            ToolStripMenuItem underMenuPunkt;
            ToolStripMenuItem foundMenu = null;//= toolStripMenuItemSSOStorageBTDF.DropDownItems[tekst] as ToolStripMenuItem;  // toolStripMenuItemSSOStorageBTDF.Items.Find(tekst, true);

            foreach (ToolStripMenuItem searchItem in solutions.DropDownItems)
            {
                if (tekst == searchItem.Text)
                {
                    foundMenu = searchItem; break;
                }
            }

            if (foundMenu == null)
            {
                underMenuPunkt = new ToolStripMenuItem(tekst);
                underMenuPunkt.Name = tekst;
                solutions.DropDownItems.Add(underMenuPunkt);
            }
            else
            {
                underMenuPunkt = foundMenu;
            }
            return underMenuPunkt;
        }

        public static void InsertMenuItem(string p, MenuStrip menuStrip, ToolStripMenuItem projectMenuItem)
        {
            List<object> allItems = new List<object>();
            foreach (object toolItem in menuStrip.Items)
            {
                allItems.Add(toolItem);
                //add sub items
                if (toolItem.GetType().Equals(typeof(ToolStripMenuItem)))
                {
                    var jall = GetItems(toolItem as ToolStripMenuItem);
                    var jdjdjdjd = jall.GetType().ToString();
                    if (jall != null) allItems.AddRange(jall);
                }
            }
            bool found = false;
            foreach (object obj in allItems)
            {
                var item = obj as ToolStripMenuItem;
                if (item.GetType().Equals(typeof(ToolStripMenuItem)))
                {

                    if (item.Text.Equals(p) || item.Name.Equals(p))
                    {
                        found = true;
                        if (projectMenuItem.Text.Equals("-"))
                            item.DropDownItems.Insert(0, new ToolStripSeparator());
                        else
                            item.DropDownItems.Insert(0, projectMenuItem);
                        break;
                    }
                }
            }

            if (!found)
            {
                menuStrip.Items.Add(new ToolStripMenuItem(p));
                InsertMenuItem(p, menuStrip, projectMenuItem);
            }
        }
        public static IEnumerable<ToolStripMenuItem> GetItems(object obj)
        {

            if (obj.GetType().Equals(typeof(ToolStripMenuItem)))
            {
                ToolStripMenuItem item = obj as ToolStripMenuItem;
                foreach (object dDownItem in item.DropDownItems)
                {
                    string jall = dDownItem.GetType().ToString();
                    if (!dDownItem.GetType().Equals(typeof(ToolStripSeparator)))
                    {
                        ToolStripMenuItem dropDownItem = dDownItem as ToolStripMenuItem;
                        if (dropDownItem.HasDropDownItems)
                        {
                            foreach (ToolStripMenuItem subItem in GetItems(dropDownItem))
                                yield return subItem;
                        }

                        yield return dropDownItem;
                    }
                }
            }
        }

        public static void OpenMenuItemTag_Click(object sender, EventArgs e)
        {
            string heading = null;
            try
            {
                heading = (sender as ToolStripMenuItem).Text;

                #region If  sender's Tag != null
                if ((sender as ToolStripMenuItem).Tag != null)
                {
                    if ((((sender as ToolStripMenuItem).Tag) as FileInfo) != null)
                        Process.Start((((sender as ToolStripMenuItem).Tag) as FileInfo).FullName);

                    else if ((((sender as ToolStripMenuItem).Tag) as Uri) != null)
                    {
                        Uri url = (((sender as ToolStripMenuItem).Tag) as Uri);//new Uri("https://www.microsoft.com/en-us/download/details.aspx?id=43716");
                                                                               // Process.Start(url.AbsoluteUri);
                       FileUtilities.Start(new FileInfo(Utilities.RegistryUtilites.GetSystemDefaultBrowser()), url.AbsoluteUri);
                    }
                    if ((((sender as ToolStripMenuItem).Tag) as DirectoryInfo) != null)
                        Process.Start((((sender as ToolStripMenuItem).Tag) as DirectoryInfo).FullName);
                    return;
                }
                #endregion
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message, ex.GetType().Name); }
        }


        public static void AddMenuitemsFromDictionary<T>(Dictionary<string, T> dic, ToolStripMenuItem menuItem)
        {
            Image img = SystemIcons.Application.ToBitmap();
            Image webImg = Utilities.ImageUtilities.GetIconFromFile(new FileInfo( Utilities.RegistryUtilites.GetSystemDefaultBrowser()));

            ToolStripMenuItem verktoy;
            try
            {
                #region init


                foreach (var item in dic.Keys)
                {
                    verktoy = new ToolStripMenuItem(item);

                    #region DirectoryInfo
                    if (dic[item].GetType().Equals(typeof(DirectoryInfo)))
                    {

                        string extension = "*.*"; // item.Equals("TestFiler") ? "*.*" : "*.exe";
                        DirectoryInfo[] dis = (dic[item] as DirectoryInfo).GetDirectories();

                        foreach (var path in dis)
                        {
                            try
                            {
                                ToolStripMenuItem verktoyItem = new ToolStripMenuItem(path.Name);//GetMenuItem(path);
                                int errCount = 0;
                                foreach (var file in (path.GetFiles(extension, SearchOption.AllDirectories)))
                                {
                                    {
                                        MenuAddFileInfo(verktoyItem, file, img);
                                    }
                                }
                                if (!verktoyItem.HasDropDownItems)
                                {
                                    verktoyItem.Tag = path; verktoyItem.Click += new EventHandler(OpenMenuItemTag_Click);
                                }
                                verktoy.DropDownItems.Add(verktoyItem);
                            }
                            catch (Exception)
                            { }
                        }
                    }
                    #endregion
                    else if (dic[item].GetType().Equals(typeof(FileInfo)))
                    {
                        FileInfo info = (dic[item] as FileInfo);
                        // ToolStripMenuItem verktoyItem = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(info.Name));//GetMenuItem(path);
                        // MenuAddFileInfo(verktoyItem, info, img);
                        // verktoy.DropDownItems.Add(verktoyItem);

                        //   img = Utilities.ImageUtilities.GetIconFromFile(info);
                        ToolStripDropDownItem projectMenuItem = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(info.FullName), img);
                        projectMenuItem.Tag = info;
                        projectMenuItem.ToolTipText = info.Directory.Name;
                        projectMenuItem.Click += new EventHandler(OpenMenuItemTag_Click);
                        verktoy.DropDown.Items.Add(projectMenuItem);

                    }
                    else if (dic[item].GetType().Equals(typeof(Uri)))
                    {

                        Uri url = (dic[item] as Uri);


                        ToolStripDropDownItem wikiMenuItem = new ToolStripMenuItem(item, webImg);
                        wikiMenuItem.ToolTipText = url.AbsoluteUri;
                        wikiMenuItem.Tag = url;
                        wikiMenuItem.Click += new EventHandler(OpenMenuItemTag_Click);
                        verktoy.DropDownItems.Add(wikiMenuItem);
                    }

                    if (verktoy.DropDownItems.Count > 0) menuItem.DropDownItems.Insert(0, verktoy);
                }
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private static void MenuAddFileInfo(ToolStripMenuItem verktoyItem, FileInfo file, Image img)
        {
            ToolStripDropDownItem projectMenuItem = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(file.FullName), img);
            projectMenuItem.Tag = file;
            projectMenuItem.ToolTipText = file.Directory.Name;
            projectMenuItem.Click += new EventHandler(OpenMenuItemTag_Click);
            verktoyItem.DropDown.Items.Add(projectMenuItem);
        }


    }
}
