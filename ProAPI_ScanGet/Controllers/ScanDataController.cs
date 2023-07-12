using Microsoft.AspNetCore.Mvc;
using Pro.Model;
using Pro.Service;
using System.Collections.Generic;
using static Pro.Service.Implements.StoryFollowsService;

namespace xStory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanDataController : ControllerBase
    {
        private readonly IScanDataService _scanDataService;
        private readonly IStoryFollowsService _storyFollowsService;

        public ScanDataController(IScanDataService scanDataService,
            IStoryFollowsService storyFollowsService)
        {
            _scanDataService = scanDataService;
            _storyFollowsService = storyFollowsService;
        }

        [HttpGet]
        [Route("StartScanData")]
        public ActionResult<bool> StartScanData() =>
            _scanDataService.StartScanData();

        [HttpGet]
        [Route("GetStoryFollows")]
        public ActionResult<List<StoryFollow>> GetStoryFollows() =>
            _storyFollowsService.GetAllStoryFollows();

        [HttpPost]
        [Route("UpdateStoryFollows")]
        public ActionResult<bool> UpdateStoryFollows(int id, string link) =>
            _storyFollowsService.UpdateStoryFollows(id, link);

        [HttpPost]
        [Route("DeleteStoryFollows")]
        public ActionResult<bool> DeleteStoryFollows(int id) =>
            _storyFollowsService.DeleteStoryFollows(id);

        [HttpPost]
        [Route("AddTableStoryFollows")]
        public ActionResult<bool> AddTableStoryFollows(List<string> links) =>
            _storyFollowsService.AddTableStoryFollows(links);

        [HttpPost]
        [Route("AddStoryFollows")]
        public ActionResult<ResultAddNewStory> AddStoryFollows(string link) =>
            _storyFollowsService.AddStoryFollows(link);
    }
}