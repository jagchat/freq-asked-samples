using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace demo.app.service
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            Assembly.GetAssembly(typeof(IServiceCollectionExtension)) //current assembly
                .ExportedTypes
                .Where(t => t.IsClass && t.FullName.EndsWith("Service"))
                .ToList()
                .ForEach(x => services.AddTransient(x));

            return services;
        }
    }
}
