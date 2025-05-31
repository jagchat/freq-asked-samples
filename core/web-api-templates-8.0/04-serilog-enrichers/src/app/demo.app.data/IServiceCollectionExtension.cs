using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace demo.app.data
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddDataDependencies(this IServiceCollection services)
        {
            Assembly.GetAssembly(typeof(IServiceCollectionExtension)) //current assemly
                .ExportedTypes
                .Where(t => t.IsClass &&
                        (t.FullName.EndsWith("Repository") || t.FullName.EndsWith("UnitOfWork")))
                .ToList()
                .ForEach(x => services.AddTransient(x));
            return services;
        }
    }
}
