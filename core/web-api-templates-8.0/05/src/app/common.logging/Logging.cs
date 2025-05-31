using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;

namespace common.logging
{
    public static class Logging
    {
        public static Action<HostBuilderContext, IServiceProvider, LoggerConfiguration> ConfigureLogger =>
            (context, serviceProvider, loggerConfiguration) =>
            {
                var env = context.HostingEnvironment;
                var httpRequestContextEnricher = serviceProvider.GetService<HttpRequestContextEnricher>();
                var log4NetLevelMapperEnricher = serviceProvider.GetService<Log4NetLevelMapperEnricher>();

                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration);

                if (httpRequestContextEnricher != null)
                {
                    loggerConfiguration.Enrich.With(httpRequestContextEnricher);
                }

                if (log4NetLevelMapperEnricher != null)
                {
                    loggerConfiguration.Enrich.With(log4NetLevelMapperEnricher);
                }
            };
    }
}
