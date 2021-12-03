using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetrySample.DistributeTracing.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenTelemetrySample.DistributeTracing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private static ActivitySource _activitySource = new ActivitySource(nameof(WeatherForecastController));

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            try
            {
                _logger.LogInformation("{Method} - was called ", "backend.Controllers.WeatherForecastController.Get");
                var weather = await GetWeatherStaticData();
                _logger.LogInformation("Weather data - {data}", weather);

                return weather;
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to fetch weather data !", ex);
                throw;
            }
        }

        private async Task<string> GetWeatherStaticData()
        {
            var rng = new Random();
            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            return JsonSerializer.Serialize(forecasts);
        }

    }
}
