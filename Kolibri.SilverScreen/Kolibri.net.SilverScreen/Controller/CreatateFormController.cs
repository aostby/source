using Kolibri.net.Common.Dal.Controller;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using OMDbApiNet.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TMDbLib.Objects.Movies;


namespace Kolibri.net.SilverScreen.Controller
{
    internal class CreatateFormController
    {
        internal static async Task<Form> GenerateFormFromActors(TMDbLib.Objects.Movies.Credits credits, Item item, ImageCacheDB icache = null)
        {
            if (credits == null) return null;
       
            var theList = credits.Cast.OrderBy(x => x.Order).Take(100);

            var ds = DataSetUtilities.AutoGenererDataSet(theList.ToList<Cast>());

            DataTable dt = new DataView(ds.Tables[0], null, "Order ASC", DataViewRowState.CurrentRows).ToTable(true, "Character", "Name", "Gender", "KnownForDepartment", "OriginalName", "ProfilePath");
            dt.TableName = DataSetUtilities.LegalTableName("Actors");

            if (dt.DataSet == null)
            {
                DataSet tmp = new DataSet();
                tmp.Tables.Add(dt);
            }

            DataColumn imageColumn = new DataColumn("Image");
            imageColumn.DataType = typeof(Image); // or System.Type.GetType("System.Byte[]");
            imageColumn.AllowDBNull = true; // Set to false if an image is always required
            imageColumn.Caption = "Image"; // Optional: A user-friendly caption
            dt.Columns.Add(imageColumn);
            imageColumn.SetOrdinal(0);
            foreach (DataRow row in dt.Rows)
            {
                var url = $"{TMDBController.TMDBImageBasePath200}{($"{row["ProfilePath"]}")}";
                if (icache != null)
                {
                    try
                    {
                        var img = await icache.FindImageAsync(url);
                        if (img != null)
                        {                            
                            row["Image"] = img.Image;
                        }
                        else {
                            var tmp = await ImageUtilities.GetImageFromUrlAsync(url);
                            if (tmp != null) {
                                row["Image"] = tmp;
                                _ = await icache.InsertImageAsync(url, img.Image);
                            }
                        }
                    }
                    catch (Exception picex)
                    {
                        row["Image"] = ImageUtilities.Base64ToImage(ImageUtilities.DefaultMissingPerson); // DBNull.Value;
                    }
                }
                else
                {

                    try
                    {
                        row["Image"] = await ImageUtilities.GetImageFromUrlAsync(url);
                    }
                    catch (Exception picex)
                    {
                        row["Image"] = ImageUtilities.Base64ToImage(ImageUtilities.DefaultMissingPerson); // DBNull.Value;
                    }
                }
            }

            Form form = Common.FormUtilities.Controller.OutputFormController.DataTableForm(item.Title, dt, dt.Columns["Image"], new Size(500,500));
            form.Text = $"{form.Text} - {dt.Rows.Count} {dt.TableName}";
            return form;
        }

    }
}
