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
            //// use this to log directly without app.settings
            //// START
            //Log.Logger = new LoggerConfiguration()
            //    .Enrich.FromLogContext()
            //    .MinimumLevel.Verbose()
            //    //.WriteTo.Console(new RenderedCompactJsonFormatter())
            //    .WriteTo.Debug(outputTemplate: DateTime.Now.ToString())
            //    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, shared: true)
            //    //.WriteTo.Seq("http://localhost:5341/") //docker run --rm -it -e ACCEPT_EULA=Y -p 5341:80 datalust/seq
            //    .CreateLogger();
            //// END


            //// use this to log based app.settings
            //// START
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            //// END

            try
            {
                Log.Verbose("Starting host (TRACE test)..");
                Log.Information("Starting host..");
                Log.Information($"{configuration.GetConnectionString("SampleDb")}"); //Just for Demo.  Bad practice.
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
