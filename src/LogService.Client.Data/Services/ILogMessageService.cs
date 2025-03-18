using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Contracts.Filters;

namespace LogService.Client.Data.Services;

public interface ILogMessageService : IDisposable
{
	LogMessageModel CreateLogMessage();
	Task<LogMessageListItemModel[]> GetItemsAsync();
	Task<LogMessageListItemModel[]> GetItemsAsync(LogMessageFilter filter);
	Task<LogMessageModel> GetItemAsync(Guid id);
	Task DeleteItemAsync(Guid id);
}