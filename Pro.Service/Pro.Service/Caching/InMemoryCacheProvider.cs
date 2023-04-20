using System;
using System.Runtime.Caching;

namespace Pro.Service.Caching
{
    public class InMemoryCacheProvider : ICacheProvider
    {
        private readonly ObjectCache Cache = MemoryCache.Default;
        private const int AbsoluteExpirationDefault = 500;

        public void Set<T>(string key, T data) where T : class
        {
            Set(key, data, AbsoluteExpirationDefault);
        }

        public void Set<T>(string key, T data, int expiredTimeInSeconds) where T : class
        {
            try
            {
                if (data == null)
                {
                    Cache.Set(key, new int[0], DateTimeOffset.UtcNow.AddSeconds(expiredTimeInSeconds));
                }
                else
                {
                    Cache.Set(key, data, DateTimeOffset.UtcNow.AddSeconds(expiredTimeInSeconds));
                }
            }
            catch (Exception ex)
            {
                //ex.WriteLog($"InMemoryCacheProvider Set {key}");
            }
        }

        public T Get<T>(string key, bool useHighLevelCache = true) where T : class
        {
            try
            {
                return Cache.Get(key) as T;
            }
            catch (Exception ex)
            {
               //ex.WriteLog($"InMemoryCacheProvider Get {key}");
                return default(T);
            }
        }

        public T Get<T>(string key, Func<T> callback) where T : class
        {
            return Get(key, callback, AbsoluteExpirationDefault);
        }

        public T Get<T>(string key, Func<T> callback, int expiredTimeInSeconds) where T : class
        {
            if (CheckExist(key))
            {
                var item = Get<T>(key);
                return item;
            }
            else
            {
                var item = callback();
                Set(key, item, expiredTimeInSeconds);
                return item;
            }
        }

        public void Remove(string key)
        {
            try
            {
                Cache.Remove(key);
            }
            catch (Exception ex)
            {
                //ex.WriteLog($"InMemoryCacheProvider Remove {key}");
            }
        }

        public bool CheckExist(string key)
        {
            try
            {
                return Cache.Contains(key);
            }
            catch (Exception ex)
            {
                //ex.WriteLog($"InMemoryCacheProvider CheckExist {key}");
                return false;
            }
        }
    }
}
