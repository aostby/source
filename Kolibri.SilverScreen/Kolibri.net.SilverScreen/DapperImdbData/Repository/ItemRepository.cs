
using OMDbApiNet.Model;


using DapperGenericRepository.Repository;
namespace Kolibri.net.SilverScreen.DapperImdbData.Repository
{
    public class ItemRepository(string dbConnecitonString) : GenericRepository<Item>(dbConnecitonString) { }
}

