using Pro.Common;

namespace Pro.Service.SubScanDataService
{
    public interface IUpData2DBService
    {
        void UpData2DB(DataStoryForSave dataStorys);
        void UpData2DBForNew(DataStoryForSave dataStorys);
    }
}
