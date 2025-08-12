using Najlot.Map;
using LogService.Client.Data.Models;
using LogService.Contracts;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Contracts.ListItems;
using System.Collections.Generic;

namespace LogService.Client.Data.Mappings;

internal sealed class LogMessageMappings
{
	public void MapToCreate(IMap map, LogMessageModel from, CreateLogMessage to)
	{
		to.DateTime = from.DateTime;
		to.LogLevel = (int)from.LogLevel;
		to.Category = from.Category;
		to.State = from.State;
		to.RawMessage = from.RawMessage;
		to.Message = from.Message;
		to.Exception = from.Exception;
		to.ExceptionIsValid = from.ExceptionIsValid;
		to.Arguments = map.From<LogArgumentModel>(from.Arguments).ToArray<KeyValuePair<string, string>>();
	}

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

	public void MapToModel(IMap map, LogMessageCreated from, LogMessageListItemModel to)
	{
		to.Id = from.Id;
		to.DateTime = from.DateTime;
		to.LogLevel = from.LogLevel;
		to.Message = from.Message;
	}

	public void MapToModel(IMap map, LogMessageUpdated from, LogMessageListItemModel to)
	{
		to.Id = from.Id;
		to.DateTime = from.DateTime;
		to.LogLevel = from.LogLevel;
		to.Message = from.Message;
	}

	public void MapToModel(IMap map, LogMessageListItem from, LogMessageListItemModel to)
	{
		to.Id = from.Id;
		to.DateTime = from.DateTime;
		to.LogLevel = from.LogLevel;
		to.Message = from.Message;
	}

	public void MapModelToModel(IMap map, LogMessageModel from, LogMessageListItemModel to)
	{
		to.Id = from.Id;
		to.DateTime = from.DateTime;
		to.LogLevel = from.LogLevel;
		to.Message = from.Message;
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
}