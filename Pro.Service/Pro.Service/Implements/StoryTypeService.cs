using Pro.Common;
using Pro.Data.Repositorys;
using Pro.Model;
using Pro.Service.Caching;
using System.Runtime.CompilerServices;

namespace Pro.Service.Implements
{
    public class StoryTypeService : IStoryTypeService
    {
        private readonly IStoryTypeRepository _storyTypesitory;
        private readonly ICacheProvider _cacheProvider;

        public StoryTypeService(IStoryTypeRepository storyTypesitory
            , ICacheProvider cacheProvider)
        {
            _storyTypesitory = storyTypesitory;
            _cacheProvider = cacheProvider;
        }

        public bool CreateNewStoryType(StoryType type)
        {
            try
            {
                _storyTypesitory.Create(type);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<StoryType> GetAllStoryTypebyID(int storyTypeID, string nameType)
        {
            if (!String.IsNullOrEmpty(nameType))
            {
                return _storyTypesitory.GetAll().Where(t => t.Name == nameType).ToList();
            }
            else
            {
                return _storyTypesitory.GetAll().Where(t => t.TypeID == storyTypeID).ToList();
            }
        }

        public List<StoryType> GetAllStoryType(bool useCache = true)
        {
            try
            {
                Func<List<StoryType>> fetchFunc = () =>
                {
                    return _storyTypesitory.GetAll().OrderBy(t => t.TypeID).ToList();
                };
                return useCache ? _cacheProvider.Get(CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.AllStoryType), fetchFunc) : fetchFunc();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get the application setting with key: {CacheKeys.ImageStoryData.AllStoryType}", ex);
                return new List<StoryType>();
            }
        }
        public bool UpdateStoryType(StoryType type)
        {
            try
            {
                _storyTypesitory.Update(type.TypeID, type);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

    }
}