using log4net;
using Pro.Common;
using Pro.Model;
using System.Text.RegularExpressions;
using System.Text;
using HtmlAgilityPack;

namespace Pro.Service.WebData.Implements
{
    public class GetDataFromWebService : IGetDataFromWebService
    {
        public Model.StoryInfoWithChaps GetStoryInfoWithChaps(string textUrl)
        {
            var result = new Model.StoryInfoWithChaps();
            var chapLists = new List<ChapPlus>();
            var listItems = new List<TempItem>();
            var storyNameShow = "";
            var des = "";
            var types = new List<string>();
            for (int timeRetry = 1; timeRetry <= 2; timeRetry++)
            {
                try
                {
                    HtmlWeb htmlWeb = new HtmlWeb()
                    {
                        AutoDetectEncoding = false,
                        OverrideEncoding = Encoding.UTF8
                    };
                    HttpClient myWebClient = new HttpClient();
                    HtmlDocument document = htmlWeb.Load(textUrl);

                    listItems = document.DocumentNode.Descendants("div").Where(node => node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains("col-xs-5 chapter"))
                        .Select(t => t.ChildNodes).SelectMany(t => t.Select(a => new TempItem()
                        {
                            text = a.GetDirectInnerText(),
                            links = a.Attributes.AttributesWithName("href").ToList(),
                        })).Where(t => t.text.Contains("Chapter")).ToList();

                    //var listItems2 = document.DocumentNode.Descendants("div").ToList();
                    storyNameShow = GetStoryNameShow(document);
                    des = GetStoryDes(document);
                    types = GetStoryTypeInfo(document);
                    break;
                }
                catch (Exception ex)
                {
                    if (timeRetry == 1)
                    {
                        listItems = new List<TempItem>();
                        System.Threading.Thread.Sleep(20 * 1000);
                        continue;
                    }
                    //log.LogError($"Error_GetAllChaps :{textUrl}" + ex);
                    new List<ChapPlus>();
                }
            }
            var items = new StorySaveInfo();
            var index = 0;

            listItems.Reverse();
            foreach (var item in listItems)
            {
                index++;
                chapLists.Add(new ChapPlus()
                {
                    ChapLink = item.links.Select(t => t.Value).First(),
                    ChapIndexNumber = index,
                    ChapName = item.text,
                });
            }
            result.StoryName = storyNameShow;
            result.Description = des;
            result.StoryTypes = types;
            result.ChapPluss = chapLists;
            return result;

            string GetStoryNameShow(HtmlDocument document)
            {
                var htmlName = document.DocumentNode.Descendants("h1").Where(node => node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains("title-detail"))
                    .FirstOrDefault();
                if (htmlName == null) return "";
                var namess = htmlName.InnerText;
                return namess;
            }

            List<string> GetStoryTypeInfo(HtmlDocument document)
            {
                var rs = new List<string>();
                var htmlName = document.DocumentNode.Descendants("div").Where(node => node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains("detail-info"))
                    .FirstOrDefault();
                if (htmlName != null)
                {
                    var dataHtml = htmlName.Descendants("li").Where(node => node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains("kind")).FirstOrDefault();
                    if (dataHtml != null)
                    {
                        var typeHtml = dataHtml.Descendants("a").ToList();
                        if (typeHtml.Any())
                        {
                            foreach (var item in typeHtml)
                            {
                                var type = GetNameFromLink(item.Attributes["href"].Value);
                                if (!String.IsNullOrEmpty(type))
                                {
                                    rs.Add(type);
                                }
                            }
                        }
                    }
                }
                return rs;
            }
            string GetNameFromLink(string url)
            {
                if (String.IsNullOrEmpty(url)) return "";
                string pattern = @"\/([^\/]+)-\d+";
                Match match = Regex.Match(url, pattern);

                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                return "";
            }
            string GetStoryDes(HtmlDocument document)
            {
                var htmlName = document.DocumentNode.Descendants("div").Where(node => node.Attributes.Contains("class") && node.Attributes["class"].Value.Contains("detail-content")).FirstOrDefault();
                if (htmlName == null) return "";
                if (htmlName != null && htmlName.HasChildNodes)
                {
                    return htmlName.ChildNodes.Where(c => c.Name == "p").FirstOrDefault()?.InnerText ?? "";
                }
                return "";
            }

        }
    }
    public class TempItem
    {
        public string text { get; set; }
        public List<HtmlAttribute> links { get; set; }
        public string StoryNameShow { get; set; }
    }
}