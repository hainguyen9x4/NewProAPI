using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pro.Model
{
    public class NewStory
    {
        [BsonId]
        public int ID { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("NameShow")]
        public string NameShow { get; set; }

        [BsonElement("Picture")]
        public string Picture { get; set; }

        [BsonElement("Link")]
        public string Link { get; set; }

        [BsonElement("StatusID")]
        public int StatusID { get; set; }

        [BsonElement("Chaps")]
        public List<Chap> Chaps { get; set; }

        [BsonElement("OtherInfo")]
        public OtherInfo OtherInfo { get; set; }

        [BsonElement("UpdatedTime")]
        public DateTime UpdatedTime { get; set; }

        public NewStory() { }
        public NewStory(string name, string nameShow, List<Chap> chaps, OtherInfo otherInfo, int statusId = 1/*Actived*/, string link = "", string picture = "")
        {
            Name = name;
            NameShow = nameShow;
            Picture = picture;
            Link = link;
            StatusID = statusId;
            Chaps = chaps;
            OtherInfo = otherInfo;
            UpdatedTime = DateTime.UtcNow;
        }
    }
    public class OtherInfo
    {
        public OtherInfo(Star star, List<StoryType> types, string author = "", string des = "", int viewTotal = 0, int followTotal = 0)
        {
            Star = star;
            Author = author;
            Des = des;
            Types = types;
            ViewTotal = viewTotal;
            FollowTotal = followTotal;
        }
        public string Author { get; set; }
        public string Des { get; set; }
        public List<StoryType> Types { get; set; }
        public Star Star { get; set; }
        public int ViewTotal { get; set; }
        public int FollowTotal { get; set; }
    }
    public class Star
    {
        public Star(double avgStar = 0, int totalRate = 0)
        {
            AvgStar = avgStar;
            TotalRate = totalRate;
        }

        public double AvgStar { get; set; }
        public int TotalRate { get; set; }
    }

    public class Chap
    {
        public Chap(string name, string link, List<Image> images, int statusID = 1)
        {
            Name = name;
            Link = link;
            Images = images;
            StatusID = statusID;
        }

        public Chap()
        {
            Name = "";
            Link = "";
            Images = new List<Image>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public int StatusID { get; set; }
        public List<Image> Images { get; set; }
    }

    public class Image
    {
        public Image(string link = "", string originLink = "")
        {
            Link = link;
            OriginLink = originLink;
        }
        public Image()
        {
            Link = "";
            OriginLink = "";
        }
        public string Link { get; set; }
        public string OriginLink { get; set; }
    }

    public class StoryType
    {
        public StoryType(int typeID, string typeName)
        {
            TypeID = typeID;
            TypeName = typeName;
        }
        public StoryType()
        {
            TypeName = "";
        }
        public int TypeID { get; set; }
        public string TypeName { get; set; }
    }
}