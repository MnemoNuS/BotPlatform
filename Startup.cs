using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GoogleSheetsLib;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ViberBot;
using ViberBot.Client;
using ViberBot.Client.Handlers;
using ViberBot.Services;

namespace BotPlatform
{
	public class Startup
	{
		private bool _webhookInitilized;
		private ViberBotClient _viberClient;
		private GoogleSheetsClient _googleSheetsClient;

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers().AddNewtonsoftJson();
			services.Configure<ClientSettings>(Configuration.GetSection("ViberBotSettings"));
			services.AddTransient<IWebhookService, WebhookService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		[Obsolete]
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<ClientSettings> clientSettings, Microsoft.Extensions.Hosting.IApplicationLifetime lifetime)
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

			lifetime.ApplicationStarted.Register(() => OnApplicationStarted(clientSettings.Value));

			app.Run(async (context) =>
			{
				OnApplicationStarted(clientSettings.Value);
			});
		}

		public async void OnApplicationStarted(ClientSettings clientSettings)
		{
			// take from settings
			if (_viberClient == null)
				_viberClient = ViberBotClient.Init(clientSettings.Token, clientSettings);
			if (_googleSheetsClient == null)
			{
				_googleSheetsClient = GoogleSheetsHandler.InitGoogleSheetsClient(clientSettings.ApplicationName, clientSettings.SpreadsheetId, "google_api_client_secrets.json");
			}
			if (!_viberClient.WebhookReady)
			{
				var url = clientSettings.Webhook;
				var result = await ViberBotClient.GetInstance().SetWebhookAsync($"{url}");
			}
		}

	}
}
