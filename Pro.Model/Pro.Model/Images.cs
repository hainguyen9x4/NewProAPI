using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Pro.Model
{
    public class ImagesOneChap
    {
        public ImagesOneChap(int storyID, int chapID, List<ImageData> images)
        {
            Images = images;
            StoryID = storyID;
            ChapID = chapID;
        }

        [BsonId]
        public int Id { get; set; }

        [BsonElement("StoryID")]
        public int StoryID { get; set; }

        [BsonElement("ChapID")]
        public int ChapID { get; set; }

        [BsonElement("Images")]
        public List<ImageData> Images { get; set; }
    }
}
