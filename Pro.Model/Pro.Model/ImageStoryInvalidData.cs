namespace Pro.Model
{
    public class ImageStoryInvalidData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string NameShow { get; set; }
        public string Link { get; set; }
        public int StatusID { get; set; }
        public List<ImagesOneChap> Chaps { get; set; }
    }
}
