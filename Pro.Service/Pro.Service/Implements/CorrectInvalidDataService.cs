using Pro.Data.Repositorys;
using Pro.Model;
using Pro.Service.Caching;
using Pro.Service.SubScanDataService;

namespace Pro.Service.Implements
{
    public class CorrectInvalidDataService : ICorrectInvalidDataService
    {
        private readonly IImageRepository _imageRepository;
        private readonly ICacheProvider _cacheProvider;
        private readonly IGetRawDataService _getRawDataService;
        private readonly IUploadImageService _uploadImageService;

        public CorrectInvalidDataService(ICacheProvider cacheProvider
            , IImageRepository imageRepository
            , IGetRawDataService getRawDataService
            , IUploadImageService uploadImageService)
        {
            _cacheProvider = cacheProvider;
            _imageRepository = imageRepository;
            _getRawDataService = getRawDataService;
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
                        imageCloudinays.Add(new ImageData(originLink: dataUrlImage));
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

        public bool UploadInvalidImageLink(List<ImageStoryInvalidData> dataUploads)
        {
            foreach (var dataUpload in dataUploads)
            {
                foreach (var chap in dataUpload.Chaps)
                {
                    if (chap.Images.Any())
                    {
                        foreach (var image in chap.Images)
                        {
                            if (!String.IsNullOrEmpty(image.OriginLink) && String.IsNullOrEmpty(image.Link))
                            {
                                var cloudUrl = _uploadImageService.UploadToCloud(image.OriginLink);
                                if (!String.IsNullOrEmpty(cloudUrl))
                                {
                                    image.Link = cloudUrl;
                                    image.OriginLink = "";
                                }
                                else
                                {
                                }
                            }
                        }
                    }
                }
            }


            var idStorys = dataUploads.Select(story => story.ID).ToList();
            foreach (var idStory in idStorys)
            {
                var chapNeedUpdates = dataUploads.Where(d => d.ID == idStory).First().Chaps;
                foreach (var chapNeedUpdate in chapNeedUpdates)
                {
                    var imageData = _imageRepository.GetAll().Where(i => i.StoryID == idStory && i.ChapID == chapNeedUpdate.ChapID).First();
                    var chapDataUpdate = new ImagesOneChap(idStory, chapNeedUpdate.ChapID, chapNeedUpdate.Images);
                    chapDataUpdate.Id = imageData.Id;
                    _imageRepository.Update(imageData.Id, chapDataUpdate);
                }
            }
            return true;
        }
    }
}