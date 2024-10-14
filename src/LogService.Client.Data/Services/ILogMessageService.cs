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
		Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync(bool forceRefresh = false);
		Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync(LogMessageFilter filter);
		Task<LogMessageModel> GetItemAsync(Guid id);
		Task<bool> DeleteItemAsync(Guid id);
	}
}
