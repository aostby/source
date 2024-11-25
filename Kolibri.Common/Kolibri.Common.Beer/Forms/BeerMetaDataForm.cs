using FastColoredTextBoxNS;
using iTextSharp.text;
using iTextSharp.tool.xml.html;
using Kolibri.Common.Beer.Entities;
using Kolibri.Common.Beer.Properties;
using Kolibri.Common.FormUtilities;
using Kolibri.Common.Utilities;
using Kolibri.XMLValidator.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace Kolibri.Common.Beer.Forms
{
    public partial class BeerMetaDataForm : Form
    {
        //  private Uri _url;
        private DataSet _dataSet;
        private FileInfo _info;
        public BeerMetaDataForm()
        {
            InitializeComponent();
            Init();
        }

        private Dictionary<string, FileInfo> _fileDic;

        private void Init()
        {
            string basePath = @"C:\xampp\htdocs\uranus\BeerAPI\Data";
            _fileDic = new Dictionary<string, FileInfo>() {
                {"Fermentables", new FileInfo ( Path.Combine(basePath,  "Fermentables.json")) },
                {"Hops", new FileInfo ( Path.Combine(basePath,  "Hops.json")) },
                {"Yeasts", new FileInfo ( Path.Combine(basePath,  "Yeasts.json")) },
            };
            Uri url = new Uri("https://www.brewersfriend.com/fermentables/");
            _info = _fileDic["Fermentables"];

            comboBox1.DataSource = _fileDic.Keys.ToList();

        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            try
            {
                Uri url = new Uri("https://www.brewersfriend.com/fermentables/");

                var temp = Kolibri.Common.Utilities.HTMLUtilities.GetHTML(url);
                _dataSet = Kolibri.Common.Utilities.DataSetUtilities.HTMLTablesToDataSet(temp);
                Kolibri.Common.FormUtilities.Visualizers.VisualizeDataSet("Fermentables", _dataSet, this.Size);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

        }

        private void buttonVisualize_Click(object sender, EventArgs e)
        {
            try
            {
                Kolibri.Common.FormUtilities.Visualizers.VisualizeDataSet("Fermentables", _dataSet, this.Size);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            try
            {
                var temp = Kolibri.Common.Utilities.FileUtilities.ExportToFormats(_dataSet, _info);
                if (temp != null && string.IsNullOrEmpty(temp)) { _info = new FileInfo(temp); }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (_info != null && _info.Exists)
                {
                    var jsonString = FileUtilities.ReadTextFile(_info.FullName);
                    var playerList = JsonConvert.DeserializeObject<List<FermentableObject>>(jsonString);
                    foreach (FermentableObject f in playerList)
                    {
                        f.Manufacturer = f.Fermentable.Split('-').FirstOrDefault().Trim();
                        f.Name = f.Fermentable.Split('-').LastOrDefault().Trim();
                    }
                    Controller.BeerMetaDataController.PrettyWrite(playerList, @"c:\temp\fermentables_manufacturer.json");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

        }

        private void buttonFermentablesDistinctValues_Click(object sender, EventArgs e)
        {
            try
            {
                if (_info != null && _info.Exists)
                {
                    var jsonString = FileUtilities.ReadTextFile(_info.FullName);
                    var playerList = JsonConvert.DeserializeObject<List<FermentableObject>>(jsonString);
                    var liste = playerList.Select(f => f.Type).Distinct().ToList();
                    string outp = string.Empty;
                    foreach (var item in liste)
                    {
                        outp += $"{Environment.NewLine}{item}";
                    }
                    OutputDialogs.ShowRichTextBox("Category", outp, Language.Custom, this.Size);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }

        }


        private void buttonDownloadHops_Click(object sender, EventArgs e)
        {
            try
            {
                List<dynamic> Hop = Controller.BeerMetaDataController.DownloadHops();
                FileInfo info = new FileInfo(@"c:\temp\hops_details.json");
                Controller.BeerMetaDataController.PrettyWrite(Hop, info.FullName);
                FileUtilities.OpenFolderHighlightFile(info);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonHopsGetTheWeirdos_Click(object sender, EventArgs e)
        {

            List<string> urlWeirdos = new List<string>() {
"https://www.hopslist.com/hops/aroma-hops/lubelski-pulawy/",
"https://www.hopslist.com/hops/aroma-hops/pacific-hallertau/",
"https://www.hopslist.com/hops/aroma-hops/precoce-d-bourgogne/",
"https://www.hopslist.com/hops/aroma-hops/czech-premiant/",
"https://www.hopslist.com/hops/aroma-hops/czech-sladek/",
"https://www.hopslist.com/hops/aroma-hops/tardif-de-burgogne/",
"https://www.hopslist.com/hops/dual-purpose-hops/bates-brewer/",
"https://www.hopslist.com/hops/dual-purpose-hops/cluster/",
"https://www.hopslist.com/hops/dual-purpose-hops/super-alpha/",
"https://www.hopslist.com/hops/dual-purpose-hops/falconer-s-flight/",
"https://www.hopslist.com/hops/dual-purpose-hops/greenburg/",
"https://www.hopslist.com/hops/dual-purpose-hops/hueller-bitterer/",
"https://www.hopslist.com/hops/dual-purpose-hops/northern-brewer/",
"https://www.hopslist.com/hops/dual-purpose-hops/alpharoma/",
            "https://www.hopslist.com/hops/aroma-hops/ahil/"
            };
            List<dynamic> list = new List<dynamic>();
            foreach (string url in urlWeirdos)
            {
                dynamic weirdo = GetHopsWerdo(url);
                if (weirdo != null)
                    list.Add(weirdo);
            }

            FileInfo info = new FileInfo(@"c:\temp\hops_details.json");
            Controller.BeerMetaDataController.PrettyWrite(list, info.FullName);
            FileUtilities.OpenFolderHighlightFile(info);
        }

        private dynamic GetHopsWerdo(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                string category = StringUtilities.FirstToUpperCamelCasing(uri.Segments[2]).Split('-').FirstOrDefault();
                string name = Kolibri.Common.Utilities.StringUtilities.FirstToUpper(string.Join(" ", uri.Segments[3].Split('-')).TrimEnd('/'));

                var temp = Controller.BeerMetaDataController.GetDynamicObject(url, name, category);
                string desc = GetDescription(uri);
                if (!string.IsNullOrEmpty(desc))
                {
                    temp.Descrition = desc;
                }
                return temp;
            }
            catch (Exception ex) { }
            return null;
        }

        private string GetDescription(Uri url)
        {
            string ret = string.Empty;


            try
            {
                var temp = Kolibri.Common.Utilities.HTMLUtilities.GetHTML(url);

                DataSet test02 = Kolibri.Common.Utilities.DataSetUtilities.HTMLULToDataSet(temp);

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(temp);

                HtmlAgilityPack.HtmlNodeCollection divs = null;

                if (doc.DocumentNode != null)
                {
                    divs = doc.DocumentNode.SelectNodes("//p");

                    foreach (var div in divs)
                    {
                        string jall = div.InnerText.Trim();
                        if (jall.Contains("As a listing requirement, all suppliers below ship nationally to their respective countries.")) { break; }
                        else ret += jall + " ";

                    }

                }

            }
            catch (Exception ex)
            { }


            return ret.Trim();

        }

        private void downloadYeasts_Click(object sender, EventArgs e)
        {
            List<dynamic> list = new List<dynamic>();
            List<string> urlWeirdos = new List<string>() {

           "https://www.beercraftr.com/beer-yeast-list/american-ale-yeast/",
           "https://www.beercraftr.com/beer-yeast-list/belgian-ale-yeasts/",
           "https://www.beercraftr.com/beer-yeast-list/british-ale-yeast/",
           "https://www.beercraftr.com/beer-yeast-list/lambic-sour-ale/",
           "https://www.beercraftr.com/beer-yeast-list/barleywine-yeast-list/",
           "https://www.beercraftr.com/beer-yeast-list/ipa/",
           "https://www.beercraftr.com/beer-yeast-list/brown-ale-yeast/",
           "https://www.beercraftr.com/beer-yeast-list/kolsch-and-altbier-yeast/",
           "https://www.beercraftr.com/beer-yeast-list/stout-and-porter-yeast-list/",
           "https://www.beercraftr.com/beer-yeast-list/wheat-beer/",
            };

            foreach (string urlStr in urlWeirdos)
            {
                try
                {
                    Uri url = new Uri(urlStr);
                    var temp = Kolibri.Common.Utilities.HTMLUtilities.GetHTML(url);

                    try
                    {
                        DataSet test02 = Kolibri.Common.Utilities.DataSetUtilities.HTMLULToDataSet(temp);

                        var doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(temp);

                        HtmlAgilityPack.HtmlNodeCollection divs = null;

                        if (doc.DocumentNode != null)
                        {
                            divs = doc.DocumentNode.SelectNodes("//h4");
                        }
                        int counter = 0;

                        foreach (DataTable table in test02.Tables)
                        {
                            if (table.Rows.Count == 5)
                            {
                                string searchable = table.Rows[4]["Value"].ToString();
                                if (searchable == "Tutorials") { continue; }
                                else
                                {
                                    dynamic dyn = new ExpandoObject();
                                    var div = divs[counter];
                                    string name = div.InnerText;
                                    searchable = searchable.Split(':').LastOrDefault();
                                    dyn.Category = Kolibri.Common.Utilities.StringUtilities.FirstToUpper(new Uri(urlStr).Segments[2].ToString().Replace('-', ' ').TrimEnd('/')); ;
                                    dyn.Type = table.Rows[0]["Value"].ToString().Split(':').LastOrDefault();
                                    dyn.Flocculation = table.Rows[1]["Value"].ToString().Split(':').LastOrDefault();
                                    dyn.Attenuation = table.Rows[2]["Value"].ToString().Split(':').LastOrDefault();
                                    dyn.Temperature = table.Rows[3]["Value"].ToString().Split(':').LastOrDefault();
                                    dyn.Flavour = table.Rows[4]["Value"].ToString().Split(':').LastOrDefault();
                                    dyn.Url = url;
                                    dyn.Name = name;

                                    list.Add(dyn);
                                    counter++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    { }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
            }
            FileInfo info = new FileInfo(@"c:\temp\yeasts_initialTry.json");
            Controller.BeerMetaDataController.PrettyWrite(list, info.FullName);
            FileUtilities.OpenFolderHighlightFile(info);
        }
        private void buttonFixYeast_Click(object sender, EventArgs e)
        {
            List<dynamic> cleaned = new List<dynamic>();
            FileInfo info = new FileInfo(@"c:\temp\Yeasts.json");
            string json = System.IO.File.ReadAllText(info.FullName);
            dynamic dyn = JsonConvert.DeserializeObject(json);
            List<string> manufacturers = new List<string>() {

          "Brewferm",
          "Coopers",
          "East Coast Yeast",
          "Fermentis",
          "Lallemand",
          "Mangrove Jack",
          "Muntons",
          "Real Brewers Yeast",
          "Siebel Inst",
          "White Labs",
          "Wyeast",
            };
            var k = 0;
            foreach (dynamic dynItem in dyn.Children())
            {
                string name = dynItem.Name;
                int index = cleaned.FindIndex(item => $"{item.DisplayName}".Contains(name));
                if (index < 0)
                {
                    dynItem.DisplayName = name;
                    if (name.Contains("Brewferm")) { }
                    string firstPart = name.Split(' ').FirstOrDefault();
                    dynItem.Manufacturer = manufacturers.FirstOrDefault(s => s.StartsWith(firstPart));
                    string manufacturer = dynItem.Manufacturer;
                    dynItem.Name = name.Replace(manufacturer, string.Empty).Trim();
                    name = dynItem.Name;
                    dynItem.Code = name.Split(' ').LastOrDefault();
                    name = name.Replace(name.Split(' ').LastOrDefault(), string.Empty).Trim();
                    if (!string.IsNullOrEmpty(name))
                        dynItem.Name = name;

                    cleaned.Add(dynItem);
                }
                else
                {
                    var existing = cleaned[index];
                    string catetory = $"{existing.Category}";
                    string newCategory = dynItem.Category;
                    if (!catetory.Contains(newCategory))
                        existing.Category = catetory + $", {newCategory}";
                }
            }
            Controller.BeerMetaDataController.PrettyWrite(cleaned, @"c:\temp\yeasts_cleaned.json");
        }


        private void buttonHopsDescription_Click(object sender, EventArgs e)
        {
            try
            {
                FileInfo info = _fileDic["Hops"];
                string json = System.IO.File.ReadAllText(info.FullName);
                dynamic dyn = JsonConvert.DeserializeObject(json);
                DataTable table = Controller.BeerMetaDataController.GetHopDescription();
                var dic = DataSetUtilities.DataTableToDictionary(table, "Key", "Value");

                foreach (dynamic dynItem in dyn.Children())
                {
                    string name = dynItem.Name;
                    if (dic.ContainsKey(name))
                    {
                        string description = dic[name];
                        if (!string.IsNullOrEmpty(description))
                            dynItem.Description = dic[name].Trim();
                    }
                }

                Controller.BeerMetaDataController.PrettyWrite(table, @"C:\temp\hopsdescription.json");
                Controller.BeerMetaDataController.PrettyWrite(dyn, @"C:\temp\Hops_description.json");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonNameDescription_Click(object sender, EventArgs e)
        {
            try
            {

                Form form = new BeerDescriptionForm(_fileDic[comboBox1.Text]);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void buttonHopsWeirdo_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in textBoxHopsWeirdo.Text.Split(';'))
            {



                try
                {
                    Uri url = new Uri(item);
                    var dyn = GetHopsWerdo(url.AbsoluteUri);
                    stringBuilder.AppendLine(JsonConvert.SerializeObject(dyn, Formatting.Indented));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name);
                }
            }
            Common.FormUtilities.OutputDialogs.ShowRichTextBoxDialog("Weirdos", stringBuilder.ToString(), this.Size);

        }

        private void buttonBRLists_Click(object sender, EventArgs e)
        {

            StringBuilder sb = new StringBuilder(); 

            List<string> list = new List<string>() {
            "http://www.recipator.com/cgi-bin/recipes?section=0",
            "http://www.recipator.com/cgi-bin/recipes?section=1",
            "http://www.recipator.com/cgi-bin/recipes?section=2",
            "http://www.recipator.com/cgi-bin/recipes?section=3",

        };
            foreach (string urlStr in list)
            { 
                try
                {  Uri url = new Uri(urlStr);
                var temp = Kolibri.Common.Utilities.HTMLUtilities.GetHTML(url);
                    //     DataSet test02 = Kolibri.Common.Utilities.DataSetUtilities.HTMLULToDataSet(temp);

                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(temp);

                    HtmlAgilityPack.HtmlNodeCollection tables = null;

                    if (doc.DocumentNode != null)
                    {
                        tables = doc.DocumentNode.SelectNodes("//a");

                     



                        var hrefList = doc.DocumentNode.SelectNodes("//a")
                  .Select(p => p.GetAttributeValue("href", "not found"))
                  .Where(a=>a.Contains("item"))                  
                  .ToList(); 

                        foreach (string urlstr in hrefList) {
                            string a = $"http://www.recipator.com/{urlstr}";
                  //          sb.AppendLine(a);

                        }
                        var jsonSettings = new JsonSerializerSettings()
                        {
                            DefaultValueHandling = DefaultValueHandling.Ignore,
                            Formatting = Formatting.Indented,
                            TypeNameHandling = TypeNameHandling.All
                        };
                        //show a messagebox for each node found that shows the content of attribute "href"
                        foreach (var node in tables)
                        {var inner = node.InnerHtml;
                            var outer = node.OuterHtml;
                            var value = node.GetAttributeValue("href", "not found");
                            if (value != null && value.Contains("item"))
                            {
                                string a = $"http://www.recipator.com/{value}";
                                Profile prof = new Profile() { url = a, name = inner };
                                sb.AppendLine(JsonConvert.SerializeObject(prof, jsonSettings)+",");

                            }
                        }



                        //var refs = tables.Select("//a");
                    }

                }
                catch (Exception ex) { MessageBox.Show(ex.Message, ex.GetType().Name); }
            }
            FileUtilities.WriteStringToFile($"[{sb.ToString().TrimEnd(',')}]", @"C:\temp\recipator.json");
        }

        private void buttonBRlistRead_click(object sender, EventArgs e)
        {
            var dyn = FileUtilities.FileInfoJsonAsDynamicObject(new FileInfo(@"C:\temp\recipator.json"));
            foreach (dynamic dynItem in dyn.Children())
            {

               

            }
        }
    }
}
#region stuff
/*
 

 

https://www.beercraftr.com/beer-yeast-list/
 https://punkapi.com/documentation/v2

https://www.hopslist.com/hops/   detaljer https://www.hopslist.com/hops/dual-purpose-hops/challenger/
https://www.hopslist.com/hops/dual-purpose-hops/challenger/
https://www.hopslist.com/style-guide/

 

begynn å lage en ølklasse
name
description
style
url - lenke til visning

 */
#endregion