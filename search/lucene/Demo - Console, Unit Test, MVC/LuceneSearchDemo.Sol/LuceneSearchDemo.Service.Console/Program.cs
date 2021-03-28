using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuceneSearchDemo.Service.Library.Employees;
using NLog;

namespace LuceneSearchDemo.Service.Console
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {

            System.Console.WriteLine("Indexing data..please wait..");
            (new EmployeeSearch()).CreateIndex();
            System.Console.WriteLine("COMPLETED!!\n\n" +
                                     "Press ENTER to Continue");
            System.Console.ReadLine();
        }
    }
}
