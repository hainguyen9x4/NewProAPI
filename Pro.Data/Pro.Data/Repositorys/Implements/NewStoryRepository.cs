using MongoDB.Driver;
using Pro.Model;
using MongoDB.Driver.Linq;

namespace Pro.Data.Repositorys.Implements
{
    public class NewStoryRepository : INewStoryRepository
    {
        private readonly IMongoCollection<NewStory> _newStorys;
        public NewStoryRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionStringMain);
            var database = client.GetDatabase(settings.DatabaseName);
            _newStorys = database.GetCollection<NewStory>(settings.XStorysCollectionNewStory);
        }
        public IMongoQueryable<NewStory> GetAll() => _newStorys.AsQueryable().Where(newStory => true);

        public NewStory GetById(int id) => _newStorys.Find(newStory => newStory.ID == id).FirstOrDefault();

        public NewStory Create(NewStory newStory)
        {
            var ID = 1 + _newStorys.AsQueryable().Count();
            newStory.ID = ID;
            _newStorys.InsertOne(newStory);
            return newStory;
        }

        public List<NewStory> Creates(List<NewStory> storys)
        {
            _newStorys.InsertMany(storys);
            return storys;
        }

        public void Update(int id, NewStory updatedentity)
        {
            foreach (var chap in updatedentity.Chaps)
                AddToChap(id, chap);
            UpdateLastModifyTime(id);
        }

        public void Update2(int id, NewStory updatedStory)
        {
            _newStorys.ReplaceOne(newStory => newStory.ID == id, updatedStory);
        }
        public void Delete(NewStory storyForDeletion) => _newStorys.DeleteOne(newStory => newStory.ID == storyForDeletion.ID);

        public void Delete(int id) => _newStorys.DeleteOne(newStory => newStory.ID == id);

        private void AddToChap(int storyID, Chap newChap)
        {
            var itemFilter = Builders<NewStory>.Filter.Eq(v => v.ID, storyID);
            var updateBuilder = Builders<NewStory>.Update.AddToSet(items => items.Chaps, newChap);
            _newStorys.UpdateOneAsync(itemFilter, updateBuilder, new UpdateOptions() { IsUpsert = true }).Wait();
        }
        private void UpdateLastModifyTime(int storyID)
        {
            var itemFilter = Builders<NewStory>.Filter.Eq(v => v.ID, storyID);
            var updateBuilder = Builders<NewStory>.Update.Set(item => item.UpdatedTime, DateTime.UtcNow);
            _newStorys.UpdateOneAsync(itemFilter, updateBuilder, new UpdateOptions() { IsUpsert = false }).Wait();
        }
    }
}
