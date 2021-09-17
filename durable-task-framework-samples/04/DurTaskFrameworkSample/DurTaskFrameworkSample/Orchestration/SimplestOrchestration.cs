using DurableTask.Core;
using DurTaskFrameworkSample.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DurTaskFrameworkSample.Orchestration
{
    public class SimplestOrchestration : TaskOrchestration<int, string>
    {
        private readonly ILogger<SimplestOrchestration> logger;
        private readonly IServiceProvider provider;
        public SimplestOrchestration(ILogger<SimplestOrchestration> logger, IServiceProvider provider)
        {
            logger.LogTrace("SimplestOrchestration.constructor: Started..");
            this.logger = logger;
        }

        public override async Task<int> RunTask(OrchestrationContext context, string msg)
        {
            logger.LogTrace("SimplestOrchestration.RunTask: Started..");
            int[] numbers = await context.ScheduleTask<int[]>(typeof(RandomNumbersTask), msg);
            int result = await context.ScheduleTask<int>(typeof(SumIntTask), numbers);
            logger.LogTrace("SimplestOrchestration.RunTask: Completed..");
            return result;
        }
    }

}
