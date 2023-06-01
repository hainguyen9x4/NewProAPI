namespace Pro.Service.Caching
{
    public static partial class CacheKeys
    {
        public static string GetCacheKey(string keyFormat, params object[] compositionKeys)
        {
            return string.Format(keyFormat, compositionKeys);
        }
    }

    public static partial class CacheKeys
    {
        public static class ApplicationSetting
        {
            public const string ByKey = "ApplicationSetting:ByKey:{0}";
            //public const string ListCustomKey = "ApplicationSetting:ListCustomKey";
        }
        public static class ImageStoryData
        {
            public const string StoryIDChapID = "ImageStoryData:StoryIDChapID:{0}{1}";
            public const string StoryID = "ImageStoryData:StoryID:{0}";
            public const string ListAllStory = "ListAllStory";
            public const string ListAllStoryOfPage = "ListAllStoryOfPage{0}{1}";
            public const string HomePage = "HomePage:{0}{1}";
            public const string HomePageFornew = "HomePageFornew:{0}{1}";
            public const string TopHotStory = "TopHostStory";
            public const string SearchStoryData = "SearchStoryData";
            public const string ListFollowStorys = "LostFollowStorys{0}";
        }
    }
}
