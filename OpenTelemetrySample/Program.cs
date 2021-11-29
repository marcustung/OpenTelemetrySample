using System;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace OpenTelemetrySample
{
    class Program
    {
        private static readonly ActivitySource MyActivitySource = new ActivitySource("MyCompany.MyProduct.MyLibrary");

        public static void Main()
        {
            using var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .SetSampler(new AlwaysOnSampler())
                .AddSource("MyCompany.MyProduct.MyLibrary")
                .AddConsoleExporter()
                .Build();

            using (var activity = MyActivitySource.StartActivity("SayHello"))
            {
                activity?.SetTag("foo", 1);
                activity?.SetTag("bar", "Hello, World!");
                activity?.SetTag("baz", new int[] { 1, 2, 3 });
            }
        }
    }
}
