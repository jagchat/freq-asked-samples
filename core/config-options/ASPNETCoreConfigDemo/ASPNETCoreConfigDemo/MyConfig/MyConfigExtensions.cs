using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCoreConfigDemo.MyConfig
{
    public static class MyConfigExtensions
    {
        public static IConfigurationBuilder AddMyConfig(this IConfigurationBuilder configuration, Action<MyConfigOptions> options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            var myConfigOptions = new MyConfigOptions();
            options(myConfigOptions);
            configuration.Add(new MyConfigSource(myConfigOptions));            
            return configuration;
        }
    }
}
