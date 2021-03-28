using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                //.WriteTo.Console(new RenderedCompactJsonFormatter())
                .WriteTo.Debug(outputTemplate: DateTime.Now.ToString())
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, shared: true)
                //.WriteTo.Seq("http://localhost:5341/")
                .CreateLogger();
            try
            {
                Log.Information("Starting host..");
                CreateHostBuilder(args).Build().Run();        
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");                
            }
            finally
            {
                Log.CloseAndFlush();
            }

            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
