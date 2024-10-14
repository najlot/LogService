using Cosei.Client.Base;

namespace LogService.Blazor.Services
{
	public interface ISubscriberProvider
	{
		Task<ISubscriber> GetSubscriber();
		Task ClearSubscriber();
	}
}
