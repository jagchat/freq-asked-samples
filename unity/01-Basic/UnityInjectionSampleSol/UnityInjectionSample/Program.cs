using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Some.Common;
using Some.Library.Interfaces;

namespace UnityInjectionSample
{
    class Program
    {
        static void Main(string[] args)
        {
            //NOTE: 
            // - once built, Some.library.Base.dll and Some.library.Custom.dll should be copied to bin of this project

            ICustomer o = UnityConfig.GetInstance<ICustomer>();
            //ICustomer o = UnityConfig.GetInstance<ICustomer>("Customer"); //using mapping name based on config
            o.ShowDummyStatus();
            Console.ReadLine();
        }
    }
}
