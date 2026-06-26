using Kolibri.Desktop.RAPT.Model;
using Kolibri.Desktop.RAPT.Repository;

namespace Kolibri.Desktop.RAPT.Service
{
    public class RaptPostsService
    {
        public bool Add(RaptPosts RaptPosts)
        {
            bool isAdded = false;
            try
            {
                RaptPostsRepository RaptPostsRepository = new RaptPostsRepository();
                isAdded = RaptPostsRepository.Insert(RaptPosts);
            }
            catch (Exception ex)
            {
            }
            return isAdded;
        }

        public List<RaptPosts> GetAll()
        {
            List<RaptPosts> RaptPostss = new List<RaptPosts>();
            try
            {
                RaptPostsRepository RaptPostsRepository = new RaptPostsRepository();
                RaptPostss = RaptPostsRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return RaptPostss;
        }

        public RaptPosts Get(int Id)
        {
            RaptPosts RaptPosts = new RaptPosts();
            try
            {
                RaptPostsRepository RaptPostsRepository = new RaptPostsRepository();
                RaptPosts = RaptPostsRepository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return RaptPosts;
        }

        public bool Update(RaptPosts RaptPosts)
        {
            bool isUpdated = false;
            try
            {
                RaptPostsRepository RaptPostsRepository = new RaptPostsRepository();
                isUpdated = RaptPostsRepository.Update(RaptPosts);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }

        public bool Delete(RaptPosts RaptPosts)
        {
            bool isDeleted = false;
            try
            {
                RaptPostsRepository RaptPostsRepository = new RaptPostsRepository();
                isDeleted = RaptPostsRepository.Delete(RaptPosts);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }
    }
}
