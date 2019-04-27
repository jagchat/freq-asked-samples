using Some.Common.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Some.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ICalc o = Some.Common.IoC.SampleIoC.GetCalcInstance();
                Console.WriteLine($"{o.DoCalc(10, 20)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
            Console.ReadLine();
        }
    }
}
