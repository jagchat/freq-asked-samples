using Microsoft.Extensions.Hosting;
using Serilog;
//using Serilog.Enrichers.Span;
using Serilog.Exceptions;

namespace common.logging
{
    public static class Logging
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger =>
            (context, loggerConfiguration) =>
            {
                var env = context.HostingEnvironment;
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", env.ApplicationName)
                    .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                    //.Enrich.With<Log4NetLevelMapperEnricher>() //optional: to translate Serilog log levels to Log4Net
                    .Enrich.WithExceptionDetails();
            };
    }
}
