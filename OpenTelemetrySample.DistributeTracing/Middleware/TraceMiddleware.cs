using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OpenTelemetrySample.WeatherForecast.Client.Middleware
{
    public class TraceMiddleware
    {
        private readonly RequestDelegate _next;

        public TraceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = Activity.Current?.Id;
            context.Response.Headers.Add("TraceId", traceId);
            await _next(context);
        }
    }
}
