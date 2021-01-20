using ASPNETCoreConfigDemo.MyConfig;
using ASPNETCoreConfigDemo.MyConfigJson;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCoreConfigDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var baseConfig = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build(); //only necessary if we need use existing config to fetch new config

                    //add more key/value pairs from custom provider
                    config.AddMyConfig(options =>
                    {
                        options.Key = baseConfig["MyKey"];
                    });

                    //add json config from another custom provider
                    config.AddMyConfigJson(options =>
                    {
                        options.Key = baseConfig["MyKey"];
                    });

                });
    }
}
