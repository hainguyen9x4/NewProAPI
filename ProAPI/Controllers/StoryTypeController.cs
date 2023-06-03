using Microsoft.AspNetCore.Mvc;
using Pro.Model;
using Pro.Service;
using System.Collections.Generic;
using System.Security.AccessControl;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace xStory.Controllers
{
    [ApiController]
    public class StoryTypeController : ControllerBase
    {
        private readonly IStoryTypeService _storyTypeService;

        public StoryTypeController(IStoryTypeService storysService)
        {
            _storyTypeService = storysService;
        }

        [Route("api/[controller]/CreateNewStoryType")]
        [HttpPost]
        public ActionResult<bool> CreateNewStoryType(StoryType type) =>
                            _storyTypeService.CreateNewStoryType(type);

        [Route("api/[controller]/GetAllStoryType")]
        [HttpGet]
        public ActionResult<List<StoryType>> GetAllStoryType([FromQuery] int id, string nameType = "") =>
                    _storyTypeService.GetAllStoryTypebyID(id, nameType);
    }
}