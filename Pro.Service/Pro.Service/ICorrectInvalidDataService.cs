using Pro.Model;

namespace Pro.Service
{
    public interface ICorrectInvalidDataService
    {
        bool UploadImageLinkByChapLink(int imageId, string chapUrl);
        bool UploadInvalidImageLink(List<ImageStoryInvalidData> dataUploads);
        List<ImageStoryInvalidData> GetInvalidImageLink(int skip = 0, int take = 50);
        bool AddStatus(int skip = 0, int take = 1000);
    }
}
