using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pro.Model
{
    public class HotStory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("StoryID")]
        public int StoryID { get; set; }
        [BsonElement("IsActive")]
        public bool IsActive { get; set; }

        public HotStory() { }
        public HotStory(int storyId, bool isActive)
        {
            StoryID = storyId;
            IsActive = isActive;
        }
    }
}