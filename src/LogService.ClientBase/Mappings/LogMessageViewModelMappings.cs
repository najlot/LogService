using Najlot.Map;
using Najlot.Map.Attributes;
using LogService.Client.Data.Models;
using LogService.ClientBase.ViewModel;
using LogService.Contracts;
using LogService.Contracts.Events;

namespace LogService.ClientBase.Mappings;

internal sealed class LogMessageViewModelMappings
{
	[MapIgnoreProperty(nameof(to.IsBusy))]
	[MapIgnoreProperty(nameof(to.IsNew))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public void MapToViewModel(IMap map, LogMessageUpdated from, LogMessageViewModel to)
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
		to.Arguments = [.. map.From<LogArgument>(from.Arguments).To<LogArgumentViewModel>()];

		foreach (var e in to.Arguments) e.ParentId = from.Id;
	}

	public void MapToModel(IMap map, LogMessageViewModel from, LogMessageModel to)
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
		to.Arguments = map.From<LogArgumentViewModel>(from.Arguments).ToList<LogArgumentModel>();
	}

	[MapIgnoreProperty(nameof(to.IsBusy))]
	[MapIgnoreProperty(nameof(to.IsNew))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public void MapToViewModel(IMap map, LogMessageModel from, LogMessageViewModel to)
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
		to.Arguments = [.. map.From<LogArgumentModel>(from.Arguments).To<LogArgumentViewModel>()];

		foreach (var e in to.Arguments) e.ParentId = from.Id;
	}
}