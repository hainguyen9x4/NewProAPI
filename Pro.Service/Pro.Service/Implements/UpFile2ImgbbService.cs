using FileManager;
using Newtonsoft.Json;
using Pro.Common;
using Pro.Common.Const;
using Pro.Model;
using System.Net;

namespace Pro.Service.Implement
{
    public class UpFile2ImgbbService : IUploadImageService
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly string ImgbbAPI;
        public UpFile2ImgbbService(IApplicationSettingService applicationSettingService)
        {
            _applicationSettingService = applicationSettingService;
            ImgbbAPI = _applicationSettingService.GetValue(ApplicationSettingKey.ImgbbAPI);
        }

        private async Task<NewStory> UpLoadDataAsyncForNew(NewStory dataStory)
        {
            var totalImages = 0;
            foreach (var chapSave in dataStory.Chaps)
            {
                var savePath = $"/Truyen-tranh2/{dataStory.Name}/{chapSave.Name}/";//Folder save on clound
                foreach (var link in chapSave.Images)
                {
                    var linkRs = PostToImgBB(link.LocalLink, ImgbbAPI);

                    if (!String.IsNullOrEmpty(linkRs))//Success
                    {
                        link.Link = linkRs;
                        link.OriginLink = "";
                        link.LocalLink = link.LocalLink;
                    }
                    else
                    {
                        LogHelper.Error($"Error UpLoadDataAsyn- cannot cloud link:{link.OriginLink};{dataStory.Name}/{chapSave.Name}: Result empty");
                    }
                }
                totalImages += chapSave.Images.Count();
                chapSave.Link = FileReader.DeleteHomePage(chapSave.Link);
            }
            return dataStory;
        }

        private string? MakeStoryPictureLinkForNewStory(string storyPictureLink)
        {
            return PostToImgBbByLink(storyPictureLink, ImgbbAPI);
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
        private string PostToImgBB(string imagFilePath, string apiKey)
        {
            try {
            if (!String.IsNullOrEmpty(imagFilePath)) return "";
            byte[] imageData;

            FileStream fileStream = File.OpenRead(imagFilePath);
            imageData = new byte[fileStream.Length];
            fileStream.Read(imageData, 0, imageData.Length);
            fileStream.Close();

            string uploadRequestString = "image=" + Uri.EscapeDataString(System.Convert.ToBase64String(imageData)) + "&key=" + apiKey;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://api.imgbb.com/1/upload");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ServicePoint.Expect100Continue = false;

            StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream());
            streamWriter.Write(uploadRequestString);
            streamWriter.Close();

            WebResponse response = webRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);

            string responseString = responseReader.ReadToEnd();
            var rs = JsonConvert.DeserializeObject<DataRes>(responseString);
            //LogHelper.Error(responseString);
            if (rs != null && rs.data != null)
            {
                return rs.data.display_url;
            }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"UploadImage-PostToImgBB2-Error,imgLink:{imagFilePath}" + ex);
            }
            return "";
        }
        private string PostToImgBB2(string imgLink, string apiKey)
        {
            try
            {
                if (!String.IsNullOrEmpty(imgLink)) return "";
                byte[] imageData;

                var fileStream = GetStreamImage(imgLink);
                imageData = new byte[fileStream.Length];
                fileStream.Read(imageData, 0, imageData.Length);
                fileStream.Close();

                string uploadRequestString = "image=" + Uri.EscapeDataString(System.Convert.ToBase64String(imageData)) + "&key=" + apiKey;

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://api.imgbb.com/1/upload");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ServicePoint.Expect100Continue = false;

                StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream());
                streamWriter.Write(uploadRequestString);
                streamWriter.Close();

                WebResponse response = webRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);

                string responseString = responseReader.ReadToEnd();
                var rs = JsonConvert.DeserializeObject<DataRes>(responseString);
                //LogHelper.Error(responseString);
                if (rs != null && rs.data != null)
                {
                    return rs.data.display_url;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"UploadImage-PostToImgBB2-Error,imgLink:{imgLink}" + ex);
            }
            return "";
        }
        private string PostToImgBbByLink(string imgLink, string apiKey)
        {
            var fileStream = GetStreamImage(imgLink);
            byte[] imageData = new byte[fileStream.Length];
            fileStream.Read(imageData, 0, imageData.Length);
            fileStream.Close();


            string uploadRequestString = "image=" + Uri.EscapeDataString(System.Convert.ToBase64String(imageData)) + "&key=" + apiKey;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://api.imgbb.com/1/upload");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ServicePoint.Expect100Continue = false;

            StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream());
            streamWriter.Write(uploadRequestString);
            streamWriter.Close();

            WebResponse response = webRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);

            string responseString = responseReader.ReadToEnd();
            //LogHelper.Error(responseString);
            var rs = JsonConvert.DeserializeObject<DataRes>(responseString);
            if (rs != null && rs.data != null)
            {
                return rs.data.display_url;
            }
            return imgLink;
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
                        acc.WaitInternetAccess("GetStreamImage-imgBB");
                        continue;
                    }
                }
            }
            return streamFile;
        }
    }
    public class DataRes
    {
        public Data data { get; set; }
    }
    public class Data
    {
        public string id { get; set; }
        public string title { get; set; }
        public string url_viewer { get; set; }
        public string url { get; set; }
        public string display_url { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string size { get; set; }
        public string time { get; set; }
        public string expiration { get; set; }
        public Image image { get; set; }
        public Thumb thumb { get; set; }
        public Medium medium { get; set; }
        public string delete_url { get; set; }
    }

    public class Image
    {
        public string filename { get; set; }
        public string name { get; set; }
        public string mime { get; set; }
        public string extension { get; set; }
        public string url { get; set; }
    }

    public class Medium
    {
        public string filename { get; set; }
        public string name { get; set; }
        public string mime { get; set; }
        public string extension { get; set; }
        public string url { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
    }

    public class Thumb
    {
        public string filename { get; set; }
        public string name { get; set; }
        public string mime { get; set; }
        public string extension { get; set; }
        public string url { get; set; }
    }
}
