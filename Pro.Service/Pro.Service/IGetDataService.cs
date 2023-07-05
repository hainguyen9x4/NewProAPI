namespace Pro.Service
{
    public interface IGetDataService
    {
        bool StartGetDataForNewStory();
        bool FindNewStory(int numberPage, string homeUrl);
    }
}
