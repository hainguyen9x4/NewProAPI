using FileManager;
using Pro.Common;
using Pro.Common.Const;
using Pro.Service.SubScanDataService;

namespace Pro.Service.Implements
{
    public class GetDataService : IGetDataService
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly IUploadImageService _uploadImageService;
        private readonly IPrepareService _prepareService;
        private readonly IGetRawDataService _getRawDataService;
        private readonly IUpData2DBService _upData2DBService;

        public GetDataService(IApplicationSettingService applicationSettingService
            , IUploadImageService uploadImageService
            , IPrepareService prepareService
            , IGetRawDataService getRawDataService
            , IUpData2DBService upData2DBService)
        {
            _uploadImageService = uploadImageService;
            _applicationSettingService = applicationSettingService;
            _prepareService = prepareService;
            _getRawDataService = getRawDataService;
            _upData2DBService = upData2DBService;
        }

        private static bool statusGetData = true;
        public bool StartGetData()
        {
            if (statusGetData)
            {
                try
                {
                    statusGetData = false;
                    LogHelper.Info($"GET---Start GetDataService");

                    var newestChapDatas = _prepareService.PrepareNewestChapDatas();
                    if (newestChapDatas.ChapLinks.Any())
                    {
                        var rawData = _getRawDataService.GetRawDatas(newestChapDatas);
                        if (rawData.ChapDataForSaves.Any())
                        {
                            var needDowloadDatas = _uploadImageService.UploadLink2Store(rawData);
                            _upData2DBService.UpData2DB(needDowloadDatas);
                            //Delete file
                            try
                            {
                                FileReader.DeleteFile(newestChapDatas.FileDataNewestPathLocal.FullName);
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error($"DeleteFile/Move {newestChapDatas.FileDataNewestPathLocal.FullName}" + ex);
                            }
                        }
                    }
                    statusGetData = true;
                    LogHelper.Info($"GET---Stop GetDataService");
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"TaskTrackingGetDataWorking" + ex);
                    statusGetData = true;
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
    }
}