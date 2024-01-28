using Microsoft.AspNetCore.Mvc;
using Pro.Common.Enum;
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
        private readonly IFileStoryService _fileStoryService;

        public ScanDataController(IScanDataService scanDataService
            , IStoryFollowsService storyFollowsService
            , IFileStoryService fileStoryService)
        {
            _scanDataService = scanDataService;
            _storyFollowsService = storyFollowsService;
            _fileStoryService = fileStoryService;
        }

        [HttpGet]
        [Route("StartScanData")]
        public ActionResult<bool> StartScanData() =>
            _scanDataService.StartScanData();

        [HttpGet]
        [Route("GetStoryFollows")]
        public ActionResult<List<StoryFollow>> GetStoryFollows(STATUS_FOLLOW status = STATUS_FOLLOW.ALL) =>
            _storyFollowsService.GetAllStoryFollows(status);

        [HttpPost]
        [Route("UpdateStoryFollows")]
        public ActionResult<bool> UpdateStoryFollows(int id, string link, STATUS_FOLLOW status) =>
            _storyFollowsService.UpdateStoryFollows(id, status, link);

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
        //
        [HttpGet]
        [Route("GetAllFileStory")]
        public ActionResult<List<FileStory>> GetAllFileStory() =>
            _fileStoryService.GetAllFileStory();

        [HttpPost]
        [Route("UpdateFileStory")]
        public ActionResult<bool> UpdateFileStory(int id, int chapStoredNewest) =>
            _fileStoryService.UpdateFileStory(id, chapStoredNewest);

        [HttpPost]
        [Route("DeleteAFileStory")]
        public ActionResult<bool> DeleteAFileStory(int id) =>
            _fileStoryService.DeleteAFileStory(id);

        [HttpPost]
        [Route("AddTableFileStory")]
        public ActionResult<bool> AddTableFileStory(List<ModelAddNewFileStory> datas) =>
            _fileStoryService.AddTableFileStory(datas);

        [HttpPost]
        [Route("AddFileStory")]
        public ActionResult<ResultAddNewFileStory> AddFileStory(ModelAddNewFileStory data) =>
            _fileStoryService.AddFileStory(data);
    }
}