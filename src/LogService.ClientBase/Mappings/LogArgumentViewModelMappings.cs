using Najlot.Map;
using Najlot.Map.Attributes;
using LogService.Client.Data.Models;
using LogService.ClientBase.Messages;
using LogService.ClientBase.ViewModel;
using LogService.Contracts;

namespace LogService.ClientBase.Mappings;

internal sealed class LogArgumentViewModelMappings
{
	public void MapToModel(IMap map, LogArgumentViewModel from, LogArgumentModel to)
	{
		to.Id = from.Id;
		to.Key = from.Key;
		to.Value = from.Value;
	}

	[MapIgnoreProperty(nameof(to.IsNew))]
	[MapIgnoreProperty(nameof(to.IsBusy))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public void MapFromViewModelToViewModel(IMap map, LogArgumentViewModel from, LogArgumentViewModel to)
	{
		to.Id = from.Id;
		to.ParentId = from.ParentId;
		to.Key = from.Key;
		to.Value = from.Value;
	}

	[MapIgnoreMethod]
	public void MapToViewModel(IMap map, SaveLogArgument from, LogArgumentViewModel to)
	{
		to.ParentId = from.ParentId;
		MapToViewModel(map, from.Item, to);
	}

	[MapIgnoreProperty(nameof(to.IsNew))]
	[MapIgnoreProperty(nameof(to.IsBusy))]
	[MapIgnoreProperty(nameof(to.ParentId))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public void MapToViewModel(IMap map, LogArgumentModel from, LogArgumentViewModel to)
	{
		to.Id = from.Id;
		to.Key = from.Key;
		to.Value = from.Value;
	}

	[MapIgnoreProperty(nameof(to.IsNew))]
	[MapIgnoreProperty(nameof(to.IsBusy))]
	[MapIgnoreProperty(nameof(to.ParentId))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public void MapToViewModel(IMap map, LogArgument from, LogArgumentViewModel to)
	{
		to.Id = from.Id;
		to.Key = from.Key;
		to.Value = from.Value;
	}
}