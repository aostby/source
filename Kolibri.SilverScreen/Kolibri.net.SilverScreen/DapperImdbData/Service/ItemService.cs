

using OMDbApiNet.Model;
using Kolibri.net.SilverScreen.DapperImdbData.Repository;

namespace Kolibri.net.SilverScreen.DapperImdbData.Service
{
    public class ItemService
    {
        private string dbConnectionString;

        public ItemService(string dbConnectionString) {
            this.dbConnectionString = dbConnectionString.TrimEnd(';') + ";Allow User Variables=True;";

        }
        public bool Add(Item Item)
        {
            bool isAdded = false;
            try
            {
                ItemRepository ItemRepository = new ItemRepository(dbConnectionString);
                isAdded = ItemRepository.Insert(Item);
            }
            catch (Exception ex)
            {
            }
            return isAdded;
        }
        public bool Update(Item Item)
        {
            bool isUpdated = false;
            try
            {
                ItemRepository ItemRepository = new ItemRepository(dbConnectionString);
                isUpdated = ItemRepository.Update(Item);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }
        public bool Upsert(Item item) 
        { bool ret = Add(item);
            if (!ret) {
            ret = Update(item);
            }
            return ret; 
        }

        public List<Item> GetAll()
        {
            List<Item> Items = new List<Item>();
            try
            {
                ItemRepository ItemRepository = new ItemRepository(dbConnectionString);
                Items = ItemRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return Items;
        }

        public Item Get(int Id)
        {
            Item Item = new Item();
            try
            {
                ItemRepository ItemRepository = new ItemRepository(dbConnectionString);
                Item = ItemRepository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return Item;
        }

      

        public bool Delete(Item Item)
        {
            bool isDeleted = false;
            try
            {
                ItemRepository ItemRepository = new ItemRepository(dbConnectionString);
                isDeleted = ItemRepository.Delete(Item);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }
    }
}
