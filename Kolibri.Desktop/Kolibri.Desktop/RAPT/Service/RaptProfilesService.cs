using Kolibri.Desktop.RAPT.Model;
using Kolibri.Desktop.RAPT.Repository;

namespace Kolibri.Desktop.RAPT.Service
{
    public class RaptProfilesService
    {
        public bool Add(RaptProfiles RaptProfiles)
        {
            bool isAdded = false;
            try
            {
                RaptProfilesRepository RaptProfilesRepository = new RaptProfilesRepository();
                isAdded = RaptProfilesRepository.Insert(RaptProfiles);
            }
            catch (Exception ex)
            {
            }
            return isAdded;
        }

        public List<RaptProfiles> GetAll()
        {
            List<RaptProfiles> RaptProfiless = new List<RaptProfiles>();
            try
            {
                RaptProfilesRepository RaptProfilesRepository = new RaptProfilesRepository();
                RaptProfiless = RaptProfilesRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return RaptProfiless;
        }

        public RaptProfiles Get(int Id)
        {
            RaptProfiles RaptProfiles = new RaptProfiles();
            try
            {
                RaptProfilesRepository RaptProfilesRepository = new RaptProfilesRepository();
                RaptProfiles = RaptProfilesRepository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return RaptProfiles;
        }

        public bool Update(RaptProfiles RaptProfiles)
        {
            bool isUpdated = false;
            try
            {
                RaptProfilesRepository RaptProfilesRepository = new RaptProfilesRepository();
                isUpdated = RaptProfilesRepository.Update(RaptProfiles);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }

        public bool Delete(RaptProfiles RaptProfiles)
        {
            bool isDeleted = false;
            try
            {
                RaptProfilesRepository RaptProfilesRepository = new RaptProfilesRepository();
                isDeleted = RaptProfilesRepository.Delete(RaptProfiles);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }
    }
}
