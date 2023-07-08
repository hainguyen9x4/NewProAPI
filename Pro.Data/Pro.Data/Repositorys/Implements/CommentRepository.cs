using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pro.Model;

namespace Pro.Data.Repositorys.Implements
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IMongoCollection<Comment> _comment;
        public CommentRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionStringAppSetting);
            var database = client.GetDatabase(settings.DatabaseName);
            _comment = database.GetCollection<Comment>(settings.XStorysCollectionComment);
        }
        public IMongoQueryable<Comment> GetAll() => _comment.AsQueryable().Where(comment => true);

        public Comment GetById(int id) => _comment.Find(comment => comment.Id == id).FirstOrDefault();

        public Comment Create(Comment comment)
        {
            var Id = 1 + _comment.AsQueryable().Count();
            comment.Id = Id;
            _comment.InsertOne(comment);
            return comment;
        }

        public List<Comment> Creates(List<Comment> comments)
        {
            var Id = 1 + _comment.AsQueryable().Count();
            foreach (var c in comments)
            {
                c.Id = Id++;
            }
            _comment.InsertMany(comments);
            return comments;
        }

        public void Update(int id, Comment updatedComment) => _comment.ReplaceOne(comment => comment.Id == id, updatedComment);

        public void Delete(Comment commentForDeletion) => _comment.DeleteOne(comment => comment.Id == commentForDeletion.Id);

        public void Delete(int id) => _comment.DeleteOne(comment => comment.Id == id);

        public void Update2(int id, Comment updatedentity)
        {
            throw new NotImplementedException();
        }
    }
}
