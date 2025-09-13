using System.Drawing.Drawing2D;

namespace Kolibri.net.Common.FormUtilities.Tools
{
    public class ProgressBarHelper
    {
        public static Progress<int> InitProgressBar(ToolStripProgressBar pb)
        {
            pb.Visible = true;
            pb.Minimum = 0;
            pb.Maximum = 100;
            pb.Value = 0;
            Progress<int> progress = new Progress<int>(value =>
            {

                if (pb.Value.Equals(value)) { return; }

                if (value <= 100 && value >=0)
                {
                    pb.Value = Math.Abs(value);
                }

                else pb.Value = 100;
                if (!pb.Visible) pb.Visible = true;
                ;
                try
                {
                    

                    using (Graphics gr = pb.ProgressBar.CreateGraphics())
                    {
                        //Switch to Antialiased drawing for better (smoother) graphic results
                        gr.SmoothingMode = SmoothingMode.AntiAlias;
                        gr.DrawString(value.ToString() + "%",
                            SystemFonts.DefaultFont,
                            Brushes.Black,
                            new PointF(pb.Width / 2 - (gr.MeasureString(value.ToString() + "%",
                                SystemFonts.DefaultFont).Width / 2.0F),
                            pb.Height / 2 - (gr.MeasureString(value.ToString() + "%",
                                SystemFonts.DefaultFont).Height / 2.0F)));
                    }
                    Thread.Sleep(3);
                }
                catch (Exception ex)
                { }
            });
            return progress;
        }

        public static int CalculatePercent(long currentLength, long totalLength)
        {
            if (totalLength < 0) return 0;

            int percentage = (int)((currentLength / (double)totalLength) * 100.0);
            if (percentage >= 100) percentage = 100;      
            return percentage;
        }
    }
}
