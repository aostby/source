using Saxon.Api;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Kolibri.net.Common.Utilities
{
    public class XSLTTransform
    {
        public static FileInfo TransformAndShow(FileInfo info, FileInfo xsltInfo, string fileExtension)
        {

            string inputXml;
            string xsltString;
            // Open the text file using a stream reader.
            using (StreamReader sr = new StreamReader(xsltInfo.FullName))
            {
                // Read the stream to a string, and write the string to the console.
                xsltString = sr.ReadToEnd();
            }


            // Open the text file using a stream reader.
            using (StreamReader sr = new StreamReader(info.FullName))
            {
                // Read the stream to a string, and write the string to the console.
                inputXml = sr.ReadToEnd();
            }



            string ret = TransformXML(inputXml, xsltString);

            info = new FileInfo(Path.Combine(Environment.SpecialFolder.LocalApplicationData.ToString(), "TestBench", info.Name.Replace(info.Extension, "." + fileExtension)));
            if (!info.Directory.Exists)
                info.Directory.Create();
            File.WriteAllText(info.FullName, ret);
            Process.Start(info.FullName);
            return info;
        }

        /// <summary>
        /// Transforms an xmlPath using an xsl with the decapricated method System.Xml.Xsl.XslTransform
        /// </summary>
        /// <param name="xsl">File used to execute the transform</param>
        /// <param name="xmlPath">Source file</param>
        /// <returns></returns>
        public static string TransformFiles(FileInfo xsl, FileInfo xmlPath)
        {
            XPathDocument xpathDoc = new XPathDocument(xmlPath.FullName);
            XslTransform xslTrans = new XslTransform();

            xslTrans.Load(xsl.FullName);

            MemoryStream buffer = new MemoryStream();
            StreamWriter sw = new StreamWriter(buffer);

            xslTrans.Transform(xpathDoc, null, sw);

            byte[] chars = buffer.ToArray();
            buffer.Dispose();
            string output = Encoding.UTF8.GetString(chars);


            return output;
        }

        public static string TransformXML(string inputXml, string xsltString)
        {
            XslCompiledTransform transform = new XslCompiledTransform();

            using (XmlReader reader = XmlReader.Create(new StringReader(xsltString)))
            {
                transform.Load(reader, new XsltSettings(false, true), null);
            }
            StringWriter results = new StringWriter();
            using (XmlReader reader = XmlReader.Create(new StringReader(inputXml)))
            {
                transform.Transform(reader, null, results);
            }
            return results.ToString();
        }

        public static string TransformMSXML(byte[] msxsl, FileInfo inputXml, FileInfo xsltString)
        {
            string ret = null;
            FileInfo msxslInfo = new FileInfo(Path.Combine(Environment.SpecialFolder.LocalApplicationData.ToString(), "TestBench", "msxsl.exe"));
            FileInfo tempInfo = new FileInfo(msxslInfo.FullName.Replace(msxslInfo.Extension, ".xmlx"));
            FileInfo batInfo = new FileInfo(msxslInfo.FullName.Replace(msxslInfo.Extension, ".bat"));
            try
            {
                File.WriteAllBytes(msxslInfo.FullName, msxsl);

                string transProc = string.Format(@"""{0}"" ""{1}"" ""{2}"" -o ""{3}""", msxslInfo.FullName, inputXml.FullName, xsltString.FullName, tempInfo.FullName);
                File.WriteAllText(batInfo.FullName, transProc);

                // string transProc = string.Format(@"""{0}"" ""{1}"" -o ""{2}""", inputXml.FullName, xsltString.FullName, tempInfo.FullName);
                //  Process.Start(msxslInfo.FullName, transProc);

                Process.Start(batInfo.FullName);
                Thread.Sleep(35);

                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(tempInfo.FullName))
                {
                    // Read the stream to a string, and write the string to the console.
                    ret = sr.ReadToEnd();
                }


            }
            catch (Exception)
            { }
            finally
            {
                //if (tempInfo.Exists)
                //    tempInfo.Delete();
                //if (msxslInfo.Exists)
                //    msxslInfo.Delete();
            }

            return ret;
        }
        public static FileInfo TransformAndShow(XmlDocument xmlDoc, FileInfo xsltInfo, string fileExtension, bool show)
        {
            string temp = Path.GetTempFileName();
            xmlDoc.Save(temp);
            FileInfo info = new FileInfo(temp);
            info = TransformAndShow(info, xsltInfo, fileExtension, show);
            return info;
        }

        public static FileInfo TransformAndShow(FileInfo info, FileInfo xsltInfo, string fileExtension, bool show)
        {

            string inputXml;
            string xsltString;
            string ret = string.Empty;
      using (StreamReader sr = new StreamReader(xsltInfo.FullName))
            {
                // Read the stream to a string, and write the string to the console.
                xsltString = sr.ReadToEnd();
            }
            if (xsltString.Contains(@"version=""2.0"""))
                ret = transform20Xml(info, xsltInfo);
            // Open the text file using a stream reader.

            else
            {
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(info.FullName))
                {
                    // Read the stream to a string, and write the string to the console.
                    inputXml = sr.ReadToEnd();
                }


                ret = TransformXML(inputXml, xsltString);
            }
            info = new FileInfo(Path.Combine(Environment.SpecialFolder.LocalApplicationData.ToString(), "TestBench", info.Name.Replace(info.Extension, "." + fileExtension)));
            if (!info.Directory.Exists)
                info.Directory.Create();
            File.WriteAllText(info.FullName, ret);
            if (show)
                Process.Start(info.FullName);
            return info;
        }


        #region xslt 2.0
        public static string transform20Xml(FileInfo sourceUri, FileInfo xsltUri)
        {
            Processor processor = new Processor(false);
            XdmNode input = processor.NewDocumentBuilder().Build(new System.Uri(sourceUri.FullName));
            XsltTransformer transformer = processor.NewXsltCompiler().Compile(new Uri(xsltUri.FullName)).Load();
            transformer.InitialContextNode = input;
 
            transformer.BaseOutputUri = new Uri(xsltUri.FullName);
            Serializer serializer = processor.NewSerializer();
            StringWriter sw = new StringWriter();
            serializer.SetOutputWriter(sw);
            transformer.Run(serializer);
            return sw.ToString();
        }
        #endregion

    }
}