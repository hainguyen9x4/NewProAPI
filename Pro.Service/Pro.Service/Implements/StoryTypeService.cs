using Pro.Data.Repositorys;
using Pro.Model;

namespace Pro.Service.Implements
{
    public class StoryTypeService : IStoryTypeService
    {
        private readonly IStoryTypeRepository _storyTypesitory;

        public StoryTypeService(IStoryTypeRepository storyTypesitory)
        {
            _storyTypesitory = storyTypesitory;
        }

        public bool CreateNewStoryType(StoryType type)
        {
            try
            {
                _storyTypesitory.Create(type);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<StoryType> GetAllStoryType(int storyTypeID, string nameType)
        {
            if (!String.IsNullOrEmpty(nameType))
            {
                return _storyTypesitory.GetAll().Where(t => t.Name == nameType).ToList();
            }
            else
            {
                return _storyTypesitory.GetAll().Where(t => t.TypeID == storyTypeID).ToList();
            }
        }
    }
}