using DurableTask.Core;
using DurableTask.SqlServer;
using DurTaskFrameworkSample.Orchestration;
using DurTaskFrameworkSample.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
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
        //static LoggerFactory loggerFactory;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Process started...");

            //init
            SetupLogger();
            var provider = GetServiceProvder();

            logger.LogInformation("Setting up db...");
            Console.WriteLine("Setting up db...");
            var orchService = await GetOrchestrationServiceAsync();

            try
            {
                Console.WriteLine("Executing Process...");
                logger.LogInformation("DoProcessAsync: Started...");
                var processResult = await DoProcessAsync(orchService, provider);
                logger.LogInformation("DoProcessAsync: Completed...");
                Console.WriteLine("------------\nResult:");
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

        static void SetupLogger() 
        {
            ////using Nlog through Microsoft Extensions 
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());

            logger = loggerFactory.CreateLogger<Program>();
            logger.LogTrace("checking Trace...");
            logger.LogDebug("checking Debug...");
            logger.LogInformation("checking Info...");
            logger.LogWarning("checking Warning...");
        }

        static IServiceProvider GetServiceProvder() 
        {
            //setting up DI
            logger.LogInformation("Setting up DI...");
            var services = new ServiceCollection()
            .AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog("nlog.config");
            });

            services.AddTransient(typeof(SumIntTask));
            services.AddTransient(typeof(RandomNumbersTask));
            services.AddTransient(typeof(SimplestOrchestration));
            var provider = services.BuildServiceProvider();

            return provider;
        }

        static async Task<SqlOrchestrationService> GetOrchestrationServiceAsync()
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

        static async System.Threading.Tasks.Task<string> DoProcessAsync(SqlOrchestrationService orchestrationService, IServiceProvider provider)
        {
            logger.LogTrace("DoProcessAsync: Started...");

            string processResult = string.Empty;
            var loggerFactory = provider.GetService<ILoggerFactory>();

            logger.LogTrace("DoProcessAsync: Creating Worker...");
            var worker = new TaskHubWorker(orchestrationService, loggerFactory);
            try
            {
                logger.LogTrace("DoProcessAsync: Adding Orchestrations & Activities...");
                worker.AddTaskOrchestrations(new ServiceProviderObjectCreator<TaskOrchestration>(typeof(SimplestOrchestration), provider));
                var activities = new List<ObjectCreator<TaskActivity>>();
                activities.Add(new ServiceProviderObjectCreator<TaskActivity>(typeof(SumIntTask), provider));
                activities.Add(new ServiceProviderObjectCreator<TaskActivity>(typeof(RandomNumbersTask), provider));
                worker.AddTaskActivities(activities.ToArray());

                logger.LogTrace("DoProcessAsync: Starting Worker...");
                await worker.StartAsync();

                logger.LogTrace("DoProcessAsync: Creating Client...");
                var client = new TaskHubClient(orchestrationService, loggerFactory: loggerFactory);

                logger.LogTrace("DoProcessAsync: Creating Orchestration Instance...");
                OrchestrationInstance id = await client.CreateOrchestrationInstanceAsync(typeof(SimplestOrchestration), null);

                logger.LogTrace("DoProcessAsync: Waiting for Orchestration to complete...");
                OrchestrationState result = await client.WaitForOrchestrationAsync(id, Debugger.IsAttached ? TimeSpan.FromSeconds(60) : TimeSpan.FromSeconds(20), new CancellationToken());
                logger.LogTrace("DoProcessAsync: Orchestration Completed...");

                processResult = result.Output;
                //if(result.OrchestrationStatus == OrchestrationStatus.Completed);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ERROR: occurred at DoProcessAsync");
                throw;
            }
            finally
            {
                await worker.StopAsync(true);
            }
            logger.LogTrace("DoProcessAsync: Completed...");
            return processResult;
        }

    }
}
