using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ViberBot;
using ViberBotLib.Models.Response;

namespace BotPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
		private static readonly string[] Summaries = new[]
{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WebhookController> _logger;

		public WebhookController(ILogger<WebhookController> logger)
		{
			_logger = logger;
		}

		[HttpPost]
		public ActionResult Post([FromBody]CallbackData data)
		{
			var client = ViberBotClient.GetInstance();
			client.OnCallback(data);
			return Ok();
		}

		[HttpGet]
		public IEnumerable<WeatherForecast> Get()
		{
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