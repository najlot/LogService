using System;
using System.Threading.Tasks;
using LogService.Models;
using LogService.Filters;

namespace LogService.Repositories;

public interface ILogMessageRepository : IDisposable
{
	Task DeleteItemAsync(Guid id);

	Task<LogMessageModel> GetItemAsync(Guid id);

	Task<LogMessageListItemModel[]> GetItemsAsync();
	Task<LogMessageListItemModel[]> GetItemsAsync(LogMessageFilter filter);
}