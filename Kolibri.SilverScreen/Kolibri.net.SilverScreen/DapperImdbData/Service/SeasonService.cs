 
using Kolibri.net.SilverScreen.DapperImdbData.Repository;
using OMDbApiNet.Model;

namespace Kolibri.net.SilverScreen.DapperImdbData.Service
{
    public class SeasonService
    {
        private string dbConnectionString;

        public SeasonService(string dbConnectionString) { this.dbConnectionString = dbConnectionString.TrimEnd(';') + ";Allow User Variables=True;"; }
        public bool Add(Season Season)
        {
            bool isAdded = false;
            try
            {
                SeasonRepository SeasonRepository = new SeasonRepository(dbConnectionString);
                isAdded = SeasonRepository.Insert(Season);
            }
            catch (Exception ex)
            {
            }
            return isAdded;
        }

        public List<Season> GetAll()
        {
            List<Season> Seasons = new List<Season>();
            try
            {
                SeasonRepository SeasonRepository = new SeasonRepository(dbConnectionString);
                Seasons = SeasonRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return Seasons;
        }

        public Season Get(int Id)
        {
            Season Season = new Season();
            try
            {
                SeasonRepository SeasonRepository = new SeasonRepository(dbConnectionString);
                Season = SeasonRepository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return Season;
        }

        public bool Update(Season Season)
        {
            bool isUpdated = false;
            try
            {
                SeasonRepository SeasonRepository = new SeasonRepository(dbConnectionString);
                isUpdated = SeasonRepository.Update(Season);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }

        public bool Delete(Season Season)
        {
            bool isDeleted = false;
            try
            {
                SeasonRepository SeasonRepository = new SeasonRepository(dbConnectionString);
                isDeleted = SeasonRepository.Delete(Season);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }
    }
}
