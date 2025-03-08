using LogService.ClientBase.Models;
using System.Threading.Tasks;
using Cosei.Client.Base;
using LogService.ClientBase.Services.Implementation;
using LogService.Client.Data.Services;

namespace LogService.ClientBase.ProfileHandler;

public abstract class AbstractProfileHandler : IProfileHandler
{
	private IProfileHandler _handler = null;

	protected ISubscriber Subscriber { get; set; }

	protected ILogMessageService LogMessageService { get; set; }
	protected LogMessageMessagingService LogMessageMessagingService { get; set; }

	protected IUserService UserService { get; set; }
	protected UserMessagingService UserMessagingService { get; set; }
	public ILogMessageService GetLogMessageService() => LogMessageService ?? _handler?.GetLogMessageService();

	public IUserService GetUserService() => UserService ?? _handler?.GetUserService();
	public IProfileHandler SetNext(IProfileHandler handler) => _handler = handler;

	public async Task SetProfile(ProfileBase profile)
	{
		if (Subscriber != null)
		{
			await Subscriber.DisposeAsync();
			Subscriber = null;
		}

		LogMessageService?.Dispose();
		LogMessageService = null;
		LogMessageMessagingService?.Dispose();
		LogMessageMessagingService = null;

		UserService?.Dispose();
		UserService = null;
		UserMessagingService?.Dispose();
		UserMessagingService = null;

		await ApplyProfile(profile);

		if (_handler != null)
		{
			await _handler.SetProfile(profile);
		}
	}

	protected abstract Task ApplyProfile(ProfileBase profile);
}