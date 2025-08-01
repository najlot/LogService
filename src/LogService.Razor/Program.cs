using Cosei.Client.Base;
using Cosei.Client.Http;
using Najlot.Log.Destinations;
using Najlot.Log.Middleware;
using Najlot.Log;
using Najlot.Log.Configuration.FileSource;
using Najlot.Log.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using LogService.Razor.Services;
using LogService.Client.Data;
using LogService.Client.Data.Identity;
using LogService.Client.Data.Repositories;
using LogService.Client.Data.Repositories.Implementation;
using LogService.Client.Data.Services;
using LogService.Client.Data.Services.Implementation;

namespace LogService.Razor;

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
		builder.Services.AddSingleton(map.RegisterDataMappings());

		// Configure Logging
		builder.Logging.ClearProviders();
		builder.Logging.AddNajlotLog(LogAdministrator.Instance);

		// Add services to the container.
		var dataServiceUrl = builder.Configuration.GetSection("DataServiceUrl")?.Get<string>() ?? throw new InvalidOperationException("DataServiceUrl not found.");

		builder.Services.AddHttpClient(Options.DefaultName, c =>
		{
			c.BaseAddress = new Uri(dataServiceUrl);
		});

		// Add HttpContextAccessor for cookie-based auth
		builder.Services.AddHttpContextAccessor();

		// Configure cookie-based authentication
		builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
			{
				options.LoginPath = "/Account/Login";
				options.LogoutPath = "/Account/Logout";
				options.AccessDeniedPath = "/Account/AccessDenied";
				options.ExpireTimeSpan = TimeSpan.FromHours(24);
				options.SlidingExpiration = true;
				options.Cookie.HttpOnly = true;
				options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
			});
		
		builder.Services.AddAuthorization();

		builder.Services.AddLocalization();

		builder.Services.AddRazorPages();
		builder.Services.AddSignalR();

		// Add custom services
		builder.Services.AddScoped<IRequestClient, HttpFactoryRequestClient>();
		builder.Services.AddScoped<ITokenService, TokenService>();
		builder.Services.AddScoped<ITokenProvider, CookieTokenProvider>();
		builder.Services.AddScoped<IUserDataStore, CookieUserDataStore>();

		builder.Services.AddScoped<IRegistrationService, RegistrationService>();
		builder.Services.AddScoped<IUserRepository, UserRepository>();
		builder.Services.AddScoped<IUserService, UserService>();
		builder.Services.AddScoped<ILogMessageRepository, LogMessageRepository>();
		builder.Services.AddScoped<ILogMessageService, LogMessageService>();

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
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapRazorPages();
		app.MapHub<LogHub>("/loghub");

		var supportedCultures = new[] { "en", "de" };
		var localizationOptions = new RequestLocalizationOptions()
			.SetDefaultCulture(supportedCultures[0])
			.AddSupportedCultures(supportedCultures)
			.AddSupportedUICultures(supportedCultures);

		app.UseRequestLocalization(localizationOptions);

		app.Run();

		LogAdministrator.Instance.Dispose();
	}
}