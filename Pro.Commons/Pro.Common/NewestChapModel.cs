namespace Pro.Common
{
    public class NewestChapModel
    {
        public NewestChapModel()
        {
            ChapLinks = new List<string>();
        }
        public string StoryName { get; set; }
        public string StoryLink { get; set; }
        public string StoryNameShow { get; set; }
        public string Author { get; set; }
        public List<string> ChapLinks { get; set; }
        public FileInfo FileDataNewestPathLocal { get; set; }
        public string Description { get; set; }
        public List<StoryTypeModel> StoryTypes { get; set; }
    }

    public class DataStoryForSave
    {
        public DataStoryForSave()
        {
            ChapDataForSaves = new List<ChapDataForSave>();
        }
        public string StoryName { get; set; }
        public string StoryNameShow { get; set; }
        public string StoryLink { get; set; }
        public string StoryPictureLink { get; set; }
        public string Author { get; set; }
        public List<ChapDataForSave> ChapDataForSaves { get; set; }
        public FileInfo FileDataNewestPathLocal { get; set; }
    }
    public class ChapDataForSave
    {
        public ChapDataForSave()
        {
            ImageDatas = new List<ImagePairLink>();
        }
        public string ChapId { get; set; }
        public string ChapName { get; set; }
        public string ChapLink { get; set; }
        public List<ImagePairLink> ImageDatas { get; set; }
    }
    public class ImagePairLink
    {
        public string ImageLinkNeedSaveDB { get; set; }
        public string ImageLinkFromWeb { get; set; }
    }
}
