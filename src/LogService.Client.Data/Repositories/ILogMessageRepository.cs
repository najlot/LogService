using System;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Contracts.Filters;

namespace LogService.Client.Data.Repositories;

public interface ILogMessageRepository : IDisposable
{
	Task DeleteItemAsync(Guid id);

	Task<LogMessageModel> GetItemAsync(Guid id);

	Task<LogMessageListItemModel[]> GetItemsAsync();
	Task<LogMessageListItemModel[]> GetItemsAsync(LogMessageFilter filter);
}