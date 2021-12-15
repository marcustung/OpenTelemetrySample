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
               .AddConsoleExporter()
               .Build();

            DoSomeWork();

            Console.WriteLine("Example work done"); 
        }

        static void DoSomeWork()
        {
            using (var a = MyActivitySource.StartActivity("SomeWork"))
            {
                StepOne();
                StepTwo();
            }

            using (var activity = MyActivitySource.StartActivity("ActivityRequest", ActivityKind.Server))
            {
                activity?.SetTag("http.method", "GET");
                if (activity != null && activity.IsAllDataRequested == true)
                {
                    activity.SetTag("http.url", "http://www.mywebsite.com");
                }
            }
        }

        static void StepOne()
        {
            using (var a = MyActivitySource.StartActivity("StepOne"))
            {
                Task.Delay(500);
            }
        }

        static void StepTwo()
        {
            using (var a = MyActivitySource.StartActivity("StepTwo"))
            {
                Task.Delay(1000);
            }
        }
    }
}
