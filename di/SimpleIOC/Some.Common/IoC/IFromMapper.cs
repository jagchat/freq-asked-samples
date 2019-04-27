using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Some.Common.IoC
{
    public interface IFromMapper
    {
        IToMapper For<T>();
        IFromMapper Register<Tfrom, TTo>() where TTo : Tfrom;
    }
}
