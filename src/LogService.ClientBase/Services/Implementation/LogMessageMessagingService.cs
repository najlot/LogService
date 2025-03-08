using Cosei.Client.Base;
using LogService.Client.MVVM;
using LogService.Contracts.Events;
using System;
using System.Threading.Tasks;

namespace LogService.ClientBase.Services.Implementation;

public class LogMessageMessagingService
{
	private readonly IMessenger _messenger;
	private readonly IDispatcherHelper _dispatcher;
	private readonly ISubscriber _subscriber;

	public LogMessageMessagingService(
		IMessenger messenger,
		IDispatcherHelper dispatcher,
		ISubscriber subscriber)
	{
		_messenger = messenger;
		_dispatcher = dispatcher;
		_subscriber = subscriber;

		subscriber.Register<LogMessageCreated>(Handle);
		subscriber.Register<LogMessageUpdated>(Handle);
		subscriber.Register<LogMessageDeleted>(Handle);
	}

	private async Task Handle(LogMessageCreated message)
	{
		await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
	}

	private async Task Handle(LogMessageUpdated message)
	{
		await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
	}

	private async Task Handle(LogMessageDeleted message)
	{
		await _dispatcher.BeginInvokeOnMainThread(async () => await _messenger.SendAsync(message));
	}

	private bool _disposedValue = false;

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			_disposedValue = true;

			if (disposing)
			{
				_subscriber.Unregister(this);
			}
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}