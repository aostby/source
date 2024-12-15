using Newtonsoft.Json;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace Kolibri.net.Common.Utilities.Extensions

{

    public static class XmlExt
    {
        #region xml extensions
        public static string Validate(this XmlDocument xmlDocument, FileInfo schema)
        {
            string ret = string.Empty;
            XMLValidator val = new XMLValidator();
            val.Validate(xmlDocument.OuterXml, schema);
            return ret;
        }

        public static FileInfo Transform(this XmlDocument xmlDocument, FileInfo stylesheet)
        {
            return XSLTTransform.TransformAndShow(xmlDocument, stylesheet, (stylesheet.Extension.ToLower().StartsWith(".xsl") ? "html" : "xml"), false);
        }

        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }

        /// <summary>
        /// Usage: var newMyClass = element.FromXElement<MyClass>();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xElement"></param>
        /// <returns></returns>
        public static T FromXElement<T>(this XElement xElement)
        {
            //var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            //return (T)xmlSerializer.Deserialize(xElement.CreateReader());
            string jsonText = JsonConvert.SerializeXNode(xElement);
            var ret = JsonConvert.DeserializeObject<T>(jsonText);
            return ret;
        }

        public static dynamic GetDynamic(this XElement xElement)
        {
            string jsonText = JsonConvert.SerializeXNode(xElement);
            dynamic data = JsonConvert.DeserializeObject(jsonText);
            return data;
        }




        public static XElement GetElement(this XElement startNode, string localName)
        {
            return GetElement(startNode, localName);
        }
        public static IEnumerable<XElement> GetElements(this XDocument xDoc, string localName)
        {
            return GetElements(xDoc.Root, localName);
        }

        public static IEnumerable<XElement> GetElements(this XElement startNode, string localName)
        {
            return GetElements(startNode, localName);
        }

        public static string GetElementValue(this XElement startNode, string localName)
        {
            string ret = null;
            try
            {
                ret = GetElement(startNode, localName).Value;

            }
            catch (Exception)
            {
                ret = null;
            }
            return ret;
        }

        public static object GetValue(this XElement element)
        {
            try
            {
                return element.Value;
            }
            catch (System.Exception)
            {
            }
            return DBNull.Value;
        }
        #endregion

        #region string extensions
        public static bool IsNumeric(this string s)
        {
            var ret = false;
            try
            { float output;
                if (!float.TryParse(s, out output))
                {
                    var jaja = float.Parse(s.Replace(".", string.Empty).Replace(".", string.Empty));
                    ret = true;
                }
                else { ret = true; }
            }
            catch (Exception)
            {
               
            }
            return ret;
           
        }

        public static bool IsInt(this string text)
        {
            int num;
            return int.TryParse(text, out num);
        }
        //get null if conversion failed
        public static int? ToInt(this string text)
        {
            int num;
            return int.TryParse(text, out num) ? num : (int?)null;
        }



        public static long? ToLong(this string text)
        {
            long num;
            return long.TryParse(text, out num) ? num : (long?)null;
        }
        public static long ToLong(this string text, int defaultVal)
        {
            long num;
            return long.TryParse(text, out num) ? num : defaultVal;
        }

        //get default value if conversion failed
        public static int ToInt(this string text, int defaultVal)
        {
            int num;
            return int.TryParse(text, out num) ? num : defaultVal;
        }

        public static DateTime ToDateTime(this string text)
        {
            DateTime num;
            DateTime.TryParse(text.Replace("+00:00", string.Empty), out num);
            return num;
        }

        //formating a string
        public static string StringFormat(this string text, params object[] formatArgs)
        {
            return string.Format(text, formatArgs);
        }

        public static (bool isFolder, bool exists) IsFileOrFolder(this string path)
        {
            try
            {
                var attr = File.GetAttributes(path);
                return attr.HasFlag(FileAttributes.Directory) ? (true, true) : (false, true);
            }
            catch (FileNotFoundException)
            {
                return (false, false);
            }
        }
        #endregion

        #region Uri extensions
        public static Uri Append(this Uri uri, params string[] paths)
        {
            return new Uri(paths.Aggregate(uri.AbsoluteUri, (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), path.TrimStart('/'))));
        }
        #endregion

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialization method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null)) return default;

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }



    }
}