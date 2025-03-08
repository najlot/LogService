using Najlot.Map;
using LogService.Client.Data.Models;
using LogService.Contracts;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Contracts.ListItems;

namespace LogService.Client.Data.Mappings;

internal sealed class LogMessageMappings
{
	public CreateLogMessage MapToCreate(IMap map, LogMessageModel from) =>
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
			map.From<LogArgumentModel>(from.Arguments).ToList<LogArgument>());

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
			map.From<LogArgumentModel>(from.Arguments).ToList<LogArgument>());

	public UpdateLogMessage MapToUpdate(IMap map, LogMessageModel from) =>
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
			map.From<LogArgumentModel>(from.Arguments).ToList<LogArgument>());

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
			map.From<LogArgumentModel>(from.Arguments).ToList<LogArgument>());

	public void MapToModel(IMap map, LogMessageListItem from, LogMessageListItemModel to)
	{
		to.Id = from.Id;
		to.DateTime = from.DateTime;
		to.LogLevel = from.LogLevel;
	}

	public void MapModelToModel(IMap map, LogMessageModel from, LogMessageListItemModel to)
	{
		to.Id = from.Id;
		to.DateTime = from.DateTime;
		to.LogLevel = from.LogLevel;
	}

	public void MapToModel(IMap map, LogMessage from, LogMessageModel to)
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
		to.Arguments = map.From<LogArgument>(from.Arguments).ToList<LogArgumentModel>();
	}

	public void MapToModel(IMap map, LogMessageUpdated from, LogMessageModel to)
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
		to.Arguments = map.From<LogArgument>(from.Arguments).ToList<LogArgumentModel>();
	}
}