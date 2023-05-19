using Microsoft.AspNetCore.Mvc;
using Pro.Model;
using Pro.Service;
using System.Collections.Generic;
using System.IO;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace xStory.Controllers
{
    [ApiController]
    public class StorysController : ControllerBase
    {
        private readonly IStorysService _storyService;

        public StorysController(IStorysService StorysService)
        {
            _storyService = StorysService;
        }

        [Route("api/[controller]/GetTopHotStorys")]
        [HttpGet]
        public ActionResult<List<ImageStoryInfo>> GetTopHotStorys() =>
                            _storyService.GetTopHotStorysForNew();

        [Route("api/[controller]/GetHomeStorys")]
        [HttpGet]
        public ActionResult<HomePageInfo> GetHomeStorys([FromQuery]int pageIndex, int storyPerPage) =>
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
    }
}