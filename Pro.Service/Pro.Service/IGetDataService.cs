namespace Pro.Service
{
    public interface IGetDataService
    {
        bool StartGetData();
        bool StartGetDataForNewStory();
        bool FindNewStory(int numberPage, string homeUrl);
    }
}
