namespace Pro.Model
{
    public class AppSettingData : IAppSettingData
    {
        public string XStorysCollectionStory { get; set; }
        public string XStorysCollectionNewStory { get; set; }
        public string XStorysCollectionChap { get; set; }
        public string XStorysCollectionImage { get; set; }
        public string XStorysCollectionUser { get; set; }
        public string XStorysCollectionComment { get; set; }
        public string XStorysCollectionStoryFollows { get; set; }
        public string XStorysCollectionAppSetting { get; set; }
        public string XStorysCollectionResultScanData { get; set; }
        public string ConnectionStringAppSetting { get; set; }
        public string ConnectionStringMain { get; set; }
        public string DatabaseName { get; set; }
        public int UseSettingGetSetNumber { get; set; }
        public string XStorysCollectionStoryType { get; set; }
        public string XStorysCollectionFileStory { get; set; }
    }

    public interface IAppSettingData
    {
        string XStorysCollectionStory { get; set; }
        string XStorysCollectionNewStory { get; set; }
        string XStorysCollectionChap { get; set; }
        string XStorysCollectionImage { get; set; }
        string XStorysCollectionUser { get; set; }
        string XStorysCollectionComment { get; set; }
        string XStorysCollectionStoryFollows { get; set; }
        string XStorysCollectionFileStory { get; set; }
        string XStorysCollectionAppSetting { get; set; }
        string XStorysCollectionResultScanData { get; set; }
        string ConnectionStringAppSetting { get; set; }
        string ConnectionStringMain { get; set; }
        string DatabaseName { get; set; }
        public int UseSettingGetSetNumber { get; set; }
        string XStorysCollectionStoryType { get; set; }
    }
}