using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Controller;
using System.Diagnostics;
using System.Reflection;

namespace Kolibri.net.Common.FormUtilities.Controller
{

    public class TransFormController
        {
            public static DirectoryInfo GetUnpackPath()
            {
                DirectoryInfo ret = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestBench", "TransformForm"));
                return ret;
            }
            public static FileInfo FindXSLTFile(string xsltFileName)
            {
                DirectoryInfo xsltPath = TransFormController.GetUnpackPath();
                if (!xsltPath.Exists)
                    TransFormController.UnpackXSLT();

                if (xsltPath.GetDirectories().Length < 1)
                    TransFormController.UnpackXSLT();
                FileInfo xsltInfo = xsltPath.GetFiles(xsltFileName, SearchOption.AllDirectories)[0];
                return xsltInfo;
            }

            public static void UnpackXSLT()
            {
                //https://msdn.microsoft.com/en-us/library/system.environment.specialfolder(v=vs.110).aspx
                //  m_tempPath = new DirectoryInfo(Path.Combine(Environment.SpecialFolder.LocalApplicationData.ToString(), "TestBench", "TransformForm"));
                //m_tempPath = new DirectoryInfo(Path.Combine(Environment.SpecialFolder.ApplicationData.ToString(), "TestBench", "TransformForm"));
                //DirectoryInfo m_tempPath = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TestBench", "TransformForm"));
                DirectoryInfo m_tempPath = GetUnpackPath();
                DirectoryInfo m_images = m_tempPath.Parent;

                FileInfo info;
                string destFolderName = destFolderName = "XSLT";
                foreach (var item in ResourceController.GetResourceNames())
                {
                    string[] nameArr = Path.GetFileName(item).Replace(Path.GetExtension(item), string.Empty).Split(".".ToCharArray()); //
                    if (item.EndsWith(".xsl", StringComparison.OrdinalIgnoreCase) || item.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase) || item.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        if (Path.GetExtension(item).EndsWith("ZIP", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (Array.IndexOf(nameArr, "KITH") >= 0)
                            {
                                destFolderName = "KITH";
                            }

                            if (Array.IndexOf(nameArr, "FHIR") >= 0)
                            {
                                destFolderName = "FHIR";
                            }

                            if (Array.IndexOf(nameArr, "biztalk2013documenter") >= 0
                                || Array.IndexOf(nameArr, "biztalk2013images") >= 0
                                )
                            { destFolderName = "biztalk2013documenter"; }
                            try
                            {
                                info = new FileInfo(Path.Combine(m_tempPath.FullName, destFolderName, nameArr[nameArr.Length - 1]) + ".zip");
                                if (!info.Directory.Exists) info.Directory.Create();
                                byte[] arr = ResourceController.GetResourceBytes(Assembly.GetAssembly(typeof(ResourceController)), item);
                                File.WriteAllBytes(info.FullName, arr);
                                if (Path.GetFileNameWithoutExtension(info.FullName).Equals("biztalk2013images"))
                                {
                                    FileInfo imageInfo = new FileInfo(Path.Combine(info.Directory.Parent.FullName, info.Name));
                                    File.WriteAllBytes(info.FullName, arr);
                                }


                            }
                            catch (Exception)
                            { }

                        }
                        else
                        {
                            destFolderName = "XSLT";
                            try
                            {
                                string fullName = Path.GetFileName(item);
                                info = new FileInfo(Path.Combine(m_tempPath.FullName, destFolderName, nameArr[nameArr.Length - 1]) + ".xsl");
                                if (!info.Directory.Exists) info.Directory.Create();
                                File.WriteAllText(info.FullName, ResourceController.GetResourceString(Assembly.GetAssembly(typeof(ResourceController)), fullName));
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }
                try
                {
                    foreach (FileInfo zFile in new DirectoryInfo(Path.Combine(m_tempPath.FullName)).GetFiles("*.zip", SearchOption.AllDirectories))
                    {
                        try
                        {
                            ZipUtilities.UnZipFile(zFile, zFile.Directory);
                        }
                        catch (Exception ex)
                        { string jall = ex.Message; }
                    }
                }
                catch (Exception) { }
                info = null;
            }

            internal static FileInfo GetXSLTPath(string p = null)
            {
                DirectoryInfo dirInfo = TransFormController.GetUnpackPath();
                if (!dirInfo.Exists)
                    TransFormController.UnpackXSLT();
                FileInfo xsltInfo;
                xsltInfo = dirInfo.GetFiles("GenericHTML01*", SearchOption.AllDirectories)[0];
                if (p != null)
                {
                    try
                    {
                        xsltInfo = dirInfo.GetFiles(p + "*", SearchOption.AllDirectories)[0];
                    }
                    catch (Exception)
                    { }
                }
                return xsltInfo;
            }

            public static void ShowFiles(FileInfo infoXml, FileInfo infoXsd)
            {
                Dictionary<string, FileInfo> m_paths = new Dictionary<string, FileInfo>();
                m_paths["xmlFile"] = infoXml;
                m_paths["xslFile"] = infoXsd;
                ShowFiles(m_paths, infoXml, infoXsd);
            }

            public static void ShowFiles(Dictionary<string, FileInfo> m_paths, FileInfo infoXml, FileInfo infoXsd)
            {
                string inputXml;
                string xsltString;
                string ret;
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(infoXml.FullName))
                {
                    // Read the stream to a string, and write the string to the console.
                    inputXml = sr.ReadToEnd();
                }

                using (StreamReader sr = new StreamReader(infoXsd.FullName))
                {
                    xsltString = sr.ReadToEnd();
                }
                int pos = inputXml.IndexOf('<');
                if (pos > 0)
                {
                    //Vi må sørge for gyldig xml
                    inputXml = inputXml.Substring(pos);
                    if (infoXml.Extension.EndsWith("ODX", StringComparison.InvariantCultureIgnoreCase))
                        inputXml = inputXml.Substring(0, inputXml.IndexOf("#endif // __DESIGNER_DATA"));
                    ret = XSLTTransform.TransformXML(inputXml, xsltString);
                }
                else
                {
                    ret = XSLTTransform.TransformFiles(m_paths["xslFile"], m_paths["xmlFile"]);
                }


                FileInfo info = new FileInfo(Path.Combine(Environment.SpecialFolder.LocalApplicationData.ToString(), "TestBench", infoXml.Name.Replace(infoXml.Extension, ".html")));
                if (!info.Directory.Exists)
                    info.Directory.Create();
                File.WriteAllText(info.FullName, ret);
                try
                {
                    if (infoXsd.Directory.Name.Equals("biztalk2013documenter"))
                    {
                        var liste = FileUtilities.GetFiles(infoXsd.Directory, "*.JPG|*.GIF|*.CSS", false);
                        foreach (var item in liste)
                        {
                            try
                            {
                                string dest = Path.Combine(info.Directory.Parent.FullName, Path.GetFileName(item.FullName));
                                if (!File.Exists(dest))
                                    File.Copy(item.FullName, dest);
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }
                catch (Exception)
                { }
                Process.Start(info.FullName);
            }
        }
    }