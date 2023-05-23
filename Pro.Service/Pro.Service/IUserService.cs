using Pro.Common.Account;
using Pro.Model;

namespace Pro.Service
{
    public interface IUserService
    {
        User GetUser(int userID, string accName = "");
        bool AddNewUser(User user);
        User UpdateBasicInfoUser(User user);
        bool DeleteUser(User user);
        User UserLogin(UserRequest user);
        void Logout(string token);
        public User UpdateLevelInfoUser(int userID, LevelUser level);
        User UpdateLevelInfoUser(int userID, int increasePercent);
        User DeleteFollowStoryInfoUser(int userID, int storyID);
        User AddFollowStoryInfoUser(int userID, int storyID);
    }
}
