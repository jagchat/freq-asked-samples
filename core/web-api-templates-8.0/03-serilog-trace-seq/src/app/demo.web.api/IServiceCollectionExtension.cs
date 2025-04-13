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
            return services;
        }
    }
}
