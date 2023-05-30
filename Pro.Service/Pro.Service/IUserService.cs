using Pro.Common;
using Pro.Common.Account;
using Pro.Model;

namespace Pro.Service
{
    public interface IUserService
    {
        User GetUser(int userID, string accName = "");
        IResult AddNewUser(User user);
        IResult UpdateBasicInfoUser(User user);
        IResult DeleteUser(User user);
        User UserLogin(UserRequest user);
        void Logout(string token);
        bool VerifyAccount(string token);
        IResult UpdateLevelInfoUser(int userID, LevelUser level);
        IResult UpdateLevelInfoUser(int userID, int increasePercent);
        IResult DeleteFollowStoryInfoUser(int userID, int storyID);
        IResult AddFollowStoryInfoUser(int userID, int storyID);
    }
}
