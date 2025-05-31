using common.logging;
using demo.app.data;
using demo.app.service;

namespace demo.web.api
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddDataDependencies();
            services.AddServiceDependencies();

            //for logging Enrichment (optional)
            services.AddHttpContextAccessor();
            services.AddSingleton<HttpRequestContextEnricher>();
            services.AddSingleton<Log4NetLevelMapperEnricher>();
            return services;
        }
    }
}
