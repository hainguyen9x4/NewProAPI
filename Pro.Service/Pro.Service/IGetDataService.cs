namespace Pro.Service
{
    public interface IGetDataService
    {
        bool StartGetData();
        bool FindNewStory(int numberPage, string homeUrl);
    }
}
