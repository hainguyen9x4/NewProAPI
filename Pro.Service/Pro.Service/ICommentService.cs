using Pro.Model;

namespace Pro.Service
{
    public interface ICommentService
    {
        bool AddNewComment(Comment comment);
        bool DeleteComment(Comment comment);
        bool UpdateComment(Comment comment);
        bool AddReplyComment(int commentId, Comment comment);
        List<CommentByStory> ReadCommentByStory(int storyId, int numberTopComment = 20);
    }
}
