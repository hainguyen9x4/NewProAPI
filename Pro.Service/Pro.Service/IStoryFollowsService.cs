using Pro.Common.Enum;
using Pro.Model;
using static Pro.Service.Implements.StoryFollowsService;

namespace Pro.Service
{
    public interface IStoryFollowsService
    {
        List<StoryFollow> GetAllStoryFollows(STATUS_FOLLOW status, bool useCache = true);
        bool UpdateStoryFollows(int id, STATUS_FOLLOW status, string link = "");
        bool AddTableStoryFollows(List<string> links);
        bool DeleteStoryFollows(int id);
        ResultAddNewStory AddStoryFollows(string link);
    }
}
