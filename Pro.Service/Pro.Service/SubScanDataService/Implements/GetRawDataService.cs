﻿using FileManager;
using MongoDB.Driver;
using Pro.Common;
using Pro.Common.Const;
using Pro.Model;
using System.Text.RegularExpressions;

namespace Pro.Service.SubScanDataService.Implements
{
    public class GetRawDataService : IGetRawDataService
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly AppBuildDataSetting _setting;

        public GetRawDataService(IApplicationSettingService applicationSettingService)
        {
            _applicationSettingService = applicationSettingService;
            var settings = _applicationSettingService.GetValueGetScan(ApplicationSettingKey.AppsettingsScanGet);
            _setting = JsonManager.StringJson2Object<AppBuildDataSetting>(settings);
#if DEBUG
            _setting.FolderSaveData = Constants.DEBUG_DATA_FOLDER;
#endif
        }

        public bool GetRawDatasForNew(NewStory newestData)
        {
            string nameTruyen, chapName = "";
            foreach (var chap in newestData.Chaps)
            {
                var dataLinks = GetImageDatasFromWeb(chap.Link);
                dataLinks.ForEach(link => chap.Images.Add(new Model.ImageData()
                {
                    OriginLink = link
                }));
                FileReader.GetChapInfo(chap.Link, out nameTruyen, out chapName);
                chap.Name = chapName;
            }
            newestData.Picture = GetPictureLinkFormStoryLinkByAPI(newestData.Link);

            if (newestData.Chaps.Where(data => data.Images.Any()).Any())
            {
                return true;
            }
            return false;
        }

        public bool FindNewStory(int numberPage, string homeUrl)
        {
            var listStorys = new List<string>();
            for (int page = 0; page <= numberPage; page++)
            {
                try
                {
                    var storysOnPage = GetStorysOnPageByAPI(numberPage, homeUrl);
                    listStorys.AddRange(storysOnPage);
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"Error_FindNewStory:{homeUrl}&{numberPage}" + ex);
                }

            }
            listStorys = listStorys.Distinct().ToList();

            var tempLists = new List<string>();
            listStorys.ForEach(link =>
            {
                var temp = Regex.Match(link, "[^/]*$").Value;
                if (!String.IsNullOrEmpty(temp))
                    tempLists.Add(temp);
            });

            if (tempLists.Any())
            {
                //Check current following story in file
                var fileStoryFollowPath = Path.Combine(_setting.FolderSaveData, _setting.DataStoryFollowsFile);
                var lstStoryFollows = FileReader.ReadListDataFromFile<string>(fileStoryFollowPath);
                var newStorys = tempLists.Where(a => !lstStoryFollows.Contains(a)).ToList();
                if (newStorys.Any())
                {
                    lstStoryFollows.AddRange(newStorys);
                    lstStoryFollows = lstStoryFollows.Distinct().ToList(); ;
                    FileReader.WriteDataToFile(fileStoryFollowPath, lstStoryFollows, 300);
                }
            }

            return true;
        }

        #region Private func
        public List<string> GetImageDatasFromWeb(string urlChap, int retryTime = 2, int delayTimeInMiniSecond = 7000)
        {
            var listImagesInChap = new List<string>();
            for (int i = 1; i <= retryTime; i++)
            {
                listImagesInChap = GetImageLinksFromWebByAPI(urlChap);
                if (!listImagesInChap.Any())
                {
                    System.Threading.Thread.Sleep(delayTimeInMiniSecond);
                    continue;
                }
                else
                {
                    break;
                }
            }

            return listImagesInChap;
        }

        private List<string> GetImageLinksFromWebByAPI(string textUrl)
        {
            var urlBase = _applicationSettingService.GetValue(ApplicationSettingKey.UrlBaseApiExGetHtmlElement); ;
            var rs = new ApiHelper().Post<List<string>>($"/api/GetImageLinksByAPI?textUrl={textUrl}", null, urlBase);
            return rs == null || !rs.Any() ? new List<string>() : rs;
        }

        private string GetPictureLinkFormStoryLinkByAPI(string urlStory)
        {
            var urlBase = _applicationSettingService.GetValue(ApplicationSettingKey.UrlBaseApiExGetHtmlElement);
            var rs = new ApiHelper().Post<string>($"/api/GetPictureLinkFormStoryLinkByAPI?url={urlStory}", null, urlBase);
            return rs == null || !rs.Any() ? urlStory : rs;
        }

        private List<string> GetStorysOnPageByAPI(int numberPage, string homeUrl)
        {
            var urlBase = _applicationSettingService.GetValue(ApplicationSettingKey.UrlBaseApiExGetHtmlElement); ;
            var rs = new ApiHelper().Get<List<string>>($"/api/FindNewStoryInPageAPI?numberPage={numberPage}&homeUrl={homeUrl}", urlBase);
            return rs == null || !rs.Any() ? new List<string>() : rs;
        }
        #endregion
    }
}
