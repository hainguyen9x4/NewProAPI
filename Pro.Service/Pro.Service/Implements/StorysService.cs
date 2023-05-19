using MongoDB.Driver;
using Pro.Common;
using Pro.Data.Repositorys;
using Pro.Model;
using Pro.Service.Caching;
using System.Collections.Generic;
using System.Xml.Linq;

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

            foreach (var topStory in topStorys)
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
                };
                results.Add(imageStoryInfo);
            }

            return results;
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

        private TempGetAllStoryData GetTotalStoryForNew(int pageIndex = 0, int dataPerPage = 16, int numberStory = 10, bool useCache = true)
        {
            try
            {
                Func<TempGetAllStoryData> fetchFunc = () =>
                {
                    var storys = _newStoryRepository.GetAll().ToList().OrderByDescending(s => s.UpdatedTime).Take(dataPerPage * numberStory).ToList();
                    foreach (var s in storys)
                    {
                        s.Chaps = s.Chaps.OrderByDescending(t => t.ID).ToList();
                    }
                    return new TempGetAllStoryData()
                    {
                        NewStorys = storys,
                        TotalStory = _newStoryRepository.GetAll().Count()

                    };
                };
                var cached = CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.ListAllStoryOfPage, pageIndex / dataPerPage, dataPerPage);
                if (dataPerPage == 0) cached = CacheKeys.GetCacheKey(CacheKeys.ImageStoryData.ListAllStory);

                return useCache ? _cacheProvider.Get<TempGetAllStoryData>(cached, fetchFunc, expiredTimeInSeconds: 400) : fetchFunc();

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
    }
}