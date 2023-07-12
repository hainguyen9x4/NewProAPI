using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pro.Model;

namespace Pro.Data.Repositorys.Implements
{
    public class StoryFollowsRepository : IStoryFollowsRepository
    {
        private readonly IMongoCollection<StoryFollow> _storyFollow;
        public StoryFollowsRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionStringAppSetting);
            var database = client.GetDatabase(settings.DatabaseName);
            _storyFollow = database.GetCollection<StoryFollow>(settings.XStorysCollectionStoryFollows);
        }
        public IMongoQueryable<StoryFollow> GetAll() => _storyFollow.AsQueryable().Where(comment => true);

        public StoryFollow GetById(int id) => _storyFollow.Find(comment => comment.Id == id).FirstOrDefault();

        public StoryFollow Create(StoryFollow comment)
        {
            var Id = 1 + _storyFollow.AsQueryable().Count();
            comment.Id = Id;
            _storyFollow.InsertOne(comment);
            return comment;
        }

        public List<StoryFollow> Creates(List<StoryFollow> storyFollows)
        {
            var Id = 1 + _storyFollow.AsQueryable().Count();
            foreach (var c in storyFollows)
            {
                c.Id = Id++;
            }
            _storyFollow.InsertMany(storyFollows);
            return storyFollows;
        }

        public void Update(int id, StoryFollow updatedComment) => _storyFollow.ReplaceOne(comment => comment.Id == id, updatedComment);

        public void Delete(StoryFollow commentForDeletion) => _storyFollow.DeleteOne(comment => comment.Id == commentForDeletion.Id);

        public void Delete(int id) => _storyFollow.DeleteOne(comment => comment.Id == id);

        public void Update2(int id, StoryFollow updatedentity)
        {
            throw new NotImplementedException();
        }
    }
}
