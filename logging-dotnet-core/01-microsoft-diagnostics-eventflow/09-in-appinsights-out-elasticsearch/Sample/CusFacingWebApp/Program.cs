using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Diagnostics.EventFlow;
using Microsoft.Diagnostics.EventFlow.ApplicationInsights;
using Microsoft.Diagnostics.EventFlow.HealthReporters;
using Microsoft.Diagnostics.EventFlow.Inputs;
using Microsoft.Diagnostics.EventFlow.Outputs;
using Microsoft.Diagnostics.EventFlow.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.AspNetCore;

namespace CusFacingWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var eventFlow = CreateEventFlow(args))
            {
                CreateWebHostBuilder(args, eventFlow).Build().Run();
            }

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, DiagnosticPipeline eventFlow) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services.AddSingleton<ITelemetryProcessorFactory>(sp => new EventFlowTelemetryProcessorFactory(eventFlow)))
                .UseStartup<Startup>()
                .UseApplicationInsights();

        private static DiagnosticPipeline CreateEventFlow(string[] args)
        {
            // Create configuration instance to access configuration information for EventFlow pipeline
            // To learn about common configuration sources take a peek at https://github.com/aspnet/MetaPackages/blob/master/src/Microsoft.AspNetCore/WebHost.cs (CreateDefaultBuilder method). 
            // Here we assume all necessary information comes from command-line arguments and environment variables.
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            if (args != null)
            {
                configBuilder.AddCommandLine(args);
            }
            var config = configBuilder.Build();

            var healthReporter = new CsvHealthReporter(new CsvHealthReporterConfiguration());
            var aiInput = new ApplicationInsightsInputFactory().CreateItem(null, healthReporter);
            var inputs = new IObservable<EventData>[] { aiInput };
            var sinks = new EventSink[]
            {
                new EventSink(new ElasticSearchOutput(new ElasticSearchOutputConfiguration {
                    ServiceUri = config["ElasticsearchServiceUri"]
                    // Set other configuration settings, as necessary
                }, healthReporter), null)
            };

            return new DiagnosticPipeline(healthReporter, inputs, null, sinks, null, disposeDependencies: true);
        }
    }
}
