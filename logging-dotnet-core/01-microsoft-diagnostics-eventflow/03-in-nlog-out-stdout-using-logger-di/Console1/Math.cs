using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Console1
{
    public class Math
    {
        private readonly ILogger<Math> _logger;

        public Math(ILogger<Math> logger)
        {
            _logger = logger;
        }

        public int GetSum(int a, int b)
        {
            _logger.LogTrace("GetSum: Started...");
            _logger.LogTrace($"Doing sum of {a} and {b}");
            var c = a + b;
            _logger.LogDebug($"Result: {c}");
            _logger.LogTrace("GetSum: Completed...");
            return c;
        }

    }
}
