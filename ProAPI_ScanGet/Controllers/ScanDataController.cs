using Microsoft.AspNetCore.Mvc;
using Pro.Service;

namespace xStory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanDataController : ControllerBase
    {
        private readonly IScanDataService _scanDataService;

        public ScanDataController(IScanDataService scanDataService)
        {
            _scanDataService = scanDataService;
        }

        [HttpGet]
        public ActionResult<bool> StartScanData() =>
            _scanDataService.StartScanData();
    }
}