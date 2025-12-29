using Najlot.Map;
using Najlot.Map.Attributes;
using LogService.Contracts;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Model;
using System.Linq.Expressions;

namespace LogService.Mappings;

[Mapping]
internal partial class LogMessageMappings
{
	public static LogMessageCreated MapToCreated(IMap map, LogMessageModel from) =>
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
	public static void MapToModel(IMap map, CreateLogMessage from, LogMessageModel to)
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

	private static partial void MapPartialToModel(IMap map, LogMessageModel from, LogMessageListItemModel to);
	public static void MapToModel(IMap map, LogMessageModel from, LogMessageListItemModel to)
	{
		MapPartialToModel(map, from, to);
		to.HasException = from.ExceptionIsValid;
	}

	public Expression<Func<LogMessageModel, LogMessageListItemModel>> MapToListItemExpression()
	{
		return from => new LogMessageListItemModel
		{
			Id = from.Id,
			DateTime = from.DateTime,
			LogLevel = from.LogLevel,
			Category = from.Category,
			Source = from.Source,
			Message = from.Message,
			HasException = from.ExceptionIsValid
		};
	}

	private static partial void MapPartialToModel(IMap map, LogMessageCreated from, LogMessageListItemModel to);
	public static void MapToModel(IMap map, LogMessageCreated from, LogMessageListItemModel to)
	{
		MapPartialToModel(map, from, to);
		to.HasException = from.ExceptionIsValid;
	}
}