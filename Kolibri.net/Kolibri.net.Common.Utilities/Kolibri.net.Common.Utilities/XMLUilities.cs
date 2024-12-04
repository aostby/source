using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Kolibri.net.Common.Utilities
{
    public class XMLUtilities
    { 
        private static string m_errors = string.Empty;
        public static String PrettyPrintXML(String sourceXML, Encoding encoding)
        {
            string ret = string.Empty;

            try
            {

                using (MemoryStream memory = new MemoryStream())
                {
                    using (XmlTextWriter xmlWriter = new XmlTextWriter(memory, encoding))
                    {
                        XmlDocument document = new XmlDocument(); document.LoadXml(sourceXML);
                        xmlWriter.Formatting = Formatting.Indented; xmlWriter.Indentation = 4;
                        document.WriteContentTo(xmlWriter); xmlWriter.Flush(); memory.Position = 0;
                        using (StreamReader reader = new StreamReader(memory))
                        {
                            ret = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string jalla = "jalla";
            }
            return ret;
        }
        public static bool ValidateXml(FileInfo xmlFile, FileInfo schemaFile, bool loggErrors)
        {
            bool ret = true;

            m_errors = string.Empty;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.ValidationEventHandler += new ValidationEventHandler(MyValidationEventHandler);
            settings.ValidationEventHandler += new ValidationEventHandler(MyValidationEventHandler);
            settings.Schemas.Add(null, schemaFile.FullName);

            using (XmlReader vr = XmlReader.Create(xmlFile.FullName, settings))
            {
                while (vr.Read())
                {
                    if (!loggErrors && !string.IsNullOrEmpty(m_errors))
                    {
                        ret = false;
                        break;
                    }
                }

                vr.Close();
            }

            if (m_errors != string.Empty)
            {
                ret = false;
                if (loggErrors)
                    Logger.Logg(Logger.LoggType.Feil, m_errors);
            }
            return ret;
        }

        public static bool ValidateXml(string xmlFile, string schemaFile, bool loggErrors)
        {
            return ValidateXml(new FileInfo(xmlFile), new FileInfo(schemaFile), loggErrors);
        }

        public static bool ValidateXml(string xml, XmlSchema schema, bool loggErrors)
        {
            bool ret = true;
            m_errors = string.Empty;
            try
            {
                MemoryStream ms = new MemoryStream(ByteUtilities.StringToByteArray(xml, Encoding.Default)); //Convert.ToByte(xml));
                XmlTextReader r = new XmlTextReader(ms);
                XmlValidatingReader v = new XmlValidatingReader(r);
                v.ValidationType = ValidationType.Schema;
                v.Schemas.Add(schema);
                v.ValidationEventHandler += new ValidationEventHandler(MyValidationEventHandler);
                while (v.Read())
                {
                    if (!string.IsNullOrEmpty(m_errors))
                    {
                        if (v.HasLineInfo())
                            m_errors += " Linjenummer:" + v.LineNumber + " Posisjon:" + v.LinePosition;
                        ret = false;
                        break;
                    }
                }
                v.Close();
                //if (m_errors != string.Empty)
                //{
                //    ret = false;
                //    if (loggErrors)
                //        Logger.Logg(Logger.LoggType.Feil, m_errors );
                //}

            }
            catch (Exception ex)
            {
                m_errors = ex.Message;
                ret = false;
            }
            if (m_errors != string.Empty)
            {
                ret = false;
                if (loggErrors)
                    Logger.Logg(Logger.LoggType.Feil, m_errors);
            }
            return ret;

        }

        private static void MyValidationEventHandler(object sender, ValidationEventArgs e)
        {
            m_errors += string.Format(" {0}", e.Exception.ToString());
        }

        /// <summary>
        /// Method to convert a custom Object to XML string
        /// </summary>
        /// <param name="pObject">Object that is to be serialized to XML</param>
        /// <returns>XML string</returns>
        public static string SerializeObject(Object pObject)
        {
            StringWriter sw = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(pObject.GetType());
            serializer.Serialize(sw, pObject);
            return sw.ToString();
        }

        /// <summary>
        /// Method to convert a custom Object to XML string
        /// </summary>
        /// <param name="pObject">Object that is to be serialized to XML</param>
        /// <returns>XML string</returns>
        public static string SerializeObject(Object pObject, Encoding enc)
        {
            string ret = string.Empty;
            try
            {
                String XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(pObject.GetType());
                // XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, enc); // encoding ="ISO-8859-1"
                xs.Serialize(xmlTextWriter, pObject);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                //  XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                XmlizedString = ByteUtilities.UTF8ByteArrayToString(memoryStream.ToArray());
                ret = XmlizedString;
            }
            catch (Exception e)
            {
                ret = string.Empty;
            }
            return ret;
        }

        /// <summary>
        /// Skriver en arraylist med objekter til en oppgitt sti
        /// </summary>
        /// <param name="serializeableList"></param>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static bool SerializeObjekt(ArrayList serializeableList, string fullPath)
        {
            bool ret = true;
            try
            {

                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(System.Collections.ArrayList));

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(fullPath))
                {
                    writer.Serialize(file, serializeableList);
                }

            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }

        public static class SerializationHelper<T> where T : class
        {
            public static XDocument Serialize(T value)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                XDocument doc = new XDocument();
                using (var writer = doc.CreateWriter())
                {
                    xmlSerializer.Serialize(writer, value);
                }

                return doc;
            }

            public static T Deserialize(XDocument doc)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                using (var reader = doc.Root.CreateReader())
                {
                    return (T)xmlSerializer.Deserialize(reader);
                }
            }
            [Obsolete("Use Serialize T to XDocument instead")]
            public static string SerializeObject(T obj)
            {
                string XmlString = String.Empty;
                try
                {
                    MemoryStream MemStream = new MemoryStream();
                    XmlSerializer Serializer = new XmlSerializer(obj.GetType());
                    XmlTextWriter XmlText = new XmlTextWriter(MemStream, Encoding.Default);
                    Serializer.Serialize(XmlText, obj);
                    XmlString = Encoding.Default.GetString(MemStream.ToArray());
                    MemStream.Close();
                }
                catch
                {
                    throw;
                }
                return XmlString;
            }

            [Obsolete("Use Deserialize t XDocument instead ")]
            public static T DeserializeObject(string xmlbody)
            {
                byte[] Buffer = Encoding.Default.GetBytes(xmlbody);
                MemoryStream input = new MemoryStream(Buffer);
                XmlTextWriter XMlWriter = new XmlTextWriter(input, Encoding.Default);
                XmlSerializer Serialzier = new XmlSerializer(typeof(T));
                return (T)Serialzier.Deserialize(input);
            }
        } 

        /// <summary>
        /// Konverter fra bytes til XmlDocument
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public static XmlDocument XmlFromBytes(byte[] fileData)
        {
            using (MemoryStream ms = new MemoryStream(fileData))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ms);
                return doc;
            }
        }

        /// <summary>
        /// Konverter XmlDocument til Byte[]
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static byte[] XmlToBytes(XmlDocument doc)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                doc.Save(ms);
                return ms.ToArray();
            }
        }

        public static XmlSchema GetXSDStringAsXMLSchema(string schemainput)
        {
            byte[] bytes = ByteUtilities.StringToByteArray(schemainput.Substring(schemainput.IndexOf("<")), Encoding.Default);
            //    string test = ByteUtilities.ByteArrayToString(jalla, Encoding.Default);

            MemoryStream ms = new MemoryStream(bytes);
            XmlSchema schema = XmlSchema.Read(ms, new ValidationEventHandler(ValidationCallBack));
            ms.Close();
            schema.Compile(ValidationCallBack);
            return schema;

        }

        public static XmlSchema GetXSDFileAsXMLSchema(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            XmlSchema schema = XmlSchema.Read(
                fs, new ValidationEventHandler(ValidationCallBack));
            fs.Close();
            schema.Compile(ValidationCallBack);
            return schema;
        }
        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            return;
        }

        public static string GetXmlDocumentDetails(XmlDocument doc)
        {
            XmlDocument xmlDoc = doc;
            StringBuilder ret = new StringBuilder();

            XPathNavigator nav = xmlDoc.CreateNavigator();

            nav.MoveToRoot();
            string name = nav.Name;
            ret.AppendLine("Root node info: ");
            ret.AppendLine("Base URI" + nav.BaseURI.ToString());
            ret.AppendLine("Name: " + nav.Name.ToString());
            ret.AppendLine("Node Type: " + nav.NodeType.ToString());
            ret.AppendLine("Node Value: " + nav.Value.ToString());

            if (nav.HasChildren)
            {
                nav.MoveToFirstChild();
                ret.Append(GetNodeInfo(nav));
            }
            return ret.ToString();
        }
        private static string GetNodeInfo(XPathNavigator nav1)
        {
            StringBuilder ret = new StringBuilder();
            ret.AppendLine("Name: " + nav1.Name.ToString());
            ret.AppendLine("Node Type: " + nav1.NodeType.ToString());
            ret.AppendLine("Node Value: " + nav1.Value.ToString());

            if (nav1.HasChildren)
            {
                nav1.MoveToFirstChild();

                while (nav1.MoveToNext())
                {
                    GetNodeInfo(nav1);
                    nav1.MoveToParent();
                }
            }
            else
            {
                nav1.MoveToNext();
                GetNodeInfo(nav1);
            }
            return ret.ToString();
        }
    }
}
