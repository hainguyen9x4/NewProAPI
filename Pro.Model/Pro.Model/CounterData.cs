using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pro.Model
{
    public class CounterData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("StoryId")]
        public int StoryId { get; set; }

        public CounterData() { }
        public CounterData(int storyID, int chapId)
        {
            StoryId = storyID;
        }

    }
}