using LogService.Contracts.Commands;
using LogService.Contracts.Filters;
using LogService.Model;

namespace LogService.Services;
public interface ILogMessageService
{
	Task CreateLogMessages(CreateLogMessage[] commands, string source, Guid userId);
	Task<LogMessageModel?> GetItemAsync(Guid id);
	Task<LogMessageListItemModel[]> GetItemsAsync();
	Task<LogMessageListItemModel[]> GetItemsAsync(LogMessageFilter filter);
}