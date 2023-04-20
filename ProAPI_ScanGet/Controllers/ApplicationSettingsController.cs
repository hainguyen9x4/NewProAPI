using Microsoft.AspNetCore.Mvc;
using Pro.Model;
using Pro.Service;
using System.Collections.Generic;

namespace xAppSetting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationSettingsController : ControllerBase
    {
        private readonly IApplicationSettingService _applicationSettingService;

        public ApplicationSettingsController(IApplicationSettingService applicationSettingService)
        {
            _applicationSettingService = applicationSettingService;
        }

        [HttpGet]
        public ActionResult<List<ApplicationSetting>> Get() =>
            _applicationSettingService.Get();

        [HttpGet("{id:length(24)}", Name = "GetAppSetting")]
        public ActionResult<ApplicationSetting> Get(int id)
        {
            var applicationSetting = _applicationSettingService.Get(id);

            if (applicationSetting == null)
            {
                return NotFound();
            }

            return applicationSetting;
        }

        [HttpPost]
        public ActionResult<ApplicationSetting> Create(ApplicationSetting applicationSetting)
        {
            _applicationSettingService.Create(applicationSetting);

            return CreatedAtRoute("GetAppSetting", new { id = applicationSetting.AppSettingId.ToString() }, applicationSetting);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(int id, ApplicationSetting appSetting)
        {
            var applicationSetting = _applicationSettingService.Get(id);

            if (applicationSetting == null)
            {
                return NotFound();
            }

            _applicationSettingService.Update(id, appSetting);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(int id)
        {
            var applicationSetting = _applicationSettingService.Get(id);

            if (applicationSetting == null)
            {
                return NotFound();
            }

            _applicationSettingService.Delete(applicationSetting.AppSettingId);

            return NoContent();
        }
    }
}