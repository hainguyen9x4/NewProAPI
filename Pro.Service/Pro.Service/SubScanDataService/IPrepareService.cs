using Pro.Common;
using Pro.Model;

namespace Pro.Service.SubScanDataService
{
    public interface IPrepareService
    {
        NewestChapModel PrepareNewestChapDatas();
        NewStory PrepareNewestChapDatasForNew();
    }
}
