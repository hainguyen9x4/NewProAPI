using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pro.Model
{
    public class Chap
    {
        [BsonId]
        public int ChapId { get; set; }

        [BsonElement("ChapName")]
        public string ChapName { get; set; }

        [BsonElement("ChapLink")]
        public string ChapLink { get; set; }

        [BsonElement("StoryId")]
        public int StoryId { get; set; }

        [BsonElement("ChapStatusID")]
        public int? ChapStatusID { get; set; }

        [BsonElement("LastModifyDatetime")]
        public DateTime? LastModifyDatetime { get; set; }

        [BsonElement("Images")]
        public List<ImagesChap> Images { get; set; }

        public Chap(int storyID, int chapIdx, string chapName, List<ImagesChap> images = null, int? chapStatusID = 1, string chapLink = "")
        {
            StoryId = storyID;
            ChapName = chapName;
            ChapStatusID = chapStatusID;
            ChapLink = chapLink;
            Images = images;
            LastModifyDatetime = DateTime.Now;
        }

    }
    public class ImagesChap
    {
        public string ImageWebLink { get; set; }
        public string ImageSavedLink { get; set; }
    }
}