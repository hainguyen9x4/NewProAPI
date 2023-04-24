using Pro.Common;
using Pro.Model;

namespace Pro.Service.SubScanDataService
{
    public interface IUpData2DBService
    {
        void UpData2DB(DataStoryForSave dataStorys);
        void UpData2DBForNew(NewStory dataStorys);
    }
}
