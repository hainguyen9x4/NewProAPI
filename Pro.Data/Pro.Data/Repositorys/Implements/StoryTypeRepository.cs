using MongoDB.Driver;
using Pro.Model;
using MongoDB.Driver.Linq;

namespace Pro.Data.Repositorys.Implements
{
    public class StoryTypeRepository : IStoryTypeRepository
    {
        private readonly IMongoCollection<StoryType> _storyType;
        public StoryTypeRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionStringAppSetting);
            var database = client.GetDatabase(settings.DatabaseName);
            _storyType = database.GetCollection<StoryType>(settings.XStorysCollectionStoryType);
        }
        public IMongoQueryable<StoryType> GetAll() => _storyType.AsQueryable().Where(storyType => true);

        public StoryType GetById(int id) => _storyType.Find(storyType => storyType.TypeID == id).FirstOrDefault();

        public StoryType Create(StoryType storyType)
        {
            var TypeID = 1 + _storyType.AsQueryable().Count();
            storyType.TypeID = TypeID;
            _storyType.InsertOne(storyType);
            return storyType;
        }

        public List<StoryType> Creates(List<StoryType> storys)
        {
            _storyType.InsertMany(storys);
            return storys;
        }
        public void Update(int id, StoryType storyType)// => _storyType.ReplaceOne(storyType => storyType.TypeID == id, updatedStory);
        {
            _storyType.ReplaceOne(t => t.TypeID == id, storyType);
        }
        public void Delete(StoryType storyForDeletion) => _storyType.DeleteOne(storyType => storyType.TypeID == storyForDeletion.TypeID);

        public void Delete(int id) => _storyType.DeleteOne(storyType => storyType.TypeID == id);

        public void Update2(int id, StoryType updatedentity)
        {
            throw new NotImplementedException();
        }
    }
}
