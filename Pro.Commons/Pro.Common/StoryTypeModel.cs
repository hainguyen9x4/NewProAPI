namespace Pro.Common
{
    public class StoryTypeModel
    {
        public string Name { get; set; }
        public string NameShow { get; set; }
        public string Des { get; set; }
        public StoryTypeModel() { }
        public StoryTypeModel(string name, string nameShow, string des = "")
        {
            Name = name;
            NameShow = nameShow;
            Des = des;
        }
    }
}
