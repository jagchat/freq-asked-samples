using DurableTask.Core;
using DurableTask.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace DurTaskFrameworkSample
{
    class Program
    {
        static ILogger logger;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Process started...");

            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false, true)
                                .Build();

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddSimpleConsole();
            });

            logger = loggerFactory.CreateLogger<Program>();
            logger.LogTrace("checking Trace...");
            logger.LogDebug("checking Debug...");
            logger.LogInformation("checking Info...");
            logger.LogWarning("checking Warning...");

            Console.WriteLine("\n---------------");
            logger.LogInformation("Setting up db...");
            var orchService = await GetOrchestrationServiceAsync(loggerFactory);
            try
            {
                logger.LogInformation("DoProcess01: Started...");
                var processResult = await DoProcess01Async(orchService, loggerFactory);
                logger.LogInformation("DoProcess01: Completed...");
                Console.WriteLine("\nResult:");
                Console.WriteLine(processResult);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await orchService.DeleteAsync();
            }

        }

        static async Task<SqlOrchestrationService> GetOrchestrationServiceAsync(ILoggerFactory loggerFactory)
        {
            logger.LogTrace("GetOrchestrationServiceAsync: Started...");

            //from https://github.com/microsoft/durabletask-mssql/blob/main/docs/quickstart.md
            var connectionString = "Server=.;Database=DurableDB;User Id=sa;Password=eXpress2019";

            var settings = new SqlOrchestrationServiceSettings(connectionString);

            var orchestrationService = new SqlOrchestrationService(settings);

            logger.LogTrace("GetOrchestrationServiceAsync: Creating schema if necessary...");
            // Install the DB schema, if necessary
            await orchestrationService.CreateIfNotExistsAsync();

            logger.LogTrace("GetOrchestrationServiceAsync: Completed...");
            return orchestrationService;
        }

        static async System.Threading.Tasks.Task<string> DoProcess01Async(SqlOrchestrationService orchestrationService, ILoggerFactory loggerFactory)
        {
            logger.LogTrace("DoProcess01Async: Started...");

            string processResult = string.Empty;

            logger.LogTrace("DoProcess01Async: Creating Worker...");
            var worker = new TaskHubWorker(orchestrationService, loggerFactory);
            try
            {
                worker.AddTaskOrchestrations(typeof(SimplestOrchestration));
                worker.AddTaskActivities(typeof(SumIntTask), typeof(RandomNumbersTask));
                logger.LogTrace("DoProcess01Async: Starting Worker...");
                await worker.StartAsync();

                logger.LogTrace("DoProcess01Async: Creating Client...");
                var client = new TaskHubClient(orchestrationService, loggerFactory: loggerFactory);

                logger.LogTrace("DoProcess01Async: Creating Orchestration Instance...");
                OrchestrationInstance id = await client.CreateOrchestrationInstanceAsync(typeof(SimplestOrchestration), null);

                logger.LogTrace("DoProcess01Async: Waiting for Orchestration to complete...");
                OrchestrationState result = await client.WaitForOrchestrationAsync(id, Debugger.IsAttached ? TimeSpan.FromSeconds(60) : TimeSpan.FromSeconds(20), new CancellationToken());
                logger.LogTrace("DoProcess01Async: Orchestration Completed...");

                processResult = result.Output;
                //if(result.OrchestrationStatus == OrchestrationStatus.Completed);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ERROR: occurred at DoProcess01Async");
            }
            finally
            {
                await worker.StopAsync(true);
            }
            logger.LogTrace("DoProcess01Async: Completed...");
            return processResult;
        }

        public class SimplestOrchestration : TaskOrchestration<int, string>
        {
            public override async Task<int> RunTask(OrchestrationContext context, string msg)
            {
                int[] numbers = await context.ScheduleTask<int[]>(typeof(RandomNumbersTask), msg);
                int result = await context.ScheduleTask<int>(typeof(SumIntTask), numbers);
                return result;
            }
        }


    }

    public sealed class RandomNumbersTask : TaskActivity<string, int[]>
    {
        protected override int[] Execute(TaskContext context, string input)
        {
            var count = RandomNumberGenerator.GetInt32(1, 10); //no. of items

            var numbers = new List<int>();
            for (int i = 0; i < count; i++)
            {
                numbers.Add(RandomNumberGenerator.GetInt32(5, 10));
            }

            return numbers.ToArray();
        }
    }

    public sealed class SumIntTask : TaskActivity<int[], long>
    {
        protected override long Execute(TaskContext context, int[] numsToAdd)
        {
            long sum = 0;
            foreach (int num in numsToAdd)
            {
                sum += num;
            }
            return sum;
        }
    }


}
