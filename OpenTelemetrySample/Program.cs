using System;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

namespace OpenTelemetrySample
{
    class Program
    {
        private static readonly ActivitySource MyActivitySource = new ActivitySource("Company.Product.Library");

        public static void Main()
        {
            using var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("SampleService"))
                .SetSampler(new AlwaysOnSampler())
                .AddSource("Company.Product.Library")
                .AddConsoleExporter()
                .Build();

            using (var activity = MyActivitySource.StartActivity("SayHello"))
            {
                activity?.SetTag("foo", 1);
                activity?.SetTag("bar", "NET Conf 2021, Hej !");
                activity?.SetTag("baz", new int[] { 1, 2, 3 });
            }
        }
    }
}
