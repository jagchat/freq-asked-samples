using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCoreConfigDemo.MyConfig
{
    public class MyConfigSource : IConfigurationSource
    {
        public string Key { get; set; }

        public MyConfigSource(MyConfigOptions options)
        {
            Key = options.Key;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MyConfigProvider(this);
        }
    }
}
