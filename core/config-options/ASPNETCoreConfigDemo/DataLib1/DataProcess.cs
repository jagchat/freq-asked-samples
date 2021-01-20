using DataLib.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace DataLib1
{
    public class DataProcess : IData
    {
        IConfiguration configuration = null;
        ILogger<DataProcess> log = null;

        public DataProcess(IConfiguration config, ILogger<DataProcess> logger)
        {
            configuration = config;
            log = logger;
        }

        public void DoProcess()
        {
            var myKeyValue = configuration["MyKey"]; //from appsettings.json
            var myUrl = configuration["MyOptions:MyStartupUrl"]; //from startup.cs
            var myCustomOption1 = configuration["MyCustomOption1"]; //from custom config provider
            var myCustomJsonOption1 = configuration["My2ndLevelOptionSet:SetOption1"]; //from custom json config provider

            log.LogInformation($"DataLib1.DataProcess.DoProcess => {myCustomJsonOption1}");
        }
    }
}
