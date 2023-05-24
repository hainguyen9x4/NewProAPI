using Pro.Common;
using Pro.Common.Account;
using Pro.Data.Repositorys;
using Pro.Model;

namespace Pro.Service.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User GetUser(int userID, string accName = "")
        {
            if (!String.IsNullOrEmpty(accName))
            {
                var ux = _userRepository.GetAll().Where(u => u.AccName == accName).FirstOrDefault();
                if (ux != null) ux.Password = "";
                return ux;
            }
            var uxx = _userRepository.GetAll().Where(u => u.Id == userID).FirstOrDefault();
            if (uxx != null) uxx.Password = "";
            return uxx;
        }

        public bool AddNewUser(User user)
        {
            var uOlde = _userRepository.GetAll().Where(u => u.AccName == user.AccName || u.Email == user.Email).FirstOrDefault();
            if (uOlde != null)
            {
                if (uOlde.AccName == user.AccName)
                {
                    return false;
                }
                if (uOlde.Email == user.Email)
                {
                    return false;
                }
            }
            user.Password = Functions.GetMD5(user.Password);
            if (_userRepository.Create(user) != null) return true;
            return false;
        }

        public User UserLogin(UserRequest user)
        {
            var ux = _userRepository.GetAll().Where(u => u.AccName == user.Username).FirstOrDefault();
            if (ux != null)
            {
                if (Functions.GetMD5(user.Password) == ux.Password)
                {
                    var ts = GetEpoch();
                    ux.Login(ts);
                    _userRepository.Update(ux.Id, ux);
                    //response = user.CreateMapped<LoginResponse>();
                    return ux;
                }
            }
            ux.Password = "";
            return null;
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
        public bool DeleteUser(User user)
        {
            throw new NotImplementedException();
        }

        public User UpdateBasicInfoUser(User user)
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
                if (!String.IsNullOrEmpty(user.Email) && ux.Email != user.Email)
                {
                    ux.Email = user.Email;
                }
                if (!String.IsNullOrEmpty(user.Avatar) && ux.Avatar != user.Avatar)
                {
                    ux.Avatar = user.Avatar;
                }
                if (ux.Gt != user.Gt)
                {
                    ux.Gt = user.Gt;
                }
                _userRepository.Update(ux.Id, ux);
            }
            ux.Password = "";
            return user;
        }

        public User UpdateLevelInfoUser(int userID, LevelUser level)
        {
            var ux = _userRepository.GetAll().Where(u => u.Id == userID).First();
            ux.LevelInfo = level;
            _userRepository.Update(ux.Id, ux);
            ux.Password = "";
            return ux;
        }

        public User UpdateLevelInfoUser(int userID, int increasePercent)
        {
            var ux = _userRepository.GetAll().Where(u => u.Id == userID).First();
            ux.LevelInfo.LevelPercent += increasePercent;
            if (ux.LevelInfo.LevelPercent >= 100)
            {
                ux.LevelInfo.LevelPercent = ux.LevelInfo.LevelPercent - 100;
                ux.LevelInfo.LevelNow += 1;
            }
            _userRepository.Update(ux.Id, ux);
            ux.Password = "";
            return ux;
        }

        public User AddFollowStoryInfoUser(int userID, int storyID)
        {
            var ux = _userRepository.GetAll().Where(u => u.Id == userID).First();
            if (!ux.FollowStorys.Contains(storyID))
            {
                ux.FollowStorys.Add(storyID);
                _userRepository.Update(ux.Id, ux);
            }
            ux.Password = "";
            return ux;
        }
        public bool VerifyAccount(string token)
        {
            var user = _userRepository.GetAll().Where(u => u.AccessToken == token).SingleOrDefault();
            var ts = GetEpoch();
            bool ok = (user?.ExpiresOn ?? 0) > ts;

            return ok;
        }
        public User DeleteFollowStoryInfoUser(int userID, int storyID)
        {
            var ux = _userRepository.GetAll().Where(u => u.Id == userID).First();
            if (ux.FollowStorys.Contains(storyID))
            {
                ux.FollowStorys.Add(storyID);
                _userRepository.Update(ux.Id, ux);
            }
            ux.Password = "";
            return ux;
        }
    }
}