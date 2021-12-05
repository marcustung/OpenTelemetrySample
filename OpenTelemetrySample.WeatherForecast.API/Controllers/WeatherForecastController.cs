using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OpenTelemetrySample.WeatherForecast.API.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace OpenTelemetrySample.WeatherForecast.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConnectionMultiplexer multiplexer)
        {
            _logger = logger;
            _connectionMultiplexer = multiplexer;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()        
        {
            var rnd = new Random();
            var test = rnd.Next(1, 3);

            if (test % 2 == 0 )
            {
                Thread.Sleep(test * 3000);
            }

            var key = Guid.NewGuid().ToString();
            var redis = _connectionMultiplexer.GetDatabase();
            redis.StringSet(key, key);

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
