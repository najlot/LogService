using Najlot.Map;
using LogService.Contracts;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Contracts.ListItems;
using LogService.Service.Model;

namespace LogService.Service.Mappings;

internal class LogMessageMappings
{
	public LogMessageCreated MapToCreated(IMap map, LogMessageModel from) =>
		new(from.Id,
			from.DateTime,
			from.LogLevel,
			from.Category,
			from.State,
			from.Source,
			from.RawMessage,
			from.Message,
			from.Exception,
			from.ExceptionIsValid,
			from.Arguments);

	public LogMessageUpdated MapToUpdated(IMap map, LogMessageModel from) =>
		new(from.Id,
			from.DateTime,
			from.LogLevel,
			from.Category,
			from.State,
			from.Source,
			from.RawMessage,
			from.Message,
			from.Exception,
			from.ExceptionIsValid,
			from.Arguments);

	public void MapToModel(IMap map, CreateLogMessage from, LogMessageModel to)
	{
		to.Id = from.Id;
		to.DateTime = from.DateTime;
		to.LogLevel = from.LogLevel;
		to.Category = from.Category;
		to.State = from.State;
		to.Source = from.Source;
		to.RawMessage = from.RawMessage;
		to.Message = from.Message;
		to.Exception = from.Exception;
		to.ExceptionIsValid = from.ExceptionIsValid;
		to.Arguments = from.Arguments;
	}

	public void MapToModel(IMap map, UpdateLogMessage from, LogMessageModel to)
	{
		to.Id = from.Id;
		to.DateTime = from.DateTime;
		to.LogLevel = from.LogLevel;
		to.Category = from.Category;
		to.State = from.State;
		to.Source = from.Source;
		to.RawMessage = from.RawMessage;
		to.Message = from.Message;
		to.Exception = from.Exception;
		to.ExceptionIsValid = from.ExceptionIsValid;
		to.Arguments = map.From<LogArgument>(from.Arguments).ToList(to.Arguments);
	}

	public void MapToModel(IMap map, LogMessageModel from, LogMessage to)
	{
		to.Id = from.Id;
		to.DateTime = from.DateTime;
		to.LogLevel = from.LogLevel;
		to.Category = from.Category;
		to.State = from.State;
		to.Source = from.Source;
		to.RawMessage = from.RawMessage;
		to.Message = from.Message;
		to.Exception = from.Exception;
		to.ExceptionIsValid = from.ExceptionIsValid;
		to.Arguments = from.Arguments;
	}

	public void MapToModel(IMap map, LogMessageModel from, LogMessageListItem to)
	{
		to.Id = from.Id;
		to.DateTime = from.DateTime;
		to.LogLevel = from.LogLevel;
	}
}