using Pro.Common;

namespace Pro.Service.SubScanDataService
{
    public interface IGetRawDataService
    {
        DataStoryForSave GetRawDatas(NewestChapModel newestDatas);
        bool FindNewStory(int numberPage, string homeUrl);
    }
}
