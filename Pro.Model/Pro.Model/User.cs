using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pro.Model
{
    public class User
    {
        public User(string accName, string email, string password, List<int> followStorys, LevelUser levelInfo,
            string firstName = "", string lastName = "", byte gt = 2, string avatar = "")
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
