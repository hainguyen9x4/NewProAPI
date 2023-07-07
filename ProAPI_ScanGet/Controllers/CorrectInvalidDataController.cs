using Microsoft.AspNetCore.Mvc;
using Pro.Model;
using Pro.Service;
using Pro.Service.SubScanDataService;
using System.Collections.Generic;

namespace xStory.Controllers
{
    [ApiController]
    public class CorrectInvalidDataController : ControllerBase
    {
        private readonly IUpData2DBService _upData2DBService;
        private readonly ICorrectInvalidDataService _correctInvalidDataService;
        public CorrectInvalidDataController(IUpData2DBService upData2DBService
            , ICorrectInvalidDataService correctInvalidDataService)
        {
            _upData2DBService = upData2DBService;
            _correctInvalidDataService = correctInvalidDataService;
        }

        [Route("api/[controller]/UploadImageLinkByChapLink")]
        [HttpPost]
        public ActionResult<bool> UploadImageLinkByChapLink(int imageId, string chapUrl) =>
            _correctInvalidDataService.UploadImageLinkByChapLink(imageId, chapUrl);

        [Route("api/[controller]/UploadInvalidImageLink")]
        [HttpPost]
        public ActionResult<bool> UploadInvalidImageLink(ImageStoryInvalidData dataUpload) =>
            _correctInvalidDataService.UploadInvalidImageLink(dataUpload);

        [Route("api/[controller]/GetInvalidImageLink")]
        [HttpGet]
        public ActionResult<StoryInvalidData> GetInvalidImageLink(int page = 0, int take = 50) =>
            _correctInvalidDataService.GetInvalidImageLink(page, take);

        [Route("api/[controller]/AddStatus")]
        [HttpPost]
        public ActionResult<bool> AddStatus(int skip = 0, int take = 1000) =>
            _correctInvalidDataService.AddStatus(skip, take);
    }
}
