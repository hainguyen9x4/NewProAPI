using Pro.Common.Enum;
using Pro.Model;
using static Pro.Service.Implements.StoryFollowsService;

namespace Pro.Service
{
    public interface IStoryFollowsService
    {
        List<StoryFollow> GetAllStoryFollows(STATUS_FOLLOW status, bool useCache = true);
        bool UpdateStoryFollows(int id, string link, STATUS_FOLLOW status);
        bool AddTableStoryFollows(List<string> links);
        bool DeleteStoryFollows(int id);
        ResultAddNewStory AddStoryFollows(string link);
    }
}
