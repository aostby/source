
                using DapperGenericRepository.Repository;
using OMDbApiNet.Model;

namespace Kolibri.net.SilverScreen.DapperImdbData.Repository
{
    public class SeasonEpisodeRepository(string dbConnecitonString) : GenericRepository<SeasonEpisode>(dbConnecitonString) { }
}
