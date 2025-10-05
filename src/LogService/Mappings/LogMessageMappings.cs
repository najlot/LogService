using Najlot.Map;
using Najlot.Map.Attributes;
using LogService.Contracts;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Model;

namespace LogService.Mappings;

internal class LogMessageMappings
{
	public LogMessageCreated MapToCreated(IMap map, LogMessageModel from) =>
		new(from.Id,
			from.CreatedBy,
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

	[MapIgnoreProperty(nameof(to.Id))]
	[MapIgnoreProperty(nameof(to.CreatedBy))]
	[MapIgnoreProperty(nameof(to.Source))]
	public void MapToModel(IMap map, CreateLogMessage from, LogMessageModel to)
	{
		to.DateTime = from.DateTime;
		to.LogLevel = (Contracts.LogLevel)from.LogLevel;
		to.Category = from.Category ?? string.Empty;
		to.State = from.State ?? string.Empty;
		to.RawMessage = from.RawMessage ?? string.Empty;
		to.Message = from.Message ?? string.Empty;
		to.Exception = from.Exception ?? string.Empty;
		to.ExceptionIsValid = from.ExceptionIsValid;
		to.Arguments = map.From<KeyValuePair<string, string>>(from.Arguments ?? []).ToList<LogArgument>();
	}

	public void MapToModel(IMap map, LogMessageModel from, LogMessageListItemModel to)
	{
		to.Id = from.Id;
		to.DateTime = from.DateTime;
		to.LogLevel = from.LogLevel;
		to.Category = from.Category;
		to.Source = from.Source;
		to.Message = from.Message;
		to.HasException = from.ExceptionIsValid;
	}

	public void MapToModel(IMap map, LogMessageCreated from, LogMessageListItemModel to)
	{
		to.Id = from.Id;
		to.DateTime = from.DateTime;
		to.LogLevel = from.LogLevel;
		to.Category = from.Category;
		to.Source = from.Source;
		to.Message = from.Message;
		to.HasException = from.ExceptionIsValid;
	}
}