using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCoreConfigDemo.MyConfigJson
{
    public static class MyConfigJsonExtensions
    {
        public static IConfigurationBuilder AddMyConfigJson(this IConfigurationBuilder configuration, Action<MyConfigJsonOptions> options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            var myConfigOptions = new MyConfigJsonOptions();
            options(myConfigOptions);
            var o = new MyConfigJsonProvider(myConfigOptions);            
            configuration.AddJsonStream(new MemoryStream(Encoding.ASCII.GetBytes(o.GetConfigJsonContent())));
            return configuration;
        }
    }
}
