using Microsoft.AspNetCore.Mvc;

namespace demo.web.api.Controllers
{
    public class StatusController : APIController
    {
        private readonly ILogger<StatusController> _logger;

        public StatusController(ILogger<StatusController> logger)
        {
            _logger = logger;
            _logger.LogTrace("StatusController.Constructor: Started...");
            //TODO:
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
                //TODO:
                result = @$"{{""working"": ""true""}}";

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
