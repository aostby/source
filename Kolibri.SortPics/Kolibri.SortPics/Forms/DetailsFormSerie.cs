using Kolibri.Common.Utilities;
using OMDbApiNet.Model;
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kolibri.Common.MovieAPI.Controller;

namespace SortPics.Forms
{
    public partial class DetailsFormSerie : DetailsFormItem
    {
        private string seasonEpisodeId;
        SeriesCache _seriesCache;

        [Obsolete("Designer only", true)]
        public DetailsFormSerie() :base(new Item(),new LiteDBController())
        {
            InitializeComponent();

        }
        public DetailsFormSerie(Item item, LiteDBController contr, string imdbId):base (item, contr)
        {  InitializeComponent();
            seasonEpisodeId = imdbId;
            Init();
        }

        private void Init()
        {
            string plot = _item.Plot;
            pbPoster.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            pbPoster.SendToBack();

            try
            {
                var ep = _contr.GetEpisode(seasonEpisodeId);
                if (ep != null)
                    plot += $"{Environment.NewLine}S{ (string.Format("{0:D2}", ep.SeasonNumber.ToInt32()))}E{(string.Format("{0:D2}", ep.EpisodeNumber.ToInt32()))}{Environment.NewLine}{ep.Plot}";
            }
            catch (Exception)
            { } 
            tbPlot.Text = plot; 
        }
    }
}
