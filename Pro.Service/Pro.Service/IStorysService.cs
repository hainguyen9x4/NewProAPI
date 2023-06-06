using Pro.Common.Enum;
using Pro.Model;
using static Pro.Service.Implements.StorysService;

namespace Pro.Service
{
    public interface IStorysService
    {
        HomePageInfo GetHomeStoryForNews(int pageIndex, int dataPerPage = 16, bool useCache = true);
        List<ImageStoryInfo> GetTopHotStorysForNew(bool useCache = true);
        List<ImageStoryInfo> GetAllStoryForSearchForNew(bool useCache = true);
        ImageStoryInfo GetAllChapByStoryIdForNew(int storyID, bool useCache = true);
        ChapInfo GetImageStorysInChapForNew(int storyID, int chapID, bool useCache = true);
        List<ImageStoryInfo> GetFollowStorys(List<int> sotryIDs, int userID, bool useCache = true);
        TempGetAllStoryByTypeName GetAllStoryByTypeName(string typeName, int pageIndex = 0, int dataPerPage = 16, int numberStory = 10, bool useCache = true);
        TempGetAllStoryByTypeName RateStory(RATE_TYPE type, int pageIndex = 0, int dataPerPage = 16);
    }
}
