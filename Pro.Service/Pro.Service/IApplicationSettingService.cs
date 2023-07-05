using Pro.Model;
using static Pro.Service.Implements.ApplicationSettingService;

namespace Pro.Service
{
    public interface IApplicationSettingService
    {

        void RemoveCacheValue(string[] settingKeys);
        void RemoveAllCacheValue();
        string GetValue(string settingKey, string defaultValue = null, bool useCache = true);
        string GetValueGetScan(string settingKey, string defaultValue = null, bool useCache = true, int useOtherSetting = 0);
        T GetValueObject<T>(string settingKey, T defaultValue = default(T), bool useCache = true);
        bool SetValue(string settingKey, string value);
        IEnumerable<string> GetListString(string settingKey);
        int GetIntValue(string settingKey, int defaultValue = 0, bool useCache = true);
        IEnumerable<int> GetListIntValue(string settingKey, bool useCache = true);
        bool GetBoolValue(string settingKey, bool defaultValue = false);
        bool AddKeyValue(string settingKey, string value, bool isActive);
        bool IsActiveAppSettingByKey(string settingKey);
        List<string> GetAllCloudarySettings(string settingKey,int numberTake = 1, bool useCache = true);

        List<ApplicationSetting> Get();
        ApplicationSetting Get(int id);
        ApplicationSettingTemp GetAvailableClound(int number);
        ApplicationSetting Create(ApplicationSetting appSetting);
        ApplicationSetting CreateCloundinary(string dataCreateCloundinary, string email, string c);
        void Delete(int id);
        void Delete(ApplicationSetting appSettingForDeletion);
        void Update(int id, ApplicationSetting updatedAppSetting);
        public TempCloudinaryData CaculateNumberCloudinaryUsed();
    }
}
