using System;
using System.Threading.Tasks;
using LogService.Service.Repository;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace LogService.Service.Services
{
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

            await foreach (var user in userRepository.GetAll())
            {
                var retention = user.Settings.LogRetentionDays;
                var deleteBefore = DateTime.Now.AddDays(retention * -1);

                await foreach (var item in repository.GetAll(user.Id))
                {
                    if (item.DateTime < deleteBefore)
                    {
                        await repository.Delete(item.Id);
                    }

                    stoppingToken.ThrowIfCancellationRequested();
                }
            }
        }
	}
}