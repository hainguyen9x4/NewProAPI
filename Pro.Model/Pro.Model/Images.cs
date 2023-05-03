using MongoDB.Bson.Serialization.Attributes;

namespace Pro.Model
{
    public class ImagesOneChap
    {
        public ImagesOneChap(int storyID, int chapID, List<Image> images)
        {
            Images = images;
            StoryID = storyID;
            ChapID = chapID;
        }

        public int Id { get; set; }

        [BsonElement("StoryID")]
        public int StoryID { get; set; }

        [BsonElement("ChapID")]
        public int ChapID { get; set; }

        [BsonElement("Images")]
        public List<Image> Images { get; set; }
    }
}
