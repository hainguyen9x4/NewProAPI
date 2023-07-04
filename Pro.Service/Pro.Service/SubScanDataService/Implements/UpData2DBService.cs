using Amazon.Runtime.Internal.Util;
using FileManager;
using Pro.Common;
using Pro.Common.Const;
using Pro.Data.Repositorys;
using Pro.Model;
using Pro.Service.Caching;

namespace Pro.Service.SubScanDataService.Implements
{
    public class UpData2DBService : IUpData2DBService
    {
        private readonly IImageRepository _imageRepository;
        private readonly INewStoryRepository _newStoryRepository;
        private readonly ICacheProvider _cacheProvider;
        private readonly IApplicationSettingService _applicationSettingService;

        public UpData2DBService(IImageRepository image
            , INewStoryRepository newStory
            , ICacheProvider cacheProvider
            , IApplicationSettingService applicationSettingService)
        {
            _imageRepository = image;
            _newStoryRepository = newStory;
            _cacheProvider = cacheProvider;
            _applicationSettingService = applicationSettingService;
        }

        private NewStory GetStoryIdFromStoryNameForNew(NewStory dataStoryForSave)
        {
            try
            {
                var story = _newStoryRepository.GetAll().Where(s => s.Name.Equals(dataStoryForSave.Name)).FirstOrDefault();
                if (story != null)
                {
                    return story;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"GetStoryIdFromStoryName {dataStoryForSave.Name}" + ex);
                return null;
            }
        }

        public void UpData2DBForNew(NewStory dataStory)
        {
            var story = GetStoryIdFromStoryNameForNew(dataStory);

            if (story != null)
            {
                //Old story
                UpdateChapId(dataStory, story.Chaps.Count + 1);

                var imagesOnChap = new List<ImagesOneChap>();
                foreach (var chapSaveData in dataStory.Chaps)
                {
                    imagesOnChap.Add(new ImagesOneChap(story.ID, chapSaveData.ID, chapSaveData.Images));
                    chapSaveData.Images = new List<Model.ImageData>();
                }
                _imageRepository.Creates(imagesOnChap);

                LogHelper.Info($"UpData2DBForNew: _1");

                story.Chaps.AddRange(dataStory.Chaps);
                _newStoryRepository.Update(story.ID, dataStory);

            }
            else
            {
                dataStory.Link = FileReader.DeleteHomePage(dataStory.Link);
                //new Story
                FakeDataOtherInfo(dataStory);
                UpdateChapId(dataStory);

                var imagesOnChap = new List<ImagesOneChap>();
                foreach (var chapSaveData in dataStory.Chaps)
                {
                    imagesOnChap.Add(new ImagesOneChap(0, chapSaveData.ID, chapSaveData.Images));
                    chapSaveData.Images = new List<Model.ImageData>();
                }

                var newStory = _newStoryRepository.Create(dataStory);

                foreach (var c in imagesOnChap)
                {
                    c.StoryID = newStory.ID;
                }
                _imageRepository.Creates(imagesOnChap);
            }
        }

        public List<ImageStoryInvalidData> GetDataInvalid(int limitNumberStoty = 5)
        {
            var dataInvalids = new List<ImageStoryInvalidData>();
            var allStoryIDs = GetAllStoryIds();

            var listIdsCheckNeedToCache = _cacheProvider.Get<List<int>>(CacheKeys.ScanGetData.ListStoryIDChecked);
            if (listIdsCheckNeedToCache == null)
            {
                listIdsCheckNeedToCache = new List<int>();
            }
            foreach (var storyID in allStoryIDs)
            {
                if (listIdsCheckNeedToCache.Contains(storyID))
                {
                    continue;
                }

                var chapIDs = _imageRepository.GetAll().Where(i => i.StoryID == storyID).Select(i => i.ChapID).ToArray();
                var listChaps = new List<ImagesOneChap>();
                var listChapIdsCheckNeedToCache = _cacheProvider.Get<List<int>>(CacheKeys.GetCacheKey(CacheKeys.ScanGetData.ListChapIDChecked, storyID));
                if (listChapIdsCheckNeedToCache == null)
                {
                    listChapIdsCheckNeedToCache = new List<int>();
                }
                foreach (var chapID in chapIDs)
                {
                    if (listChapIdsCheckNeedToCache.Contains(chapID))
                    {
                        continue;
                    }

                    var chapData = _imageRepository.GetAll().Where(i => i.StoryID == storyID && i.ChapID == chapID).First();
                    var hasInvalidImage = false;
                    for (int index = 0; index < chapData.Images.Count; index++)
                    {
                        if (!String.IsNullOrEmpty(chapData.Images[index].OriginLink))
                        {
                            hasInvalidImage = true;
                            break;
                        }
                    }
                    if (hasInvalidImage)
                    {
                        listChaps.Add(chapData);
                    }
                    else
                    {
                        listChapIdsCheckNeedToCache.Add(chapID);
                        _cacheProvider.Set(key: CacheKeys.GetCacheKey(CacheKeys.ScanGetData.ListChapIDChecked, storyID), data: listChapIdsCheckNeedToCache, expiredTimeInSeconds: 600);
                    }
                }
                if (listChaps.Any())
                {
                    var storyInValid = new ImageStoryInvalidData();
                    var storyData = _newStoryRepository.GetAll().Where(s => s.ID == storyID).First();

                    storyInValid.ID = storyData.ID;
                    storyInValid.Name = storyData.Name;
                    storyInValid.StatusID = storyData.StatusID;
                    storyInValid.NameShow = storyData.NameShow;
                    storyInValid.Link = storyData.Link;

                    var lstFinal = new List<ImagesOneChapForUpdate>();
                    foreach (var lst in listChaps)
                    {
                        var homeLinkWithSub = _applicationSettingService.GetValue(ApplicationSettingKey.HomePage) + _applicationSettingService.GetValue(ApplicationSettingKey.SubDataForHomePage);
                        var chapLink = homeLinkWithSub + storyData.Chaps.Where(c => c.ID == lst.ChapID).First().Link;
                        lstFinal.Add(new ImagesOneChapForUpdate(lst.Id, lst.ChapID, lst.Images, chapLink));
                    }
                    storyInValid.Chaps = lstFinal;
                    dataInvalids.Add(storyInValid);
                }
                else
                {
                    listIdsCheckNeedToCache.Add(storyID);
                    _cacheProvider.Set(key: CacheKeys.ScanGetData.ListStoryIDChecked, data: listIdsCheckNeedToCache, expiredTimeInSeconds: 600);
                }
                if (listChaps.Count >= limitNumberStoty) break;
            }
            return dataInvalids;
        }
        public List<int> GetAllStoryIds()
        {
            try
            {
                Func<List<int>> fetchFunc = () =>
                {
                    return _newStoryRepository.GetAll().Select(story => story.ID).ToList();
                };

                return _cacheProvider.Get<List<int>>(CacheKeys.GetCacheKey(CacheKeys.ScanGetData.ListStoryIDForCheckInvalid), fetchFunc, expiredTimeInSeconds: 600);

            }
            catch (Exception ex)
            {
                LogHelper.Error($"Error when GetAllStoryIds", ex);
                return new List<int>();
            }
        }
        private void FakeDataOtherInfo(NewStory dataStory)
        {
            dataStory.OtherInfo = new OtherInfo(new Star(4.5, RandomRate(3000, 9000)), new List<int>(dataStory.OtherInfo.TypeIDs), "", des: dataStory.OtherInfo.Des, RandomRate(80000, 99000), RandomRate(80000, 99000));

            int RandomRate(int start, int end)
            {
                var random = new Random();
                return random.Next(start, end);
            }
        }

        private void UpdateChapId(NewStory dataStory, int startId = 1)
        {
            foreach (var chap in dataStory.Chaps)
            {
                chap.ID = startId;
                startId++;
            }
        }
    }
}
