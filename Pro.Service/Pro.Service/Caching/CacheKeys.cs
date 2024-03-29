﻿namespace Pro.Service.Caching
{
    public static class CacheKeys
    {
        public static string GetCacheKey(string keyFormat, params object[] compositionKeys)
        {
            return string.Format(keyFormat, compositionKeys);
        }
        public static class ApplicationSetting
        {
            public const string ByKey = "ApplicationSetting:ByKey:{0}";
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
            public const string ListStoryTypes = "ListStoryTypes";
            public const string AllStoryType = "AllStoryType";
            public const string ListAllStoryOfPageStoryType = "ListAllStoryOfPageStoryType{0}{1}{2}";
            public const string ListAllStoryOfRateType = "ListAllStoryOfRateType{0}{1}{2}";
            public const string ListStoryGenderType = "ListStoryGenderType{0}{1}{2}";
            public const string CountAllStory = "CountAllStory";
            public const string ListAllStorys = "ListAllStorys";
        }
        public static class ScanGetData
        {
            public const string ListStoryIDForCheckInvalid = "ListStoryIDForCheckInvalid";
            public const string ListStoryIDChecked = "ListStoryIDChecked";
            public const string ListChapIDChecked = "ListChapIDChecked{0}";
            public const string ListStoryFollows = "ListStoryFollows";
        }
    }
}
