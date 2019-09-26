using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using xyz.shared.Interfaces;
using xyz.shared.Models;

namespace xyz.backend
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class backend : StatelessService, IBackendService
    {
        public backend(StatelessServiceContext context)
            : base(context)
        { }

        public Task<List<Employee>> FetchEmployees()
        {
            return Task.FromResult(new List<Employee>() {
                new Employee(){empno="1001", ename="Jag", sal="4500", deptno="30" },
                new Employee(){empno="1002", ename="Chat", sal="5800", deptno="20" },
                new Employee(){empno="1003", ename="Scott", sal="9800", deptno="20" },
                new Employee(){empno="1004", ename="Smith", sal="6300", deptno="10" }
            });
        }

        public Task<string> HelloWorldAsync()
        {
            return Task.FromResult("Hello Backend!");
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            //return new ServiceInstanceListener[0];
            return new[]
            {
                new ServiceInstanceListener((c) =>
                {
                    return new FabricTransportServiceRemotingListener(c, this);

                })
            };
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
