using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleEsNetSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            var esUrl = "http://127.0.0.1:9200";
            var settings = new ConnectionConfiguration(new Uri(esUrl)).RequestTimeout(TimeSpan.FromMinutes(2));

            var lowlevelClient = new ElasticLowLevelClient(settings);
            var searchResponse = lowlevelClient.Search<string>("employee", "employee", new
            {
                from = 0,
                size = 10,
                query = new
                {
                    multi_match = new
                    {
                        fields = "Ename",
                        query = "Scott"
                    }
                }
            });

            var successful = searchResponse.Success;
            var responseJson = searchResponse.Body;
            Console.WriteLine(responseJson);
            Console.ReadLine();
        }
    }
}
