using Pro.Model;

namespace Pro.Service
{
    public interface IStoryTypeService
    {
        bool CreateNewStoryType(StoryType type);
        List<StoryType> GetAllStoryTypebyID(int storyTypeID, string nameType);
        List<StoryType> GetAllStoryType(bool useCache = true);
    }
}
