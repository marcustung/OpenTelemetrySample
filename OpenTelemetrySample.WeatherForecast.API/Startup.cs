using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetrySample.WeatherForecast.API.Models;
using StackExchange.Redis;
using System;

namespace OpenTelemetrySample.WeatherForecast.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddOpenTelemetryTracing(Configuration);

            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("DemoDB"));

            services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = "Redis Cache";
                options.Configuration = "localhost:6379";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }


    static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddOpenTelemetryTracing(this IServiceCollection services, IConfiguration configuration)
        {
            var zipkinServiceName = configuration.GetValue<string>("Zipkin:ServiceName");
            var zipkinEndpoint = configuration.GetValue<string>("Zipkin:Endpoint");
            var redisEndpoint = configuration.GetValue<string>("Redis:Endpoint");

            var connection = ConnectionMultiplexer.Connect(redisEndpoint);
            services.AddSingleton<IConnectionMultiplexer>(connection);
            
            services.AddOpenTelemetryTracing((builder) => builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(zipkinServiceName))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRedisInstrumentation(connection)
                    .AddEntityFrameworkCoreInstrumentation((config) =>
                    {
                        config.SetDbStatementForStoredProcedure = true;
                        config.SetDbStatementForText = true;
                    })
                    .AddZipkinExporter(zipkinOptions =>
                    {
                        zipkinOptions.Endpoint = new Uri($"{zipkinEndpoint}");
                    })
                    .AddConsoleExporter());

            return services;
        }
    }
}