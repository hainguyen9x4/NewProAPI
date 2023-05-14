using FileManager;
using Pro.Common;
using Pro.Data.Repositorys;
using Pro.Data.Repositorys.Implements;
using Pro.Model;

namespace Pro.Service.SubScanDataService.Implements
{
    public class UpData2DBService : IUpData2DBService
    {
        private readonly IStoryRepository _storyRepository;
        private readonly IChapRepository _chapRepository;
        private readonly IImageRepository _imageRepository;
        private readonly INewStoryRepository _newStoryRepository;

        public UpData2DBService(IStoryRepository storys
            , IChapRepository chaps
            , IImageRepository image
            , INewStoryRepository newStory)
        {
            _storyRepository = storys;
            _chapRepository = chaps;
            _imageRepository = image;
            _newStoryRepository = newStory;
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

        private NewStory GetStoryIdFromStoryNameForNew(NewStory dataStoryForSave)
        {
            try
            {
                var story = _newStoryRepository.GetAll().Where(s => s.Name.Equals(dataStoryForSave.Name)).FirstOrDefault();
                if (story != null)
                {
                    return story;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error($"GetStoryIdFromStoryName {dataStoryForSave.Name}" + ex);
                return null;
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

        public void UpData2DBForNew(NewStory dataStory)
        {
            var story = GetStoryIdFromStoryNameForNew(dataStory);

            if (story != null)
            {
                //Old story
                UpdateChapId(dataStory, story.Chaps.Count + 1);

                var imagesOnChap = new List<ImagesOneChap>();
                foreach (var chapSaveData in dataStory.Chaps)
                {
                    imagesOnChap.Add(new ImagesOneChap(story.ID, chapSaveData.ID, chapSaveData.Images));
                    chapSaveData.Images = new List<Model.ImageData>();
                }
                _imageRepository.Creates(imagesOnChap);

                LogHelper.Info($"UpData2DBForNew: _1");

                story.Chaps.AddRange(dataStory.Chaps);
                _newStoryRepository.Update(story.ID, dataStory);

            }
            else
            {
                dataStory.Link = FileReader.DeleteHomePage(dataStory.Link);
                //new Story
                FakeDataOtherInfo(dataStory);
                UpdateChapId(dataStory);

                var imagesOnChap = new List<ImagesOneChap>();
                foreach (var chapSaveData in dataStory.Chaps)
                {
                    imagesOnChap.Add(new ImagesOneChap(0, chapSaveData.ID, chapSaveData.Images));
                    chapSaveData.Images = new List<Model.ImageData>();
                }

                var newStory = _newStoryRepository.Create(dataStory);

                foreach (var c in imagesOnChap)
                {
                    c.StoryID = newStory.ID;
                }
                _imageRepository.Creates(imagesOnChap);
            }
        }

        private void FakeDataOtherInfo(NewStory dataStory)
        {
            dataStory.OtherInfo = new OtherInfo(new Star(4.5, RandomRate(3000, 9000)), new List<StoryType>() { }, "", "", RandomRate(80000, 99000), RandomRate(80000, 99000));

            int RandomRate(int start, int end)
            {
                var random = new Random();
                return random.Next(start, end);
            }
        }

        private void UpdateChapId(NewStory dataStory, int startId = 1)
        {
            foreach (var chap in dataStory.Chaps)
            {
                chap.ID = startId;
                startId++;
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
