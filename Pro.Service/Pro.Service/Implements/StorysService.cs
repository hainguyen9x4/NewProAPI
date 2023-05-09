using MongoDB.Driver;
using Pro.Common;
using Pro.Data.Repositorys;
using Pro.Model;
using Pro.Service.Caching;

namespace Pro.Service.Implements
{
    public class StorysService : IStorysService
    {
        private readonly INewStoryRepository _newStoryRepository;
        private readonly IImageRepository _imageRepository;
        private readonly ICacheProvider _cacheProvider;

        public StorysService(ICacheProvider cacheProvider
            , IImageRepository imageRepository
            , INewStoryRepository newStoryRepository)
        {
            _cacheProvider = cacheProvider;
            _newStoryRepository = newStoryRepository;
            _imageRepository = imageRepository;
        }

        public HomePageInfo GetHomeStoryForNews(int pageIndex, int dataPerPage = 16, bool useCache = true)
        {
            //useCache = false;
            try
            {
                Func<HomePageInfo> fetchFunc = () =>
                {
                    var results = new HomePageInfo();
                    var newStorys = new List<NewStory>();

                    var tempAllStory = GetTotalStoryForNew();
                    var allStorys = tempAllStory.NewStorys;
                    var totalStory = tempAllStory.TotalStory;

                    var totalPage = totalStory / dataPerPage + (totalStory % dataPerPage > 0 ? 1 : 0);
                    results.TotalPage = totalPage;
                    results.CurrentPage = pageIndex;

                    var storys = allStorys.OrderByDescending(s => s.UpdatedTime).Skip(pageIndex * dataPerPage).Take(dataPerPage).ToList();
                    foreach (var s in storys)
                    {
                        s.Chaps = s.Chaps.OrderByDescending(t => t.ID).Take(3).ToList();
                        newStorys.Add(s);
                    }
                    results.NewStorys = newStorys;
                    return results;
                };
                return useCache ? _cacheProvider.Get(CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.HomePageFornew, pageIndex, dataPerPage), fetchFunc) : fetchFunc();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get Home with key:{pageIndex}-{dataPerPage}", ex);
                return new HomePageInfo();
            }
        }

        public List<ImageStoryInfo> GetTopHotStorysForNew(bool useCache = true)
        {
            try
            {
                Func<List<ImageStoryInfo>> fetchFunc = () =>
                {
                    var results = new List<ImageStoryInfo>();

                    var storys = GetTotalStoryForNew().NewStorys;

                    var topStorys = storys.OrderBy(x => Guid.NewGuid()).Take(8).ToList();

                    var hotSoryIds = topStorys.Select(t => t.ID).ToList();

                    if (!hotSoryIds.Any()) return results;
                    //var allChaps = _chapRepository.GetAll();

                    foreach (var topStory in topStorys)
                    {
                        var chapInfos = new List<ChapInfoForHome>();

                        var chap = topStory.Chaps.OrderByDescending(ac => ac.ID).FirstOrDefault();                                                                                                           //    LastModifyDatetime = c.LastModifyDatetime,
                        if (chap != null)
                        {
                            chapInfos.Add(new ChapInfoForHome()
                            {
                                ChapName = chap.Name,
                                ChapLink = chap.Link,
                                ChapID = chap.ID,
                                LastModifyDatetime = topStory.UpdatedTime,
                            });
                        }
                        var imageStoryInfo = new ImageStoryInfo()
                        {
                            StoryID = topStory.ID,
                            StoryLink = topStory.Link,
                            StoryName = topStory.Name,
                            StoryPictureLink = topStory.Picture,
                            StoryNameShow = topStory.NameShow,
                            Chaps = chapInfos,
                        };
                        results.Add(imageStoryInfo);
                    }

                    return results;
                };
                return useCache ? _cacheProvider.Get(CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.TopHotStory), fetchFunc) : fetchFunc();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get TopHot with key", ex);
                return null;
            }
        }

        public List<ImageStoryInfo> GetAllStoryForSearchForNew(bool useCache = true)
        {
            try
            {
                var storys = GetTotalStoryForNew().NewStorys;

                var results = storys.Select(s => new ImageStoryInfo
                {
                    StoryID = s.ID,
                    StoryLink = s.Link,
                    StoryName = s.Name,
                    StoryPictureLink = s.Picture,
                    StoryNameShow = s.NameShow,

                });
                return results.ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get Search SptyData with key", ex);
                return null;
            }
        }

        private TempGetAllStoryData GetTotalStoryForNew(bool useCache = true)
        {
            try
            {
                Func<TempGetAllStoryData> fetchFunc = () =>
                {
                    return new TempGetAllStoryData()
                    {
                        NewStorys = _newStoryRepository.GetAll().Take(100).ToList(),
                        TotalStory = _newStoryRepository.GetAll().Count()

                    };
                };
                var value = useCache ? _cacheProvider.Get<TempGetAllStoryData>(CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.ListAllStory), fetchFunc, expiredTimeInSeconds: 400) : fetchFunc();

                return value;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when caculate page", ex);
                return new TempGetAllStoryData();
            }
        }

        public ImageStoryInfo GetAllChapByStoryIdForNew(int storyID, bool useCache = true)
        {
            try
            {
                Func<ImageStoryInfo> fetchFunc = () =>
                {
                    var storyInfor = _newStoryRepository.GetAll().Where(s => s.ID == storyID).FirstOrDefault();
                    if (storyInfor == null) return null;

                    var imageStoryInfo = new ImageStoryInfo()
                    {
                        StoryID = storyID,
                        StoryLink = storyInfor.Link,
                        StoryName = storyInfor.Name,
                        StoryPictureLink = storyInfor.Picture,
                        StoryNameShow = storyInfor.NameShow,
                        Chaps = new List<ChapInfoForHome>(),
                        LastUpdateTime = storyInfor.UpdatedTime,
                    };
                    foreach (var chap in storyInfor.Chaps)
                    {
                        imageStoryInfo.Chaps.Add(new ChapInfoForHome()
                        {
                            ChapID = chap.ID,
                            ChapLink = chap.Link,
                            ChapName = chap.Name,
                        });
                    }
                    return imageStoryInfo;
                };
                return useCache ? _cacheProvider.Get(CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.StoryID, storyID), fetchFunc) : fetchFunc();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get with key:{storyID}", ex);
                return null;
            }
        }

        public ChapInfo GetImageStorysInChapForNew(int storyID, int chapID, bool useCache = true)
        {
            try
            {
                Func<ChapInfo> fetchFunc = () =>
                {
                    var storyInfor = _newStoryRepository.GetAll().Where(s => s.ID == storyID).FirstOrDefault();
                    if (storyInfor == null) return null;

                    var storyShortInfos = storyInfor.Chaps.Select(a => new ShortStoryInfo()
                    {
                        ChapLink = a.Link,
                        ChapName = a.Name,
                        ChapId = a.ID,
                    }).ToList();

                    var chap = storyInfor.Chaps.Where(c => c.ID == chapID).FirstOrDefault();
                    if (chap == null) return null;

                    var image = _imageRepository.GetAll().Where(i => i.StoryID == storyID && i.ChapID == chapID).FirstOrDefault();
                    var imageStoryInfo = new ChapInfo()
                    {
                        ChapID = chap.ID,
                        ChapLink = chap.Link,
                        ChapName = chap.Name,
                        StoryNameShow = storyInfor.NameShow,
                        StoryName = storyInfor.Name,
                        ImageStoryLinks = image.Images,
                        LastModifyDatetime = storyInfor.UpdatedTime,
                        StoryShortInfos = storyShortInfos,
                    };
                    return imageStoryInfo;
                };
                return useCache ? _cacheProvider.Get(CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.StoryIDChapID, storyID, chapID), fetchFunc) : fetchFunc();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get with key:{storyID}, {chapID}", ex);
                return null;
            }
        }
    }
}