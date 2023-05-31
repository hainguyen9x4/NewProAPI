using Pro.Common;
using Pro.Model;

namespace Pro.Service.SubScanDataService
{
    public interface IGetRawDataService
    {
        void GetRawDatasForNew(NewStory newestDatas);
        bool FindNewStory(int numberPage, string homeUrl);
    }
}
