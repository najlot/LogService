using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Contracts.Filters;

namespace LogService.Client.Data.Repositories
{
	public interface ILogMessageRepository : IDisposable
	{
		Task<bool> DeleteItemAsync(Guid id);

		Task<LogMessageModel> GetItemAsync(Guid id);

		Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync(bool forceRefresh = false);
		Task<IEnumerable<LogMessageListItemModel>> GetItemsAsync(LogMessageFilter filter);
	}
}
