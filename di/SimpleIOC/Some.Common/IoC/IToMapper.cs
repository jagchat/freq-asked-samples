using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Some.Common.IoC
{
    public interface IToMapper
    {
        IFromMapper Inject<T>();
        IFromMapper Inject(string AssemblyName, string ClassName);
        IFromMapper Inject(Type t);
    }
}
