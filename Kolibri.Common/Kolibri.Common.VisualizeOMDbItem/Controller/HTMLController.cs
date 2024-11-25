using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.Common.VisualizeOMDbItem.Controller
{
  public   class HTMLController
    {

        public static string GetMovieDiv() {

            string ret = $@"	<div class=""browse-movie-wrap col-xs-10 col-sm-5"">
		<a href=""https://yts.mx/movies/legacy-of-love-2021"" class=""browse-movie-link"">
			<figure>
				<img class=""img-responsive"" src=""/assets/images/movies/legacy_of_love_2021/medium-cover.jpg"" alt=""Legacy of Love (2021) download"" width=""210"" height=""315""/>
				<img class=""quality-banner img-responsive"" src=""/assets/images/website/banner2160p.png"" alt=""Legacy of Love (2021) download 2160p"" width=""118"" height=""91"">
					<figcaption class=""hidden-xs hidden-sm"">
						<span class=""icon-star""/>
						<h4 class=""rating"">6 / 10</h4>
						<h4>Drama</h4>
						<span class=""button-green-download2-big"">View Details</span>
					</figcaption>
				</figure>
			</a>
			<div class=""browse-movie-bottom"">
				<a href=""https://yts.mx/movies/legacy-of-love-2021"" class=""browse-movie-title"">Legacy of Love</a>
				<div class=""browse-movie-year"">2021</div>
			</div>
		</div>
	</div>";
			return ret;

        }

    }
}
