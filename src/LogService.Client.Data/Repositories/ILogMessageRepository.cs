using System;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Contracts.Filters;

namespace LogService.Client.Data.Repositories;

public interface ILogMessageRepository : IDisposable
{
	Task<LogMessageListItemModel[]> GetItemsAsync();

	Task<LogMessageListItemModel[]> GetItemsAsync(LogMessageFilter filter);

	Task<LogMessageModel> GetItemAsync(Guid id);

	Task AddItemAsync(LogMessageModel item);

	Task UpdateItemAsync(LogMessageModel item);

	Task DeleteItemAsync(Guid id);
}