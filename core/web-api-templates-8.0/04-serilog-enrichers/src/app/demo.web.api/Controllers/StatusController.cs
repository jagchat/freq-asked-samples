using demo.app.service;
using Microsoft.AspNetCore.Mvc;

namespace demo.web.api.Controllers
{
    public class StatusController : APIController
    {
        private readonly ILogger<StatusController> _logger;
        private readonly StatusService _statusService;

        public StatusController(ILogger<StatusController> logger, StatusService statusService)
        {
            _logger = logger;
            _logger.LogTrace("StatusController.Constructor: Started...");
            _statusService = statusService;
            _logger.LogTrace("StatusController.Constructor: Completed...");
        }

        [HttpGet]
        [Route("")]
        public async Task<ContentResult> GetCurrentStatus()
        {
            _logger.LogTrace("StatusController.GetCurrentStatus: Started...");
            var result = "";
            try
            {
                result = await _statusService.GetCurrentStatus();
                _logger.LogTrace("StatusController.GetCurrentStatus: Completed...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR at StatusController.GetCurrentStatus ");
                result = GetErrorResultJsonString(ex);
            }

            return new ContentResult()
            {
                Content = result,
                ContentType = "application/json"
            };
        }
    }
}
