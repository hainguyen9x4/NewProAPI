namespace Pro.Model
{
    public class ImageStoryInvalidData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string NameShow { get; set; }
        public string Link { get; set; }
        public int StatusID { get; set; }
        public List<ImagesOneChapForUpdate> Chaps { get; set; }
    }
    public class StoryInvalidData
    {
        public int Take { get; set; }
        public int Page { get; set; }
        public int TotalPage { get; set; }
        public List<ImageStoryInvalidData> ImageStoryInvalidDatas { get; set; }
    }
}
