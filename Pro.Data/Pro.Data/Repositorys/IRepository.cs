using MongoDB.Driver.Linq;

namespace Pro.Data.Repositorys
{
    public interface IRepository<T> where T : class
    {
        IMongoQueryable<T> GetAll();
        T GetById(int id);
        T Create(T entity);
        List<T> Creates(List<T> entities);
        void Delete(int id);
        void Delete(T entityForDeletion);
        void Update(int id, T updatedentity);
    }
}
