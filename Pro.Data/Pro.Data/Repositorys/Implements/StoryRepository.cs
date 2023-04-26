using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pro.Model;
using static System.Net.Mime.MediaTypeNames;

namespace Pro.Data.Repositorys.Implements
{
    public class StoryRepository : IStoryRepository
    {
        private readonly IMongoCollection<Story> _storys;
        public StoryRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionStringMain);
            var database = client.GetDatabase(settings.DatabaseName);
            _storys = database.GetCollection<Story>(settings.XStorysCollectionStory);
        }
        public IMongoQueryable<Story> GetAll() => _storys.AsQueryable().Where(story => true);

        public Story GetById(int id) => _storys.Find(story => story.StoryId == id).FirstOrDefault();

        public Story Create(Story story)
        {
            var storyId = 1 + _storys.AsQueryable().Count();
            story.StoryId = storyId;
            _storys.InsertOne(story);
            return story;
        }

        public List<Story> Creates(List<Story> storys)
        {
            _storys.InsertMany(storys);
            return storys;
        }
        public void Update(int id, Story updatedStory) => _storys.ReplaceOne(story => story.StoryId == id, updatedStory);

        public void Delete(Story storyForDeletion) => _storys.DeleteOne(story => story.StoryId == storyForDeletion.StoryId);

        public void Delete(int id) => _storys.DeleteOne(story => story.StoryId == id);
    }
}
