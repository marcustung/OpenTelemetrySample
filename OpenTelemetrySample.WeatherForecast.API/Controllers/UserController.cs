using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenTelemetrySample.WeatherForecast.API.Models;
using System.Diagnostics;

namespace OpenTelemetrySample.WeatherForecast.API.Controllers
{
    [Route("api/[controller]/[action]")] 
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private static readonly ActivitySource _source = new ActivitySource("OpenTelemetry.Instrumentation.AspNetCore");
        public UserController(DataContext context)
        {
            _context = context;
        }
       
        [HttpGet]
        public bool Get()
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            Activity.ForceDefaultIdFormat = true;
          
            using (Activity a = _source.StartActivity("Insert Data - User"))
            {
                var testUser1 = new User
                {
                    Id = "abc123",
                    FirstName = "Luke",
                    LastName = "Skywalker"
                };
                _context.Users.Add(testUser1);
                _context.SaveChanges();
            }

            using (Activity a = _source.StartActivity("Insert Data - User"))
            {
                var testPost1 = new Post
                {
                    Id = "def234",
                    UserId = "abc123",
                    Content = "What a piece of junk!"
                };

                _context.Posts.Add(testPost1);
                _context.SaveChanges();
            }

            return true;
        }
    }
}
