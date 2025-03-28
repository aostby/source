using OMDbApiNet.Model;
using OMDbApiNet.Model;
using Kolibri.net.SilverScreen.DapperImdbData.Repository;


namespace Kolibri.net.SilverScreen.DapperImdbData.Service
{
    public class EpisodeService
    {
        private string dbConnectionString;

        public EpisodeService(string dbConnectionString) { this.dbConnectionString = dbConnectionString.TrimEnd(';') + ";Allow User Variables=True;"; } 
        public bool Add(Episode Episode)
        {
            bool isAdded = false;
            try
            {
                EpisodeRepository EpisodeRepository = new EpisodeRepository(dbConnectionString);
                isAdded = EpisodeRepository.Insert(Episode);
            }
            catch (Exception ex)
            {
            }
            return isAdded;
        }

        public List<Episode> GetAll()
        {
            List<Episode> Episodes = new List<Episode>();
            try
            {
                EpisodeRepository EpisodeRepository = new EpisodeRepository(dbConnectionString);
                Episodes = EpisodeRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return Episodes;
        }

        public Episode Get(int Id)
        {
            Episode Episode = new Episode();
            try
            {
                EpisodeRepository EpisodeRepository = new EpisodeRepository(dbConnectionString);
                Episode = EpisodeRepository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return Episode;
        }

        public bool Update(Episode Episode)
        {
            bool isUpdated = false;
            try
            {
                EpisodeRepository EpisodeRepository = new EpisodeRepository(dbConnectionString);
                isUpdated = EpisodeRepository.Update(Episode);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }

        public bool Delete(Episode Episode)
        {
            bool isDeleted = false;
            try
            {
                EpisodeRepository EpisodeRepository = new EpisodeRepository(dbConnectionString);
                isDeleted = EpisodeRepository.Delete(Episode);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }
    }
}
