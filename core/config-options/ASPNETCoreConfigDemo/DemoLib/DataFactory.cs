using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace DemoLib
{
    public class DataFactory
    {
        IConfiguration configuration = null;
        ILogger<DataFactory> log = null;

        public DataFactory(IConfiguration config, ILogger<DataFactory> logger)
        {
            configuration = config;
            log = logger;
        }

        public void UpdateData()
        {
            var myKeyValue = configuration["MyKey"]; //from appsettings.json
            var myUrl = configuration["MyOptions:MyStartupUrl"]; //from startup.cs
            var myCustomOption1 = configuration["MyCustomOption1"]; //from custom config provider
            var myCustomJsonOption1 = configuration["My2ndLevelOptionSet:SetOption1"]; //from custom json config provider

            log.LogInformation($"DemoLib.DataFactory.UpdateDate => {myCustomJsonOption1}");
            //do some dummy here
        }
    }
}
