using Microsoft.Diagnostics.EventFlow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;

namespace Console1
{
    class Program
    {
        private static IConfigurationRoot BuildConfig()
        {
            return new ConfigurationBuilder()
               .SetBasePath(System.IO.Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.FileExtensions
               .Build();
        }

        private static IServiceProvider BuildProvider(IConfiguration config)
        {
            return new ServiceCollection()
               .AddTransient<Math>() // Runner is the custom class
               .AddLogging(loggingBuilder =>
               {
                   // configure Logging with NLog
                   loggingBuilder.ClearProviders();
                   loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                   loggingBuilder.AddNLog(config);
               })
               .BuildServiceProvider();
        }

        static void Main(string[] args)
        {
            var pipeline = DiagnosticPipelineFactory.CreatePipeline("eventFlowConfig.json");
            pipeline.ConfigureNLogInput(NLog.LogLevel.Trace);
            
            
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                var servicesProvider = BuildProvider(BuildConfig());
                using (servicesProvider as IDisposable)
                {
                    var o = servicesProvider.GetRequiredService<Math>();
                    var sum = o.GetSum(10,20);
                    Console.WriteLine($"Sum: {sum}");
                    Console.WriteLine("\n\nPress ANY key to exit");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
                pipeline.Dispose();
            }

        }
    }
}
