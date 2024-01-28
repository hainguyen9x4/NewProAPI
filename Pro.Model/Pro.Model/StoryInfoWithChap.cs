using Pro.Common;

namespace Pro.Model
{
    public class StoryInfoWithChaps
    {
        public List<ChapPlus> ChapPluss { get; set; }
        public string StoryName { get; set; }
        public string Description { get; set; }
        public List<string> StoryTypes { get; set; }
    }
}
