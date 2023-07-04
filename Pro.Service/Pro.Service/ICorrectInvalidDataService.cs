namespace Pro.Service
{
    public interface ICorrectInvalidDataService
    {
        bool UploadImageLinkByChapLink(int imageId, string chapUrl);
    }
}
