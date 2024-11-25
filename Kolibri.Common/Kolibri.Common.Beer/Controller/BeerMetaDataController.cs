using Kolibri.Common.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.Common.Beer.Controller
{
    internal class BeerMetaDataController
    {
        public static List<dynamic> DownloadHops()
        {
            List<dynamic> Hop = new List<dynamic>();

            Uri url = new Uri(@"https://www.hopslist.com/hops//");
            var temp = Kolibri.Common.Utilities.HTMLUtilities.GetHTML(url);
            DataSet dataSet = Kolibri.Common.Utilities.DataSetUtilities.HTMLULToDataSet(temp);
            int counter = 0;
            foreach (DataTable table in dataSet.Tables)
            {

                if (counter >= 2 && counter <= 4)
                {
                    table.Columns[0].ColumnName = "Url";
                    table.Columns[1].ColumnName = "Name";
                    if (counter == 2) { table.TableName = "bittering-hops"; }
                    if (counter == 3) { table.TableName = "aroma-hops"; }
                    if (counter == 4) { table.TableName = "dual-purpose-hops"; }

                    string category = StringUtilities.FirstToUpperCamelCasing(table.TableName).Split('-').FirstOrDefault();
                    foreach (DataRow row in table.Rows)
                    {
                        try
                        {
                            string name = $"{row["Name"]}";
                            if (name.Contains("â„¢")) row["Name"] = $"{row["Name"]}".Substring(0, name.IndexOf("â„¢"));


                            string urlTemp = $@"https://www.hopslist.com/hops/{table.TableName}/{row["Name"].ToString().ToLower()
                                .Replace(" ", "-")
                                .Replace("(", string.Empty)
                                .Replace(")", string.Empty)
                                .Replace("â®", string.Empty).Replace("â„¢", string.Empty)}";
                            urlTemp = urlTemp.Replace("/early-bird", "/amos-early-bird");
                            urlTemp = urlTemp.Replace("/golding-bc", "/british-columbia-golding");
                            urlTemp = urlTemp.Replace("/hallertau-nz", "/hallertauer-new-zealand");
                            urlTemp = urlTemp.Replace("/hã¼ll-melon", "/hull-melon");
                            urlTemp = urlTemp.Replace("/lubelska", "/lublin-lubelski");



                            row["Url"] = urlTemp;

                            dynamic dyn = GetDynamicObject(urlTemp, $"{row["Name"]}".Replace("â®", string.Empty).Replace("â„¢", string.Empty), category);

                            Hop.Add(dyn);
                        }
                        catch (Exception ex)
                        {
                            dynamic dyn = new System.Dynamic.ExpandoObject();
                            dyn.Name = dyn.Name = $"{row["Name"]}".Replace("â®", string.Empty).Replace("â„¢", string.Empty);
                            dyn.Url = $"https://www.hopslist.com/hops/{table.TableName}";
                            dyn.Category = category;

                            Hop.Add(dyn);
                        }

                    }
                }
                counter++;
            }
            return Hop;

        }

        public static  dynamic GetDynamicObject(string url, string name, string category)
        {
            var temp = Kolibri.Common.Utilities.HTMLUtilities.GetHTML(new Uri(url));
            DataSet htmlDS = Kolibri.Common.Utilities.DataSetUtilities.HTMLTablesToDataSet(temp);
            DataTable details = null;
            if (htmlDS.Tables.Count == 2 || htmlDS.Tables.Count == 1)
                details = htmlDS.Tables[0];
            else
                details = htmlDS.Tables[1];


            foreach (DataRow detailRow in details.Rows)
            {
                detailRow[0] = detailRow[0].ToString().Replace(" ", string.Empty).Replace("-", string.Empty);
                if (detailRow[0].ToString().Contains("Pinene")) detailRow[0] = "BetaPinene";
                if (detailRow[0].ToString().Contains("HopStorageIndex")) detailRow[0] = "HopStorageIndex";
                if (detailRow[0].ToString().Contains("Xanthohumol")) detailRow[0] = "Xanthohumol";
            }

            var dict = details.AsEnumerable().ToDictionary<DataRow, string, string>(x => x[0].ToString(), x => x[1].ToString());

            string json = JsonConvert.SerializeObject(dict);
            dynamic dyn = JsonConvert.DeserializeObject(json);
            dyn.Name = name;
            dyn.Url = url;
            dyn.Category = category;
            return dyn;
        }

        public static DataTable GetHopDescription()
        {
            try
            {
                Uri url = new Uri($@"https://en.wikipedia.org/wiki/List_of_hop_varieties");
                var temp = Kolibri.Common.Utilities.HTMLUtilities.GetHTML(url);
                DataSet dataSet = Kolibri.Common.Utilities.DataSetUtilities.HTMLULToDataSet(temp);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(temp);

                HtmlAgilityPack.HtmlNodeCollection divs = null;
                DataTable hopsTable = Common.Utilities.DataSetUtilities.ColumnNamesToDataTable("Key", "Value").Tables[0];
                if (doc.DocumentNode != null)
                {
                    divs = doc.DocumentNode
                        .SelectNodes("//h3");


                    foreach (var div in divs)
                    {
                        try
                        {
                            var key = div.InnerText.Replace("[edit]", string.Empty).Trim();
                            var value = div.NextSibling.NextSibling.InnerText;
                            hopsTable.Rows.Add(key, value);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                return hopsTable;

                //Kolibri.Common.FormUtilities.Visualizers.VisualizeDataSet("Fermentables", hopsList.DataSet, this.Size);
            }
            catch (Exception ex)
            {
                throw ex;
                // MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }
        public static void PrettyWrite(object obj, string fileName)
        {

            var jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented);
            System.IO.File.WriteAllText(fileName, jsonString);
        }
    }
}
