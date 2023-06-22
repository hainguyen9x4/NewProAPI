using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Pro.Common;
using Pro.Common.Const;
using Pro.Data.Repositorys;
using Pro.Model;
using Pro.Service.Caching;
using System.Text.RegularExpressions;

namespace Pro.Service.Implements
{
    public class ApplicationSettingService : IApplicationSettingService
    {
        private readonly IApplicationSettingRepository _appSettingRepository;
        private readonly ICacheProvider _cacheProvider;
        public ApplicationSettingService(
            IApplicationSettingRepository appSettingRepository
            , ICacheProvider cacheProvider)
        {
            _appSettingRepository = appSettingRepository;
            _cacheProvider = cacheProvider;
        }

        public bool GetBoolValue(string settingKey, bool defaultValue = false)
        {
            var stringValue = GetValue(settingKey);
            return bool.TryParse(stringValue, out bool returnValue) ? returnValue : defaultValue;
        }

        public int GetIntValue(string settingKey, int defaultValue = 0, bool useCache = true)
        {
            var stringValue = GetValue(settingKey, null, useCache);
            return int.TryParse(stringValue, out int returnValue) ? returnValue : defaultValue;
        }

        public IEnumerable<int> GetListIntValue(string settingKey, bool useCache = true)
        {
            try
            {
                var stringValue = "";
                if (useCache)
                {
                    stringValue = GetValue(settingKey);
                }
                else
                {
                    stringValue = GetValueNoCache(settingKey);
                }
                return GetIntList(stringValue);
            }
            catch (Exception ex)
            {
                //ex.WriteLog($"{nameof(GetListIntValue)}, key: {settingKey}");
                return Enumerable.Empty<int>();
            }
        }
        private static List<int> GetIntList(string text)
        {
            List<int> list = new List<int>();
            if (string.IsNullOrEmpty(text))
                return list;

            var matches = new Regex(@"[-\d]+").Matches(text);
            foreach (var match in matches)
            {
                if (int.TryParse(match.ToString(), out var matchNumber))
                {
                    list.Add(matchNumber);
                }
            }
            return list;
        }
        public string GetValueNoCache(string settingKey, string defaultValue = null)
        {
            try
            {
                return _appSettingRepository.GetAll().Where(i => i.AppSettingName == settingKey).Select(i => i.AppSettingValue).FirstOrDefault();
            }
            catch (Exception ex)
            {

                LogHelper.Error($"Error when get the application no cache setting with key: {settingKey}", ex);
                return defaultValue;
            }
        }
        public IEnumerable<string> GetListString(string settingKey)
        {
            var setting = GetValue(settingKey);
            var values = new List<string>();

            if (!string.IsNullOrWhiteSpace(setting))
            {
                values.AddRange(Regex.Replace(setting, @"\s*[\,\;\?\'\|\$\&\/]\s*|\s+", " ").Split(' '));
            }

            return values;
        }

        public string GetValue(string settingKey, string defaultValue = null, bool useCache = true)
        {
            try
            {
                Func<string> fetchFunc = () =>
                {
                    return _appSettingRepository.GetAll().Where(i => i.AppSettingName == settingKey).Select(i => i.AppSettingValue).FirstOrDefault();
                };

                return useCache ? _cacheProvider.Get(CacheKeys.GetCacheKey(CacheKeys.ApplicationSetting.ByKey, settingKey), fetchFunc) : fetchFunc();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get the application setting with key: {settingKey}", ex);
                return defaultValue;
            }
        }
        public string GetValueGetScan(string settingKey, string defaultValue = null, bool useCache = true, int useOtherSetting = 0)
        {
            if (!string.IsNullOrWhiteSpace(settingKey) && useOtherSetting > 0)
            {
                if (settingKey == ApplicationSettingKey.AppsettingsScanGet)
                    settingKey += "_other_" + useOtherSetting.ToString();
                if (settingKey == ApplicationSettingKey.Log4netFolder)
                    settingKey += "Other" + useOtherSetting.ToString();
            }
            try
            {
                Func<string> fetchFunc = () =>
                {
                    return _appSettingRepository.GetAll().Where(i => i.AppSettingName == settingKey).Select(i => i.AppSettingValue).FirstOrDefault();
                };

                return useCache ? _cacheProvider.Get(CacheKeys.GetCacheKey(CacheKeys.ApplicationSetting.ByKey, settingKey), fetchFunc) : fetchFunc();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get the application setting with key: {settingKey}", ex);
                return defaultValue;
            }
        }
        public T GetValueObject<T>(string settingKey, T defaultValue = default(T), bool useCache = true)
        {
            try
            {
                Func<string> fetchFunc = () =>
                {
                    return _appSettingRepository.GetAll().Where(i => i.AppSettingName == settingKey).Select(i => i.AppSettingValue).FirstOrDefault();
                };

                var obj = useCache ? _cacheProvider.Get(CacheKeys.GetCacheKey(CacheKeys.ApplicationSetting.ByKey, settingKey), fetchFunc) : fetchFunc();
                return JsonConvert.DeserializeObject<T>(obj);
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get the application setting with key: {settingKey}", ex);
                return defaultValue;
            }
        }

        public void RemoveCacheValue(string[] settingKeys)
        {
            foreach (var settingKey in settingKeys)
            {
                _cacheProvider.Remove(CacheKeys.GetCacheKey(CacheKeys.ApplicationSetting.ByKey, settingKey));
            }
        }

        public bool SetValue(string settingKey, string value)
        {
            var item = _appSettingRepository.GetAll().FirstOrDefault(i => i.AppSettingName == settingKey);
            if (item != null)
            {
                item.AppSettingValue = value;
                _appSettingRepository.Update(item.AppSettingId, item);
                return true;
            }
            return false;
        }
        public bool AddKeyValue(string settingKey, string value, bool isActive)
        {
            var item = _appSettingRepository.GetAll().FirstOrDefault(i => i.AppSettingName == settingKey);
            if (item == null)
            {
                try
                {
                    _appSettingRepository.Create(new Model.ApplicationSetting
                    {
                        AppSettingName = settingKey,
                        AppSettingValue = value,
                        AppSettingIsActive = isActive,
                    });
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        public bool IsActiveAppSettingByKey(string settingKey)
        {
            //return _appSettingRepository.GetAll().Where(i => i.AppSettingName == settingKey).Select(i => i.AppSettingIsActive).FirstOrDefault();
            var rs = GetActiveValue(settingKey);
            if (GetActiveValue(settingKey) == "1")
            {
                return true;
            }
            return false;
        }
        public string GetActiveValue(string settingKey, string defaultValue = "0", bool useCache = true)
        {
            try
            {
                Func<string> fetchFunc = () =>
                {
                    if (_appSettingRepository.GetAll().Where(i => i.AppSettingName == settingKey).Select(i => i.AppSettingIsActive).FirstOrDefault())
                    {
                        return "1";
                    }
                    else
                    {
                        return defaultValue;
                    }
                };

                return useCache ? _cacheProvider.Get(CacheKeys.GetCacheKey(CacheKeys.ApplicationSetting.ByKey, settingKey + "_IsAvtive"), fetchFunc) : fetchFunc();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get the application setting with key: {settingKey + "_IsAvtive"}", ex);
                return defaultValue;
            }
        }

        public List<string> GetAllCloudarySettings(string settingKey, bool useCache = true)
        {
            try
            {
                Func<List<string>> fetchFunc = () =>
                {
                    var keys = _appSettingRepository.GetAll().Where(k => k.AppSettingIsActive
                    && k.AppSettingName.Contains(settingKey) && k.NumberImage < GetIntValue(ApplicationSettingKey.MaxImageClound, 6000, true)).Take(1).ToArray();
                    var settings = new List<string>();
                    foreach (var key in keys)
                    {
                        settings.Add(key.AppSettingValue);
                    }
                    return settings;
                };

                return useCache ? _cacheProvider.Get(CacheKeys.GetCacheKey(CacheKeys.ApplicationSetting.ByKey, settingKey), fetchFunc) : fetchFunc();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get the application setting with key: {settingKey}", ex);
                return new List<string>();
            }
        }
        public void RemoveAllCacheValue()
        {
            var settingKeys = _appSettingRepository.GetAll().Select(a => a.AppSettingName).ToList();
            foreach (var settingKey in settingKeys)
            {
                _cacheProvider.Remove(CacheKeys.GetCacheKey(CacheKeys.ApplicationSetting.ByKey, settingKey));
            }
        }

        public List<ApplicationSetting> Get() => _appSettingRepository.GetAll().ToList();

        public ApplicationSetting Get(int id) => _appSettingRepository.GetById(id);

        public class ApplicationSettingTemp
        {
            public int NumberAvailable { get; set; }
            public List<ApplicationSetting> ListAvailables { get; set; }
        }
        public ApplicationSettingTemp GetAvailableClound(int number)
        {
            var lst = _appSettingRepository.GetAll().Where(s => s.AppSettingName.Contains("CloundSetting") && s.NumberImage <= number).ToList();
            return new ApplicationSettingTemp()
            {
                ListAvailables = lst,
                NumberAvailable = lst.Count,
            };
        }

        public ApplicationSetting Create(ApplicationSetting appSetting)
        {
            return _appSettingRepository.Create(appSetting);
        }
        public ApplicationSetting CreateCloundinary(string dataCreateCloundinary, string email, string c)
        {
            var appSetting = new ApplicationSetting();
            var clound = GetData(dataCreateCloundinary, c);
            if (clound.Any() && clound.Count() >= 3)
            {
                appSetting.AppSettingName = "CloundSetting";
                appSetting.AppSettingValue = $"{{\"CloudName\":\"{clound[0]}\",\"ApiKey\":\"{clound[1]}\",\"ApiSecret\":\"{clound[2]}\"}}";
                appSetting.AppSettingIsActive = true;
                appSetting.Descriptions = email;
            }
            //CheckValid before create:
            if (_appSettingRepository.GetAll().Where(s => s.AppSettingIsActive == true && s.AppSettingName == "CloundSetting"
            && s.AppSettingValue.Contains(clound[0])).FirstOrDefault() == null)
            {
                return _appSettingRepository.Create(appSetting);
            }
            return new ApplicationSetting();
        }
        private List<string> GetData(string dataCreateCloundinary, string c)
        {
            Regex regex = new Regex("\"([^\"]*)\"");
            if (c == "'") regex = new Regex("'([^']*)'");
            MatchCollection matches = regex.Matches(dataCreateCloundinary);
            var rs = new List<string>();
            foreach (Match match in matches)
            {
                rs.Add(match.Groups[1].Value);
            }
            return rs;
        }
        public void Update(int id, ApplicationSetting updatedAppSetting) => _appSettingRepository.Update(id, updatedAppSetting);

        public void Delete(ApplicationSetting appSettingForDeletion) => _appSettingRepository.Delete(appSettingForDeletion);

        public void Delete(int id) => _appSettingRepository.Delete(id);
        
        public class TempCloudinaryData
        {
            public int NumberCloudinary { get; set; }
            public int NumberImage { get; set; }
        }
        public TempCloudinaryData CaculateNumberCloudinaryUsed()
        {
            var rs = new TempCloudinaryData();
            var datas = _appSettingRepository.GetAll().Where(s => s.AppSettingName == "CloundSetting" && s.NumberImage > 0).ToList();
            if (datas.Any())
            {
                rs.NumberCloudinary = datas.Count();
                datas.ForEach(data => rs.NumberImage += data.NumberImage);
            }
            return rs;
        }
    }
}