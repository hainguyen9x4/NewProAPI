using Pro.Data.Repositorys;
using Pro.Model;

namespace Pro.Service.Implements
{
    public class FileStoryService : IFileStoryService
    {
        private readonly IFileStoryRepository _fileStoryRepository;

        public FileStoryService(
            IFileStoryRepository fileStoryRepository)
        {
            _fileStoryRepository = fileStoryRepository;
        }
        public List<FileStory> GetAllFileStory()
        {
            return _fileStoryRepository.GetAll().OrderBy(s => s.Id).ToList();
        }

        public bool UpdateAllFileStory(List<FileStory> datas)
        {
            try
            {
                foreach (var data in datas)
                {
                    if (data.Id != 0)
                    {
                        _fileStoryRepository.Update(data.Id, data);
                    }
                    else//new
                    {
                        _fileStoryRepository.Create(data);
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool UpdateFileStory(int id, int chapStoredNewest)
        {
            try
            {
                var fileStory = _fileStoryRepository.GetAll().Where(s => s.Id == id).First();
                fileStory.ChapStoredNewest = chapStoredNewest;
                _fileStoryRepository.Update(id, fileStory);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public ResultAddNewFileStory AddFileStory(ModelAddNewFileStory data)
        {
            var rs = new ResultAddNewFileStory();
            try
            {
                var fileStorys = _fileStoryRepository.GetAll().ToList();
                var fileStorySames = new List<FileStory>();

                foreach (var fileStory in fileStorys)
                {
                    if (fileStory.StoryName.Equals(data.StoryName))
                    {
                        fileStorySames.Add(fileStory);
                    }
                }

                if (fileStorySames.Any())
                {
                    rs.Result = -1;
                    var lstStrings = new List<string>();
                    foreach (var fileStory2 in fileStorySames)
                    {
                        lstStrings.Add($"ID: {fileStory2.Id}: {fileStory2.StoryName}");
                    }
                    rs.Message = String.Join("||", lstStrings);
                }
                else
                {
                    rs.Result = 0;
                    _fileStoryRepository.Create(new FileStory(data.StoryName, data.ChapStoredNewest));
                }
            }
            catch (Exception ex)
            {
                return new ResultAddNewFileStory() { Result = -1, Message = ex.Message };
            }
            return rs;
        }

        public bool DeleteAFileStory(int id)
        {
            try
            {
                _fileStoryRepository.Delete(id);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool AddTableFileStory(List<ModelAddNewFileStory> datas)
        {
            try
            {
                var fileStory = new List<FileStory>();
                foreach (var data in datas)
                {
                    fileStory.Add(new FileStory(data.StoryName, data.ChapStoredNewest));
                }
                _fileStoryRepository.Creates(fileStory);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}