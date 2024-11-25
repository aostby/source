using Kolibri.Common.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Kolibri.Common.Utilities;
using DevExpress.Data.Filtering.Helpers;

namespace Kolibri.Common.Beer.Controller
{
    public class BeerXMLParser
    {
        public XDocument BeerDoc { get; set; }

        public BeerXMLParser(XDocument xDoc)
        {
            BeerDoc = xDoc;
        }

        internal bool RootIsOK()
        {
            bool ret = true;
            if (BeerDoc.Root.Name.LocalName != "RECIPES") { ret = false; }
            return ret; 
        }

        internal string CheckFermentables()
        {
            StringBuilder ret = new StringBuilder();
            string element = "FERMENTABLES";
            try
            {
                var list = BeerDoc.Root.GetElements(element);
                foreach (var item in list) {
                    element = "SUPPLIER";
                    if (item.GetElement(element) == null) { ret.AppendLine($"{element} is missing from {item.Name} {GetName(item)} in {System.Reflection.MethodBase.GetCurrentMethod().Name} for {GetName(BeerDoc.Root)}"); }
                }
            }
            catch (Exception ex)
            {
                ret.AppendLine(ex.Message); 
            }
            return ret.ToString();
        }

        private object GetName(XElement ele)
        {
            try
            {
                return ele.GetElementValue("NAME");
            }
            catch (Exception)
            {
                return "No name found for this element in this BeerXML";
            }
        }

        internal string CheckHops()
        {
            StringBuilder ret = new StringBuilder();
            string element = "HOPS";
            try
            {
                var list = BeerDoc.Root.GetElements(element);
                foreach (var item in list)
                {
                    element = "NAME";
                    if (item.GetElement(element) == null) { ret.AppendLine($"{element} is missing from {item.Name} {GetName(item)} in {System.Reflection.MethodBase.GetCurrentMethod().Name} for {GetName(BeerDoc.Root)}"); }
                }
            }
            catch (Exception ex)
            {
                ret.AppendLine(ex.Message);
            }
            return ret.ToString();
        }

        internal string CheckYeasts()
        {
            StringBuilder ret = new StringBuilder();
            string element = "YEASTS";
            try
            {
                var list = BeerDoc.Root.GetElements(element);
                foreach (var item in list)
                {
                    element = "LABORATORY";
                    if (item.GetElement(element) == null) { ret.AppendLine($"{element} is missing from {item.Name} {GetName(item)} in {System.Reflection.MethodBase.GetCurrentMethod().Name} for {GetName(BeerDoc.Root)}"); }
                }
            }
            catch (Exception ex)
            {
                ret.AppendLine(ex.Message);
            }
            return ret.ToString();
        }
    }
}
