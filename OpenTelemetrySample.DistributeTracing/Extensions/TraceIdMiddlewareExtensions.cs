using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OpenTelemetrySample.WeatherForecast.Client.Middleware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OpenTelemetrySample.WeatherForecast.Client.Extensions
{
    public static class TraceMiddlewareExtensions
    {
        public static void UseTraceId(this IApplicationBuilder app)
        {
            app.UseMiddleware<TraceMiddleware>();
        }
    }
}
