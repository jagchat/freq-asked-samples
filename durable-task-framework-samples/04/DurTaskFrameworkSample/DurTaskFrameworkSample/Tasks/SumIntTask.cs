using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DurTaskFrameworkSample.Tasks
{
    public sealed class SumIntTask : TaskActivity<int[], long>
    {

        private readonly ILogger<SumIntTask> logger;

        public SumIntTask(ILogger<SumIntTask> logger)
        {
            logger.LogTrace("SumIntTask.constructor: Started..");
            this.logger = logger;
        }

        protected override long Execute(TaskContext context, int[] numsToAdd)
        {
            logger.LogTrace("SumIntTask.Execute: Started..");
            logger.LogTrace($"SumIntTask.Execute: numbers received: [{String.Join(',', numsToAdd)}]");
            long sum = 0;
            foreach (int num in numsToAdd)
            {
                sum += num;
            }
            logger.LogTrace($"SumIntTask.Execute: calculated sum: {sum}");
            logger.LogTrace("SumIntTask.Execute: Completed..");
            return sum;
        }

    }

}
