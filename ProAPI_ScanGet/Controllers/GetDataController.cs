using Microsoft.AspNetCore.Mvc;
using Pro.Model;
using Pro.Service;
using System.Collections.Generic;

namespace xStory.Controllers
{
    [ApiController]
    public class GetDataController : ControllerBase
    {
        private readonly IGetDataService _getDataService;

        public GetDataController(IGetDataService getDataService)
        {
            _getDataService = getDataService;
        }

        [Route("api/[controller]/StartGetData")]
        [HttpGet]
        public ActionResult<bool> StartGetData() =>
            _getDataService.StartGetDataForNewStory();

        [Route("api/[controller]/FindNewStory")]
        [HttpGet]
        public ActionResult<bool> FindNewStory(int numberPage, string homeUrl) =>
            _getDataService.FindNewStory(numberPage, homeUrl);
    }
}