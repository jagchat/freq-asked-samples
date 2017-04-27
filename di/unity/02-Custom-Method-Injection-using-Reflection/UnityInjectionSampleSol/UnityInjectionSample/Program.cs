using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Some.Common;
using Some.Library.Base;
using Some.Library.Interfaces;

namespace UnityInjectionSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Customer o = new Customer() { FirstName = "Mary", LastName = "Williams" };
            Console.WriteLine(o.ComputeLabelName());
            o.ShowDummyStatus();
            Console.ReadLine();
        }
    }
}
