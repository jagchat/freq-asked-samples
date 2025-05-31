using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.logging
{
    public class Log4NetLevelMapperEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var log4NetLevel = string.Empty;

            switch (logEvent.Level)
            {
                case LogEventLevel.Debug:
                    log4NetLevel = "DEBUG";
                    break;

                case LogEventLevel.Error:
                    log4NetLevel = "ERROR";
                    break;

                case LogEventLevel.Fatal:
                    log4NetLevel = "FATAL";
                    break;

                case LogEventLevel.Information:
                    log4NetLevel = "INFO";
                    break;

                case LogEventLevel.Verbose:
                    log4NetLevel = "ALL";
                    break;

                case LogEventLevel.Warning:
                    log4NetLevel = "WARN";
                    break;
            }

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Log4NetLevel", log4NetLevel));
        }
    }
}
