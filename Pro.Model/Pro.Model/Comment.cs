using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pro.Model
{
    public class Comment
    {
        public Comment(int userID, int chapID, int storyID, List<CommentDetail> comments, string content = "")
        {
            //UserID = userID;
            StoryID = storyID;
            //ChapID = chapID;
            //Content = content;
            CommentDetails = comments;
        }

        [BsonId]
        public int Id { get; set; }

        [BsonElement("StoryID")]
        public int StoryID { get; set; }

        [BsonElement("CommentDetail")]
        public List<CommentDetail> CommentDetails { get; set; }
    }
    public class CommentDetail
    {
        [BsonElement("UserID")]
        public int UserID { get; set; }

        [BsonElement("ChapID")]
        public int ChapID { get; set; }

        [BsonElement("Content")]
        public string Content { get; set; }

        [BsonElement("Like")]
        public int Like { get; set; }

        [BsonElement("Dislike")]
        public int Dislike { get; set; }

        [BsonElement("LastUpdated")]
        public DateTime LastUpdated { get; set; }

        [BsonElement("CommentDetail")]
        public List<CommentDetail> CommentDetails { get; set; }
    }
}
