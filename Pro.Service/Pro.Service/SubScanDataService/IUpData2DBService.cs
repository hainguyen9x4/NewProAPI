using Pro.Model;

namespace Pro.Service.SubScanDataService
{
    public interface IUpData2DBService
    {
        void UpData2DBForNew(NewStory dataStorys);
        List<ImageStoryInvalidData> GetDataInvalid(int limitNumberStoty = 5);
    }
}
