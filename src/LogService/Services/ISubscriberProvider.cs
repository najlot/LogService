using Cosei.Client.Base;

namespace LogService.Services;

public interface ISubscriberProvider
{
	Task<ISubscriber> GetSubscriber();
	Task ClearSubscriber();
}