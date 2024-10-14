using Cosei.Service.Base;
using Cosei.Service.Http;
using Cosei.Service.RabbitMq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using LogService.Service.Configuration;
using LogService.Service.Repository;
using LogService.Service.Services;

namespace LogService.Service
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			var rmqConfig = ConfigurationReader.ReadConfiguration<RabbitMqConfiguration>();
			var fileConfig = ConfigurationReader.ReadConfiguration<FileConfiguration>();
			var mysqlConfig = ConfigurationReader.ReadConfiguration<MySqlConfiguration>();
			var mongoDbConfig = ConfigurationReader.ReadConfiguration<MongoDbConfiguration>();
			var serviceConfig = ConfigurationReader.ReadConfiguration<ServiceConfiguration>();

			if (string.IsNullOrWhiteSpace(serviceConfig?.Secret))
			{
				throw new Exception($"Please set {nameof(ServiceConfiguration.Secret)} in the {nameof(ServiceConfiguration)}!");
			}

			services.AddSingleton(serviceConfig);

			if (mongoDbConfig != null)
			{
				services.AddSingleton(mongoDbConfig);
				services.AddScoped(sp => new MongoDbContext(sp.GetRequiredService<MongoDbConfiguration>()));
				services.AddScoped<IUserRepository, MongoDbUserRepository>();
				services.AddScoped<ILogMessageRepository, MongoDbLogMessageRepository>();
			}
			else if (mysqlConfig != null)
			{
				services.AddSingleton(mysqlConfig);
				services.AddScoped<IUserRepository, MySqlUserRepository>();
				services.AddScoped<ILogMessageRepository, MySqlLogMessageRepository>();
				services.AddScoped<MySqlDbContext>();
			}
			else
			{
				if (fileConfig == null) fileConfig = new FileConfiguration();

				services.AddSingleton(fileConfig);
				services.AddScoped<IUserRepository, FileUserRepository>();
				services.AddScoped<ILogMessageRepository, FileLogMessageRepository>();
			}

			if (rmqConfig != null)
			{
				rmqConfig.QueueName = "LogService.Service";
				services.AddCoseiRabbitMq(rmqConfig);
			}

			services.AddCoseiHttp();

			services.AddScoped<IUserService, UserService>();
			services.AddScoped<LogMessageService>();
			services.AddScoped<TokenService>();
			services.AddSignalR();

			var validationParameters = TokenService.GetValidationParameters(serviceConfig.Secret);

			services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(x =>
			{
				x.RequireHttpsMetadata = false;
				x.TokenValidationParameters = validationParameters;
			});

			services.AddControllers();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			app.UseCors(c =>
			{
				c.AllowAnyOrigin();
				c.AllowAnyMethod();
				c.AllowAnyHeader();
			});

			app.UseAuthentication();
			// app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHub<CoseiHub>("/cosei");
			});

			app.UseCosei();

			using var scope = app.ApplicationServices.CreateScope();
			scope.ServiceProvider.GetService<MySqlDbContext>()?.Database?.EnsureCreated();
		}
	}
}
