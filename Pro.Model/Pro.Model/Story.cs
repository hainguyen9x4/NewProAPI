using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pro.Model
{
    public class Story
    {
        [BsonId]
        public int StoryId { get; set; }

        [BsonElement("StoryName")]
        public string StoryName { get; set; }

        [BsonElement("StoryStatusID")]
        public int? StoryStatusID { get; set; }

        [BsonElement("StoryTypeID")]
        public int? StoryTypeID { get; set; }

        [BsonElement("StoryLink")]
        public string StoryLink { get; set; }

        [BsonElement("StoryNameShow")]
        public string StoryNameShow { get; set; }

        [BsonElement("StoryPicture")]
        public string StoryPicture { get; set; }

        [BsonElement("Author")]
        public string Author { get; set; }

        public Story(string storyName, int? storyStatusId = 1, int? storyTypeID = 0,
            string storyLink = "", string author = "", string storyPictureLink = "", string storyNameShow = "")
        {
            StoryName = storyName;
            StoryStatusID = storyStatusId;
            StoryTypeID = storyTypeID;
            StoryLink = storyLink;
            Author = author;
            StoryPicture = storyPictureLink;
            StoryNameShow = storyNameShow;
        }

    }
}