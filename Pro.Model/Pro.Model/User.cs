using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using Constants = Pro.Common.Const.Constants;

namespace Pro.Model
{
    public class User
    {
        public User(string accName, string email, string password, List<int> followStorys, LevelUser levelInfo,
            string firstName = "", string lastName = "", byte gt = 2, string avatar = "", bool isDeleted = false)
        {
            AccName = accName;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Gt = gt;
            Email = email;
            Avatar = avatar;
            FollowStorys = followStorys;
            LevelInfo = levelInfo;
            IsDeleted = isDeleted;
        }

        [BsonId]
        public int Id { get; set; }

        [BsonElement("AccName")]
        public string AccName { get; set; }

        [BsonElement("Gt")]
        public byte Gt { get; set; }

        [BsonElement("FirstName")]
        public string FirstName { get; set; }

        [BsonElement("LastName")]
        public string LastName { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Avatar")]
        public string Avatar { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("FollowStory")]
        public List<int> FollowStorys { get; set; }

        [BsonElement("Level")]
        public LevelUser LevelInfo { get; set; }

        [BsonElement("IsDeleted")]
        public bool IsDeleted { get; set; }


        [BsonElement("AccessToken")]
        public string AccessToken { get; set; }
        [BsonElement("RefreshToken")]
        public string RefreshToken { get; set; }
        [BsonElement("ExpiresIn")]
        public int? ExpiresIn { get; set; }
        [BsonElement("ExpiresOn")]
        public long? ExpiresOn { get; set; }
        [BsonElement("LastLogin")]
        public DateTime? LastLogin { get; set; }

        [NotMapped]
        public List<ImageStoryInfo> FollowStoryLists { get; set; }
        public void Login(long ts)
        {
            AccessToken = Guid.NewGuid().ToString();
            RefreshToken = Guid.NewGuid().ToString();
            ExpiresIn = Constants.ONE_DAY_IN_SECONDS;
            ExpiresOn = ts + ExpiresIn;
            LastLogin = DateTime.Now;
        }

        public void Logout()
        {
            AccessToken = null;
            RefreshToken = null;
            ExpiresIn = null;
            ExpiresOn = null;
        }

    }
    public class LevelUser
    {
        public LevelUser()
        {
            LevelNow = 0;
            LevelPercent = 0;
            LevelType = LevelType.Normal;
        }
        public int LevelNow { get; set; }
        public int LevelPercent { get; set; }
        public LevelType LevelType { get; set; }
    }
    public enum LevelType
    {
        Normal = 1,
        TuTien = 2,
    }
}
