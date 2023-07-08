using Pro.Common.Enum;
using Pro.Model;
using static Pro.Service.Implements.CorrectInvalidDataService;

namespace Pro.Service
{
    public interface ICorrectInvalidDataService
    {
        bool UploadImageLinkByChapLink(int imageId, string chapUrl);
        bool UploadInvalidImageLink(ImageStoryInvalidData dataUpload);
        StoryInvalidData GetInvalidImageLink(int page = 0, int take = 50);
        bool AddStatusToImagesInEachChap(int skip = 0, int take = 1000);
        bool AddStatuByChap(int skip = 0, int take = 1000);
        List<ChapInvalideEmptyImgage> FindInvalidChap();
        bool OnlyChangeFlagGetStatus(int storyId, int chapid, IMAGE_STATUS flagStatus = IMAGE_STATUS.OK);
    }
}
