using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xyz.shared.Interfaces
{
    public interface IEmpCatalogService: IService
    {
        Task<string> HelloWorldAsync();
    }
}
