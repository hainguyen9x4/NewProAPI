using Pro.Common;
using Pro.Common.Account;
using Pro.Common.Enum;
using Pro.Model;

namespace Pro.Service
{
    public interface IUserService
    {
        User GetUser(int userID, string accName = "");
        IResult AddNewUser(User user);
        IResult UpdateBasicInfoUser(User user);
        IResult DeleteUser(User user);
        IResult UserLogin(UserRequest user);
        void Logout(string token);
        bool VerifyAccount(string token);
        IResult UpdateLevelInfoUser(int userID, LevelUser level);
        IResult UpdateLevelInfoUser(int userID, int increasePercent);
        IResult ChangePassword(ChangePasswordRequest data);
        IResult DeleteFollowStoryInfoUser(int userID, int storyID);
        IResult AddFollowStoryInfoUser(int userID, int storyID);
    }
    public class APIResultAndUser : IResult
    {
        public APIResultAndUser()
        {
            Result = RESUTL_API.SUCCESS;
            Message = string.Empty;
        }
        public RESUTL_API Result { get; set; }
        public string Message { get; set; }
        public User UserData { get; set; }
    }
}
