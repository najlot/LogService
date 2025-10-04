using Cosei.Client.Base;
using Cosei.Client.Http;
using Cosei.Service.Base;
using Cosei.Service.Http;
using Cosei.Service.RabbitMq;
using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using Najlot.Log;
using Najlot.Log.Configuration.FileSource;
using Najlot.Log.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using LogService.Configuration;
using LogService.Identity;
using LogService.Mappings;
using LogService.Repository;
using LogService.Services;
using LogService.Services.Implementation;
using LogService.Client.Data;
using LogService.Client.Data.Identity;
using LogService.Client.Data.Repositories;
using LogService.Client.Data.Repositories.Implementation;
using LogService.Client.Data.Services;
using LogService.Client.Data.Services.Implementation;

namespace LogService;

public class Program
{
	private static void LogErrorOccured(object? sender, LogErrorEventArgs e)
	{
		Console.WriteLine(e.Message + Environment.NewLine + e.Exception);
	}

	public static void Main(string[] args)
	{
		var configFolderPath = Path.GetFullPath("config");
		var configPath = Path.Combine(configFolderPath, "Log.config");
		configPath = Path.GetFullPath(configPath);
		Directory.CreateDirectory(configFolderPath);

		LogErrorHandler.Instance.ErrorOccured += LogErrorOccured;

		LogAdministrator.Instance
			.SetLogLevel(Najlot.Log.LogLevel.Debug)
			.SetCollectMiddleware<ConcurrentCollectMiddleware, FileDestination>()
			.SetCollectMiddleware<ConcurrentCollectMiddleware, ConsoleDestination>()
			.AddConsoleDestination(useColors: true)
			.AddFileDestination(
				Path.Combine("logs", "log.txt"),
				30,
				Path.Combine("logs", ".logs"),
				true)
			.ReadConfigurationFromXmlFile(configPath, true, true);

		var builder = WebApplication.CreateBuilder(args);
		var map = new Najlot.Map.Map();
		
		// Register both mapping extensions
		LogService.Mappings.ServiceCollectionExtensions.RegisterDataMappings(map);
		LogService.Client.Data.MapRegisterExtensions.RegisterDataMappings(map);
		builder.Services.AddSingleton(map);

		// Configure Logging
		builder.Logging.ClearProviders();
		builder.Logging.AddNajlotLog(LogAdministrator.Instance);

		// Add services to the container.
		// Configure database and repository storage
		var rmqConfig = builder.Configuration.ReadConfiguration<RabbitMqConfiguration>();
		var fileConfig = builder.Configuration.ReadConfiguration<FileConfiguration>();
		var mysqlConfig = builder.Configuration.ReadConfiguration<MySqlConfiguration>();
		var mongoDbConfig = builder.Configuration.ReadConfiguration<MongoDbConfiguration>();
		var serviceConfig = builder.Configuration.ReadConfiguration<ServiceConfiguration>();

		if (string.IsNullOrWhiteSpace(serviceConfig?.Secret))
		{
			throw new Exception($"Please set {nameof(ServiceConfiguration.Secret)} in the {nameof(ServiceConfiguration)}!");
		}

		builder.Services.AddSingleton(serviceConfig);

		if (mongoDbConfig != null)
		{
			builder.Services.AddSingleton(mongoDbConfig);
			builder.Services.AddSingleton<MongoDbContext>();
			builder.Services.AddScoped<Repository.IUserRepository, MongoDbUserRepository>();
			builder.Services.AddScoped<Repository.ILogMessageRepository, MongoDbLogMessageRepository>();
		}
		else if (mysqlConfig != null)
		{
			builder.Services.AddSingleton(mysqlConfig);
			builder.Services.AddScoped<MySqlDbContext>();
			builder.Services.AddScoped<Repository.IUserRepository, MySqlUserRepository>();
			builder.Services.AddScoped<Repository.ILogMessageRepository, MySqlLogMessageRepository>();
		}
		else
		{
			builder.Services.AddSingleton(fileConfig ?? new FileConfiguration());
			builder.Services.AddScoped<Repository.IUserRepository, FileUserRepository>();
			builder.Services.AddScoped<Repository.ILogMessageRepository, FileLogMessageRepository>();
		}

		if (rmqConfig != null)
		{
			rmqConfig.QueueName = "LogService";
			builder.Services.AddCoseiRabbitMq(rmqConfig);
		}

		builder.Services.AddCoseiHttp();

		// Add JWT Authentication
		var validationParameters = Services.TokenService.GetValidationParameters(serviceConfig.Secret);
		builder.Services.AddAuthentication(x =>
		{
			x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(x =>
		{
			x.RequireHttpsMetadata = false;
			x.TokenValidationParameters = validationParameters;
		})
		.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

		builder.Services.AddAuthorization();
		builder.Services.AddSignalR();

		// Backend services
		builder.Services.AddScoped<Services.IUserService, Services.UserService>();
		builder.Services.AddScoped<Services.LogMessageService>();
		builder.Services.AddScoped<Services.TokenService>();
		builder.Services.AddHostedService<LogMessageCleanUpService>();

		// Client services for Blazor UI
		builder.Services.AddScoped<IRequestClient, HttpFactoryRequestClient>();
		builder.Services.AddScoped<Client.Data.Identity.ITokenService, Client.Data.Identity.TokenService>();
		builder.Services.AddScoped<ITokenProvider, RefreshingTokenProvider>();
		builder.Services.AddScoped<IUserDataStore, UserDataStore>();
		builder.Services.AddScoped<IRegistrationService, RegistrationService>();
		builder.Services.AddScoped<Client.Data.Repositories.IUserRepository, Client.Data.Repositories.Implementation.UserRepository>();
		builder.Services.AddScoped<Client.Data.Services.IUserService, Client.Data.Services.Implementation.UserService>();
		builder.Services.AddScoped<Client.Data.Repositories.ILogMessageRepository, Client.Data.Repositories.Implementation.LogMessageRepository>();
		builder.Services.AddScoped<Client.Data.Services.ILogMessageService, Client.Data.Services.Implementation.LogMessageService>();
		builder.Services.AddScoped<ISubscriberProvider, SubscriberProvider>();

		builder.Services.AddLocalization();

		builder.Services.AddRazorPages();
		builder.Services.AddServerSideBlazor();
		builder.Services.AddControllers();

		builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationService>();
		builder.Services.AddScoped(c => (IAuthenticationService)c.GetRequiredService<AuthenticationStateProvider>());

		var app = builder.Build();
		var serviceProvider = app.Services;
		map.RegisterFactory(t =>
		{
			if (t.GetConstructor(Type.EmptyTypes) is not null)
			{
				return Activator.CreateInstance(t) ?? throw new NullReferenceException($"Could not create {t.FullName}. Result is null.");
			}

			return serviceProvider.GetRequiredService(t);
		});

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseCors(c =>
		{
			c.AllowAnyOrigin();
			c.AllowAnyMethod();
			c.AllowAnyHeader();
		});

		app.UseHttpsRedirection();

		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();
		app.MapRazorPages();
		app.MapBlazorHub();
		app.MapHub<CoseiHub>("/cosei");
		app.MapFallbackToPage("/_Host");

		app.UseCosei();

		var supportedCultures = new[] { "en", "de" };
		var localizationOptions = new RequestLocalizationOptions()
			.SetDefaultCulture(supportedCultures[0])
			.AddSupportedCultures(supportedCultures)
			.AddSupportedUICultures(supportedCultures);

		app.UseRequestLocalization(localizationOptions);

		// Ensure database is created
		using (var scope = app.Services.CreateScope())
		{
			scope.ServiceProvider.GetService<MySqlDbContext>()?.Database?.EnsureCreated();
		}

		app.Run();

		LogAdministrator.Instance.Dispose();
	}
}