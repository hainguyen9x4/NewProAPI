using System;

namespace Pro.Service.Caching
{
    public interface ICacheProvider
    {
        void Set<T>(string key, T data) where T : class;
        void Set<T>(string key, T data, int expiredTimeInSeconds) where T : class;

        T Get<T>(string key, bool useHighLevelCache = true) where T : class;

        T Get<T>(string key, Func<T> callback) where T : class;
        T Get<T>(string key, Func<T> callback, int expiredTimeInSeconds) where T : class;

        bool CheckExist(string key);
        void Remove(string key);
    }

    public interface ISessionCacheProvider : ICacheProvider
    {

    }
}
