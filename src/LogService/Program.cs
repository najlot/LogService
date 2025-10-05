using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using Najlot.Log;
using Najlot.Log.Configuration.FileSource;
using Najlot.Log.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Najlot.Map;
using LogService.Repository;
using LogService.Services;
using LogService.Configuration;
using LogService.Identity;
using LogService.Mappings;

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

		var map = new Map();
		map.RegisterDataMappings();
		builder.Services.AddSingleton<IMap>(map);

		// Configure Logging
		builder.Logging.ClearProviders();
		builder.Logging.AddNajlotLog(LogAdministrator.Instance);

		// Read configuration from Startup.cs
		var fileConfig = builder.Configuration.ReadConfiguration<LiteDbConfiguration>();
		var mysqlConfig = builder.Configuration.ReadConfiguration<MySqlConfiguration>();
		var mongoDbConfig = builder.Configuration.ReadConfiguration<MongoDbConfiguration>();
		var serviceConfig = builder.Configuration.ReadConfiguration<ServiceConfiguration>();

		if (string.IsNullOrWhiteSpace(serviceConfig?.Secret))
		{
			throw new Exception($"Please set {nameof(ServiceConfiguration.Secret)} in the {nameof(ServiceConfiguration)}!");
		}

		builder.Services.AddSingleton(serviceConfig);

		// Storage backend selection from Startup.cs
		if (mongoDbConfig != null)
		{
			builder.Services.AddSingleton(mongoDbConfig);
			builder.Services.AddSingleton<MongoDbContext>();
			builder.Services.AddScoped<IUserRepository, MongoDbUserRepository>();
			builder.Services.AddScoped<ILogMessageRepository, MongoDbLogMessageRepository>();
		}
		else if (mysqlConfig != null)
		{
			builder.Services.AddSingleton(mysqlConfig);
			builder.Services.AddScoped<MySqlDbContext>();
			builder.Services.AddScoped<IUserRepository, MySqlUserRepository>();
			builder.Services.AddScoped<ILogMessageRepository, MySqlLogMessageRepository>();
		}
		else
		{
			builder.Services.AddSingleton(fileConfig ?? new LiteDbConfiguration());
			builder.Services.AddSingleton<LiteDbContext>();
			builder.Services.AddScoped<IUserRepository, LiteDbUserRepository>();
			builder.Services.AddScoped<ILogMessageRepository, LiteDbLogMessageRepository>();
		}

		// JWT Authentication from Startup.cs
		var validationParameters = TokenService.GetValidationParameters(serviceConfig.Secret);

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
		.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme); // Keep cookie auth for Blazor

		builder.Services.AddAuthorization();

		builder.Services.AddLocalization();

		builder.Services.AddRazorPages();
		builder.Services.AddServerSideBlazor();

		// Add controllers from Startup.cs
		builder.Services.AddControllers();

		// Add SignalR from Startup.cs
		builder.Services.AddSignalR();

		builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationService>();
		builder.Services.AddScoped(c => (IAuthenticationService)c.GetRequiredService<AuthenticationStateProvider>());

		builder.Services.AddScoped<ITokenService, TokenService>();
		builder.Services.AddScoped<IUserDataStore, UserDataStore>();

		// Service registrations from Startup.cs - update existing ones to use local Blazor classes
		builder.Services.AddScoped<IUserService, UserService>();
		builder.Services.AddScoped<LogMessageService>();
		builder.Services.AddScoped<TokenService>();

		builder.Services.AddScoped<ILogMessageService, LogMessageService>();

		// Register messaging services
		builder.Services.AddSingleton<IMessenger, Messenger>();
		
		// Add hosted service from Startup.cs
		builder.Services.AddHostedService<LogMessageCleanUpService>();

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

		app.UseHttpsRedirection();

		app.UseStaticFiles();

		app.UseRouting();

		// CORS configuration from Startup.cs
		app.UseCors(c =>
		{
			c.AllowAnyOrigin();
			c.AllowAnyMethod();
			c.AllowAnyHeader();
		});

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();
		app.MapRazorPages();
		app.MapBlazorHub();
		app.MapFallbackToPage("/_Host");

		var supportedCultures = new[] { "en", "de" };
		var localizationOptions = new RequestLocalizationOptions()
			.SetDefaultCulture(supportedCultures[0])
			.AddSupportedCultures(supportedCultures)
			.AddSupportedUICultures(supportedCultures);

		app.UseRequestLocalization(localizationOptions);

		// Database initialization from Startup.cs
		using var scope = app.Services.CreateScope();
		scope.ServiceProvider.GetService<MySqlDbContext>()?.Database?.EnsureCreated();

		app.Run();

		LogAdministrator.Instance.Dispose();
	}
}