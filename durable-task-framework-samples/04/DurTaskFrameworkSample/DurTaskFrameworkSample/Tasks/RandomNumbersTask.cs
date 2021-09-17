using DurableTask.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DurTaskFrameworkSample.Tasks
{
    public sealed class RandomNumbersTask : TaskActivity<string, int[]>
    {

        private readonly ILogger<RandomNumbersTask> logger;

        public RandomNumbersTask(ILogger<RandomNumbersTask> logger)
        {
            logger.LogTrace("RandomNumbersTask.constructor: Started..");
            this.logger = logger;
        }

        protected override int[] Execute(TaskContext context, string input)
        {
            logger.LogTrace("RandomNumbersTask.Execute: Started..");
            var count = RandomNumberGenerator.GetInt32(1, 10); //no. of items

            var numbers = new List<int>();
            for (int i = 0; i < count; i++)
            {
                numbers.Add(RandomNumberGenerator.GetInt32(5, 10));
            }
            var numberArray = numbers.ToArray();
            logger.LogTrace($"RandomNumbersTask.Execute: random numbers picked: [{String.Join(',', numberArray)}]");
            logger.LogTrace("RandomNumbersTask.Execute: Completed..");
            return numberArray;
        }

    }

}
