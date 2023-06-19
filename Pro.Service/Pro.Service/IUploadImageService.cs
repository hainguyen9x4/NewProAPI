using Pro.Common;
using Pro.Model;

namespace Pro.Service
{
    public interface IUploadImageService
    {
        void UploadLink2StoreWith3ThreadsForNew(NewStory dataStory);
        bool HasValidCloudinary();
    }
}
