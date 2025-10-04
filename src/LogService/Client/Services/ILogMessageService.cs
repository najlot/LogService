using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Models;
using LogService.Filters;

namespace LogService.Services;

public interface ILogMessageService : IDisposable
{
	LogMessageModel CreateLogMessage();
	Task<LogMessageListItemModel[]> GetItemsAsync();
	Task<LogMessageListItemModel[]> GetItemsAsync(LogMessageFilter filter);
	Task<LogMessageModel> GetItemAsync(Guid id);
	Task DeleteItemAsync(Guid id);
}