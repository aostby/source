
                using DapperGenericRepository.Repository;
using Kolibri.Desktop.RAPT.Model;
namespace Kolibri.Desktop.RAPT.Repository
{ public class RaptHydrometerRepository(string dbconnectionstring = null) : GenericRepository<RaptHydrometer>(dbconnectionstring) { } 
}
