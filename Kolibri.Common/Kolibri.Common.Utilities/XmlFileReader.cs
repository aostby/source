using System;
using System.IO;
using System.Xml;

namespace Kolibri.Common.Utilities
{
    public class XmlFileReader
    {
        // Fields
        private XmlDocument _xmlDoc = new XmlDocument();

        /// <summary>
        /// Leser ett xml dokument, enten fil eller string
        /// </summary>
        /// <param name="xml"></param>
        public XmlFileReader(string xml)
        {
            if(xml.StartsWith("<"))
                this.LoadXmlString(xml);
            else
                this.LoadXmlFile(xml);
            
        }
        /// <summary>
        /// Initialiserer med ett xmldocument
        /// </summary>
        /// <param name="xml"></param>
        public XmlFileReader(XmlDocument xml)
        {
            _xmlDoc = xml;
        }

        private string GetNodeValue(XmlNodeList nodes, string key)
        {
            string nodeValue = null;
            try
            {
                foreach (XmlNode node in nodes)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        if ((node.NodeType == XmlNodeType.Element) && (node.Name == key))
                        {
                            return node.InnerText;
                        }
                        if (node.HasChildNodes)
                        {
                            nodeValue = this.GetNodeValue(node.ChildNodes, key);
                        }
                        if (nodeValue != null)
                        {
                            return nodeValue;
                        }
                    }
                }
                return nodeValue;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return nodeValue;
        }

        private XmlNodeList GetSectionList(XmlNodeList nodes, string section)
        {
            XmlNodeList sectionList = null;
            try
            {
                foreach (XmlNode node in nodes)
                {
                    if ((node.NodeType == XmlNodeType.Element) && (node.Name == section))
                    {
                        return node.ChildNodes;
                    }
                    if (node.HasChildNodes)
                    {
                        sectionList = this.GetSectionList(node.ChildNodes, section);
                    }
                }
                return sectionList;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return sectionList;
        }

        public string GetValue(string key)
        {
            return this.GetValue(key, null);
        }

        public string GetValue(string key, string section)
        {
            return this.GetValue(key, section, null);
        }

        public string GetValue(string key, string section, string defaultValue)
        {
            string nodeValue = null;
            try
            {
                if ((section == "") || (section == null))
                {
                    return this.GetNodeValue(this._xmlDoc.ChildNodes, key);
                }
                nodeValue = this.GetNodeValue(this.GetSectionList(this._xmlDoc.ChildNodes, section), key);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return nodeValue;
        }

        private void LoadXmlFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    this._xmlDoc.Load(fileName);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private void LoadXmlString(string xmlFileString)
        {
            try
            {
                this._xmlDoc.LoadXml(xmlFileString);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}



    
