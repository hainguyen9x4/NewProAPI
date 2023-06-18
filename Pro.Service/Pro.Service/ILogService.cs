namespace Pro.Service
{
    public interface ILogService
    {
        public List<string> GetLogFiles(string folderLog);
        public string GetLogInfo(string fullPathFile);
    }
}
