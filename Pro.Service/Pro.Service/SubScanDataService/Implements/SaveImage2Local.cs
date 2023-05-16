using Pro.Common;
using Pro.Common.Const;
using Pro.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System.Net;
using Image = SixLabors.ImageSharp.Image;

namespace Pro.Service.SubScanDataService.Implements
{
    public class SaveImage2Local : ISaveImage2Local
    {
        private readonly IApplicationSettingService _applicationSettingService;
        private readonly AppBuildDataSetting _setting;

        public SaveImage2Local(IApplicationSettingService applicationSettingService)
        {
            _applicationSettingService = applicationSettingService;
            var settings = _applicationSettingService.GetValueGetScan(ApplicationSettingKey.AppsettingsScanGet);
            _setting = JsonManager.StringJson2Object<AppBuildDataSetting>(settings);
        }

        public void SaveImage2LocalFunc(NewStory dataStory)
        {
            var listDatas = DividingObject(dataStory, 5);
            List<Task<NewStory>> tasks = new List<Task<NewStory>>();
            foreach (var item in listDatas)
            {
                tasks.Add(Task.Run(async () => await SaveDataAsyncForNew(item)));
            }
            var t = Task.WhenAll(tasks);
            t.Wait();

            dataStory.Chaps.Clear();
            foreach (var rs in t.Result)
            {
                dataStory.Chaps.AddRange(rs.Chaps);
            }
        }
        private async Task<NewStory> SaveDataAsyncForNew(NewStory dataStory)
        {
            foreach (var data in dataStory.Chaps)
            {
                var imageName = 0;
                foreach (var img in data.Images)
                {
                    imageName++;
                    var streamFile = GetStreamImage(img.OriginLink);
                    if (streamFile != null)
                    {
                        var savePath = $@"\TT\{dataStory.Name}\{data.Name}\";
#if !DEBUG
                        var local = SaveToLocal(streamFile, savePath, $"_{imageName.ToString().PadLeft(4, '0')}", disk:_applicationSettingService.GetValue(ApplicationSettingKey.DiskSaveImageLocal));
#elif DEBUG
                        var local = SaveToLocal(streamFile, savePath, $"_{imageName.ToString().PadLeft(4, '0')}");
#endif
                        img.LocalLink = local;
                    }
                    else
                    {
                        LogHelper.Error($"SaveDataAsyncForNew-streamFile is NULL:{img.OriginLink}");
                    }
                }

            }
            return dataStory;
        }
        private string SaveToLocal(Stream streamFile, string path, string imageName, string disk = @"D:\xStory")
        {
            var subFolderPath = disk + path;

            if (!Directory.Exists(subFolderPath))
            {
                Directory.CreateDirectory(subFolderPath);
            }
            var subPath = path + imageName;
            var fullPathLocal = disk + subPath + ".jpg"; 
            var acc = new WaitForInternetAccess();
            acc.WaitInternetAccess("SaveToLocal");
            using (var image = Image.Load(streamFile))
            {
                image.Save(fullPathLocal);
            }
            return fullPathLocal;
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

        private List<NewStory> DividingObject(NewStory dataStory, int numberObject)
        {
            var rs = new List<NewStory>();

            var subChapLists = Chunk(dataStory.Chaps, 5).ToList();
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
    }
}
