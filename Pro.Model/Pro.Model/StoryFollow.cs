using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pro.Common.Enum;

namespace Pro.Model
{
    public class StoryFollow
    {
        public StoryFollow(string link, STATUS_FOLLOW status)
        {
            Link = link;
            Status = status;
        }
        public StoryFollow() { }
        [BsonId]
        public int Id { get; set; }

        [BsonElement("Link")]
        public string Link { get; set; }

        [BsonElement("Status")]
        public STATUS_FOLLOW Status { get; set; }

    }
}
