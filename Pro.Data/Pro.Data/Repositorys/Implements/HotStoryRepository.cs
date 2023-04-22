using MongoDB.Driver;
using Pro.Model;

namespace Pro.Data.Repositorys.Implements
{
    public class HotStoryRepository : IHotStoryRepository
    {
        private readonly IMongoCollection<HotStory> _hotStorys;
        public HotStoryRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionStringAppSetting);
            var database = client.GetDatabase(settings.DatabaseName);
            _hotStorys = database.GetCollection<HotStory>(settings.XStorysCollectionStory);
        }
        public IQueryable<HotStory> GetAll() => _hotStorys.AsQueryable().Where(hotStory => true);

        public HotStory GetById(int id) => _hotStorys.Find(hotStory => hotStory.Id == id).FirstOrDefault();

        public HotStory Create(HotStory hotStory)
        {
            var id = 1 + _hotStorys.AsQueryable().Count();
            hotStory.Id = id;
            _hotStorys.InsertOne(hotStory);
            return hotStory;
        }

        public List<HotStory> Creates(List<HotStory> storys)
        {
            _hotStorys.InsertMany(storys);
            return storys;
        }
        public void Update(int id, HotStory updatedStory) => _hotStorys.ReplaceOne(hotStory => hotStory.Id == id, updatedStory);

        public void Delete(HotStory storyForDeletion) => _hotStorys.DeleteOne(hotStory => hotStory.Id == storyForDeletion.Id);

        public void Delete(int id) => _hotStorys.DeleteOne(hotStory => hotStory.Id == id);
    }
}
