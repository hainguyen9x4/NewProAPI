using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FileManager;
using MongoDB.Bson.Serialization.Serializers;
using Pro.Common;
using Pro.Common.Const;
using Pro.Model;
using System.Net;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;

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
            var number = _cloudinarys.Count;
            try
            {
                return _cloudinarys[random.Next(number)];
            }
            catch (Exception ex)
            {
                LogHelper.Error($"GetCloundaryRandom: number = {number}, total={_cloudinarys.Count}" + ex);
                return _cloudinarys[0];
            }
            //var random = new Random();
            //var totalCloud = _cloudinarys.Count();
            //var number = random.Next(0, totalCloud);

            //if (number >= totalCloud)
            //{
            //    while (number >= totalCloud)
            //    {
            //        number = number - 1;
            //    }
            //}

            //if (number < 0)
            //    number = 0;
            //try
            //{

            //    return _cloudinarys[number];
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Error($"GetCloundaryRandom: number = {number}, total={_cloudinarys.Count}");
            //    return _cloudinarys[0];
            //}
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
        public async void UploadLink2StoreWith3Threads(DataStoryForSave dataStory)
        {
            var dataThread1 = new DataStoryForSave();
            dataThread1.StoryName = dataStory.StoryName;
            dataThread1.StoryNameShow = dataStory.StoryNameShow;
            dataThread1.StoryLink = dataStory.StoryLink;
            dataThread1.StoryPictureLink = dataStory.StoryPictureLink;
            dataThread1.Author = dataStory.Author;

            var dataThread2 = new DataStoryForSave();
            dataThread2.StoryName = dataStory.StoryName;
            dataThread2.StoryNameShow = dataStory.StoryNameShow;
            dataThread2.StoryLink = dataStory.StoryLink;
            dataThread2.StoryPictureLink = dataStory.StoryPictureLink;
            dataThread2.Author = dataStory.Author;

            var dataThread3 = new DataStoryForSave();
            dataThread3.StoryName = dataStory.StoryName;
            dataThread3.StoryNameShow = dataStory.StoryNameShow;
            dataThread3.StoryLink = dataStory.StoryLink;
            dataThread3.StoryPictureLink = dataStory.StoryPictureLink;
            dataThread3.Author = dataStory.Author;

            var dataThread4 = new DataStoryForSave();
            dataThread4.StoryName = dataStory.StoryName;
            dataThread4.StoryNameShow = dataStory.StoryNameShow;
            dataThread4.StoryLink = dataStory.StoryLink;
            dataThread4.StoryPictureLink = dataStory.StoryPictureLink;
            dataThread4.Author = dataStory.Author;

            var dataThread5 = new DataStoryForSave();
            dataThread5.StoryName = dataStory.StoryName;
            dataThread5.StoryNameShow = dataStory.StoryNameShow;
            dataThread5.StoryLink = dataStory.StoryLink;
            dataThread5.StoryPictureLink = dataStory.StoryPictureLink;
            dataThread5.Author = dataStory.Author;

            var total = dataStory.ChapDataForSaves.Count();
            var value = (total / 20);
            if (value > 4)
            {
                dataThread1.ChapDataForSaves = dataStory.ChapDataForSaves.Take(20).ToList();
                dataThread2.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20).Take(20).ToList();
                dataThread3.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20 * 2).Take(20).ToList();
                dataThread4.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20 * 3).Take(20).ToList();
                dataThread5.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20 * 4).Take(total - 20 * 4).ToList();

            }
            else if (value > 3)
            {
                dataThread1.ChapDataForSaves = dataStory.ChapDataForSaves.Take(20).ToList();
                dataThread2.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20).Take(20).ToList();
                dataThread3.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20 * 2).Take(20).ToList();
                dataThread4.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20 * 3).Take(20).ToList();
                dataThread5.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20 * 4).Take(total - 20 * 4).ToList();
            }
            else if (value > 2)
            {
                dataThread1.ChapDataForSaves = dataStory.ChapDataForSaves.Take(20).ToList();
                dataThread2.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20).Take(20).ToList();
                dataThread3.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20 * 2).Take(20).ToList();
                dataThread4.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20 * 3).Take(total - 20 * 3).ToList();
            }
            else if (value > 1)
            {
                dataThread1.ChapDataForSaves = dataStory.ChapDataForSaves.Take(20).ToList();
                dataThread2.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20).Take(20).ToList();
                dataThread3.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20 * 2).Take(total - 20 * 2).ToList();
            }
            else if (value > 0)
            {
                dataThread1.ChapDataForSaves = dataStory.ChapDataForSaves.Take(20).ToList();
                dataThread2.ChapDataForSaves = dataStory.ChapDataForSaves.Skip(20).Take(total - 20 * 1).ToList();
            }

            //Create 3 thread
            var loadDataTasks = new Task[]
            {
                Task.Run(async () => dataThread1 = await UpLoadDataAsync(dataThread1)),
                Task.Run(async () => dataThread2 = await UpLoadDataAsync(dataThread2)),
                Task.Run(async () => dataThread3 = await UpLoadDataAsync(dataThread3)),
                Task.Run(async () => dataThread4 = await UpLoadDataAsync(dataThread4)),
                Task.Run(async () => dataThread5 = await UpLoadDataAsync(dataThread5))
            };

            try
            {
                var t = Task.WhenAll(loadDataTasks);
                t.Wait();
            }
            catch (Exception ex)
            {
                // handle exception
                var x = 12;
            }

            dataStory.ChapDataForSaves.Clear();
            dataStory.ChapDataForSaves.AddRange(dataThread1.ChapDataForSaves);
            dataStory.ChapDataForSaves.AddRange(dataThread2.ChapDataForSaves);
            dataStory.ChapDataForSaves.AddRange(dataThread3.ChapDataForSaves);
            dataStory.ChapDataForSaves.AddRange(dataThread4.ChapDataForSaves);
            dataStory.ChapDataForSaves.AddRange(dataThread5.ChapDataForSaves);

            dataStory.StoryPictureLink = MakeStoryPictureLinkForNewStory(dataStory.StoryPictureLink) ?? dataStory.StoryPictureLink;
            //return dataStory;
        }

        private async Task<NewStory> UpLoadDataAsyncForNew(NewStory dataStory)
        {
            foreach (var chapSave in dataStory.Chaps)
            {
                var savePath = $"/Truyen-tranh2/{dataStory.Name}/{chapSave.Name}/";//Folder save on clound
                foreach (var link in chapSave.Images)
                {
                    var rsUp = UploadImage("0", link.OriginLink, savePath, true);

                    if (rsUp.ResultStatus > 0)//Success
                    {
                        link.Link = rsUp.Url;
                        link.OriginLink = "";
                    }
                    else
                    {
                        LogHelper.Error($"Error UpLoadDataAsyn- cannot cloud link:{link.OriginLink};{dataStory.Name}/{chapSave.Name},ErrorMes:{rsUp.ErrorMessage}");
                    }
                }
                chapSave.Link = FileReader.DeleteHomePage(chapSave.Link);
            }
            return dataStory;
        }

        private async Task<DataStoryForSave> UpLoadDataAsync(DataStoryForSave dataStory)
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

        public void UploadLink2StoreWith3ThreadsForNew(NewStory dataStory)
        {
            var dataThread1 = new NewStory();
            dataThread1.Name = dataStory.Name;
            dataThread1.NameShow = dataStory.NameShow;
            dataThread1.Link = dataStory.Link;
            dataThread1.Picture = dataStory.Picture;
            dataThread1.OtherInfo = dataStory.OtherInfo;
            dataThread1.Chaps = new List<Chap>();

            var dataThread2 = new NewStory();
            dataThread2.Name = dataStory.Name;
            dataThread2.NameShow = dataStory.NameShow;
            dataThread2.Link = dataStory.Link;
            dataThread2.Picture = dataStory.Picture;
            dataThread2.OtherInfo = dataStory.OtherInfo;
            dataThread2.Chaps = new List<Chap>();

            var dataThread3 = new NewStory();
            dataThread3.Name = dataStory.Name;
            dataThread3.NameShow = dataStory.NameShow;
            dataThread3.Link = dataStory.Link;
            dataThread3.Picture = dataStory.Picture;
            dataThread3.OtherInfo = dataStory.OtherInfo;
            dataThread3.Chaps = new List<Chap>();

            var dataThread4 = new NewStory();
            dataThread4.Name = dataStory.Name;
            dataThread4.NameShow = dataStory.NameShow;
            dataThread4.Link = dataStory.Link;
            dataThread4.Picture = dataStory.Picture;
            dataThread4.OtherInfo = dataStory.OtherInfo;
            dataThread4.Chaps = new List<Chap>();

            var dataThread5 = new NewStory();
            dataThread5.Name = dataStory.Name;
            dataThread5.NameShow = dataStory.NameShow;
            dataThread5.Link = dataStory.Link;
            dataThread5.Picture = dataStory.Picture;
            dataThread5.OtherInfo = dataStory.OtherInfo;
            dataThread5.Chaps = new List<Chap>();

            var total = dataStory.Chaps.Count();
            var value = (total / 20);
            if (value > 4)
            {
                dataThread1.Chaps = dataStory.Chaps.Take(20).ToList();
                dataThread2.Chaps = dataStory.Chaps.Skip(20).Take(20).ToList();
                dataThread3.Chaps = dataStory.Chaps.Skip(20 * 2).Take(20).ToList();
                dataThread4.Chaps = dataStory.Chaps.Skip(20 * 3).Take(20).ToList();
                dataThread5.Chaps = dataStory.Chaps.Skip(20 * 4).Take(total - 20 * 4).ToList();

            }
            else if (value > 3)
            {
                dataThread1.Chaps = dataStory.Chaps.Take(20).ToList();
                dataThread2.Chaps = dataStory.Chaps.Skip(20).Take(20).ToList();
                dataThread3.Chaps = dataStory.Chaps.Skip(20 * 2).Take(20).ToList();
                dataThread4.Chaps = dataStory.Chaps.Skip(20 * 3).Take(20).ToList();
                dataThread5.Chaps = dataStory.Chaps.Skip(20 * 4).Take(total - 20 * 4).ToList();
            }
            else if (value > 2)
            {
                dataThread1.Chaps = dataStory.Chaps.Take(20).ToList();
                dataThread2.Chaps = dataStory.Chaps.Skip(20).Take(20).ToList();
                dataThread3.Chaps = dataStory.Chaps.Skip(20 * 2).Take(20).ToList();
                dataThread4.Chaps = dataStory.Chaps.Skip(20 * 3).Take(total - 20 * 3).ToList();
            }
            else if (value > 1)
            {
                dataThread1.Chaps = dataStory.Chaps.Take(20).ToList();
                dataThread2.Chaps = dataStory.Chaps.Skip(20).Take(20).ToList();
                dataThread3.Chaps = dataStory.Chaps.Skip(20 * 2).Take(total - 20 * 2).ToList();
            }
            else if (value > 0)
            {
                dataThread1.Chaps = dataStory.Chaps.Take(20).ToList();
                dataThread2.Chaps = dataStory.Chaps.Skip(20).Take(total - 20 * 1).ToList();
            }
            else
            {
                dataThread1.Chaps = dataStory.Chaps.ToList();
            }

            //Create 3 thread
            var loadDataTasks = new Task[]
            {
                Task.Run(async () => dataThread1 = await UpLoadDataAsyncForNew(dataThread1)),
                Task.Run(async () => dataThread2 = await UpLoadDataAsyncForNew(dataThread2)),
                Task.Run(async () => dataThread3 = await UpLoadDataAsyncForNew(dataThread3)),
                Task.Run(async () => dataThread4 = await UpLoadDataAsyncForNew(dataThread4)),
                Task.Run(async () => dataThread5 = await UpLoadDataAsyncForNew(dataThread5))
            };

            try
            {
                var t = Task.WhenAll(loadDataTasks);
                t.Wait();
            }
            catch (Exception ex)
            {
                // handle exception
                var x = 12;
            }

            dataStory.Chaps.Clear();
            dataStory.Chaps.AddRange(dataThread1.Chaps);
            dataStory.Chaps.AddRange(dataThread2.Chaps);
            dataStory.Chaps.AddRange(dataThread3.Chaps);
            dataStory.Chaps.AddRange(dataThread4.Chaps);
            dataStory.Chaps.AddRange(dataThread5.Chaps);

            dataStory.Picture = MakeStoryPictureLinkForNewStory(dataStory.Picture) ?? dataStory.Picture;
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
