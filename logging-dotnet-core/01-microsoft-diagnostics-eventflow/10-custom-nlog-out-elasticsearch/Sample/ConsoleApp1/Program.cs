using Microsoft.Diagnostics.EventFlow;
using NLog;
//using MoreNLog;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var pipeline = DiagnosticPipelineFactory.CreatePipeline("eventFlowConfig.json");
            pipeline.ConfigureMoreNLogInput(NLog.LogLevel.Trace);

            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                Console.WriteLine("Writing to eventflow..");
                logger.Trace("this is a trace message");
                logger.Debug("this is a debug message");
                Console.WriteLine("Done!");
                Console.WriteLine("\n\nPress ANY key to exit");
                Console.ReadKey();
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
