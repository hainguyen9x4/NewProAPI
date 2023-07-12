using FileManager;
using MongoDB.Driver;
using Pro.Common;
using Pro.Common.Const;
using Pro.Model;

namespace Pro.Service.Implements
{
    public class ScanDataService : IScanDataService
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IStoryTypeService _storyTypeService;

        private readonly AppBuildDataSetting _setting;
        private readonly IAppSettingData _settingData;

        public ScanDataService(IApplicationSettingService applicationSettingService,
            IAppSettingData appSettingData,
            IStoryTypeService storyTypeService)
        {
            _applicationSettingService = applicationSettingService;
            _settingData = appSettingData;
            _storyTypeService = storyTypeService;

            var settings = _applicationSettingService.GetValueGetScan(ApplicationSettingKey.AppsettingsScanGet, useOtherSetting: _settingData.UseSettingGetSetNumber);
            _setting = JsonManager.StringJson2Object<AppBuildDataSetting>(settings);
            _storyTypeService = storyTypeService;
#if DEBUG
            _setting.FolderSaveData = Constants.DEBUG_DATA_FOLDER;
#endif
        }

        public bool StartScanData()
        {
            var fileStoryFollowPath = Path.Combine(_setting.FolderSaveData, _setting.DataStoryFollowsFile);
            var lstStoryForllows = FileReader.ReadListDataFromFile<string>(fileStoryFollowPath);
            StartScanJob(lstStoryForllows.Where(t => !String.IsNullOrEmpty(t)).ToList());
            return true;
        }
        private static bool statusScan = true;
        private void StartScanJob(List<string> lstStoryFollows)
        {
            if (statusScan)
            {
                try
                {
                    statusScan = false;
                    var homeLinkWithSub = _applicationSettingService.GetValue(ApplicationSettingKey.HomePage) + _applicationSettingService.GetValue(ApplicationSettingKey.SubDataForHomePage);
                    var urlBase = _applicationSettingService.GetValue(ApplicationSettingKey.UrlBaseApiExGetHtmlElement);

                    var fileStoreData = Path.Combine(_setting.FolderSaveData, _setting.DataFile);
                    var filePathNewInChap = _setting.FolderSaveData + _setting.FolderNewestData + _setting.NewestDataFile;

                    lstStoryFollows = FileReader.AddHomeUrlLink(lstStoryFollows, homeLinkWithSub);
                    LogHelper.Info($"SCAN---lstStoryFollows(s):{lstStoryFollows.Count}");
                    var allCurrentStoryListStores = FileReader.ReadListDataFromFile<StorySaveInfo>(fileStoreData);

                    var all_rs_eachChaps = new List<string>();
                    foreach (var lstStoryFollow in lstStoryFollows)
                    {
                        var rs_eachChaps = new List<string>();
                        var dataNewestChap = FindNewChapInStory(allCurrentStoryListStores, lstStoryFollow, ref rs_eachChaps, urlBase);
                        SaveDataToFile(filePathNewInChap, rs_eachChaps, dataNewestChap);
                    }
                    FileReader.WriteDataToFile<StorySaveInfo>(fileStoreData, allCurrentStoryListStores);
                    LogHelper.Info($"SCAN---Found CHAP newest:" + $"--:{""}");

                    statusScan = true;
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"SCAN---StartScanJob" + ex);
                    statusScan = true;
                }
            }
            else
            {
                LogHelper.Info("SCAN--Still in before scan");
            }
        }
        private static List<string> SaveDataToFile(string filePathNewInChap, List<string> rs_eachChaps, List<NewestChapModel> newestChapModel)
        {
            if (rs_eachChaps.Any())
            {
                rs_eachChaps = UpdateDeleteHomePage(rs_eachChaps);
                newestChapModel[0].ChapLinks = rs_eachChaps;
                var chapPerFile = 100;

                if (rs_eachChaps.Count > chapPerFile)
                {
                    //var newestChapModels = SplitNewestChapModel(newestChapModel[0], chapPerFile);
                    //for (var index = 0; index < newestChapModels.Count; index++)
                    //{
                    //    var fullFilePath2 = filePathNewInChap + $"_{newestChapModel[0].StoryName}" + $"_{index.ToString().PadLeft(5, '0')}" + ".json";
                    //    FileReader.WriteDataToFile(fullFilePath2, new List<NewestChapModel>() { newestChapModels[index] }, 300);
                    //    Thread.Sleep(100);
                    //}
                    var fullFilePath2 = filePathNewInChap + $"_{newestChapModel[0].StoryName}" + ".json";
                    FileReader.WriteDataToFile(fullFilePath2, newestChapModel, 300);
                }
                else
                {
                    var fullFilePath = filePathNewInChap + $"_{newestChapModel[0].StoryName}" + $"_00000" + ".json";
                    FileReader.WriteDataToFile(fullFilePath, newestChapModel, 300);
                }
                List<string> UpdateDeleteHomePage(List<string> urls)
                {
                    var newUrls = new List<string>();
                    foreach (var urlx in urls)
                    {
                        var str = FileReader.DeleteHomePage(urlx);
                        str = DeleteNameTruyen(str);
                        if (!String.IsNullOrEmpty(str)) newUrls.Add(str);
                    }
                    return newUrls;
                }
                string DeleteNameTruyen(string url)
                {
                    var urlShort = url.Replace(newestChapModel[0].StoryName, "");
                    return urlShort;
                }
            }

            return rs_eachChaps;
        }

        private static List<NewestChapModel> SplitNewestChapModel(NewestChapModel newestChapModel, int chapPerFile = 10)
        {
            var rs = new List<NewestChapModel>();
            var count = newestChapModel.ChapLinks.Count / chapPerFile;
            for (int i = 0; i < count; i++)
            {
                var data = new NewestChapModel()
                {
                    Author = newestChapModel.Author,
                    StoryLink = newestChapModel.StoryLink,
                    StoryName = newestChapModel.StoryName,
                    StoryNameShow = newestChapModel.StoryNameShow,
                };

                for (int j = 0; j < chapPerFile; j++)
                {
                    data.ChapLinks.Add(newestChapModel.ChapLinks[i * chapPerFile + j]);
                }
                rs.Add(data);

                if (i == count - 1)
                {
                    var other = newestChapModel.ChapLinks.Count % chapPerFile;
                    if (other > chapPerFile / 2)
                    {
                        var data2 = new NewestChapModel()
                        {
                            Author = newestChapModel.Author,
                            StoryLink = newestChapModel.StoryLink,
                            StoryName = newestChapModel.StoryName,
                            StoryNameShow = newestChapModel.StoryNameShow,
                        };
                        for (int o = 0; o < other; o++)
                        {
                            data2.ChapLinks.Add(newestChapModel.ChapLinks[count * chapPerFile + o]);
                        }
                        rs.Add(data2);
                    }
                    else
                    {
                        for (int o = 0; o < other; o++)
                        {
                            data.ChapLinks.Add(newestChapModel.ChapLinks[count * chapPerFile + o]);
                        }
                    }
                }
            }
            return rs;
        }
        public bool StartScanNewStory()
        {

            return true;
        }
        private List<NewestChapModel> FindNewChapInStory(List<StorySaveInfo> allCurrentStoryListStores, string urlStory, ref List<string> rs_eachChaps, string urlBase = "")
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
                    StoryTypes = ConvertStoryTypes(fetchedData.StoryTypes),
                });
                //LogHelper.Info($"newestChapModel: {JsonConvert.SerializeObject(newestChapModel)}");
                if (!allNameStorySaved.Contains(storyName))//new story
                {
                    allCurrentStoryListStores.Add(
                        new StorySaveInfo()
                        {
                            StoryName = storyName,
                            ChapStoredNewest = fetchedData.ChapPluss.Any() ? fetchedData.ChapPluss.Select(t => t.ChapIndexNumber).Max() : 0,
                        });
                    rs_eachChaps = fetchedData.ChapPluss.Select(t => t.ChapLink).ToList();
                }
                else//old story
                {
                    foreach (var stored in allCurrentStoryListStores)
                    {
                        if (stored.StoryName == storyName)
                        {
                            var newestChapIndexNumbere = fetchedData.ChapPluss.Any() ? fetchedData.ChapPluss.Select(t => t.ChapIndexNumber).Max() : stored.ChapStoredNewest;
                            if (newestChapIndexNumbere > stored.ChapStoredNewest)
                            {
                                //Has new chap
                                var temps = fetchedData.ChapPluss.Where(c => c.ChapIndexNumber > stored.ChapStoredNewest).ToList();
                                stored.ChapStoredNewest = newestChapIndexNumbere;
                                rs_eachChaps = temps.Select(t => t.ChapLink).ToList();
                            }
                            else
                            {
                                if (!fetchedData.ChapPluss.Any())
                                {
                                    //Error get chap info:

                                }
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

        private List<int> ConvertStoryTypes(List<string> storyTypes)
        {
            var allTypes = _storyTypeService.GetAllStoryType();
            var rs = new List<int>();
            foreach (var storyType in storyTypes)
            {
                var t = allTypes.Where(t => t.Name == storyType).FirstOrDefault();
                if (t != null)
                {
                    rs.Add(t.TypeID);
                }
            }
            return rs;
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