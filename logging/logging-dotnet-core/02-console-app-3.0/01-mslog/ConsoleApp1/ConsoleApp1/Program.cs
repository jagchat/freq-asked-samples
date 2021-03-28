using Microsoft.Extensions.Logging;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddConsole();
            });
            
            ILogger logger = loggerFactory.CreateLogger<Program>();
            
            logger.LogTrace("Example log trace message"); //not shown at default "info" level
            logger.LogDebug("Example log debug message"); //not shown at default "info" level
            logger.LogInformation("Example log info message");
            logger.LogWarning("Example log warning");
            logger.LogError("Example log error");
            logger.LogCritical("Example log critical");
            logger.Log(LogLevel.Critical, "Example log critical");
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
