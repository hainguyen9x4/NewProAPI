using Pro.Common;
using Pro.Data.Repositorys;
using Pro.Model;
using Pro.Service.Caching;
using System.Runtime.CompilerServices;

using System.Diagnostics;
using System.Net.WebSockets;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace Pro.Service.Implements
{
    public class StorysService : IStorysService
    {
        private readonly IStoryRepository _storyRepository;
        private readonly INewStoryRepository _newStoryRepository;
        private readonly IChapRepository _chapRepository;
        private readonly IHotStoryRepository _hotStoryRepository;

        private readonly ICacheProvider _cacheProvider;
        public StorysService(IStoryRepository storyRepository
            , IChapRepository chapRepository
            , IHotStoryRepository hotStoryRepository
            , ICacheProvider cacheProvider
            , INewStoryRepository newStoryRepository)
        {
            _storyRepository = storyRepository;
            _chapRepository = chapRepository;
            _hotStoryRepository = hotStoryRepository;
            _cacheProvider = cacheProvider;
            _newStoryRepository = newStoryRepository;
        }

        public List<ImageStoryInfo> GetTopHotStorys(bool useCache = true)
        {
            try
            {
                Func<List<ImageStoryInfo>> fetchFunc = () =>
                {
                    var results = new List<ImageStoryInfo>();

                    var topStorys = _storyRepository.GetAll().Take(8).ToList();

                    var hotSoryIds = topStorys.Select(t => t.StoryId).ToList();

                    if (!hotSoryIds.Any()) return results;
                    var allChaps = _chapRepository.GetAll();

                    foreach (var topStory in topStorys)
                    {
                        var chapInfos = new List<ChapInfoForHome>();

                        var chap = allChaps.Where(ac => ac.StoryId == topStory.StoryId).OrderByDescending(ac => ac.ChapId).FirstOrDefault();                                                                                                           //    LastModifyDatetime = c.LastModifyDatetime,
                        if (chap != null)
                        {
                            chapInfos.Add(new ChapInfoForHome()
                            {
                                ChapName = chap.ChapName,
                                ChapLink = chap.ChapLink,
                                ChapID = chap.ChapId,
                                LastModifyDatetime = chap.LastModifyDatetime,
                            });
                        }
                        var imageStoryInfo = new ImageStoryInfo()
                        {
                            StoryID = topStory.StoryId,
                            StoryLink = topStory.StoryLink,
                            StoryName = topStory.StoryName,
                            StoryPictureLink = topStory.StoryPicture,
                            StoryNameShow = topStory.StoryNameShow,
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

        public HomePageInfo GetHomeStorys(int pageIndex, int dataPerPage = 16, bool useCache = true)
        {
            //useCache = false;
            try
            {
                Func<HomePageInfo> fetchFunc = () =>
                {
                    var results = new HomePageInfo();

                    var allChaps = _chapRepository.GetAll()
                        .OrderByDescending(x => x.ChapId).GroupBy(x => x.StoryId).Select(g => g.First()).Skip(pageIndex * dataPerPage).Take(dataPerPage).ToList();

                    var allStorys = GetTotalStory();
                    var totalPage = allStorys.Count / dataPerPage + (allStorys.Count % dataPerPage > 0 ? 1 : 0);

                    var listIdStory = allChaps.Select(t => t.StoryId).ToArray();
                    var topStorys = allStorys.Where(s => listIdStory.Contains(s.StoryId)).ToList();
                    if (topStorys.Any())
                    {
                        var storyIds = topStorys.Select(t => t.StoryId).ToList();
                        if (!allChaps.Any()) return null;

                        foreach (var storyId in listIdStory)
                        {
                            var chapInfos = new List<ChapInfoForHome>();

                            _chapRepository.GetAll().Where(ac => ac.StoryId == storyId).OrderByDescending(ac => ac.ChapId).Take(3).ToList()
                                .ForEach(c => chapInfos.Add(new ChapInfoForHome()
                                {
                                    ChapID = c.ChapId,
                                    ChapName = c.ChapName,
                                    ChapLink = c.ChapLink,
                                    LastModifyDatetime = c.LastModifyDatetime,
                                }));
                            var topStory = topStorys.Where(s => s.StoryId == storyId).FirstOrDefault();
                            var imageStoryInfo = new ImageStoryInfo()
                            {
                                StoryID = storyId,
                                StoryLink = topStory.StoryLink,
                                StoryName = topStory.StoryName,
                                StoryPictureLink = topStory.StoryPicture,
                                StoryNameShow = topStory.StoryNameShow,
                                Chaps = chapInfos,
                            };
                            results.TotalPage = totalPage;
                            results.CurrentPage = pageIndex;
                            results.ImageStoryInfos.Add(imageStoryInfo);
                        }
                    }

                    return results;
                };
                return useCache ? _cacheProvider.Get(CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.HomePage, pageIndex, dataPerPage), fetchFunc) : fetchFunc();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get Home with key:{pageIndex}-{dataPerPage}", ex);
                return null;
            }
        }

        public ImageStoryInfo GetAllChapByStoryId(int storyID, bool useCache = true)
        {
            try
            {
                Func<ImageStoryInfo> fetchFunc = () =>
                {
                    var storyInfor = _storyRepository.GetAll().Where(s => s.StoryId == storyID).FirstOrDefault();
                    if (storyInfor == null) return null;
                    var chaps = _chapRepository.GetAll().Where(c => c.StoryId == storyInfor.StoryId).ToList();
                    if (!chaps.Any()) return null;

                    var imageStoryInfo = new ImageStoryInfo()
                    {
                        StoryID = storyID,
                        StoryLink = storyInfor.StoryLink,
                        StoryName = storyInfor.StoryName,
                        StoryPictureLink = storyInfor.StoryPicture,
                        StoryNameShow = storyInfor.StoryNameShow,
                        Chaps = new List<ChapInfoForHome>(),
                    };
                    foreach (var chap in chaps)
                    {
                        imageStoryInfo.Chaps.Add(new ChapInfoForHome()
                        {
                            ChapID = chap.ChapId,
                            ChapLink = chap.ChapLink,
                            ChapName = chap.ChapName,
                            LastModifyDatetime = chap.LastModifyDatetime,
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

        public ChapInfo GetImageStorysInChap(int storyID, int chapID, bool useCache = true)
        {
            try
            {
                Func<ChapInfo> fetchFunc = () =>
                {
                    var storyInfor = _storyRepository.GetAll().Where(s => s.StoryId == storyID).FirstOrDefault();
                    if (storyInfor == null) return null;

                    var allChaps = _chapRepository.GetAll().Where(c => c.StoryId == storyID).ToList();
                    var chap = allChaps.Where(c => c.ChapId == chapID).FirstOrDefault();
                    if (chap == null) return null;

                    var storyShortInfos = allChaps.Select(a => new ShortStoryInfo()
                    {
                        ChapLink = a.ChapLink,
                        ChapName = a.ChapName,
                        ChapId = a.ChapId,
                    }).ToList();

                    var imageStoryInfo = new ChapInfo()
                    {
                        ChapID = chap.ChapId,
                        ChapLink = chap.ChapLink,
                        ChapName = chap.ChapName,
                        StoryNameShow = storyInfor.StoryNameShow,
                        StoryName = storyInfor.StoryName,
                        ImageStoryLinks = chap.Images,
                        LastModifyDatetime = chap.LastModifyDatetime,
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

        public List<ImageStoryInfo> GetAllStoryForSearch(bool useCache = true)
        {
            try
            {
                Func<List<ImageStoryInfo>> fetchFunc = () =>
                {
                    var results = _storyRepository.GetAll().Select(s => new ImageStoryInfo
                    {
                        StoryID = s.StoryId,
                        StoryLink = s.StoryLink,
                        StoryName = s.StoryName,
                        StoryPictureLink = s.StoryPicture,
                        StoryNameShow = s.StoryNameShow,

                    });
                    return results.ToList();
                };
                return useCache ? _cacheProvider.Get(CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.SearchStoryData), fetchFunc, expiredTimeInSeconds: 1800) : fetchFunc();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get Search SptyData with key", ex);
                return null;
            }
        }

        private List<Story> GetTotalStory(bool useCache = true)
        {
            try
            {
                Func<List<Story>> fetchFunc = () =>
                {
                    return _storyRepository.GetAll().ToList();
                };
                var value = useCache ? _cacheProvider.Get<List<Story>>(CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.ListAllStory), fetchFunc, expiredTimeInSeconds: 400) : fetchFunc();

                return value;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when caculate page", ex);
                return new List<Story>();
            }
        }

        //////******************************************************************************///////
        public HomePageInfo GetHomeStoryForNews(int pageIndex, int dataPerPage = 16, bool useCache = true)
        {
            //useCache = false;
            try
            {
                Func<HomePageInfo> fetchFunc = () =>
                {
                    var results = new HomePageInfo();
                    var newStorys = new List<NewStory>();
                    var totalStory = _newStoryRepository.GetAll().Count();

                    var totalPage = totalStory / dataPerPage + (totalStory % dataPerPage > 0 ? 1 : 0);
                    results.TotalPage = totalPage;
                    results.CurrentPage = pageIndex;
                    //var projection = Builders<NewStory>.Projection.Slice("Chaps", 0, 3);


                    //var client = new MongoClient("mongodb://xstory-db:rqs3aZ45cnVTkoAVSwkd3Wg48oR3uLRL5U9C2ORkWmSPQNW12rDx5ckjDeeZ9VTmb7tL2KQW9gPrACDbMsJEPQ%3D%3D@xstory-db.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@xstory-db@");
                    //var database = client.GetDatabase("xStory");
                    //var _newStorys = database.GetCollection<NewStory>("NewStorys");

                    //var sort = Builders<NewStory>.Sort.Descending("UpdatedTime");

                    //var result = _newStorys.Find(n => true).Sort(sort)
                    //.Skip(pageIndex * dataPerPage).Limit(dataPerPage).Project(projection)
                    //.ToList();

                    //foreach (var r in result)
                    //{

                    //    results.Add(BsonSerializer.Deserialize<NewStory>(r));
                    //}
                    var storys = _newStoryRepository.GetAll().OrderByDescending(s => s.UpdatedTime).Skip(pageIndex * dataPerPage).Take(dataPerPage).ToList();
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

    }
}