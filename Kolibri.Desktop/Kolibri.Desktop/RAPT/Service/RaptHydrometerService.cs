using Kolibri.Desktop.RAPT.Model;
using Kolibri.Desktop.RAPT.Repository;
using System.Data.Common;

namespace Kolibri.Desktop.RAPT.Service
{
    public class RaptHydrometerService
    {
        private readonly string _dbonnectionstring;

        public RaptHydrometerService(string dbconnectionstring = null)
        {
            this._dbonnectionstring = dbconnectionstring;
        }
        public bool Add(RaptHydrometer RaptHydrometer)
        {
            bool isAdded = false;
            try
            {
                RaptHydrometerRepository RaptHydrometerRepository = new RaptHydrometerRepository();
                isAdded = RaptHydrometerRepository.Insert(RaptHydrometer);
            }
            catch (Exception ex)
            {
            }
            return isAdded;
        }

        public List<RaptHydrometer> GetAll()
        {
            List<RaptHydrometer> RaptHydrometers = new List<RaptHydrometer>();
            try
            {
                RaptHydrometerRepository RaptHydrometerRepository = new RaptHydrometerRepository();
                RaptHydrometers = RaptHydrometerRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return RaptHydrometers;
        }

        public RaptHydrometer Get(int Id)
        {
            RaptHydrometer RaptHydrometer = new RaptHydrometer();
            try
            {
                RaptHydrometerRepository RaptHydrometerRepository = new RaptHydrometerRepository();
                RaptHydrometer = RaptHydrometerRepository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return RaptHydrometer;
        }

        public bool Update(RaptHydrometer RaptHydrometer)
        {
            bool isUpdated = false;
            try
            {
                RaptHydrometerRepository RaptHydrometerRepository = new RaptHydrometerRepository();
                isUpdated = RaptHydrometerRepository.Update(RaptHydrometer);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }

        public bool Delete(RaptHydrometer RaptHydrometer)
        {
            bool isDeleted = false;
            try
            {
                RaptHydrometerRepository RaptHydrometerRepository = new RaptHydrometerRepository();
                isDeleted = RaptHydrometerRepository.Delete(RaptHydrometer);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }
    }
}
