using Kolibri.Desktop.RAPT.Model;
using Kolibri.Desktop.RAPT.Repository;

namespace Kolibri.Desktop.RAPT.Service
{
    public class RaptTokenService
    {
        public bool Add(RaptToken RaptToken)
        {
            bool isAdded = false;
            try
            {
                RaptTokenRepository RaptTokenRepository = new RaptTokenRepository();
                isAdded = RaptTokenRepository.Insert(RaptToken);
            }
            catch (Exception ex)
            {
            }
            return isAdded;
        }

        public List<RaptToken> GetAll()
        {
            List<RaptToken> RaptTokens = new List<RaptToken>();
            try
            {
                RaptTokenRepository RaptTokenRepository = new RaptTokenRepository();
                RaptTokens = RaptTokenRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return RaptTokens;
        }

        public RaptToken Get(int Id)
        {
            RaptToken RaptToken = new RaptToken();
            try
            {
                RaptTokenRepository RaptTokenRepository = new RaptTokenRepository();
                RaptToken = RaptTokenRepository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return RaptToken;
        }

        public bool Update(RaptToken RaptToken)
        {
            bool isUpdated = false;
            try
            {
                RaptTokenRepository RaptTokenRepository = new RaptTokenRepository();
                isUpdated = RaptTokenRepository.Update(RaptToken);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }

        public bool Delete(RaptToken RaptToken)
        {
            bool isDeleted = false;
            try
            {
                RaptTokenRepository RaptTokenRepository = new RaptTokenRepository();
                isDeleted = RaptTokenRepository.Delete(RaptToken);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }
    }
}
