
                using DapperGenericRepository.Repository;
using OMDbApiNet.Model;
namespace Kolibri.net.SilverScreen.DapperImdbData.Repository
{
    public class SeasonRepository(string dbConnecitonString) : GenericRepository<Season >(dbConnecitonString) { }
}
