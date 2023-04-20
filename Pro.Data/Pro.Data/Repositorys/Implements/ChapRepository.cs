using MongoDB.Driver;
using Pro.Model;

namespace Pro.Data.Repositorys.Implements
{
    public class ChapRepository : IChapRepository
    {
        private readonly IMongoCollection<Chap> _chaps;
        public ChapRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionStringMain);
            var database = client.GetDatabase(settings.DatabaseName);
            _chaps = database.GetCollection<Chap>(settings.XStorysCollectionChap);
        }
        public IQueryable<Chap> GetAll() => _chaps.AsQueryable().Where(chap => true);
        //public List<Chap> GetAll() => _chaps.Find(chap => true).ToList();

        public Chap GetById(int id) => _chaps.Find(chap => chap.ChapId == id).FirstOrDefault();

        public Chap Create(Chap chap)
        {
            var chapId = 1 + _chaps.AsQueryable().Count();
            chap.ChapId = chapId;
            _chaps.InsertOne(chap);
            return chap;
        }

        public List<Chap> Creates(List<Chap> chaps)
        {
            _chaps.InsertMany(chaps);
            return chaps;
        }

        public void Update(int id, Chap updatedStory) => _chaps.ReplaceOne(story => story.ChapId == id, updatedStory);

        public void Delete(Chap storyForDeletion) => _chaps.DeleteOne(story => story.ChapId == storyForDeletion.ChapId);

        public void Delete(int id) => _chaps.DeleteOne(story => story.ChapId == id);
    }
}
