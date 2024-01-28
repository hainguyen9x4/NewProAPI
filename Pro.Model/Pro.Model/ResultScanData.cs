using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pro.Model
{
    [Table("ResultScanData")]
    public class ResultScanData
    {
        [BsonId]
        public int Id { get; set; }

        [BsonElement("StoryID")]
        public int StoryID { get; set; }

        [BsonElement("ChapLink")]
        public string ChapLink { get; set; }

        [BsonElement("Status")]
        public int Status { get; set; }
    }
}
