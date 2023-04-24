using Pro.Common;
using Pro.Model;

namespace Pro.Service.SubScanDataService
{
    public interface IGetRawDataService
    {
        DataStoryForSave GetRawDatas(NewestChapModel newestDatas);
        DataStoryForSave GetRawDatasForNew(NewStory newestDatas);
        bool FindNewStory(int numberPage, string homeUrl);
    }
}
