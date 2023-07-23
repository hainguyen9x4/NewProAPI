using Microsoft.AspNetCore.Mvc;
using Pro.Common.Enum;
using Pro.Model;
using Pro.Service;
using System.Collections.Generic;
using System.IO;
using static Pro.Service.Implements.StorysService;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace xStory.Controllers
{
    [ApiController]
    public class StorysController : ControllerBase
    {
        private readonly IStorysService _storyService;

        public StorysController(IStorysService storysService)
        {
            _storyService = storysService;
        }

        [Route("api/[controller]/GetTopHotStorys")]
        [HttpGet]
        public ActionResult<List<ImageStoryInfo>> GetTopHotStorys() =>
                            _storyService.GetTopHotStorysForNew();

        [Route("api/[controller]/GetHomeStorys")]
        [HttpGet]
        public ActionResult<HomePageInfo> GetHomeStorys([FromQuery] int pageIndex, int storyPerPage) =>
                    _storyService.GetHomeStoryForNews(pageIndex, storyPerPage);

        [Route("api/[controller]/GetAllChapByStoryId")]
        [HttpGet]
        public ActionResult<ImageStoryInfo> GetAllChapByStoryId([FromQuery] int storyID) =>
            _storyService.GetAllChapByStoryIdForNew(storyID);

        [Route("api/[controller]/GetImageStorysInChap")]
        [HttpGet]
        public ActionResult<ChapInfo> GetImageStorysInChap([FromQuery] int storyID, int ChapId) =>
            _storyService.GetImageStorysInChapForNew(storyID, ChapId);

        [Route("api/[controller]/GetAllStoryForSearch")]
        [HttpGet]
        public ActionResult<List<ImageStoryInfo>> GetAllStoryForSearch() =>
            _storyService.GetAllStoryForSearchForNew();

        [Route("api/[controller]/GetAllStoryByTypeName")]
        [HttpGet]
        public ActionResult<TempGetAllStoryByTypeName> GetAllStoryByTypeName([FromQuery] string nameType, int pageIndex, int storyPerPage) =>
            _storyService.GetAllStoryByTypeName(nameType, pageIndex, storyPerPage);

        [Route("api/[controller]/RateStory")]
        [HttpGet]
        public ActionResult<TempGetAllStoryByTypeName> RateStory([FromQuery] RATE_TYPE rateType, int pageIndex = 0, int dataPerPage = 16) =>
            _storyService.RateStory(rateType, pageIndex, dataPerPage);

        public class TempObject
        {
            public List<int> TypeIDs { get; set; }
            public int PageIndex { get; set; }
            public int StoryPerPage { get; set; }
        }
        [Route("api/[controller]/GetAllStoryByTypeIDs")]
        [HttpPost]
        public ActionResult<TempGetAllStoryByTypeName> GetAllStoryByTypeIDs(TempObject data) =>
            _storyService.GetAllStoryByTypeIDs(data.TypeIDs, data.PageIndex, data.StoryPerPage);

        [Route("api/[controller]/GetAllStory")]
        [HttpGet]
        public ActionResult<bool> GetAllStory([FromQuery] int call = 0)
        {
            if (_storyService.GetAllStory(call).Count > 0) return true;
            return false;
        }
    }
}