using DurableTask.Core;
using DurableTask.SqlServer;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DurTaskFrameworkSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Process started...");

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(options => options.SingleLine = true);
            });

            Console.WriteLine("\nSetting up db...");
            var orchService = await GetOrchestrationServiceAsync(loggerFactory);
            try
            {
                Console.WriteLine("\n------------------");
                Console.WriteLine("DoProcess01: Started...");
                Console.WriteLine("------------------");
                var processResult = await DoProcess01Async(orchService, loggerFactory);
                Console.WriteLine("\nResult:");
                Console.WriteLine(processResult);
                Console.WriteLine("\nDoProcess01: Completed...\n\n");
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
            //from https://github.com/microsoft/durabletask-mssql/blob/main/docs/quickstart.md
            var connectionString = "Server=.;Database=DurableDB;User Id=sa;Password=eXpress2019";

            var settings = new SqlOrchestrationServiceSettings(connectionString);


            var orchestrationService = new SqlOrchestrationService(settings);

            // Install the DB schema, if necessary
            await orchestrationService.CreateIfNotExistsAsync();

            return orchestrationService;
        }

        
        static async System.Threading.Tasks.Task<string> DoProcess01Async(SqlOrchestrationService orchestrationService, ILoggerFactory loggerFactory)
        {
            string processResult = string.Empty;

            var worker = new TaskHubWorker(orchestrationService, loggerFactory);
            try
            {
                
                worker.AddTaskOrchestrations(typeof(SimplestOrchestration));
                worker.AddTaskActivities(typeof(SumIntTask));
                await worker.StartAsync();

                var client = new TaskHubClient(orchestrationService, loggerFactory: loggerFactory);
                
                OrchestrationInstance id = await client.CreateOrchestrationInstanceAsync(typeof(SimplestOrchestration), new object[] { 10, 20, 30, 40, 50 });

                OrchestrationState result = await client.WaitForOrchestrationAsync(id, TimeSpan.FromSeconds(20), new CancellationToken());
                
                processResult = result.Output;
                //if(result.OrchestrationStatus == OrchestrationStatus.Completed);
            }
            finally
            {
                await worker.StopAsync(true);                
            }
            return processResult;
        }
                
    }

    public class SimplestOrchestration : TaskOrchestration<int, int[]>
    {
        public override async Task<int> RunTask(OrchestrationContext context, int[] input)
        {
            int result = await context.ScheduleTask<int>(typeof(SumIntTask), input);
            return result;
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
