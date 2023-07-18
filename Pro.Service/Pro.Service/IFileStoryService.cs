using Pro.Model;

namespace Pro.Service
{
    public interface IFileStoryService
    {
        List<FileStory> GetAllFileStory();
        bool UpdateAllFileStory(List<FileStory> datas);
        bool UpdateFileStory(int id, int chapStoredNewest);
        bool AddTableFileStory(List<ModelAddNewFileStory> datas);
        bool DeleteAFileStory(int id);
        ResultAddNewFileStory AddFileStory(ModelAddNewFileStory datas);
    }
}
