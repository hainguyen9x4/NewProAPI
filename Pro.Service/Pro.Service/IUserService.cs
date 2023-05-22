using Pro.Model;

namespace Pro.Service
{
    public interface IUserService
    {
        bool AddNewUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(User user);
    }
}
