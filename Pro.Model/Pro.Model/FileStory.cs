using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pro.Model
{
    public class FileStory
    {
        public FileStory(string storyName, int chapStoredNewest)
        {
            StoryName = storyName;
            ChapStoredNewest = chapStoredNewest;
        }
        public FileStory()
        {
        }
        [BsonId]
        public int Id { get; set; }

        [BsonElement("StoryName")]
        public string StoryName { get; set; }

        [BsonElement("ChapStoredNewest")]
        public int ChapStoredNewest { get; set; }

    }
    public class ModelAddNewFileStory
    {
        public string StoryName { get; set; }
        public int ChapStoredNewest { get; set; }
    }
    public class ResultAddNewFileStory
    {
        public int Result { get; set; }
        public string Message { get; set; }
    }
}
