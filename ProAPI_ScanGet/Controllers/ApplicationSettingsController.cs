using Microsoft.AspNetCore.Mvc;
using Pro.Model;
using Pro.Service;
using System.Collections.Generic;
using static Pro.Service.Implements.ApplicationSettingService;

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
        [Route("Get")]
        public ActionResult<List<ApplicationSetting>> Get() =>
            _applicationSettingService.Get();

        [HttpGet("{id}", Name = "GetAppSetting")]
        public ActionResult<ApplicationSetting> Get(int id)
        {
            var applicationSetting = _applicationSettingService.Get(id);

            if (applicationSetting == null)
            {
                return NotFound();
            }

            return applicationSetting;
        }
        [HttpGet]
        [Route("GetAvailableClound")]
        public ActionResult<ApplicationSettingTemp> GetAvailableClound(int number = 0)
        {
            var applicationSetting = _applicationSettingService.GetAvailableClound(number);

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

            return CreatedAtRoute("GetAppSetting", new { id = applicationSetting.AppSettingId }, applicationSetting);
        }
        [HttpPost]
        [Route("CreateCloundinary")]
        public ActionResult<ApplicationSetting> CreateCloundinary(string dataCreateCloundinary, string email, string c = "\"")
        {
            var sppSetting = _applicationSettingService.CreateCloundinary(dataCreateCloundinary, email, c);

            return CreatedAtRoute("GetAppSetting", new { id = sppSetting.AppSettingId }, sppSetting);
        }
        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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