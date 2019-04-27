using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Some.Common.IoC
{
    public interface ISampleIoC
    {
        T Resolve<T>() where T : class;
        IFromMapper UseSingleton();
        Dictionary<Type, Type> GetAllRegisteredTypes();
        bool IsRegistered<Type>();
    }
}
