using FileManager;
using Pro.Common;

namespace Pro.Service
{
    public class ScanWebForNew
    {
        public List<NewestChapModel> FindNewChapInStory(List<StorySaveInfo> allCurrentStoryListStores, string urlStory, ref List<string> rs_eachChaps, string urlBase = "")
        {
            var newestChapModel = new List<NewestChapModel>();
            var storyName = "";
            //rs_eachChaps = new List<string>();
            try
            {
                string storyNameShow = "";
                var fetchedChaps = GetAllChapsByAPI(urlStory, urlBase, ref storyNameShow);
                storyName = FileReader.GetStoryInfoFromUrlStory(urlStory);
                var allNameStorySaved = allCurrentStoryListStores.Select(t => t.StoryName).ToList();

                if (fetchedChaps.Any())
                {
                    //LogHelper.Info($"fetchedChaps: {JsonConvert.SerializeObject(fetchedChaps)}");
                    newestChapModel.Add(new NewestChapModel()
                    {
                        StoryName = storyName,
                        StoryLink = FileReader.DeleteHomePage(urlStory),
                        StoryNameShow = storyNameShow,
                    });
                    //LogHelper.Info($"newestChapModel: {JsonConvert.SerializeObject(newestChapModel)}");
                    if (!allNameStorySaved.Contains(storyName))//new story
                    {
                        allCurrentStoryListStores.Add(
                            new StorySaveInfo()
                            {
                                StoryName = storyName,
                                ChapStoredNewest = fetchedChaps.Select(t => t.ChapIndexNumber).Max(),
                            });
                        rs_eachChaps = fetchedChaps.Select(t => t.ChapLink).ToList();
                    }
                    else//old story
                    {
                        foreach (var stored in allCurrentStoryListStores)
                        {
                            if (stored.StoryName == storyName)
                            {
                                var newestChapIndexNumbere = fetchedChaps.Select(t => t.ChapIndexNumber).Max();
                                if (newestChapIndexNumbere > stored.ChapStoredNewest)
                                {
                                    //Has new chap
                                    var temps = fetchedChaps.Where(c => c.ChapIndexNumber > stored.ChapStoredNewest).ToList();
                                    stored.ChapStoredNewest = newestChapIndexNumbere;
                                    rs_eachChaps = temps.Select(t => t.ChapLink).ToList();
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //Get no data chap of a story
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error_FindNewChapInStory :{urlStory}" + ex);
            }
            return newestChapModel;
        }

        private List<ChapPlus> GetAllChapsByAPI(string textUrl, string urlBase, ref string storyNameShow)
        {
            storyNameShow = "";
            var chapDatas = new ApiHelper().Post<List<ChapPlus>>($"/api/GetAllChaps?textUrl={textUrl}", null, urlBase);
            if (chapDatas != null && chapDatas.Any())
            {
                storyNameShow = chapDatas.FirstOrDefault().StoryNameShow;
            }
            return chapDatas == null || !chapDatas.Any() ? new List<ChapPlus>() : chapDatas;
        }

        private List<string> FindNewStoryByAPI(int numberPage, string homeUrl, string urlBase)
        {
            var rs = new List<string>();
            //Call api to get data
            var listStorys = new ApiHelper().Post<List<string>>($"/api/FindNewStoryInPageAPI?homeUrl={homeUrl}&numberPage={numberPage}", null, urlBase);
            return listStorys ?? new List<string>();
        }
    }
}
