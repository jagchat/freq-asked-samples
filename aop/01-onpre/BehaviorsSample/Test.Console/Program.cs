using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Some.Library.Base;

namespace Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Customer o = new Customer() { FirstName = "Mary", LastName = "Williams" };
            System.Console.WriteLine(o.ComputeLabelName());

            System.Console.ReadLine();
        }
    }
}
