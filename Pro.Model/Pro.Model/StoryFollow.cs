using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pro.Model
{
    public class StoryFollow
    {
        public StoryFollow(string link, int status)
        {
            Link = link;
            Status = status;
        }

        [BsonId]
        public int Id { get; set; }

        [BsonElement("Link")]
        public string Link { get; set; }

        [BsonElement("Status")]
        public int Status { get; set; }

    }
}
