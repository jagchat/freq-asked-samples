using Microsoft.Extensions.Logging;
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
            _logger.LogTrace($"StatusRepository.GetStatusAsync: Started...");

            //TODO:
            dynamic o = new
            {
                DbStatus = "Connected!"
            };

            _logger.LogTrace($"StatusRepository.GetStatusAsync: Completed...");
            return o;
        }

    }
}
