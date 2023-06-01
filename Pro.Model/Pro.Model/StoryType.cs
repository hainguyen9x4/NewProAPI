using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pro.Model
{
    public class StoryType
    {
        [BsonId]
        public int TypeID { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("NameShow")]
        public string NameShow { get; set; }

        [BsonElement("Des")]
        public string Des { get; set; }

        public StoryType() { }
        public StoryType(string name, string nameShow, string des ="")
        {
            Name = name;
            NameShow = nameShow;
            Des = des;
        }
    }
}