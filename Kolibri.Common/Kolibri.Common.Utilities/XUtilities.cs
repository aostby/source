using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Kolibri.Common.Utilities
{
    public class XUtilities
    {
        public static XElement GetElement(XElement startNode, string localName)
        {
            try
            {
                return startNode.Descendants().FirstOrDefault(p => p.Name.LocalName == localName);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static XElement GetLastElement(XElement startNode, string localName)
        {
            try
            {
                return startNode.Descendants().LastOrDefault(p => p.Name.LocalName == localName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static XElement  GetElement(XElement startNode, string localName, string attributename, string attributeValue)
        {
            //var elems = XUtilities.GetElements(startNode, localName);
           // var ret = elems.FirstOrDefault(el => el.Attribute(attributename) != null && el.Attribute(attributename).Value.Equals(attributeValue));
            var ret = GetElements(startNode, localName, attributename);
            return ret.FirstOrDefault(el => el.Attribute(attributename) != null && el.Attribute(attributename).Value.Equals(attributeValue)); 
        }


        public static IEnumerable<XElement> GetElements(XElement startNode, string localName)
        {
            IEnumerable<XElement> ret = startNode.Descendants().Where(p => p.Name.LocalName == localName);
            return ret;
        }

        public static IEnumerable<XElement> GetElements(XElement startNode, string localName, string attributename) 
        { var elems = XUtilities.GetElements(startNode, localName);
            var ret = elems.Where  (el => el.Attribute(attributename) != null);
            return ret;
        }


    }

    /*public static class DocumentExtensions
    {
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

        //public static XElement GetElement(this XElement startNode, string localName)
        //{
        //    return Utilities.XUtilities.GetElement(startNode, localName);
        //}
    }*/
}