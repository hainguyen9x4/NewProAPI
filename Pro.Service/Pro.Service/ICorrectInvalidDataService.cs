using Pro.Model;
using static Pro.Service.Implements.CorrectInvalidDataService;

namespace Pro.Service
{
    public interface ICorrectInvalidDataService
    {
        bool UploadImageLinkByChapLink(int imageId, string chapUrl);
        bool UploadInvalidImageLink(ImageStoryInvalidData dataUpload);
        StoryInvalidData GetInvalidImageLink(int page = 0, int take = 50);
        bool AddStatus(int skip = 0, int take = 1000);
        bool AddStatuByChap(int skip = 0, int take = 1000);
        List<ChapInvalideEmptyImgage> FindInvalidChap();
    }
}
