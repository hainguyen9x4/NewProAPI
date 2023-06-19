using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FileManager;
using Pro.Common;
using Pro.Common.Const;
using Pro.Model;
using System.Net;

namespace Pro.Service.Implement
{
    public class Cloudinarys
    {
        public bool IsActived { get; set; }
        public Cloudinary CloudinaryItem { get; set; }
    }

    public class UpFile2CloudinaryService : IUploadImageService
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private Cloudinary _cloudinary;

        public UpFile2CloudinaryService(IApplicationSettingService applicationSettingService)
        {
            _applicationSettingService = applicationSettingService;

            var allSettings = _applicationSettingService.GetAllCloudarySettings(ApplicationSettingKey.CloundSetting, useCache: false);
            if (allSettings.Any())
            {
                var cloudinarySettings = JsonManager.StringJson2Object<CloudinarySettings>(allSettings.First());
                Account acc = new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
                _cloudinary = new Cloudinary(acc);
                _cloudinary.Api.Timeout = 60000;//60s
            }
            else
            {
            }
        }
        private IResultUpload UploadImage(string fileName, string localLink, string pathSave = "", bool isNeedConvert = false, int retryTimes = 2, int sleepNextRetryTime = 15 * 1000)
        {
            var acc = new WaitForInternetAccess();

            var result = new IResultUpload();
            result.ResultStatus = 0;

            if (_cloudinary != null)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, localLink),
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
                            uploadResult = _cloudinary.Upload(uploadParams);
                        }
                        catch (Exception ex)
                        {
                            if (retry == retryTimes)
                            {
                                LogHelper.Error($"UploadImage-Exception-Retrytime:{retry},{localLink},pathSave:{pathSave}" + ex);
                                //throw;
                            }
                        }
                        isOK = IsUploadResultOK(uploadResult);
                        if (isOK) break;
                        System.Threading.Thread.Sleep(sleepNextRetryTime);
                        acc.WaitInternetAccess("Uploadcloudinary");
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
                    LogHelper.Error($"UploadImage-Exception-url={localLink}" + ex);
                    result.ErrorMessage = ex.Message;
                }
            }
            return result;
        }
        private IResultUpload UploadImageByLink(string fileName, string link, string pathSave = "", bool isNeedConvert = false, int retryTimes = 2, int sleepNextRetryTime = 15 * 1000)
        {
            var acc = new WaitForInternetAccess();

            var result = new IResultUpload();
            result.ResultStatus = 0;

            if (_cloudinary != null)
            {
                var stream = GetStreamImage(link);
                if (stream != null)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(Guid.NewGuid().ToString(), stream),
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
                                uploadResult = _cloudinary.UploadAsync(uploadParams).Result;
                            }
                            catch (Exception ex)
                            {
                                if (retry == retryTimes)
                                {
                                    LogHelper.Error($"UploadImage-Exception-Retrytime:{retry},{link},pathSave:{pathSave}" + ex);
                                    //throw;
                                }
                            }
                            isOK = IsUploadResultOK(uploadResult);
                            if (isOK) break;
                            System.Threading.Thread.Sleep(sleepNextRetryTime);
                            acc.WaitInternetAccess("Uploadcloudinary");
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
                        LogHelper.Error($"UploadImage-Exception-url={link}" + ex);
                        result.ErrorMessage = ex.Message;
                    }
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

        private async Task<NewStory> UpLoadDataAsyncForNew(NewStory dataStory)
        {
            var totalImages = 0;
            foreach (var chapSave in dataStory.Chaps)
            {
                var savePath = $"/Truyen-tranh2/{dataStory.Name}/{chapSave.Name}/";//Folder save on clound
                foreach (var link in chapSave.Images)
                {
                    var rsUp = UploadImageByLink("0", link.OriginLink, savePath, true);

                    if (rsUp.ResultStatus > 0)//Success
                    {
                        link.Link = rsUp.Url;
                        link.OriginLink = "";
                        link.LocalLink = link.LocalLink;
                    }
                    else
                    {
                        LogHelper.Error($"Error UpLoadDataAsyn- cannot cloud link:{link.OriginLink};{dataStory.Name}/{chapSave.Name},ErrorMes:{rsUp.ErrorMessage}");
                    }
                }
                totalImages += chapSave.Images.Count();
                chapSave.Link = FileReader.DeleteHomePage(chapSave.Link);
            }
            _cloudinary = UploadDataToAppSetting(_cloudinary, totalImages);
            return dataStory;
        }
        private Cloudinary GetCloudinary()
        {
            var allSettings = _applicationSettingService.GetAllCloudarySettings(ApplicationSettingKey.CloundSetting, useCache: false);

            var cloudinarySettings = JsonManager.StringJson2Object<CloudinarySettings>(allSettings.First());
            Account acc = new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
            var cloudinary = new Cloudinary(acc);
            cloudinary.Api.Timeout = 60000;//60s

            return cloudinary;
        }
        private Cloudinary UploadDataToAppSetting(Cloudinary cloudinary, int newNumberImages)
        {
            var clound = _applicationSettingService.Get().Where(s => s.AppSettingValue.Contains(cloudinary.Api.Account.ApiKey)).FirstOrDefault();
            if (clound != null)
            {
                clound.NumberImage += newNumberImages;
                if (clound.NumberImage >= Constants.MAX_IMAGE)
                {
                    clound.AppSettingIsActive = false;
                }
                _applicationSettingService.Update(clound.AppSettingId, clound);
                if (clound.NumberImage >= Constants.MAX_IMAGE)
                {
                    return GetCloudinary();
                }
            }
            return cloudinary;
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
        public bool HasValidCloudinary()
        {
            var allSettings = _applicationSettingService.GetAllCloudarySettings(ApplicationSettingKey.CloundSetting, useCache: false);
            if (!allSettings.Any())
            {
                return false;
            }
            return true;
        }
        public void UploadLink2StoreWith3ThreadsForNew(NewStory dataStory)
        {
            var listDatas = DividingObject(dataStory, 15);
            List<Task<NewStory>> tasks = new List<Task<NewStory>>();
            foreach (var item in listDatas)
            {
                tasks.Add(Task.Run(async () => await UpLoadDataAsyncForNew(item)));
            }
            //var ss = Task.Run(async () => await UpLoadDataAsyncForNew(listDatas[0])).Result;
            var t = Task.WhenAll(tasks);
            t.Wait();

            dataStory.Chaps.Clear();
            foreach (var rs in t.Result)
            {
                dataStory.Chaps.AddRange(rs.Chaps);
            }
            dataStory.Picture = MakeStoryPictureLinkForNewStory(dataStory.Picture) ?? dataStory.Picture;
        }
        private List<NewStory> DividingObject(NewStory dataStory, int numberObject)
        {
            var rs = new List<NewStory>();

            var subChapLists = Chunk(dataStory.Chaps, numberObject).ToList();
            foreach (var subChapList in subChapLists)
            {
                rs.Add(new NewStory()
                {
                    Chaps = subChapList,
                    Name = dataStory.Name,
                    NameShow = dataStory.NameShow,
                    Link = dataStory.Link,
                    Picture = dataStory.Picture,
                    OtherInfo = dataStory.OtherInfo
                });
            }

            return rs;
        }
        private static List<List<Chap>> Chunk(List<Chap> source, int chunksize)
        {
            var rs = new List<List<Chap>>();
            while (source.Any())
            {
                rs.Add(source.Take(chunksize).ToList());
                source = source.Skip(chunksize).ToList();
            }
            return rs;
        }
        private Stream GetStreamImage(string url, int retryTimes = 2, int sleepTime = 400)
        {
            Stream streamFile = null;
            for (int retryTime = 0; retryTime < retryTimes; retryTime++)
            {
                try
                {
                    System.Net.HttpWebRequest wr = (System.Net.HttpWebRequest)WebRequest.Create(url);
                    wr.Referer = _applicationSettingService.GetValue(ApplicationSettingKey.HomePage);//No need sub
                    wr.Proxy = null;
                    wr.Timeout = 1200000;
                    wr.ReadWriteTimeout = 1200000;
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
                        System.Threading.Thread.Sleep(sleepTime);

                        var acc = new WaitForInternetAccess();
                        acc.WaitInternetAccess("GetStreamImage");
                        continue;
                    }
                }
            }
            return streamFile;
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
