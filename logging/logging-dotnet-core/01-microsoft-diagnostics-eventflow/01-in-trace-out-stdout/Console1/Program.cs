using Microsoft.Diagnostics.EventFlow;
using System;

namespace Console1
{
    class Program
    {
        static void Main(string[] args)
        {

            var pipeline = DiagnosticPipelineFactory.CreatePipeline("eventFlowConfig.json");

            Console.WriteLine("First msg...");
            System.Diagnostics.Trace.TraceWarning("EventFlow is working!");
            Console.WriteLine("Trace sent stdoutput. Press any key to exit...");
            Console.ReadKey(intercept: true);

            pipeline.Dispose();
        }
    }
}
