using Pro.Model;
using System.Collections.Generic;

namespace Pro.Service
{
    public interface IStorysService
    {
        List<ImageStoryInfo> GetTopHotStorys(bool useCache = true);
        List<ImageStoryInfo> GetAllStoryForSearch(bool useCache = true);
        HomePageInfo GetHomeStorys(int pageIndex, int dataPerPage = 16, bool useCache = true);
        List<NewStory> GetHomeStoryForNews(int pageIndex, int dataPerPage = 16, bool useCache = true);
        ImageStoryInfo GetAllChapByStoryId(int storyID, bool useCache = true);
        ChapInfo GetImageStorysInChap(int storyID, int chapID, bool useCache = true);
    }
}
