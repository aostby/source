 
using MySql.Data.MySqlClient;
using OMDbApiNet.Model;
using System.Collections.Generic;
using Z.Dapper.Plus;

using Kolibri.net.Common.Dal.Controller;

using Kolibri.net.Common.Dal.Entities;
using Kolibri.net.Common.Utilities;
using Kolibri.net.Common.Utilities.Extensions;
using OMDbApiNet.Model;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kolibri.net.Common.Dal.DapperGenericRepository.Controller;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using TMDbLib.Objects.TvShows;
using DapperGenericRepository.Controller;
using System.Diagnostics;
using percentCalc = Kolibri.net.Common.FormUtilities.Tools.ProgressBarHelper;
using DapperGenericRepository.Service;

using System.Data;
using System.Data.SqlClient;
namespace Kolibri.net.Common.Dal.DapperGenericRepository.Controller
{
	public class DapperBulkInsertController
	{	
        private MySqlConnection _connection;
        public DapperBulkInsertController(string dbConnectionString) {
            var connStr = dbConnectionString.TrimEnd(';') + ";Allow User Variables=True;";

            _connection = new MySqlConnection(connStr);
		}
        public async Task<DapperPlusActionSet<List<T>>>   BulkInsert<T>(List<T> list) //where T : MyClassBase
        {
			Type type = typeof(T);
			// Map your entity
			DapperPlusManager.Entity<Episode>().Table("Episode").Identity("ImdbId").Key(x => x.ImdbId); 
			DapperPlusManager.Entity<Item>().Table(nameof(Item)).Identity((x => x.ImdbId)).Key(x => x.ImdbId);
			DapperPlusManager.Entity<SeasonEpisode>().Table(nameof(SeasonEpisode)).Identity((x => x.ImdbId)).Key(x => x.ImdbId);
            DapperPlusManager.Entity<Season>().Table(nameof(Season)).Identity(x => x.Title).Key(x => x.Title);
            
			// Bulk Insert
            

				//var res = _connection.UseBulkOptions(options => options.InsertIfNotExists = true).BulkInsert(episodes);
				var res = _connection.UseBulkOptions(options => options.InsertIfNotExists = true).BulkInsert(list);

                return res;
			

		}
	}
}
