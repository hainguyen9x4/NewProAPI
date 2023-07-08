using Microsoft.AspNetCore.Mvc;
using Pro.Common.Enum;
using Pro.Model;
using Pro.Service;
using Pro.Service.SubScanDataService;
using System.Collections.Generic;
using static Pro.Service.Implements.CorrectInvalidDataService;

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
            _correctInvalidDataService.AddStatusToImagesInEachChap(skip, take);

        [Route("api/[controller]/AddStatuByChap")]
        [HttpPost]
        public ActionResult<bool> AddStatuByChap(int skip = 0, int take = 1000) =>
            _correctInvalidDataService.AddStatuByChap(skip, take);

        [Route("api/[controller]/FindInvalidChap")]
        [HttpPost]
        public ActionResult<List<ChapInvalideEmptyImgage>> FindInvalidChapHasEmpltyImages() =>
            _correctInvalidDataService.FindInvalidChap();

        [Route("api/[controller]/OnlyChangeFlagGetStatus")]
        [HttpPost]
        public ActionResult<bool> OnlyChangeFlagGetStatus(int storyId, int chapid, IMAGE_STATUS flagStatus = IMAGE_STATUS.OK) =>
            _correctInvalidDataService.OnlyChangeFlagGetStatus(storyId, chapid, flagStatus);

    }
}
