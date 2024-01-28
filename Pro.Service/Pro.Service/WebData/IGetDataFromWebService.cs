using Pro.Model;

namespace Pro.Service.WebData
{
    public interface IGetDataFromWebService
    {
        public StoryInfoWithChaps GetStoryInfoWithChaps(string textUrl);

    }
}
