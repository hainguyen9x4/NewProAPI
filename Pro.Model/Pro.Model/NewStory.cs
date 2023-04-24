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

        [BsonElement("UpdatedTime")]
        public DateTime UpdatedTime { get; set; }

        public NewStory(string name, int statusId = 1/*Actived*/,
            string link = "", string picture = "", string nameShow = "")
        {
            Name = name;
            NameShow = nameShow;
            Picture = picture;
            Link = link;
            StatusID = statusId;
            UpdatedTime = DateTime.UtcNow;
        }
    }
    public class OtherInfo
    {
        public OtherInfo()
        {
            Star = new Star();
            Author = "";
            Des = "";
        }
        public string Author { get; set; }
        public string Des { get; set; }
        public int TypeID { get; set; }
        public Star Star { get; set; }
        public int ViewTotal { get; set; }
    }
    public class Star
    {
        public int AvgStar { get; set; }
        public int TotalRate { get; set; }
    }
}