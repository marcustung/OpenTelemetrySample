using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using OpenTelemetrySample.DistributeTracing.Models;

namespace OpenTelemetrySample.DistributeTracing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
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
                using (var client = new HttpClient())
                {
                    try
                    {
                        _logger.LogInformation("{Method} - was called ", "backend.Controllers.WeatherForecastController.Get");

                        HttpResponseMessage response = await client.GetAsync("https://localhost:44321/weatherforecast");
                        response.EnsureSuccessStatusCode();
                        var weather = await response.Content.ReadAsStringAsync();

                        _logger.LogInformation("Weather data - {data}", weather);

                        return weather;
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine("Message :{0} ", e.Message);
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to fetch weather data !", ex);
                throw;
            }
        }
    }
}
