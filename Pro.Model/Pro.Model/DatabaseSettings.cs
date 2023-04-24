namespace Pro.Model
{
    public class AppSettingData : IAppSettingData
    {
        public string XStorysCollectionStory { get; set; }
        public string XStorysCollectionNewStory { get; set; }
        public string XStorysCollectionChap { get; set; }
        public string XStorysCollectionAppSetting { get; set; }
        public string ConnectionStringAppSetting { get; set; }
        public string ConnectionStringMain { get; set; }
        public string DatabaseName { get; set; }
        public int UseSettingGetSetNumber { get; set; }
    }

    public interface IAppSettingData
    {
        string XStorysCollectionStory { get; set; }
        string XStorysCollectionNewStory { get; set; }
        string XStorysCollectionChap { get; set; }
        string XStorysCollectionAppSetting { get; set; }
        string ConnectionStringAppSetting { get; set; }
        string ConnectionStringMain { get; set; }
        string DatabaseName { get; set; }
        public int UseSettingGetSetNumber { get; set; }
    }
}