using MongoDB.Driver;
using Pro.Model;

namespace Pro.Data.Repositorys.Implements
{
    public class NewStoryRepository : INewStoryRepository
    {
        private readonly IMongoCollection<NewStory> _storys;
        public NewStoryRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionStringMain);
            var database = client.GetDatabase(settings.DatabaseName);
            _storys = database.GetCollection<NewStory>(settings.XStorysCollectionNewStory);
        }
        public IQueryable<NewStory> GetAll() => _storys.AsQueryable().Where(newStory => true);

        public NewStory GetById(int id) => _storys.Find(newStory => newStory.ID == id).FirstOrDefault();

        public NewStory Create(NewStory newStory)
        {
            var ID = 1 + _storys.AsQueryable().Count();
            newStory.ID = ID;
            _storys.InsertOne(newStory);
            return newStory;
        }

        public List<NewStory> Creates(List<NewStory> storys)
        {
            _storys.InsertMany(storys);
            return storys;
        }
        public void Update(int id, NewStory updatedStory)// => _storys.ReplaceOne(newStory => newStory.ID == id, updatedStory);
        {
            foreach (var chap in updatedStory.Chaps)
                AddToChap(id, chap);
        }
        public void Delete(NewStory storyForDeletion) => _storys.DeleteOne(newStory => newStory.ID == storyForDeletion.ID);

        public void Delete(int id) => _storys.DeleteOne(newStory => newStory.ID == id);

        private void AddToChap(int storyID, Chap newChap)
        {
            var itemFilter = Builders<NewStory>.Filter.Eq(v => v.ID, storyID);
            var updateBuilder = Builders<NewStory>.Update.AddToSet(items => items.Chaps, newChap);
            _storys.UpdateOneAsync(itemFilter, updateBuilder, new UpdateOptions() { IsUpsert = true }).Wait();
        }
    }
}
