using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetrySample.WeatherForecast.Client.Extensions;
using System;
using System.Diagnostics;

namespace OpenTelemetrySample.DistributeTracing
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddOpenTelemetryTracing(Configuration);

            // External weather forecast API
            //services.AddHttpClient<IAzureMapWeatherSvc, AzureMapWeatherSvc>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseTraceId();
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddOpenTelemetryTracing(this IServiceCollection services, IConfiguration configuration)
        {
            var exporter = configuration.GetValue<string>("UseExporter").ToLowerInvariant();
            var zipkinServiceName = configuration.GetValue<string>("Zipkin:ServiceName");
            var zipkinEndpoint = configuration.GetValue<string>("Zipkin:Endpoint");
            var jaegerEndpoint = configuration.GetValue<string>("Jaeger:Endpoint");

            if (!String.IsNullOrEmpty(exporter) && exporter == "zipkin")
            {
                services.AddOpenTelemetryTracing((builder) => builder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(zipkinServiceName))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddZipkinExporter(zipkinOptions =>
                        {
                            zipkinOptions.Endpoint = new Uri($"{zipkinEndpoint}");
                        })
                        .AddJaegerExporter(option =>
                        {
                            option.Endpoint = new Uri($"{jaegerEndpoint}");
                        }
                        ));
            }
            else
            {
                services.AddOpenTelemetryTracing((builder) => builder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddConsoleExporter());
            }


            return services;
        }
    }
}
