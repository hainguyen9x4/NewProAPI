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
        TempGetAllStoryByTypeName GetAllStoryByTypeName(string typeName, bool useCache = true);
    }
}
