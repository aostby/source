

using Dapper;
using Kolibri.net.SilverScreen.DapperImdbData.Repository;
using LiteDB;
using MySql.Data.MySqlClient;
using OMDbApiNet.Model;

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

        public Item Get(string Id)
        {
            Item Item = new Item();
            try
            {
                ItemRepository ItemRepository = new ItemRepository(dbConnectionString);
                Item = ItemRepository.GetById(Id);
            }
            catch (Exception ex)
            {
                Item = null;
            }

            return Item;
        }

        public Item GetItemFromMysql(string Id)
        {
            string sql = $@"SELECT 
st.primarytitle as Title,
CAST(st.startYear as int) as Year ,
convert( st.startYear, varchar (4))+'01-01' as Released ,
st.runtimeMinutes, 
st.genres as Genre,
n.primaryName as Director, 
n.primaryName  as Writer,  
'TV-MA' as Rated, 
r.averageRating as ImdbRating,
r.numVotes as ImdbVotes,
st.id as ImdbId,
/* st.titleType  as Type,*/
'series' as type,
max(e.seasonNumber) as TotalSeasons
FROM  titles AS st
INNER JOIN       episodes  e ON ( e.parentId  = st.id )
LEFT  OUTER JOIN ratings   r ON ( st.id = r.id )
LEFT  OUTER JOIN principals   p ON ( st.id = p.title_id and category = 'writer' )
LEFT  OUTER JOIN names   n ON ( p.name_id  = n.name_id  )
-- WHERE st.primarytitle = 'Better Off Ted'
AND   st.titleType  = 'tvSeries'
where st.Id = '{Id}'";

            using (var connection = new MySqlConnection(dbConnectionString))
            {
                connection.Open();
                var students = connection.Query<Item>(sql).ToList();
                return students.FirstOrDefault();
            }
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
