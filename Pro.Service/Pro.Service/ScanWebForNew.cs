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
                //string storyNameShow = "";
                var fetchedData = GetStoryInfoWithChapByAPI(urlStory, urlBase);
                storyName = FileReader.GetStoryInfoFromUrlStory(urlStory);
                var allNameStorySaved = allCurrentStoryListStores.Select(t => t.StoryName).ToList();
                //LogHelper.Info($"fetchedChaps: {JsonConvert.SerializeObject(fetchedChaps)}");
                newestChapModel.Add(new NewestChapModel()
                {
                    StoryName = storyName,
                    StoryLink = FileReader.DeleteHomePage(urlStory),
                    StoryNameShow = fetchedData.StoryName,
                    Description = fetchedData.Description,
                    //StoryTypes = ConvertStoryTypes(fetchedData.StoryTypes),
                });
                //LogHelper.Info($"newestChapModel: {JsonConvert.SerializeObject(newestChapModel)}");
                if (!allNameStorySaved.Contains(storyName))//new story
                {
                    allCurrentStoryListStores.Add(
                        new StorySaveInfo()
                        {
                            StoryName = storyName,
                            ChapStoredNewest = fetchedData.ChapPluss.Select(t => t.ChapIndexNumber).Max(),
                        });
                    rs_eachChaps = fetchedData.ChapPluss.Select(t => t.ChapLink).ToList();
                }
                else//old story
                {
                    foreach (var stored in allCurrentStoryListStores)
                    {
                        if (stored.StoryName == storyName)
                        {
                            var newestChapIndexNumbere = fetchedData.ChapPluss.Select(t => t.ChapIndexNumber).Max();
                            if (newestChapIndexNumbere > stored.ChapStoredNewest)
                            {
                                //Has new chap
                                var temps = fetchedData.ChapPluss.Where(c => c.ChapIndexNumber > stored.ChapStoredNewest).ToList();
                                stored.ChapStoredNewest = newestChapIndexNumbere;
                                rs_eachChaps = temps.Select(t => t.ChapLink).ToList();
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error_FindNewChapInStory :{urlStory}" + ex);
            }
            return newestChapModel;
        }

        public class StoryInfoWithChaps
        {
            public List<ChapPlus> ChapPluss { get; set; }
            public string StoryName { get; set; }
            public string Description { get; set; }
            public List<string> StoryTypes { get; set; }
        }
        private StoryInfoWithChaps GetStoryInfoWithChapByAPI(string textUrl, string urlBase)
        {
            var data = new ApiHelper().Post<StoryInfoWithChaps>($"/api/GetStoryInfoWithChaps?textUrl={textUrl}", null, urlBase);
            return data == null ? new StoryInfoWithChaps() : data;
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
