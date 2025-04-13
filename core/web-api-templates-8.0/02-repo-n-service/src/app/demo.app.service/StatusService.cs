using demo.app.data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace demo.app.service
{
    public class StatusService
    {
        private readonly ILogger<StatusService> _logger;
        private readonly StatusRepository _statusRepository;
        private readonly IConfiguration _configuration;

        public StatusService(ILogger<StatusService> logger, StatusRepository statusRepository, IConfiguration configuration)
        {
            _logger = logger;
            _logger.LogTrace("StatusService.Constructor: Started...");
            _configuration = configuration;
            _statusRepository = statusRepository;
            _logger.LogTrace("StatusService.Constructor: Completed...");
        }

        public async Task<string> GetCurrentStatus()
        {
            _logger.LogTrace("StatusService.GetCurrentStatus: Started...");
            var oStatus = await _statusRepository.GetStatusAsync();
            dynamic o = new
            {
                CurrentDate = DateTime.Now,
                Status = oStatus,
                DeploymentId = _configuration.GetValue<string>("DeploymentId")
            };
            _logger.LogTrace("StatusService.GetCurrentStatus: Completed...");
            return JsonSerializer.Serialize(o);
        }
    }
}
