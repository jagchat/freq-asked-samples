using System.Linq;
using Microsoft.Diagnostics.EventFlow;
using Microsoft.Diagnostics.EventFlow.Inputs;
using NLog;
using Validation;

//namespace MoreNLog
namespace NLog
{
    public static class DiagnosticPipelineExtensions
    {
        public static MoreNLogInput ConfigureMoreNLogInput(this DiagnosticPipeline diagnosticPipeline, NLog.LogLevel minLogLevel = null, string loggerNamePattern = "*", NLog.Config.LoggingConfiguration loggingConfig = null)
        {
            Requires.NotNull(diagnosticPipeline, nameof(diagnosticPipeline));

            var input = diagnosticPipeline.Inputs.OfType<MoreNLogInput>().FirstOrDefault();
            if (input == null)
                return null;

            if (minLogLevel != null)
            {
                var config = loggingConfig ?? LogManager.Configuration ?? new NLog.Config.LoggingConfiguration();
                config.AddRule(minLogLevel, NLog.LogLevel.Fatal, input, loggerNamePattern);
                if (loggingConfig == null)
                {
                    if (LogManager.Configuration == null)
                        LogManager.Configuration = config;
                    else
                        LogManager.ReconfigExistingLoggers();
                }
            }

            return input;
        }
    }
}
