using Pro.Model;

namespace Pro.Service
{
    public interface ICorrectInvalidDataService
    {
        bool UploadImageLinkByChapLink(int imageId, string chapUrl);
        bool UploadInvalidImageLink(List<ImageStoryInvalidData> dataUploads);
        List<ImageStoryInvalidData> GetInvalidImageLink(int limitNumberStoty = 2);
    }
}
