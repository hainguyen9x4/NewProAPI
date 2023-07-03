using Pro.Model;

namespace Pro.Service
{
    public interface IGetDataService
    {
        bool StartGetDataForNewStory();
        bool FindNewStory(int numberPage, string homeUrl);
        List<ImageStoryInvalidData> GetInvalidImageLink(int limitNumberStoty = 5);
    }
}
