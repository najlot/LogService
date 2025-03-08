using Najlot.Map;
using LogService.Client.Data.Models;
using LogService.Contracts;

namespace LogService.Client.Data.Mappings;

internal sealed class LogArgumentMappings
{
	public void MapFromModel(IMap map, LogArgumentModel from, LogArgument to)
	{
		to.Id = from.Id;
		to.Key = from.Key;
		to.Value = from.Value;
	}

	public void MapToModel(IMap map, LogArgument from, LogArgumentModel to)
	{
		to.Id = from.Id;
		to.Key = from.Key;
		to.Value = from.Value;
	}

	public void MapFromModelToModel(IMap map, LogArgumentModel from, LogArgumentModel to)
	{
		to.Id = from.Id;
		to.Key = from.Key;
		to.Value = from.Value;
	}
}