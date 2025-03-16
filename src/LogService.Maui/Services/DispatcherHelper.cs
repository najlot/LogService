using LogService.Client.MVVM;

namespace LogService.Maui.Services;

public class DispatcherHelper : IDispatcherHelper
{
	private readonly IDispatcher _dispatcher;

	public DispatcherHelper(IDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}

	public void BeginInvokeOnMainThread(Action action)
	{
		_dispatcher.Dispatch(action);
	}

	public async Task BeginInvokeOnMainThread(Func<Task> action)
	{
		await _dispatcher.DispatchAsync(action);
	}
}