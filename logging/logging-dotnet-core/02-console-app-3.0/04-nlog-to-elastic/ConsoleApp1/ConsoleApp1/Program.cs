using NLog;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                Console.WriteLine("Writing to log..");
                logger.Trace("this is a trace message");
                logger.Debug("this is a debug message");
                logger.Trace("this is an object {@value1}", new { OrderId = 12, Status = "Processing" });
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
            }

        }
    }
}
