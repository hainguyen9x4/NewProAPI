using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pro.Model
{
    public class HotStory
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("StoryID")]
        public int StoryID { get; set; }

        [BsonElement("IsActive")]
        public bool IsActive { get; set; }

        public HotStory(int storyId, bool isActive)
        {
            StoryID = storyId;
            IsActive = isActive;
        }
    }
}