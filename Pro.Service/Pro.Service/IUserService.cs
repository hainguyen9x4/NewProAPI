using Pro.Common.Account;
using Pro.Model;

namespace Pro.Service
{
    public interface IUserService
    {
        User GetUser(int userID, string accName = "");
        bool AddNewUser(User user);
        bool UpdateBasicInfoUser(User user);
        bool DeleteUser(User user);
        User UserLogin(UserRequest user);
        void Logout(string token);
        bool VerifyAccount(string token);
        bool UpdateLevelInfoUser(int userID, LevelUser level);
        bool UpdateLevelInfoUser(int userID, int increasePercent);
        bool DeleteFollowStoryInfoUser(int userID, int storyID);
        bool AddFollowStoryInfoUser(int userID, int storyID);
    }
}
