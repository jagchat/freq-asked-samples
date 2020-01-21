using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                        
            IConfigurationRoot configuration = builder.Build();
            
            var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddConsole();
            });

            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogTrace("Example log trace message"); 
            logger.LogDebug("Example log debug message"); 
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
