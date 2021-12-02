using System;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System.Threading.Tasks;

namespace OpenTelemetrySample.API
{
    class Program
    {
        private static readonly ActivitySource MyActivitySource = new ActivitySource("Company.Product.Library", "1.0");
        static void Main(string[] args)
        {
            using var tracerProvider = Sdk.CreateTracerProviderBuilder()
               .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyService"))
               .AddSource("Company.Product.Library")
               .AddZipkinExporter(zipkinOptions =>
               {
                   zipkinOptions.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
               })               
               .Build();

            DoSomeWork();
            Console.WriteLine("Example work done"); 
        }

        static void DoSomeWork()
        {
            using (Activity a = MyActivitySource.StartActivity("SomeWork"))
            {
                StepOne();
                StepTwo();
            }
        }

        static void StepOne()
        {
            using (Activity a = MyActivitySource.StartActivity("StepOne"))
            {
                Task.Delay(500);
            }
        }

        static void StepTwo()
        {
            using (Activity a = MyActivitySource.StartActivity("StepTwo"))
            {
                Task.Delay(1000);
            }
        }
    }
}
