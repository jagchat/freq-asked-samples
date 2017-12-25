using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using ConsoleEsNet.Entities;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleEsNet
{
    class Program
    {
        public static void Main(string[] args)
        {
            Func<Task<int>> main = async () =>
            {
                Console.WriteLine("Started upload");
                var result = await BulkUpload();
                Console.WriteLine("Done! Output ->");
                Console.WriteLine(result);
                return 0;
            };
            main().Wait();
            Console.ReadLine();
        }

        static async Task<string> BulkUpload()
        {
            var esUrl = "http://127.0.0.1:9200";
            var settings = new ConnectionConfiguration(new Uri(esUrl)).RequestTimeout(TimeSpan.FromMinutes(2));

            var lowlevelClient = new ElasticLowLevelClient(settings);
            var indexResponse = await lowlevelClient.BulkAsync<Stream>(GetFormattedData());
            Stream responseStream = indexResponse.Body;
            var resp = responseStream.ToConvertedString();
            return resp;
        }

        static object[] GetFormattedData()
        {
            var lst = GetEmployees();
            var lstFormatted = new List<object>();
            lst.ForEach(o =>
            {
                lstFormatted.Add(new { index = new { _index = "employee", _type = "employee", _id = o.Empno.ToString() } });
                lstFormatted.Add(new { Ename = o.Ename, Sal = o.Sal.ToString(), Deptno = o.Deptno.ToString() });
            });
            return lstFormatted.ToArray(); //TODO: refactor
        }

        static List<Employee> GetEmployees()
        {
            var lstEmps = new List<Employee>();
            lstEmps.Add(new Employee()
            {
                Empno = 1001,
                Ename = "Jag",
                Sal = 3400,
                Deptno = 10
            });
            lstEmps.Add(new Employee()
            {
                Empno = 1002,
                Ename = "Chat",
                Sal = 4400,
                Deptno = 20
            });
            lstEmps.Add(new Employee()
            {
                Empno = 1003,
                Ename = "Scott",
                Sal = 2300,
                Deptno = 10
            });
            lstEmps.Add(new Employee()
            {
                Empno = 1004,
                Ename = "Smith",
                Sal = 5300,
                Deptno = 20
            });
            lstEmps.Add(new Employee()
            {
                Empno = 1005,
                Ename = "Williams",
                Sal = 4700,
                Deptno = 20
            });

            return lstEmps;
        }
    }
}
