using System.Text;

namespace Pro.Service.Implements
{
    public class LogService : ILogService
    {
        List<string> ILogService.GetLogFiles(string folderLog)
        {
            var results = new List<string>();
            DirectoryInfo d = new DirectoryInfo(folderLog); //Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files
            foreach (FileInfo file in Files)
            {
                results.Add(file.FullName);
            }
            results = results.OrderByDescending(s => s).ToList();
            return results;
        }

        string ILogService.GetLogInfo(string fullPathFile)
        {
            if (!string.IsNullOrEmpty(fullPathFile))
            {
                string readText = File.ReadAllText(fullPathFile, Encoding.UTF8);
                return readText;
            }
            return "";
        }

        public LogService()
        {

        }
    }
}