
using Kolibri.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.Common.FormUtilities.Forms
{
    public partial class ConvertImageForm : Form
    {
        string[] arr = "PNG,ICO,JPEG,TIFF,GIF,Raw,".Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        public ConvertImageForm()
        {
            InitializeComponent();
        }

        private void buttonLoadImage_Click(object sender, EventArgs e)
        {
            try
            {
   openFileDialog.Filter = FileUtilities.GetFileDialogFilter(arr);

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pictureBox1.Image = Bitmap.FromFile(openFileDialog.FileName);
            }
            }
            catch (Exception ex)
            {

            }
         
        }

        private void buttonSaveImage_Click(object sender, EventArgs e)
        {
           
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = FileUtilities.GetFileDialogFilter(arr);
            saveFileDialog.FileName = Path.GetFileName( openFileDialog.FileName);
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
               // pictureBox1.Image = Bitmap.FromFile(openFileDialog.FileName);
                try
                {
  //pictureBox1.Image.Save(openFileDialog.FileName, GetImageFormat(saveFileDialog.FileName));

                    new Bitmap(pictureBox1.Image).Save(openFileDialog.FileName,  GetImageFormat(saveFileDialog.FileName));
                }
                catch (Exception)
                {//new Bitmap(oldbitmap).Save(openFileDialog.FileName);
                    //Bitmap bmp = new Bitmap(pictureBox1.Image); 
                    //using (var m = new MemoryStream())
                    //{
                    //    bmp.Save(m, GetImageFormat(saveFileDialog.FileName));

                    //    var img = Image.FromStream(m);

                    //    //TEST
                    //    img.Save(openFileDialog.FileName);
                    //  //  var bytes = PhotoEditor.ConvertImageToByteArray(img);


                    //    //return img;
                    //}
                }
                FileUtilities.OpenFolderHighlightFile(new FileInfo(saveFileDialog.FileName)); 
            }
        }
        private static ImageFormat GetImageFormat(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentException(
                    string.Format("Unable to determine file extension for fileName: {0}", fileName));

            switch (extension.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".ico":
                    return ImageFormat.Icon;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                case @".tif":
                case @".tiff":
                    return ImageFormat.Tiff;

                case @".wmf":
                    return ImageFormat.Wmf;

                default:
                    throw new NotImplementedException();
            }
        }
        public static string GetFilenameExtension(ImageFormat format)
        {
            return ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == format.Guid).FilenameExtension;
        }

    }
}