using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.net.C64Sorter.Controllers
{
    public class PrinterController
    {
        private static List<FileInfo> _imagesToPrint = null;
        private static int _pageIndex = 0;
        public static void PrintImage(List<FileInfo> fileinfos, bool preview = true)
        {
            _imagesToPrint = fileinfos;

            PrintDocument pd = GetPrintDocument(); 

            if (preview)
            {
                //PrintDialog printDialog = new PrintDialog();
                //if (printDialog.ShowDialog() == DialogResult.OK)
                //{
                //    PrintPreviewDialog printPreview = new PrintPreviewDialog();
                //    printPreview.Document = pd;
                //    // Apply the settings chosen in the PrintDialog
                //    printPreview.Document.PrinterSettings = printDialog.PrinterSettings;
                //    printPreview.ShowDialog();
                //}


                PrintPreviewDialog previewDialog = new PrintPreviewDialog();
                previewDialog.Document = pd;
                previewDialog.ShowDialog();
            }
            else
            {
                // This prints the document to the default printer without a dialog
                pd.Print();
            }
            try { pd.PrintPage -= PrintDocument_PrintPage; pd.Dispose(); }
            catch (Exception ex) { }
        }

        private static PrintDocument GetPrintDocument()
        {   _pageIndex = 0;
         
            PrintDocument pd = new PrintDocument();
            // Find and set a standard paper size (e.g., A4)
            PaperSize p = null;
            foreach (PaperSize ps in pd.PrinterSettings.PaperSizes)
            {
                if (ps.PaperName.Equals("A4", StringComparison.OrdinalIgnoreCase))
                {
                    p = ps;
                    pd.DefaultPageSettings.PaperSize = p;
                    break; // Exit the loop once found
                }
            }
         
            pd.PrintPage += PrintDocument_PrintPage;
            return pd;
        }

        private static  void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            // 2. Draw the image for the current page index
            Image currentImage =   Image.FromFile(_imagesToPrint[_pageIndex].FullName);

            // You can adjust drawing position and size. This example fits the image within the page margins while maintaining aspect ratio.
            var printableArea = e.MarginBounds;
            var imageSize = currentImage.Size;
            float scale = Math.Min((float)printableArea.Width / imageSize.Width, (float)printableArea.Height / imageSize.Height);

            // Position the image at the top-left of the printable area
            e.Graphics.DrawImage(currentImage,
                                 printableArea.X,
                                 printableArea.Y,
                                 imageSize.Width * scale,
                                 imageSize.Height * scale);

            // 3. Increment page index
            _pageIndex++;
            
            // 4. Indicate whether there are more pages/images to print
            e.HasMorePages = (_pageIndex < _imagesToPrint.Count);
        }
    }
}
