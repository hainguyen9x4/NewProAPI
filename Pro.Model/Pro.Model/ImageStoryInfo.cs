namespace Pro.Model
{
    public class ImageStoryInfo
    {
        public int StoryID { get; set; }
        public string StoryName { get; set; }
        public string StoryNameShow { get; set; }
        public string StoryLink { get; set; }
        public string StoryPictureLink { get; set; }
        public List<ChapInfo> Chaps { get; set; }
    }
    public class HomePageInfo
    {
        public HomePageInfo()
        {
            ImageStoryInfos = new List<ImageStoryInfo>();
        }
        public List<ImageStoryInfo> ImageStoryInfos { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
    }
    public class ShortStoryInfo
    {
        public string ChapLink { get; set; }
        public int ChapId { get; set; }
        public string ChapName { get; set; }
    }
    public class ChapInfo
    {
        public int ChapID { get; set; }
        public string ChapName { get; set; }
        public string ChapLink { get; set; }
        public string StoryName { get; set; }
        public string StoryNameShow { get; set; }
        public DateTime? LastModifyDatetime { get; set; }
        public List<ImagesChap> ImageStoryLinks { get; set; }
        public string Updated { get; set; }
        public List<ShortStoryInfo> StoryShortInfos { get; set; }
    }
}
