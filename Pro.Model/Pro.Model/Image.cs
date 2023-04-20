using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pro.Model
{
    public class Image
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("ChapID")]
        public string ChapID { get; set; }

        [BsonElement("FileKey")]
        public string FileKey { get; set; }

        [BsonElement("ImageGetLink")]
        public string ImageGetLink { get; set; }

        [BsonElement("ImageStatusID")]
        public int ImageStatusID { get; set; }

        public Image() { }
        public Image(string chapId, string fileKey = "", string imageLink = "", int status = 0)
        {
            ChapID = chapId;
            FileKey = fileKey;
            ImageGetLink = imageLink;
            ImageStatusID = status;
        }
    }
}