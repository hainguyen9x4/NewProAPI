using FileManager;
using MongoDB.Driver;
using Pro.Common;
using Pro.Common.Const;
using Pro.Common.Enum;
using Pro.Data.Repositorys;
using Pro.Model;
using Pro.Service.WebData;

namespace Pro.Service.Implements
{
    public class ScanDataService : IScanDataService
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IStoryFollowsService _storyFollowsService;
        private readonly IStoryTypeService _storyTypeService;
        private readonly IFileStoryService _fileStoryService;
        private readonly IGetDataFromWebService _getDataFromWebService;
        private readonly IResultScanDataRepository _resultScanDataRepository;

        private readonly AppBuildDataSetting _setting;
        private readonly IAppSettingData _settingData;

        public ScanDataService(IApplicationSettingService applicationSettingService,
            IAppSettingData appSettingData,
            IStoryFollowsService storyFollowsService,
            IStoryTypeService storyTypeService,
            IFileStoryService fileStoryService,
            IGetDataFromWebService getDataFromWebService,
            IResultScanDataRepository resultScanDataRepository)
        {
            _applicationSettingService = applicationSettingService;
            _settingData = appSettingData;
            _storyFollowsService = storyFollowsService;
            _storyTypeService = storyTypeService;
            _fileStoryService = fileStoryService;
            _getDataFromWebService = getDataFromWebService;
            var settings = _applicationSettingService.GetValueGetScan(ApplicationSettingKey.AppsettingsScanGet, useOtherSetting: _settingData.UseSettingGetSetNumber);
            _setting = JsonManager.StringJson2Object<AppBuildDataSetting>(settings);
#if DEBUG
            _setting.FolderSaveData = Constants.DEBUG_DATA_FOLDER;
#endif
            _storyTypeService = storyTypeService;
            _resultScanDataRepository = resultScanDataRepository;
        }

        public bool StartScanData()
        {
            var fileStoryFollowPath = Path.Combine(_setting.FolderSaveData, _setting.DataStoryFollowsFile);
            //var lstStoryForllows = FileReader.ReadListDataFromFile<string>(fileStoryFollowPath);
            var lstStoryForllows = _storyFollowsService.GetAllStoryFollows(STATUS_FOLLOW.ENABLE, false);
            var orignalLstStoryForllows = new List<StoryFollow>();
            foreach (var lst in lstStoryForllows)
            {
                var s = new StoryFollow();
                s.Link = lst.Link;
                s.Status = lst.Status;
                s.Id = lst.Id;
                orignalLstStoryForllows.Add(s);
            }
            if (lstStoryForllows.Any())
            {
                StartScanJob(lstStoryForllows);
                //Update flowStoryStatus
                foreach (var lst in lstStoryForllows)
                {
                    var originalItem = orignalLstStoryForllows.Where(ori => ori.Id == lst.Id && ori.Status != lst.Status).FirstOrDefault();
                    if (originalItem != null)
                    {
                        _storyFollowsService.UpdateStoryFollows(originalItem.Id, lst.Status, "");
                    }
                }
                return true;
            }
            return false;
        }
        private static bool statusScan = true;
        private void StartScanJob(List<StoryFollow> lstStoryFollows)
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

                    foreach (var lstStoryFollow in lstStoryFollows)
                    {
                        lstStoryFollow.Link = FileReader.AddHomeUrlLink(lstStoryFollow.Link, homeLinkWithSub);
                    }

                    LogHelper.Info($"SCAN---lstStoryFollows(s):{lstStoryFollows.Count}");
                    //var allCurrentStoryListStores = FileReader.ReadListDataFromFile<StorySaveInfo>(fileStoreData);
                    var allCurrentStoryListStores = _fileStoryService.GetAllFileStory();

                    var all_rs_eachChaps = new List<string>();
                    var storyIDsChange = new List<int>();
                    foreach (var storyFollow in lstStoryFollows)
                    {
                        var rs_eachChaps = new List<string>();
                        var dataNewestChap = FindNewChapInStory(allCurrentStoryListStores, storyFollow, ref rs_eachChaps, urlBase);
                        if (storyFollow.Status != STATUS_FOLLOW.DISABLE)
                        {
                            SaveDataToFile2DB(filePathNewInChap, rs_eachChaps, dataNewestChap);
                        }
                        storyIDsChange.Add(dataNewestChap.Select(s => s.StoryID).First());
                    }
                    //FileReader.WriteDataToFile<StorySaveInfo>(fileStoreData, allCurrentStoryListStores);
                    //Update to DB
                    _fileStoryService.UpdateAllFileStory(allCurrentStoryListStores.Where(a => storyIDsChange.Contains(a.Id)).ToList());

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
                    var fullFilePath2 = filePathNewInChap + $"_{newestChapModel[0].StoryName}" + $"_{Guid.NewGuid().ToString()}" + ".json";
                    FileReader.WriteDataToFile(fullFilePath2, newestChapModel, 300);
                }
                else
                {
                    var fullFilePath = filePathNewInChap + $"_{newestChapModel[0].StoryName}" + $"_{Guid.NewGuid().ToString()}" + ".json";
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
        private List<string> SaveDataToFile2DB(string filePathNewInChap, List<string> rs_eachChaps, List<NewestChapModel> newestChapModel)
        {
            if (rs_eachChaps.Any())
            {
                rs_eachChaps = UpdateDeleteHomePage(rs_eachChaps);
                newestChapModel[0].ChapLinks = rs_eachChaps;
                var rsScanDatas = new List<ResultScanData>();
                foreach (var chap in rs_eachChaps)
                {
                    rsScanDatas.Add(new ResultScanData()
                    {
                        StoryID = newestChapModel[0].StoryID,
                        Status = 0,
                        ChapLink = chap
                    });
                }
                _resultScanDataRepository.Creates(rsScanDatas);
                var fullFilePath = filePathNewInChap + $"_{newestChapModel[0].StoryName}" + $"_{Guid.NewGuid().ToString()}" + ".json";
                FileReader.WriteDataToFile(fullFilePath, newestChapModel, 300);

                //Local func.
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
        private List<NewestChapModel> FindNewChapInStory(List<FileStory> allCurrentStoryListStores, StoryFollow storyFollow, ref List<string> rs_eachChaps, string urlBase = "")
        {
            var newestChapModel = new List<NewestChapModel>();
            var storyName = "";
            //rs_eachChaps = new List<string>();
            try
            {
                //string storyNameShow = "";
                var fetchedData = GetStoryInfoWithChapByAPI(storyFollow.Link, urlBase);
                if (fetchedData.ChapPluss != null && fetchedData.ChapPluss.Any())
                {

                    storyName = FileReader.GetStoryInfoFromUrlStory(storyFollow.Link);
                    var allNameStorySaved = allCurrentStoryListStores.Select(t => t.StoryName).ToList();
                    //LogHelper.Info($"fetchedChaps: {JsonConvert.SerializeObject(fetchedChaps)}");
                    newestChapModel.Add(new NewestChapModel()
                    {
                        StoryID = storyFollow.Id,
                        StoryName = storyName,
                        StoryLink = FileReader.DeleteHomePage(storyFollow.Link),
                        StoryNameShow = fetchedData.StoryName,
                        Description = fetchedData.Description,
                        StoryTypes = ConvertStoryTypes(fetchedData.StoryTypes),
                    });
                    //LogHelper.Info($"newestChapModel: {JsonConvert.SerializeObject(newestChapModel)}");
                    if (!allNameStorySaved.Contains(storyName))//new story
                    {
                        allCurrentStoryListStores.Add(
                            new FileStory()
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
                    storyFollow.Status = STATUS_FOLLOW.ENABLE;
                }
                else//Cant get data from link
                {
                    storyFollow.Status = STATUS_FOLLOW.DISABLE;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error_FindNewChapInStory :{storyFollow}" + ex);
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
        private Model.StoryInfoWithChaps GetStoryInfoWithChapByAPI(string textUrl, string urlBase)
        {
            var data = _getDataFromWebService.GetStoryInfoWithChaps(textUrl);
            //var data = new ApiHelper().Post<StoryInfoWithChaps>($"/api/GetStoryInfoWithChaps?textUrl={textUrl}", null, urlBase);
            return data == null ? new Model.StoryInfoWithChaps() : data;
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