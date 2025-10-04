using System;
using System.Threading.Tasks;
using LogService.Repository;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace LogService.Services;

public class LogMessageCleanUpService(IServiceProvider serviceProvider) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await CleanUp(stoppingToken).ConfigureAwait(false);
			await Task.Delay(TimeSpan.FromHours(24), stoppingToken).ConfigureAwait(false);
		}
	}

	private async Task CleanUp(CancellationToken stoppingToken)
	{
		using var scope = serviceProvider.CreateScope();

		var repository = scope.ServiceProvider.GetRequiredService<ILogMessageRepository>();
		var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

		var users = await userRepository
			.GetAll()
			.ToListAsync(stoppingToken)
			.ConfigureAwait(false);

		foreach (var user in users)
		{
			var retention = user.Settings.LogRetentionDays;
			var deleteBefore = DateTime.Now.AddDays(retention * -1);

			var logs = await repository
				.GetAll(user.Id)
				.Where(e => e.DateTime < deleteBefore)
				.ToListAsync(stoppingToken)
				.ConfigureAwait(false);

			foreach (var item in logs)
			{
				await repository.Delete(item.Id).ConfigureAwait(false);
				stoppingToken.ThrowIfCancellationRequested();
			}
		}
	}
}