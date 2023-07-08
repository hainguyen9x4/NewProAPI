using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pro.Model;

namespace Pro.Data.Repositorys.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;
        public UserRepository(IAppSettingData settings)
        {
            var client = new MongoClient(settings.ConnectionStringAppSetting);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<User>(settings.XStorysCollectionUser);
        }
        public IMongoQueryable<User> GetAll() => _users.AsQueryable().Where(user => true);

        public User GetById(int id) => _users.Find(user => user.Id == id).FirstOrDefault();

        public User Create(User user)
        {
            var Id = 1 + _users.AsQueryable().Count();
            user.Id = Id;
            _users.InsertOne(user);
            return user;
        }

        public List<User> Creates(List<User> users)
        {
            var Id = 1 + _users.AsQueryable().Count();
            foreach (var c in users)
            {
                c.Id = Id++;
            }
            _users.InsertMany(users);
            return users;
        }

        public void Update(int id, User updatedUser) => _users.ReplaceOne(user => user.Id == id, updatedUser);

        public void Delete(User userForDeletion) => _users.DeleteOne(user => user.Id == userForDeletion.Id);

        public void Delete(int id) => _users.DeleteOne(user => user.Id == id);

        public void Update2(int id, User updatedentity)
        {
            throw new NotImplementedException();
        }
    }
}
