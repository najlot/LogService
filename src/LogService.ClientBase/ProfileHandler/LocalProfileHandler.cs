using System.Threading.Tasks;
using Najlot.Map;
using LogService.Client.Data.Services.Implementation;
using LogService.Client.MVVM;
using LogService.ClientBase.Models;
using LogService.ClientBase.Services.Implementation;

namespace LogService.ClientBase.ProfileHandler;

public sealed class LocalProfileHandler : AbstractProfileHandler
{
	private readonly IMessenger _messenger;
	private readonly IDispatcherHelper _dispatcher;
	private readonly IMap _map;

	public LocalProfileHandler(IMessenger messenger, IDispatcherHelper dispatcher, IMap map)
	{
		_messenger = messenger;
		_dispatcher = dispatcher;
		_map = map;
	}

	protected override async Task ApplyProfile(ProfileBase profile)
	{
		if (profile is LocalProfile localProfile)
		{
			var subscriber = new LocalSubscriber();

			var userStore = new LocalUserStore(localProfile.FolderName, subscriber);
			UserService = new UserService(userStore);
			UserMessagingService = new UserMessagingService(_messenger, _dispatcher, subscriber);
			var logMessageStore = new LocalLogMessageStore(localProfile.FolderName, subscriber, _map);
			LogMessageService = new LogMessageService(logMessageStore);
			LogMessageMessagingService = new LogMessageMessagingService(_messenger, _dispatcher, subscriber);

			await subscriber.StartAsync();

			Subscriber = subscriber;
		}
	}
}