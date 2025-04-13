using Microsoft.Extensions.Logging;
using Serilog.Context;
using SerilogTracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo.app.data
{
    public class StatusRepository
    {
        private readonly ILogger<StatusRepository> _logger;

        public StatusRepository(IServiceProvider provider, ILogger<StatusRepository> logger)
        {
            _logger = logger;
            _logger.LogTrace($"StatusRepository.Constructor: Started...");
            //TODO:
            _logger.LogTrace($"StatusRepository.Constructor: Completed...");
        }

        public async Task<dynamic> GetStatusAsync()
        {
            LogContext.PushProperty("Application", "demo.app.data");
            using var activity = Serilog.Log.Logger.StartActivity("StatusRepository: Processing Started");
            _logger.LogTrace($"StatusRepository.GetStatusAsync: Started...");

            //TODO:
            dynamic o = new
            {
                DbStatus = "Connected!"
            };

            _logger.LogTrace($"StatusRepository.GetStatusAsync: Completed...");
            activity.Complete();
            return o;
        }

    }
}
