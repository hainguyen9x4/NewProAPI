using Microsoft.AspNetCore.Mvc;
using Pro.Service;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace xStory.Controllers
{
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [Route("api/[controller]/ListLogFile")]
        [HttpGet]
        public ActionResult<List<string>> ListLogFile(string folderLog = "d:/www2/scangetapi/data/appdata/")
        {
            return _logService.GetLogFiles(folderLog);
        }

        [Route("api/[controller]/ViewLogFileByName")]
        [HttpGet]
        public ActionResult<string> ViewLogFileByName(string fullPathFile)
        {
            return _logService.GetLogInfo(fullPathFile);
        }
    }
}