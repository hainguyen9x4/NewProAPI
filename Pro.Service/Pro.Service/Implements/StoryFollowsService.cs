using Pro.Common;
using Pro.Common.Enum;
using Pro.Data.Repositorys;
using Pro.Model;
using Pro.Service.Caching;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pro.Service.Implements
{
    public class StoryFollowsService : IStoryFollowsService
    {
        private readonly IStoryFollowsRepository _storyFollowsRepository;
        private readonly ICacheProvider _cacheProvider;
        public StoryFollowsService(
            IStoryFollowsRepository storyFollowsRepository
            , ICacheProvider cacheProvider)
        {
            _storyFollowsRepository = storyFollowsRepository;
            _cacheProvider = cacheProvider;
        }
        public List<StoryFollow> GetAllStoryFollows(STATUS_FOLLOW status, bool useCache = true)
        {
            switch (status)
            {
                case STATUS_FOLLOW.DISABLE:
                    return _storyFollowsRepository.GetAll().Where(s => s.Status == status).OrderBy(s => s.Id).ToList();
                case STATUS_FOLLOW.ALL:
                    return _storyFollowsRepository.GetAll().OrderBy(s => s.Id).ToList();
                case STATUS_FOLLOW.ENABLE:
                    try
                    {
                        Func<List<StoryFollow>> fetchFunc = () =>
                        {
                            return _storyFollowsRepository.GetAll().Where(s => s.Status == STATUS_FOLLOW.ENABLE).OrderBy(s => s.Id).ToList();
                        };

                        return useCache ? _cacheProvider.Get(CacheKeys.GetCacheKey(CacheKeys.ScanGetData.ListStoryFollows), fetchFunc) : fetchFunc();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error($"Error when get the lst follow story", ex);
                        return new List<StoryFollow>();
                    }
                default:
                    return new List<StoryFollow>();
            }
        }
        public bool UpdateStoryFollows(int id, STATUS_FOLLOW status, string link = "")
        {
            try
            {
                var storyFollow = _storyFollowsRepository.GetAll().Where(s => s.Id == id).First();
                if (!String.IsNullOrEmpty(link)) storyFollow.Link = link;
                storyFollow.Status = status;
                _storyFollowsRepository.Update(id, storyFollow);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public class ResultAddNewStory
        {
            public int Result { get; set; }
            public string Message { get; set; }
        }
        public ResultAddNewStory AddStoryFollows(string link)
        {
            var rs = new ResultAddNewStory();
            try
            {
                var tempLink = RemoveNumberAtEnd(link);
                var storyFollows = _storyFollowsRepository.GetAll().ToList();
                var storyFollowSames = new List<StoryFollow>();
                foreach (var storyFollow in storyFollows)
                {
                    if (RemoveNumberAtEnd(storyFollow.Link).Contains(tempLink))
                    {
                        storyFollowSames.Add(storyFollow);
                    }
                }
                if (storyFollowSames.Any())
                {
                    rs.Result = -1;
                    var lstStrings = new List<string>();
                    foreach (var storyFollow in storyFollowSames)
                    {
                        lstStrings.Add($"ID: {storyFollow.Id}: {storyFollow.Link}");
                    }
                    rs.Message = String.Join("||", lstStrings);
                }
                else
                {
                    rs.Result = 0;
                    _storyFollowsRepository.Create(new StoryFollow(link, 0));
                }
            }
            catch (Exception ex)
            {
                return new ResultAddNewStory() { Result = -1, Message = ex.Message };
            }
            return rs;
        }
        static string RemoveNumberAtEnd(string input)
        {
            string pattern = @"\d+$"; // Matches one or more digits at the end of the string
            string replacement = ""; // Replaces the matched number with an empty string

            string result = Regex.Replace(input, pattern, replacement);
            return result;
        }
        public bool DeleteStoryFollows(int id)
        {
            try
            {
                _storyFollowsRepository.Delete(id);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public bool AddTableStoryFollows(List<string> links)
        {
            try
            {
                var storyFollows = new List<StoryFollow>();
                foreach (var link in links)
                {
                    storyFollows.Add(new StoryFollow(link, 0));
                }
                _storyFollowsRepository.Creates(storyFollows);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}