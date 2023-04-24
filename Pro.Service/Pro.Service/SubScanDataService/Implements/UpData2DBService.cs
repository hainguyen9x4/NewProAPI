using FileManager;
using Pro.Common;
using Pro.Data.Repositorys;
using Pro.Model;

namespace Pro.Service.SubScanDataService.Implements
{
    public class UpData2DBService : IUpData2DBService
    {
        private readonly IStoryRepository _storyRepository;
        private readonly IChapRepository _chapRepository;

        public UpData2DBService(IStoryRepository storys
            , IChapRepository chaps)
        {
            _storyRepository = storys;
            _chapRepository = chaps;
        }

        private int GetStoryIdFromStoryName(DataStoryForSave dataStoryForSave)
        {
            try
            {
                var story = _storyRepository.GetAll().Where(s => s.StoryName.Equals(dataStoryForSave.StoryName)).FirstOrDefault();
                if (story != null)
                {
                    return story.StoryId;
                }
                else
                {
                    //New story-> insert new story
                    dataStoryForSave.StoryLink = FileReader.DeleteHomePage(dataStoryForSave.StoryLink);
                    var newStory = new Story(dataStoryForSave.StoryName, 1, 0, dataStoryForSave.StoryLink, dataStoryForSave.Author, dataStoryForSave.StoryPictureLink, dataStoryForSave.StoryNameShow);
                    _storyRepository.Create(newStory);
                    return newStory.StoryId;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"GetStoryIdFromStoryName {dataStoryForSave.StoryName}" + ex);
                return 0;
            }
        }

        public void UpData2DB(DataStoryForSave dataStory)
        {
            var storyId = GetStoryIdFromStoryName(dataStory);
            if (storyId > 0)
            {
                foreach (var chapSaveData in dataStory.ChapDataForSaves)
                {
                    var chap = _chapRepository.GetAll().Where(c => c.ChapName == chapSaveData.ChapName && c.StoryId == storyId).FirstOrDefault();
                    if (chap == null)
                    {
                        //Create chap with all info
                        var chapId = CreateChapWithAllInfo(storyId, storyId, chapSaveData);
                    }
                }
            }
            else
            {
                LogHelper.Error($"UpData2DB: Can't get storyId, StoryLink:{dataStory.StoryLink}");
            }
        }

        public void UpData2DBForNew(DataStoryForSave dataStory)
        {
            var storyId = GetStoryIdFromStoryName(dataStory);
            if (storyId > 0)
            {
                foreach (var chapSaveData in dataStory.ChapDataForSaves)
                {
                    var chap = _chapRepository.GetAll().Where(c => c.ChapName == chapSaveData.ChapName && c.StoryId == storyId).FirstOrDefault();
                    if (chap == null)
                    {
                        //Create chap with all info
                        var chapId = CreateChapWithAllInfo(storyId, storyId, chapSaveData);
                    }
                }
            }
            else
            {
                LogHelper.Error($"UpData2DB: Can't get storyId, StoryLink:{dataStory.StoryLink}");
            }
        }

        private int CreateChapWithAllInfo(int storyID, int storyIdx, ChapDataForSave chapSaveData)
        {
            chapSaveData.ChapLink = FileReader.DeleteHomePage(chapSaveData.ChapLink);
            var images = new List<ImagesChap>();
            chapSaveData.ImageDatas.ForEach(data =>
                images.Add(new ImagesChap()
                {
                    ImageSavedLink = data.ImageLinkNeedSaveDB,
                    ImageWebLink = !String.IsNullOrEmpty(data.ImageLinkNeedSaveDB) ? null : data.ImageLinkFromWeb,
                }));
            var newChap = new OldChap(storyID, storyIdx, chapSaveData.ChapName, images, 1, chapSaveData.ChapLink);
            _chapRepository.Create(newChap);
            return newChap.ChapId;
        }
    }
}
