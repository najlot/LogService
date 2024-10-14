using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Contracts.Filters;

namespace LogService.Client.Data.Services
{
	public interface ILogMessageService : IDisposable
	{
		LogMessageModel CreateLogMessage();
		Task<bool> AddItemAsync(LogMessageModel item);
		Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync(bool forceRefresh = false);
		Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync(LogMessageFilter filter);
		Task<LogMessageModel> GetItemAsync(Guid id);
		Task<bool> UpdateItemAsync(LogMessageModel item);
		Task<bool> DeleteItemAsync(Guid id);
	}
}
