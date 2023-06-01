using Pro.Model;

namespace Pro.Service
{
    public interface IStoryTypeService
    {
        bool CreateNewStoryType(StoryType type);
        List<StoryType> GetAllStoryType(int storyTypeID, string nameType);
    }
}
