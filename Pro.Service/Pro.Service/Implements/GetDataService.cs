using FileManager;
using Pro.Common;
using Pro.Common.Const;
using Pro.Model;
using Pro.Service.SubScanDataService;

namespace Pro.Service.Implements
{
    public class GetDataService : IGetDataService
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IUploadImageService _uploadImageService;
        private readonly IPrepareService _prepareService;
        private readonly IGetRawDataService _getRawDataService;
        private readonly ISaveImage2Local _saveImage2Local;
        private readonly IUpData2DBService _upData2DBService;

        public GetDataService(IApplicationSettingService applicationSettingService
            , IUploadImageService uploadImageService
            , IPrepareService prepareService
            , IGetRawDataService getRawDataService
            , ISaveImage2Local saveImage2Local
            , IUpData2DBService upData2DBService)
        {
            _uploadImageService = uploadImageService;
            _applicationSettingService = applicationSettingService;
            _prepareService = prepareService;
            _getRawDataService = getRawDataService;
            _saveImage2Local = saveImage2Local;
            _upData2DBService = upData2DBService;
        }

        private static bool statusGetData = true;
        private static bool statusGetData2 = true;

        public bool StartGetDataForNewStory()
        {
            if (statusGetData2)
            {
                try
                {
                    statusGetData2 = false;
                    LogHelper.Info($"GET---Start GetDataService");
                    string localPath = "";
                    var newestChapDatas = _prepareService.PrepareNewestChapDatasForNew(ref localPath);
                    if (newestChapDatas.Chaps != null && newestChapDatas.Chaps.Any())
                    {
                        _getRawDataService.GetRawDatasForNew(newestChapDatas);
                        SaveData2File($@"D:\Debug\RawData{newestChapDatas.Name}.json", newestChapDatas);

                        //Save to file
                        //newestChapDatas = ReadDataFromFile($@"D:\Debug\RawData{newestChapDatas.Name}.json");
                        LogHelper.Info($"GET---Start SaveImage2LocalFunc");
                        _saveImage2Local.SaveImage2LocalFunc(newestChapDatas);
                        SaveData2File($@"D:\Debug\SavedLocal_{newestChapDatas.Name}.json", newestChapDatas);

                        //Uplpad to Clound
                        //newestChapDatas = ReadDataFromFile($@"D:\Debug\SavedLocal_{newestChapDatas.Name}.json");
                        LogHelper.Info($"GET---Start UploadLink2StoreWith3ThreadsForNew");
                        _uploadImageService.UploadLink2StoreWith3ThreadsForNew(newestChapDatas);
                        SaveData2File($@"D:\Debug\HasClound_{newestChapDatas.Name}.json", newestChapDatas);

                        //Save to DB
                        _upData2DBService.UpData2DBForNew(newestChapDatas);
                        //Delete file
                        try
                        {
                            FileReader.DeleteFile(localPath);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error($"DeleteFile/Move {localPath}" + ex);
                        }

                    }
                    statusGetData2 = true;
                    LogHelper.Info($"GET---Stop GetDataService");
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"TaskTrackingGetDataWorking" + ex);
                    statusGetData2 = true;
                    LogHelper.Info($"GET---Stop GetDataService");
                }
            }
            else
            {
                LogHelper.Info($"GET---GetDataService in before process!");
            }
            if (statusGetData && _applicationSettingService.GetIntValue(ApplicationSettingKey.IsNeedReStart) == 1)
            {
                //Need re-start
                _applicationSettingService.SetValue(ApplicationSettingKey.IsNeedReStart, "0");
                string domainName = "";// HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                statusGetData = true;
                new ApiHelper().Get<string>($"api/ResetApp", domainName);
                return true;
            }
            return true;
        }

        public bool FindNewStory(int numberPage, string homeUrl)
        {
            return _getRawDataService.FindNewStory(numberPage, homeUrl);
        }
        private void SaveData2File(string path, NewStory data)
        {
            FileReader.WriteDataToFile2<NewStory>(path, data);
        }
        private NewStory ReadDataFromFile(string filePath)
        {
            return FileReader.ReadListDataFromFile2<NewStory>(filePath);
        }
    }
}