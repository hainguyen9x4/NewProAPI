using Newtonsoft.Json;
using Pro.Common;

namespace FileManager
{
    public static class FileReader
    {
        public static void WriteDataToFile<T>(string filePath, List<T> datas, int delay = 100, int redoTime = 5)
        {
            if (!File.Exists(filePath))
            {
                var folder = new FileInfo(filePath).Directory.FullName;
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                File.Create(filePath).Close();
            }

            for (int i = 0; i < redoTime; i++)
            {
                if (!IsFileLocked(filePath))
                {
                    var results = JsonConvert.SerializeObject(datas);
                    try
                    {
                        File.WriteAllText(filePath, results);
                        break;
                    }
                    catch { }
                }
                else
                {
                    Thread.Sleep(delay);
                }
            }
        }
        public static List<T> ReadListDataFromFile<T>(string filePath)
        {
            var objData = new List<T>();
            if (!File.Exists(filePath))
            {
                return objData;
            }

            if (File.Exists(filePath))
            {
                try
                {
                    using (StreamReader r = new StreamReader(filePath))
                    {
                        try
                        {
                            objData = JsonConvert.DeserializeObject<List<T>>(r.ReadToEnd());
                        }
                        catch
                        {
                            return new List<T>();
                        }
                        finally
                        {
                            r.Close();
                        }
                    }
                }
                catch
                {
                    return new List<T>();
                }
            }
            return objData ?? new List<T>();
        }
        public static bool IsFileLocked(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            //file is not locked
            return false;
        }
        public static bool DeleteFile(string filePath)
        {
            var rs = true;
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    rs = false;
                }
            }
            return rs;
        }
        public static bool GetChapInfo(string urlChap, out string nameTruyen, out string chapNumber)
        {
            nameTruyen = "";
            chapNumber = "";
            //https://www.nettruyenin.com/truyen-tranh/azusa-seo-kouji/chap-1/137350
            try
            {

                var pointGetIndex1 = "/truyen-tranh/";
                var pointGetIndex2 = "/chap-";
                var pointGetIndex3 = "/";

                var start = urlChap.LastIndexOf(pointGetIndex1) + pointGetIndex1.Length;
                var end = urlChap.LastIndexOf(pointGetIndex2);
                nameTruyen = urlChap.Substring(start, end - start);//azusa-seo-kouji

                //Get chapNumber
                start = end + 1;
                end = urlChap.LastIndexOf(pointGetIndex3);
                chapNumber = urlChap.Substring(start, end - start);//1
                chapNumber = chapNumber.ToUpper();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetChapInfo: {urlChap}" + ex);
                return false;
            }
        }
        public static string GetStoryInfoFromUrlStory(string urlChap)
        {
            string nameTruyen = "";
            //https://www.nettruyentv.com/truyen-tranh/vo-luyen-dinh-phong-176960
            try
            {
                var pointGetIndex1 = "/truyen-tranh/";
                var pointGetIndex2 = "-";

                var start = urlChap.LastIndexOf(pointGetIndex1) + pointGetIndex1.Length;
                var end = urlChap.LastIndexOf(pointGetIndex2);
                nameTruyen = urlChap.Substring(start, urlChap.Length - start - (urlChap.Length - end));//vo-luyen-dinh-phong
                return nameTruyen;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetChapInfo: {urlChap}" + ex);
                return nameTruyen;
            }
        }
        public static List<string> AddHomeUrlLink(List<string> urls, string homePage)
        {
            //https://www.nettruyenme.com/truyen-tranh/  +  vo-luyen-dinh-phong-176960
            var newUrls = new List<string>();
            foreach (string url in urls)
            {
                if (!string.IsNullOrEmpty(url))
                    newUrls.Add(homePage + url);
            }
            return newUrls;
        }
        public static string DeleteHomePage(string urlx)
        {
            try
            {
                var last = "/truyen-tranh/";
                var start = urlx.LastIndexOf(last);
                var num = urlx.Length - start - last.Length;
                return urlx.Substring(start + last.Length, num);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error_UpdateDeleteHomePage :{urlx}" + ex);
            }
            return urlx;
        }
        public static List<string> DeleteHomePageList(List<string> urlxs)
        {
            var urls = new List<string>();
            foreach (var urlx in urlxs)
                urls.Add(DeleteHomePage(urlx));
            return urls;
        }
        public static List<string> AddStoryNameToUrlLink(List<string> chapLinks, string storyName)
        {
            var tem = new List<string>();
            foreach (string url in chapLinks)
            {
                tem.Add(storyName + url);
            }
            return tem;
        }
    }
}