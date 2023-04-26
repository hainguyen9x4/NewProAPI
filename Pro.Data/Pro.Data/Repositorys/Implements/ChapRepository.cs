using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pro.Model;

namespace Pro.Data.Repositorys.Implements
{
    public class ChapRepository : IChapRepository
    {
        private readonly IMongoCollection<OldChap> _chaps;
        public ChapRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionStringMain);
            var database = client.GetDatabase(settings.DatabaseName);
            _chaps = database.GetCollection<OldChap>(settings.XStorysCollectionChap);
        }
        public IMongoQueryable<OldChap> GetAll() => _chaps.AsQueryable().Where(chap => true);
        //public List<Chap> GetAll() => _chaps.Find(chap => true).ToList();

        public OldChap GetById(int id) => _chaps.Find(chap => chap.ChapId == id).FirstOrDefault();

        public OldChap Create(OldChap chap)
        {
            var chapId = 1 + _chaps.AsQueryable().Count();
            chap.ChapId = chapId;
            _chaps.InsertOne(chap);
            return chap;
        }

        public List<OldChap> Creates(List<OldChap> chaps)
        {
            _chaps.InsertMany(chaps);
            return chaps;
        }

        public void Update(int id, OldChap updatedStory) => _chaps.ReplaceOne(story => story.ChapId == id, updatedStory);

        public void Delete(OldChap storyForDeletion) => _chaps.DeleteOne(story => story.ChapId == storyForDeletion.ChapId);

        public void Delete(int id) => _chaps.DeleteOne(story => story.ChapId == id);
    }
}
