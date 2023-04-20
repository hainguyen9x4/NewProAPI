using MongoDB.Driver;
using Pro.Model;

namespace Pro.Data.Repositorys.Implements
{
    public class HotStoryRepository : IHotStoryRepository
    {
        private readonly IMongoCollection<HotStory> _storys;
        public HotStoryRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _storys = database.GetCollection<HotStory>(settings.XStorysCollectionStory);
        }
        public IQueryable<HotStory> GetAll() => _storys.AsQueryable().Where(hotStory => true);

        public HotStory GetById(string id) => _storys.Find(hotStory => hotStory.Id == id).FirstOrDefault();

        public HotStory Create(HotStory hotStory)
        {
            _storys.InsertOne(hotStory);
            return hotStory;
        }

        public List<HotStory> Creates(List<HotStory> storys)
        {
            _storys.InsertMany(storys);
            return storys;
        }
        public void Update(string id, HotStory updatedStory) => _storys.ReplaceOne(hotStory => hotStory.Id == id, updatedStory);

        public void Delete(HotStory storyForDeletion) => _storys.DeleteOne(hotStory => hotStory.Id == storyForDeletion.Id);

        public void Delete(string id) => _storys.DeleteOne(hotStory => hotStory.Id == id);

        public HotStory GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(int id, HotStory updatedentity)
        {
            throw new NotImplementedException();
        }
    }
}
