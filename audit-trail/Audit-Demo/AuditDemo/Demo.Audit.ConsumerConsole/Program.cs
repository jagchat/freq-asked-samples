using Confluent.Kafka;
using Demo.MongoDb;
using System;
using System.Threading;
using System.Linq;

namespace Demo.Audit.ConsumerConsole
{
    //TODO: refactor as necessary
    class Program
    {
        static void Main(string[] args)
        {
            //TestInsertAuditEntry();
            //TestFetchAuditEntry();
            Console.WriteLine("Starting Listener...");
            KickOffListener();
            Console.WriteLine("STOPPED!");
            Console.ReadLine();
        }

        static void TestInsertAuditEntry()
        {
            var rep = new AuditRepository();
            var s = @"
                {
	                ""TenantInfo"": {
		                ""Name"":""Test""
	                },
	                ""AppInfo"": {
		                ""Name"":""TCP""
	                },
	                ""RecordInfo"": {
		                ""EntityType"":""Demo.Data.Models.Employee"",
		                ""TableName"":""Emp""
	                },
	                ""Key"":""Id"",
	                ""Value"":""1003"",
	                ""UpdateDate"":""2021-11-18T12:52:43.9461415-06:00"",
	                ""Data"": {
		                ""Id"":1003,
		                ""Name"":""Paula"",
		                ""Salary"":5600,
		                ""DeptId"":10
	                }
                }
            ";
            rep.AddBsonEntry(s);
        }

        static void TestFetchAuditEntry()
        {
            var rep = new AuditRepository();
            var s = @"
                {
	                ""_v.TenantInfo.Name"" : ""Test"",
                    ""_v.AppInfo.Name"": ""TCP"",
                    ""_v.Key"": ""Id""
                }
            ";
            var result = rep.FindEntries(s);
            if (result.Count == 0)
            {
                Console.WriteLine("No entries found!");
            }
            else
            {
                Console.WriteLine("Entries found..");
                foreach (var item in result)
                {
                    Console.WriteLine($"{item["_v"].GetValue("Value", null)} - {item["_v"].GetValue("Data", null)}");
                }
            }
        }

        static void KickOffListener()
        {
            //TODO: refactor as necessary
            var rep = new AuditRepository();
            var conf = new ConsumerConfig
            {
                GroupId = "demo-consumer-group", //TODO: configurable
                BootstrapServers = "localhost:9092", //TODO: configurable
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // earliest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                c.Subscribe("topic_sample"); //TODO: configurable

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };
                
                Console.WriteLine("Ready...");

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            var msg = cr.Message.Value;
                            Console.WriteLine($"\n---\nConsumed message '{msg}' at: '{cr.TopicPartitionOffset}'.");
                            rep.AddBsonEntry(msg);
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }
        }
    }
}
