using Pro.Common;
using Pro.Common.Const;
using Pro.Common.Enum;
using Pro.Data.Repositorys;
using Pro.Model;
using Pro.Service.Caching;
using Pro.Service.SubScanDataService;

namespace Pro.Service.Implements
{
    public class CorrectInvalidDataService : ICorrectInvalidDataService
    {
        private readonly IImageRepository _imageRepository;
        private readonly INewStoryRepository _newStoryRepository;
        private readonly ICacheProvider _cacheProvider;
        private readonly IGetRawDataService _getRawDataService;
        private readonly IUploadImageService _uploadImageService;
        private readonly IApplicationSettingService _applicationSettingService;

        public CorrectInvalidDataService(ICacheProvider cacheProvider
            , IImageRepository imageRepository
            , INewStoryRepository newStoryRepository
            , IGetRawDataService getRawDataService
            , IApplicationSettingService applicationSettingService
            , IUploadImageService uploadImageService)
        {
            _cacheProvider = cacheProvider;
            _imageRepository = imageRepository;
            _newStoryRepository = newStoryRepository;
            _getRawDataService = getRawDataService;
            _applicationSettingService = applicationSettingService;
            _uploadImageService = uploadImageService;
        }

        public bool UploadImageLinkByChapLink(int imageId, string chapUrl)
        {
            try
            {
                var datas = _getRawDataService.GetImageDatasFromWeb(chapUrl);
                //Get data to cloudinay
                var imageCloudinays = new List<ImageData>();//
                foreach (var dataUrlImage in datas)
                {
                    var cloudUrl = _uploadImageService.UploadToCloud(dataUrlImage);
                    if (!String.IsNullOrEmpty(cloudUrl))
                    {
                        imageCloudinays.Add(new ImageData(cloudUrl));
                    }
                    else
                    {
                        imageCloudinays.Add(new ImageData(originLink: dataUrlImage, status: IMAGE_STATUS.ERROR));
                    }
                }
                //Update to DB
                var image = _imageRepository.GetById(imageId);
                var dataUpdate = new ImagesOneChap(image.StoryID, image.ChapID, imageCloudinays);
                dataUpdate.Id = image.Id;
                _imageRepository.Update(image.Id, dataUpdate);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UploadInvalidImageLink(ImageStoryInvalidData dataUpload)
        {

            foreach (var chap in dataUpload.Chaps)
            {
                if (chap.Images.Any())
                {
                    foreach (var image in chap.Images)
                    {
                        if (image.Status == IMAGE_STATUS.ERROR)
                        {
                            var cloudUrl = _uploadImageService.UploadToCloud(image.OriginLink);
                            if (!String.IsNullOrEmpty(cloudUrl))
                            {
                                image.Link = cloudUrl;
                                image.OriginLink = "";
                                image.Status = IMAGE_STATUS.OK;
                            }
                        }
                    }
                }
            }

            var chapNeedUpdates = dataUpload.Chaps;
            var idStory = dataUpload.ID;
            foreach (var chapNeedUpdate in chapNeedUpdates)
            {
                var imageData = _imageRepository.GetAll().Where(i => i.StoryID == idStory && i.ChapID == chapNeedUpdate.ChapID).First();
                var chapDataUpdate = new ImagesOneChap(idStory, chapNeedUpdate.ChapID, chapNeedUpdate.Images);
                chapDataUpdate.Id = imageData.Id;
                _imageRepository.Update(imageData.Id, chapDataUpdate);
            }
            return true;
        }

        public StoryInvalidData GetInvalidImageLink(int page = 0, int take = 50)
        {
            var dataInvalids = new List<ImageStoryInvalidData>();
            var allStoryIDs = GetAllStoryIds().Skip(page * take).Take(take).ToList();

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

                var chapDatas = _imageRepository.GetAll().Where(i => i.StoryID == storyID).ToArray();
                var chapIDs = chapDatas.Select(s => s.ChapID).ToList();
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

                    var chapData = chapDatas.Where(i => i.StoryID == storyID && i.ChapID == chapID).First();
                    var hasInvalidImage = false;
                    for (int index = 0; index < chapData.Images.Count; index++)
                    {
                        if (chapData.Images[index].Status == IMAGE_STATUS.ERROR)
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
                    //if (listChaps.Count >= limitNumberStoty) break;
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
                        var getChapOrigin = storyData.Chaps.Where(c => c.ID == lst.ChapID).First();
                        var myChapLink = storyData.Link + $"/{storyData.ID}/{lst.ChapID}";

                        var homeLinkWithSub = _applicationSettingService.GetValue(ApplicationSettingKey.HomePage) + _applicationSettingService.GetValue(ApplicationSettingKey.SubDataForHomePage);
                        var chapLink = homeLinkWithSub + getChapOrigin.Link;
                        var temp = new ImagesOneChapForUpdate(storyID, lst.ChapID, lst.Images, chapLink, myChapLink);
                        temp.Id = lst.Id;
                        lstFinal.Add(temp);
                    }
                    storyInValid.Chaps = lstFinal;
                    dataInvalids.Add(storyInValid);
                }
                else
                {
                    listIdsCheckNeedToCache.Add(storyID);
                    _cacheProvider.Set(key: CacheKeys.ScanGetData.ListStoryIDChecked, data: listIdsCheckNeedToCache, expiredTimeInSeconds: 600);
                }
            }
            //Save Data to table
            var totalPage = (GetAllStoryIds().Count / take) + (GetAllStoryIds().Count % take > 0 ? 1 : 0);
            return new StoryInvalidData()
            {
                ImageStoryInvalidDatas = dataInvalids,
                Page = page,
                Take = take,
                TotalPage = totalPage
            };
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
        public bool AddStatus(int skip = 0, int take = 1000)
        {
            var imageDatas = _imageRepository.GetAll().Skip(skip).Take(take).ToList();
            var imageNeedUpdates = new List<ImagesOneChap>();
            foreach (var imageData in imageDatas)
            {
                foreach (var image in imageData.Images)
                {
                    if (!String.IsNullOrEmpty(image.OriginLink))
                    {
                        image.Status = IMAGE_STATUS.ERROR;
                    }
                    else
                    {
                        image.Status = IMAGE_STATUS.OK;
                    }
                }
                imageNeedUpdates.Add(imageData);
            }
            if (imageNeedUpdates.Any())
            {
                foreach (var imageNeedUpdate in imageNeedUpdates)
                {
                    _imageRepository.Update(imageNeedUpdate.Id, imageNeedUpdate);
                }
            }
            return true;
        }
    }
}