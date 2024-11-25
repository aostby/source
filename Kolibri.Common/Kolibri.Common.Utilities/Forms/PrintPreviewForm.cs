using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kolibri.Common.Utilities.Forms
{  
  public  class PrintPreviewForm
    {
        private static  Image _img;
        internal static System.Windows.Forms.PrintPreviewDialog PrintPreviewDialog1;
        // Declare a PrintDocument object named document.
        private System.Drawing.Printing.PrintDocument document =
            new System.Drawing.Printing.PrintDocument();

        public static void PrintPreview(Image image)
        {
            _img = image;
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintImage);
            PrintPreviewDialog pdi = new PrintPreviewDialog();
            pdi.Document = pd;
            if (pdi.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
            else
            {
                MessageBox.Show("Print Cancelled");
            }
        }

        static void PrintImage(object o, PrintPageEventArgs e)
        {
            int x = SystemInformation.WorkingArea.X;
            int y = SystemInformation.WorkingArea.Y;
            int width = _img.Width;
            int height = _img.Height;

            Rectangle bounds = new Rectangle(x, y, width, height);

            Bitmap img = _img as Bitmap;         
            Point p = new Point(100, 100);
            e.Graphics.DrawImage(img, p);
        }

        public static void PrintPreview(FileInfo fileInfo)
        {
            //https://stackoverflow.com/questions/19662130/print-image-using-windows-print-image-dialog

            var p = new Process();
            p.StartInfo.FileName = fileInfo.FullName;
            p.StartInfo.Verb = "Print";
            p.Start();
        }
    }
}
