using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pro.Model;

namespace Pro.Data.Repositorys.Implements
{
    public class FileStoryRepository : IFileStoryRepository
    {
        private readonly IMongoCollection<FileStory> _fileStory;
        public FileStoryRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionStringAppSetting);
            var database = client.GetDatabase(settings.DatabaseName);
            _fileStory = database.GetCollection<FileStory>(settings.XStorysCollectionFileStory);
        }
        public IMongoQueryable<FileStory> GetAll() => _fileStory.AsQueryable().Where(comment => true);

        public FileStory GetById(int id) => _fileStory.Find(comment => comment.Id == id).FirstOrDefault();

        public FileStory Create(FileStory comment)
        {
            var Id = 1 + _fileStory.AsQueryable().Count();
            comment.Id = Id;
            _fileStory.InsertOne(comment);
            return comment;
        }

        public List<FileStory> Creates(List<FileStory> storyFollows)
        {
            var Id = 1 + _fileStory.AsQueryable().Count();
            foreach (var c in storyFollows)
            {
                c.Id = Id++;
            }
            _fileStory.InsertMany(storyFollows);
            return storyFollows;
        }

        public void Update(int id, FileStory updatedComment) => _fileStory.ReplaceOne(comment => comment.Id == id, updatedComment);

        public void Delete(FileStory commentForDeletion) => _fileStory.DeleteOne(comment => comment.Id == commentForDeletion.Id);

        public void Delete(int id) => _fileStory.DeleteOne(comment => comment.Id == id);

        public void Update2(int id, FileStory updatedentity)
        {
            throw new NotImplementedException();
        }
    }
}
