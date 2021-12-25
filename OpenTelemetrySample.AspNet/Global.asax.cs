using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace OpenTelemetrySample.AspNet
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private IDisposable tracerProvider;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            var builder = Sdk.CreateTracerProviderBuilder()
                 .AddAspNetInstrumentation()
                 .AddHttpClientInstrumentation();

            switch (ConfigurationManager.AppSettings["UseExporter"].ToLowerInvariant())
            {
                case "jaeger":
                    builder.AddJaegerExporter(jaegerOptions =>
                    {
                        jaegerOptions.AgentHost = ConfigurationManager.AppSettings["JaegerHost"];
                        jaegerOptions.AgentPort = int.Parse(ConfigurationManager.AppSettings["JaegerPort"]);
                    });
                    break;
                case "zipkin":
                    builder.AddZipkinExporter(zipkinOptions =>
                    {
                        zipkinOptions.Endpoint = new Uri(ConfigurationManager.AppSettings["ZipkinEndpoint"]);
                    });
                    break;
                case "otlp":
                    builder.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(ConfigurationManager.AppSettings["OtlpEndpoint"]);
                    });
                    break;
                default:
                    builder.AddConsoleExporter();
                    break;
            }

            this.tracerProvider = builder.Build();
        }
    }
}
