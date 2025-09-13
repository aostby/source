
using com.sun.org.apache.xpath.@internal.operations;
using com.sun.tools.javac.comp;
using Kolibri.net.SilverScreen.DapperImdbData.Repository;

using OMDbApiNet.Model;

namespace Kolibri.net.SilverScreen.DapperImdbData.Service
{
    public class SeasonEpisodeService
    {
        private string dbConnectionString;
        /// <summary>
        /// User Defined Variables in MySQL/MariaDB
        ///In order to use Non-parameter SQL variables with MySql Connector, you have to add the following option to your connection string:
        ///Allow User Variables=True
        //Make sure you don't provide Dapper with a property to map.
        /// </summary>
        /// <param name="dbConnectionString">connectionstring which will be padded with allow user variables</param>
        public SeasonEpisodeService(string dbConnectionString)
        {

            this.dbConnectionString = dbConnectionString.TrimEnd(';') + ";Allow User Variables=True;";

        }
        public bool Add(SeasonEpisode SeasonEpisode)
        {
            bool isAdded = false;
            try
            {
                SeasonEpisodeRepository SeasonEpisodeRepository = new SeasonEpisodeRepository(dbConnectionString);
                isAdded = SeasonEpisodeRepository.Insert(SeasonEpisode);
            }
            catch (Exception ex)
            {
            }
            return isAdded;
        }

        public List<SeasonEpisode> GetAll()
        {
            List<SeasonEpisode> SeasonEpisodes = new List<SeasonEpisode>();
            try
            {
                SeasonEpisodeRepository SeasonEpisodeRepository = new SeasonEpisodeRepository(dbConnectionString);
                SeasonEpisodes = SeasonEpisodeRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return SeasonEpisodes;
        }

        public SeasonEpisode Get(string Id)
        {
            SeasonEpisode SeasonEpisode = new SeasonEpisode();
            try
            {
                SeasonEpisodeRepository SeasonEpisodeRepository = new SeasonEpisodeRepository(dbConnectionString);
                SeasonEpisode = SeasonEpisodeRepository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return SeasonEpisode;
        }

        public bool Update(SeasonEpisode SeasonEpisode)
        {
            bool isUpdated = false;
            try
            {
                SeasonEpisodeRepository SeasonEpisodeRepository = new SeasonEpisodeRepository(dbConnectionString);
                isUpdated = SeasonEpisodeRepository.Update(SeasonEpisode);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }

        public bool Delete(SeasonEpisode SeasonEpisode)
        {
            bool isDeleted = false;
            try
            {
                SeasonEpisodeRepository SeasonEpisodeRepository = new SeasonEpisodeRepository(dbConnectionString);
                isDeleted = SeasonEpisodeRepository.Delete(SeasonEpisode);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }
    }
}