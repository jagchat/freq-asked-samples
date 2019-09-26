using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xyz.shared.Models;

namespace xyz.shared.Interfaces
{
    public interface IBackendService : IService
    {
        Task<string> HelloWorldAsync();
        Task<List<Employee>> FetchEmployees();
    }
}
