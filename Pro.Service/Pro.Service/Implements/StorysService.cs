using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using Pro.Common;
using Pro.Common.Enum;
using Pro.Data.Repositorys;
using Pro.Model;
using Pro.Service.Caching;

namespace Pro.Service.Implements
{
    public class StorysService : IStorysService
    {
        private readonly INewStoryRepository _newStoryRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IStoryTypeRepository _typeRepository;
        private readonly ICacheProvider _cacheProvider;

        public StorysService(ICacheProvider cacheProvider
            , IImageRepository imageRepository
            , IStoryTypeRepository typeRepository
            , INewStoryRepository newStoryRepository)
        {
            _cacheProvider = cacheProvider;
            _newStoryRepository = newStoryRepository;
            _typeRepository = typeRepository;
            _imageRepository = imageRepository;
        }

        public HomePageInfo GetHomeStoryForNews(int pageIndex, int dataPerPage = 16, bool useCache = true)
        {
            var results = new HomePageInfo();
            var tempAllStory = GetTotalStoryForNew(pageIndex, dataPerPage);

            results.TotalPage = tempAllStory.TotalStory / dataPerPage + (tempAllStory.TotalStory % dataPerPage > 0 ? 1 : 0);
            results.CurrentPage = pageIndex;
            var datas = tempAllStory.NewStorys.Skip(pageIndex * dataPerPage).Take(dataPerPage).ToList();
            foreach (var s in datas)
            {
                s.Chaps = s.Chaps.Take(3).ToList();
            }
            results.NewStorys = datas;

            return results;

        }

        public List<ImageStoryInfo> GetTopHotStorysForNew(bool useCache = true)
        {
            var results = new List<ImageStoryInfo>();

            var storys = GetTotalStoryForNew().NewStorys;

            var topStorys = storys.OrderBy(x => Guid.NewGuid()).Take(78).ToList();
            MakeMoreDetailStoryWithChaps(topStorys, results, 1);
            //foreach (var topStory in topStorys)
            //{
            //    var chapInfos = new List<Chap>();

            //    var chap = topStory.Chaps.OrderByDescending(ac => ac.ID).FirstOrDefault();                                                                                                           //    LastModifyDatetime = c.LastModifyDatetime,
            //    if (chap != null)
            //    {
            //        chapInfos.Add(new Chap()
            //        {
            //            ID = chap.ID,
            //            Link = chap.Link,
            //            Name = chap.Name,
            //        });
            //    }
            //    var imageStoryInfo = new ImageStoryInfo()
            //    {
            //        StoryID = topStory.ID,
            //        StoryLink = topStory.Link,
            //        StoryName = topStory.Name,
            //        StoryPictureLink = topStory.Picture,
            //        StoryNameShow = topStory.NameShow,
            //        Chaps = chapInfos,
            //        LastUpdateTime = topStory.UpdatedTime,
            //        View = topStory.OtherInfo.ViewTotal,
            //    };
            //    results.Add(imageStoryInfo);
            //}

            return results;
        }

        public List<ImageStoryInfo> GetAllStoryForSearchForNew(bool useCache = true)
        {
            try
            {
                var storys = GetTotalStoryForNew(dataPerPage: 0).NewStorys;

                var results = storys.Select(s => new ImageStoryInfo
                {
                    StoryID = s.ID,
                    StoryLink = s.Link,
                    StoryName = s.Name,
                    StoryPictureLink = s.Picture,
                    StoryNameShow = s.NameShow,
                    StoryTypes = GetStoryTypeInfoByList(s.OtherInfo.TypeIDs),
                    Chaps = new List<Chap>() { s.Chaps.First() }
                });
                return results.ToList();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get Search SptyData with key", ex);
                return null;
            }
        }

        private TempGetAllStoryData GetTotalStoryForNew(int pageIndex = 0, int dataPerPage = 16, int numberStory = 10, bool useCache = true, int typeID = 0, string cachedKey = "")
        {
            try
            {
                Func<TempGetAllStoryData> fetchFunc = () =>
                {
                    var storys = new List<NewStory>();
                    var total = 0;
                    if (typeID > 0)
                    {
                        var types = GetAllStoryType().ToList();
                        var type = types.Where(s => s.TypeID == typeID).FirstOrDefault();
                        if (type != null)
                        {
                            storys = GetAllStory().Where(s => s.StatusID != 0)
                            .Where(s => s.OtherInfo.TypeIDs.Contains(type.TypeID))
                            .ToList();
                            total = storys.Count();
                            storys = storys.OrderByDescending(s => s.UpdatedTime).Take(dataPerPage * numberStory).ToList();
                        }
                    }
                    else
                    {
                        if (dataPerPage * numberStory == 0)//For search
                        {
                            storys = GetAllStory().Where(s => s.StatusID != 0).ToList();
                        }
                        else
                        {
                            storys = GetAllStory()
                            .OrderByDescending(s => s.UpdatedTime).Take(dataPerPage * numberStory).ToList();
                            total = GetAllStory().Count();
                        }
                    }
                    foreach (var s in storys)
                    {
                        s.Chaps = s.Chaps.OrderByDescending(t => t.ID).ToList();
                    }
                    return new TempGetAllStoryData()
                    {
                        NewStorys = storys,
                        TotalStory = total
                    };
                };
                var cached = "";
                if (!String.IsNullOrEmpty(cachedKey))
                {
                    cached = cachedKey;// CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.ListAllStoryOfPageStoryType, pageIndex / dataPerPage, dataPerPage, typeName);
                }
                else
                {
                    cached = CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.ListAllStoryOfPage, pageIndex, dataPerPage);
                    if (dataPerPage == 0) cached = CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.ListAllStory);
                }

                return useCache ? _cacheProvider.Get<TempGetAllStoryData>(cached, fetchFunc, expiredTimeInSeconds: 433) : fetchFunc();

            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when caculate page", ex);
                return new TempGetAllStoryData();
            }
        }
        public List<NewStory> GetAllStory(int call = 0)
        {
            if (call == 2)
            {
                LogHelper.Info("Call from function!");
            }
            var totalStorts = _cacheProvider.Get<List<NewStory>>(CacheKeys.ImageStoryData.ListAllStorys);
            if (totalStorts != null && totalStorts.Any())
            {
                return totalStorts;
            }
            else
            {
                var storys = _newStoryRepository.GetAll().Where(s => s.StatusID != 0).ToList();
                foreach (var story in storys)
                {
                    story.Chaps = story.Chaps.OrderByDescending(c => c.ID).Take(3).ToList();
                }
                _cacheProvider.Set<List<NewStory>>(CacheKeys.ImageStoryData.ListAllStorys, storys, expiredTimeInSeconds: 60 * 6);
                return storys;
            }
        }
        public ImageStoryInfo GetAllChapByStoryIdForNew(int storyID, bool useCache = true)
        {
            try
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
                    Chaps = new List<Chap>(),
                    LastUpdateTime = storyInfor.UpdatedTime,
                    StoryTypes = GetStoryTypeInfoByList(storyInfor.OtherInfo.TypeIDs),
                    Des = storyInfor.OtherInfo.Des
                };
                foreach (var chap in storyInfor.Chaps)
                {
                    imageStoryInfo.Chaps.Add(new Chap()
                    {
                        ID = chap.ID,
                        Link = chap.Link,
                        Name = chap.Name,
                        UpdatedTime = chap.UpdatedTime
                    });
                }
                return imageStoryInfo;
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
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when get with key:{storyID}, {chapID}", ex);
                return null;
            }
        }

        public List<ImageStoryInfo> GetFollowStorys(List<int> storyIDs, int userID, bool useCache = true)
        {
            try
            {
                Func<List<ImageStoryInfo>> fetchFunc = () =>
                {
                    var results = new List<ImageStoryInfo>();

                    var storys = GetAllStory().Where(s => storyIDs.Contains(s.ID)).ToList();

                    foreach (var topStory in storys)
                    {
                        var chapInfos = new List<Chap>();

                        var chap = topStory.Chaps.OrderByDescending(ac => ac.ID).FirstOrDefault();                                                                                                           //    LastModifyDatetime = c.LastModifyDatetime,
                        if (chap != null)
                        {
                            chapInfos.Add(new Chap()
                            {
                                ID = chap.ID,
                                Link = chap.Link,
                                Name = chap.Name,
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
                            LastUpdateTime = topStory.UpdatedTime,
                            View = topStory.OtherInfo.ViewTotal,
                        };
                        results.Add(imageStoryInfo);
                    }

                    return results;
                };

                return useCache ? _cacheProvider.Get<List<ImageStoryInfo>>(CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.ListFollowStorys, userID), fetchFunc, expiredTimeInSeconds: 400) : fetchFunc();

            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when GetFollowStorys", ex);
                return new List<ImageStoryInfo>();
            }
        }

        private List<StoryType> GetStoryTypeInfoByList(List<int> types)
        {
            return GetAllStoryType().Where(s => types.Contains(s.TypeID)).Select(t => new StoryType()
            {
                Name = t.Name,
                NameShow = t.NameShow,
            }).ToList();
        }
        private List<StoryType> GetAllStoryType(bool useCache = true)
        {
            try
            {
                Func<List<StoryType>> fetchFunc = () =>
                {
                    return _typeRepository.GetAll().ToList();
                };
                var cachedKey = CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.ListStoryTypes);
                return useCache ? _cacheProvider.Get<List<StoryType>>(cachedKey, fetchFunc, expiredTimeInSeconds: 4000) : fetchFunc();

            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when caculate page", ex);
                return new List<StoryType>();
            }
        }
        public class TempGetAllStoryByTypeName
        {
            public List<ImageStoryInfo> ImageStoryInfos { get; set; }
            public StoryType StoryType { get; set; }
            public List<StoryType> StoryTypes { get; set; }
            public int TotalPage { get; set; }
            public int CurrentPage { get; set; }
        }
        public TempGetAllStoryByTypeName GetAllStoryByTypeName(string typeName, int pageIndex = 0, int dataPerPage = 16, int numberStory = 10, bool useCache = true)
        {
            var results = new TempGetAllStoryByTypeName();
            var types = GetAllStoryType().ToList();
            var typeID = 1;
            var listTypeIDs = new List<int>();
            var type = new StoryType();
            if (typeName == "tim-truyen")
            {
                listTypeIDs = types.Select(x => x.TypeID).ToList();
                results.StoryType = types.Where(s => s.TypeID == 1).First();
                typeID = 0;
            }
            else
            {
                type = types.Where(s => s.Name.Equals(typeName)).FirstOrDefault();
                if (type != null)
                {
                    listTypeIDs.Add(type.TypeID);
                    results.StoryType = type;
                    typeID = type.TypeID;
                }
            }
            results.ImageStoryInfos = new List<ImageStoryInfo>();

            var dataStory = GetTotalStoryForNew(pageIndex: pageIndex, typeID: typeID, dataPerPage: dataPerPage,
                numberStory: numberStory, useCache: useCache,
                cachedKey: CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.ListAllStoryOfPageStoryType, pageIndex / dataPerPage, dataPerPage, typeID));
            var storys = dataStory.NewStorys.Skip(pageIndex * dataPerPage).Take(dataPerPage).ToList();

            var temp = new List<ImageStoryInfo>();
            results.TotalPage = dataStory.TotalStory / dataPerPage + (dataStory.TotalStory % dataPerPage > 0 ? 1 : 0);
            results.CurrentPage = pageIndex;
            MakeMoreDetailStoryWithChaps(storys, temp, 3);

            results.ImageStoryInfos = temp;
            results.StoryTypes = types;
            results.StoryType = type;
            return results;
        }

        public TempGetAllStoryByTypeName GetAllStoryByTypeIDs(List<int> typeIDs, int pageIndex = 0, int dataPerPage = 16, int numberStory = 10, bool useCache = true)
        {
            try
            {
                Func<TempGetAllStoryByTypeName> fetchFunc = () =>
                {
                    var results = new TempGetAllStoryByTypeName();

                    results.ImageStoryInfos = new List<ImageStoryInfo>();

                    var types = GetAllStoryType().ToList();
                    if (!typeIDs.Any())
                    {
                        typeIDs = types.Select(t => t.TypeID).ToList();
                    }

                    var storys = GetAllStory().Where(s => s.StatusID != 0 && typeIDs.Contains(s.ID)).ToList()
                    .OrderByDescending(s => s.UpdatedTime).Skip(pageIndex * dataPerPage).Take(dataPerPage).ToList();
                    var total = GetAllStory().Count();

                    var temp = new List<ImageStoryInfo>();
                    MakeMoreDetailStoryWithChaps(storys, temp, 3);
                    results.ImageStoryInfos.AddRange(temp);

                    results.TotalPage = total / dataPerPage + (total % dataPerPage > 0 ? 1 : 0);
                    results.CurrentPage = pageIndex;
                    results.StoryTypes = types;

                    return results;
                };

                var cachedKey = CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.ListStoryGenderType, pageIndex, dataPerPage, !typeIDs.Any() ? 1 : 0);

                return useCache ? _cacheProvider.Get<TempGetAllStoryByTypeName>(cachedKey, fetchFunc, expiredTimeInSeconds: 4000) : fetchFunc();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when caculate page", ex);
                return new TempGetAllStoryByTypeName();
            }
        }

        private static void MakeMoreDetailStoryWithChaps(List<NewStory> storys, List<ImageStoryInfo> temp, int numberChap)
        {
            foreach (var story in storys)
            {
                var chapInfos = new List<Chap>();

                var chaps = story.Chaps.OrderByDescending(ac => ac.ID).Take(numberChap).ToList();                                                                                                           //    LastModifyDatetime = c.LastModifyDatetime,
                if (chaps.Any())
                {
                    foreach (var chap in chaps)
                    {
                        chapInfos.Add(new Chap()
                        {
                            ID = chap.ID,
                            Link = chap.Link,
                            Name = chap.Name,
                            UpdatedTime = chap.UpdatedTime,
                        });
                    }
                }
                var imageStoryInfo = new ImageStoryInfo()
                {
                    StoryID = story.ID,
                    StoryLink = story.Link,
                    StoryName = story.Name,
                    StoryPictureLink = story.Picture,
                    StoryNameShow = story.NameShow,
                    Chaps = chapInfos,
                    LastUpdateTime = story.UpdatedTime,
                    View = story.OtherInfo.ViewTotal,
                };
                temp.Add(imageStoryInfo);
            }
        }

        public TempGetAllStoryByTypeName RateStory(RATE_TYPE type, int pageIndex = 0, int dataPerPage = 16)
        {
            var results = new TempGetAllStoryByTypeName();
            var types = GetAllStoryType().ToList();

            var temp = new List<ImageStoryInfo>();
            var story = new TempGetAllStoryData();
            var cached = CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.ListAllStoryOfRateType, pageIndex, dataPerPage, Convert.ToInt32(type));
            switch (type)
            {
                case
                    RATE_TYPE.TOP_ALL:
                case
                    RATE_TYPE.TOP_MONTH:
                case
                    RATE_TYPE.TOP_WEEK:
                case
                    RATE_TYPE.TOP_DAY:
                case
                    RATE_TYPE.TOP_LIKE:
                    story = GetTotalStoryForNew2(pageIndex, dataPerPage, type: type);
                    story.NewStorys = story.NewStorys.OrderByDescending(s => s.ID).Skip(pageIndex * dataPerPage).Take(dataPerPage).ToList();
                    MakeMoreDetailStoryWithChaps(story.NewStorys, temp, 3);
                    break;
                case
                    RATE_TYPE.NEW_STORY:
                    story = GetTotalStoryForNew(pageIndex, dataPerPage, cachedKey: cached);
                    story.NewStorys = story.NewStorys.OrderByDescending(s => s.ID).Skip(pageIndex * dataPerPage).Take(dataPerPage).ToList();
                    MakeMoreDetailStoryWithChaps(story.NewStorys, temp, 3);
                    break;
                case
                    RATE_TYPE.NEWEST_UPDATED:
                    story = GetTotalStoryForNew(pageIndex, dataPerPage, cachedKey: cached);
                    story.NewStorys = story.NewStorys.OrderByDescending(s => s.ID).Skip(pageIndex * dataPerPage).Take(dataPerPage).ToList();
                    MakeMoreDetailStoryWithChaps(story.NewStorys, temp, 3);
                    break;
            }

            results.ImageStoryInfos = temp;
            results.StoryTypes = types;
            results.TotalPage = story.TotalStory / dataPerPage + (story.TotalStory % dataPerPage > 0 ? 1 : 0);
            results.CurrentPage = pageIndex;
            return results;
        }
        private TempGetAllStoryData GetTotalStoryForNew2(int pageIndex = 0, int dataPerPage = 16,
            int numberStory = 10, bool useCache = true, string typeName = "", string cachedKey = "",
            RATE_TYPE type = RATE_TYPE.TOP_ALL)
        {
            try
            {
                Func<TempGetAllStoryData> fetchFunc = () =>
                {
                    var storys = new List<NewStory>();
                    var total = 0;

                    storys = GetAllStory().Where(s => s.StatusID != 0 && s.OtherInfo.ViewTotal > 1000).Take(1000).ToList();

                    total = storys.Count();

                    storys = storys.OrderByDescending(s => s.OtherInfo.ViewTotal).Take(dataPerPage * numberStory).ToList();

                    foreach (var s in storys)
                    {
                        s.Chaps = s.Chaps.OrderByDescending(t => t.ID).ToList();
                    }
                    return new TempGetAllStoryData()
                    {
                        NewStorys = storys,
                        TotalStory = total
                    };
                };
                var cached = CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.ListAllStoryOfRateType, pageIndex, dataPerPage, Convert.ToInt32(type));

                return useCache ? _cacheProvider.Get<TempGetAllStoryData>(cached, fetchFunc, expiredTimeInSeconds: 433) : fetchFunc();

            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when caculate page", ex);
                return new TempGetAllStoryData();
            }
        }
    }
}