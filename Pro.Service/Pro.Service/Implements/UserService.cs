using Newtonsoft.Json;
using Pro.Common;
using Pro.Common.Account;
using Pro.Common.Const;
using Pro.Data.Repositorys;
using Pro.Model;
using System;

namespace Pro.Service.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStorysService _storyService;
        public UserService(IUserRepository userRepository,
            IStorysService storysService)
        {
            _userRepository = userRepository;
            _storyService = storysService;
        }

        public User GetUser(int userID, string accName = "")
        {
            if (!String.IsNullOrEmpty(accName))
            {
                var ux = _userRepository.GetAll().Where(u => u.AccName == accName).FirstOrDefault();
                if (ux != null) ux.Password = "";
                return ux;
            }
            var userData = _userRepository.GetAll().Where(u => u.Id == userID).FirstOrDefault();

            if (userData != null)
            {
                userData.Password = "";
                //Get data following story info
                if (userData.FollowStorys.Any())
                {
                    userData.FollowStoryLists = _storyService.GetFollowStorys(userData.FollowStorys, userID).ToList();
                }
            }
            return userData ?? new User();
        }

        public IResult AddNewUser(User user)
        {
            var rs = new APIResultAndUser();
            try
            {
                var uOlde = _userRepository.GetAll().Where(u => u.Email == user.Email).FirstOrDefault();
                if (uOlde != null)
                {
                    rs.Result = Common.Enum.RESUTL_API.EXISTED_USER;
                    var ts = GetEpoch();
                    uOlde.Login(ts);
                    _userRepository.Update(uOlde.Id, uOlde);
                    uOlde.Password = "";
                    rs.UserData = uOlde;
                    return rs;
                }

                if (!String.IsNullOrEmpty(user.Password))
                {
                    user.Password = Functions.GetMD5(user.Password);
                    user.AccessToken = Guid.NewGuid().ToString();
                    user.RefreshToken = Guid.NewGuid().ToString();
                    user.ExpiresIn = Constants.ONE_DAY_IN_SECONDS;
                    user.ExpiresOn = GetEpoch() + user.ExpiresIn;
                    user.LastLogin = DateTime.Now;
                }

                var newUser = _userRepository.Create(user);
                if (newUser != null)
                {
                    rs.Result = Common.Enum.RESUTL_API.SUCCESS;
                    user.Password = "";
                    rs.UserData = newUser;
                }
            }
            catch (Exception ex)
            {
                rs.Result = Common.Enum.RESUTL_API.ERROR_SERVER;
                LogHelper.Error($"AddNewUser: {JsonConvert.SerializeObject(user)}", ex);
                rs.Message = "Server error!";
            }
            return rs;
        }

        public IResult UserLogin(UserRequest user)
        {
            var rs = new APIResultAndUser();
            var ux = _userRepository.GetAll().Where(u => u.Email == user.Email).FirstOrDefault();
            if (ux != null)
            {
                if (Functions.GetMD5(user.Password) == ux.Password)
                {
                    var ts = GetEpoch();
                    ux.Login(ts);
                    _userRepository.Update(ux.Id, ux);
                    //response = user.CreateMapped<LoginResponse>();
                    ux.Password = "";
                    rs.UserData = ux;
                    return rs;
                }
                rs.Result = Common.Enum.RESUTL_API.LOGIN_FAIL;
            }
            else
            {
                rs.Result = Common.Enum.RESUTL_API.LOGIN_FAIL;
            }
            return rs;
        }

        public void Logout(string token)
        {
            var ux = _userRepository.GetAll().Single(u => u.AccessToken == token);
            ux.Logout();
            _userRepository.Update(ux.Id, ux);
        }

        private long GetEpoch()
        {
            var ts = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;   // We declare the epoch to be 1/1/1970.

            return ts;
        }
        public IResult DeleteUser(User user)
        {
            throw new NotImplementedException();
        }

        public IResult UpdateBasicInfoUser(User user)
        {
            var rs = new APIResult();
            try
            {
                var ux = _userRepository.GetAll().Where(u => u.AccName == user.AccName).FirstOrDefault();
                if (ux != null)
                {
                    if (!String.IsNullOrEmpty(user.FirstName) && ux.FirstName != user.FirstName)
                    {
                        ux.FirstName = user.FirstName;
                    }
                    if (!String.IsNullOrEmpty(user.LastName) && ux.LastName != user.LastName)
                    {
                        ux.LastName = user.LastName;
                    }
                    if (!String.IsNullOrEmpty(user.Avatar) && ux.Avatar != user.Avatar)
                    {
                        ux.Avatar = user.Avatar;
                    }
                    if (ux.Gt != user.Gt)
                    {
                        ux.Gt = user.Gt;
                    }
                    var newLevel = JsonConvert.SerializeObject(user.LevelInfo);
                    if (newLevel != null)
                    {
                        var oldLevel = JsonConvert.SerializeObject(ux.LevelInfo);
                        if (oldLevel != newLevel)
                        {
                            ux.LevelInfo = user.LevelInfo;
                        }
                    }
                    _userRepository.Update(ux.Id, ux);
                }
                else
                {
                    rs.Result = Common.Enum.RESUTL_API.ERROR;
                }
            }
            catch (Exception ex)
            {
                rs.Result = Common.Enum.RESUTL_API.ERROR_SERVER;
                LogHelper.Error($"UpdateBasicInfoUser: {JsonConvert.SerializeObject(user)}", ex);
                rs.Message = "Server error!";
            }
            return rs;
        }

        public IResult UpdateLevelInfoUser(int userID, LevelUser level)
        {
            var rs = new APIResult();
            try
            {
                var ux = _userRepository.GetAll().Where(u => u.Id == userID).First();
                ux.LevelInfo = level;
                _userRepository.Update(ux.Id, ux);
            }
            catch (Exception ex)
            {
                rs.Result = Common.Enum.RESUTL_API.ERROR_SERVER;
                LogHelper.Error($"UpdateLevelInfoUser: {userID};{JsonConvert.SerializeObject(level)}", ex);
                rs.Message = "Server error!";
            }
            return rs;
        }

        public IResult UpdateLevelInfoUser(int userID, int increasePercent)
        {
            var rs = new APIResult();
            try
            {
                var ux = _userRepository.GetAll().Where(u => u.Id == userID).First();
                ux.LevelInfo.LevelPercent += increasePercent;
                if (ux.LevelInfo.LevelPercent >= 100)
                {
                    ux.LevelInfo.LevelPercent = ux.LevelInfo.LevelPercent - 100;
                    ux.LevelInfo.LevelNow += 1;
                }
                _userRepository.Update(ux.Id, ux);
            }
            catch (Exception ex)
            {
                rs.Result = Common.Enum.RESUTL_API.ERROR_SERVER;
                LogHelper.Error($"UpdateLevelInfoUser: {userID};{increasePercent}", ex);
                rs.Message = "Server error!";
            }
            return rs;
        }

        public IResult ChangePassword(ChangePasswordRequest data)
        {
            var rs = new APIResultAndUser();
            try
            {
                var ux = _userRepository.GetAll().Where(u => u.Id == data.UserID).First();
                ux.Password = Functions.GetMD5(data.NewPassword);
                _userRepository.Update(ux.Id, ux);

                var ts = GetEpoch();
                ux.Login(ts);
                _userRepository.Update(ux.Id, ux);
                ux.Password = "";
                rs.UserData = ux;
                return rs;
            }
            catch (Exception ex)
            {
                rs.Result = Common.Enum.RESUTL_API.ERROR_SERVER;
                LogHelper.Error($"ChangePassword: {data.UserID}", ex);
                rs.Message = "Server error!";
            }
            return rs;
        }


        public IResult AddFollowStoryInfoUser(int userID, int storyID)
        {
            var rs = new APIResult();
            try
            {
                var ux = _userRepository.GetAll().Where(u => u.Id == userID).First();
                if (!ux.FollowStorys.Contains(storyID))
                {
                    if (!ux.FollowStorys.Contains(storyID))
                    {
                        ux.FollowStorys.Add(storyID);
                        _userRepository.Update(ux.Id, ux);
                    }
                    else
                    {
                        rs.Result = Common.Enum.RESUTL_API.ERROR;
                    }
                }
                {
                    rs.Result = Common.Enum.RESUTL_API.LOGIN_FAIL;
                }
            }
            catch (Exception ex)
            {
                rs.Result = Common.Enum.RESUTL_API.ERROR_SERVER;
                LogHelper.Error($"AddFollowStoryInfoUser: {userID};{storyID}", ex);
                rs.Message = "Server error!";
            }
            return rs;
        }
        public bool VerifyAccount(string token)
        {
            var user = _userRepository.GetAll().Where(u => u.AccessToken == token).SingleOrDefault();
            return (user?.ExpiresOn ?? 0) > GetEpoch();
        }
        public IResult DeleteFollowStoryInfoUser(int userID, int storyID)
        {
            var rs = new APIResult();
            try
            {
                var ux = _userRepository.GetAll().Where(u => u.Id == userID).First();
                if (ux.FollowStorys.Contains(storyID))
                {
                    ux.FollowStorys.RemoveAll(x => x == storyID);
                    _userRepository.Update(ux.Id, ux);
                }
                else
                {
                    rs.Result = Common.Enum.RESUTL_API.ERROR;
                }
            }
            catch (Exception ex)
            {
                rs.Result = Common.Enum.RESUTL_API.ERROR_SERVER;
                LogHelper.Error($"DeleteFollowStoryInfoUser: {userID};{storyID}", ex);
                rs.Message = "Server error!";
            }
            return rs;
        }
    }
}