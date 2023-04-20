using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Pro.Common;
using Pro.Common.Const;
using System.Net;

namespace Pro.Service.Implement
{
    public class UpFile2CloudinaryService : IUploadImageService
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private List<Cloudinary> _cloudinarys = new List<Cloudinary>();

        public UpFile2CloudinaryService(IApplicationSettingService applicationSettingService)
        {
            _applicationSettingService = applicationSettingService;

            var allSettings = _applicationSettingService.GetAllCloudarySettings(ApplicationSettingKey.CloundSetting);
            if (allSettings.Any())
            {
                foreach (var setting in allSettings)
                {
                    var cloudinarySettings = JsonManager.StringJson2Object<CloudinarySettings>(setting);
                    Account acc = new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
                    var cloudinary = new Cloudinary(acc);
                    cloudinary.Api.Timeout = 60000;//60s
                    _cloudinarys.Add(cloudinary);
                }
            }

        }
        public IResultUpload UploadImage(string filePath)
        {
            var result = new IResultUpload();
            result.ResultStatus = 0;
            var cloudinary = GetCloundaryRandom();
            if (cloudinary != null)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(filePath)
                };
                var uploadResult = cloudinary.Upload(uploadParams);
                if (uploadResult != null && uploadResult.JsonObj != null)
                {
                    var rs = uploadResult.JsonObj.ToObject<JsonObj>();
                    if (rs != null && !String.IsNullOrEmpty(rs.url))
                    {
                        result.ResultStatus = 1;
                        result.Url = rs.url;
                    }
                }
            }
            return result;
        }
        public IResultUpload UploadImage(string fileName, string filePath, string pathSave = "")
        {
            var result = new IResultUpload();
            result.ResultStatus = 0;
            var cloudinary = GetCloundaryRandom();
            if (cloudinary != null && !string.IsNullOrEmpty(filePath))
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(filePath),
                    Format = "jpg",
                    Folder = pathSave,
                    DisplayName = fileName,
                };
                var uploadResult = cloudinary.Upload(uploadParams);
                if (uploadResult != null && uploadResult.JsonObj != null)
                {
                    var rs = uploadResult.JsonObj.ToObject<JsonObj>();
                    if (rs != null && !String.IsNullOrEmpty(rs.url))
                    {
                        result.ResultStatus = 1;
                        result.Url = rs.url;
                    }
                }
            }
            return result;
        }
        public IResultUpload UploadImage(string fileName, string url, string pathSave = "", bool isNeedConvert = false, int retryTimes = 2, int sleepNextRetryTime = 15 * 1000)
        {
            Stream streamFile = null;
            if (isNeedConvert)
            {
                for (int retryTime = 1; retryTime <= retryTimes; retryTime++)
                {
                    try
                    {

                        System.Net.HttpWebRequest wr = (System.Net.HttpWebRequest)WebRequest.Create(url);

                        wr.Referer = _applicationSettingService.GetValue(ApplicationSettingKey.HomePage);//No need sub

                        System.Net.WebResponse res = wr.GetResponse();

                        streamFile = res.GetResponseStream();
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (retryTime == retryTimes)
                        {
                            //LogHelper.Error($"UploadImage-isNeedConvert-Error,pathSave:{pathSave}" + ex);
                        }
                        else if (retryTime < retryTimes)
                        {
                            System.Threading.Thread.Sleep(sleepNextRetryTime);
                            continue;
                        }
                    }
                }
                if (streamFile == null)
                {
                    LogHelper.Info($"UploadImage-isNeedConvert:{url}");
                    return new IResultUpload();
                }
            }

            var result = new IResultUpload();
            result.ResultStatus = 0;
            var cloudinary = GetCloundaryRandom();
            if (cloudinary != null)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = streamFile != null ? new FileDescription(fileName, streamFile) : new FileDescription(fileName, url),
                    Format = "jpg",
                    Folder = pathSave,
                    DisplayName = fileName,
                    UseFilename = true,
                };
                try
                {
                    bool isOK = false;
                    var uploadResult = new ImageUploadResult();
                    for (int retry = 1; retry <= retryTimes; retry++)
                    {
                        isOK = false;
                        try
                        {
                            uploadResult = cloudinary.Upload(uploadParams);
                        }
                        catch (Exception ex)
                        {
                            if (retry == retryTimes)
                            {
                                LogHelper.Error($"UploadImage-Exception-Retrytime:{retry},{url},pathSave:{pathSave}" + ex);
                                //throw;
                            }
                        }
                        isOK = IsUploadResultOK(uploadResult);
                        if (isOK) break;
                        System.Threading.Thread.Sleep(sleepNextRetryTime);
                    }

                    if (isOK)
                    {
                        var rs = uploadResult.JsonObj.ToObject<JsonObj>();
                        if (rs != null && !String.IsNullOrEmpty(rs.url))
                        {
                            result.ResultStatus = 1;
                            result.Url = rs.url;
                        }
                    }
                    else
                    {
                        //LogHelper.Info($"UploadImage-allResult:{JsonConvert.SerializeObject(uploadResult)}");
                        result.ErrorMessage = uploadResult.Error?.Message;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"UploadImage-Exception-url={url}" + ex);
                    result.ErrorMessage = ex.Message;
                    return result;
                }
            }
            return result;
        }
        private bool IsUploadResultOK(ImageUploadResult uploadResult)
        {
            if (uploadResult != null && uploadResult.JsonObj != null)
            {
                var rs = uploadResult.JsonObj.ToObject<JsonObj>();
                if (rs != null && !String.IsNullOrEmpty(rs.url))
                {
                    return true;
                }
            }
            return false;
        }
        Cloudinary GetCloundaryRandom()
        {
            var random = new Random();
            var number = random.Next(0, _cloudinarys.Count());
            var rs = _cloudinarys[number];
            return rs;
        }
        public DataStoryForSave UploadLink2Store(DataStoryForSave dataStory)
        {

            foreach (var chapSave in dataStory.ChapDataForSaves)
            {
                var savePath = $"/Truyen-tranh2/{dataStory.StoryName}/{chapSave.ChapName}/";//Folder save on clound
                foreach (var link in chapSave.ImageDatas)
                {
                    var rsUp = UploadImage("0", link.ImageLinkFromWeb, savePath, true);

                    if (rsUp.ResultStatus > 0)//Success
                    {
                        link.ImageLinkNeedSaveDB = rsUp.Url;
                    }
                    else
                    {
                        LogHelper.Error($"Error DownLoadLinks- cannot get cloud link{link.ImageLinkFromWeb},ErrorMes:{rsUp.ErrorMessage}");
                    }
                }
            }
            dataStory.StoryPictureLink = MakeStoryPictureLinkForNewStory(dataStory.StoryPictureLink) ?? dataStory.StoryPictureLink;
            return dataStory;
        }

        private string? MakeStoryPictureLinkForNewStory(string storyPictureLink)
        {
            var linkCloundary = UploadImage("fileName", storyPictureLink, "Truyen-tranh2/StoryPictures", false);
            if (linkCloundary.ResultStatus > 0 && !String.IsNullOrEmpty(linkCloundary.Url))
            {
                return linkCloundary.Url;
            }
            return null;
        }
    }
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
    public class JsonObj
    {
        public string asset_id { get; set; }
        public string public_id { get; set; }
        public string version { get; set; }
        public string version_id { get; set; }
        public string signature { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string format { get; set; }
        public string resource_type { get; set; }
        public string created_at { get; set; }
        public int bytes { get; set; }
        public string type { get; set; }
        public string etag { get; set; }
        public bool placeholder { get; set; }
        public string url { get; set; }
        public string secure_url { get; set; }
        public string folder { get; set; }
        public string original_filename { get; set; }
        public string api_key { get; set; }
    }
}
