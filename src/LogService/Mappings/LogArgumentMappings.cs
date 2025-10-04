using Najlot.Map;
using LogService.Contracts;
using System.Collections.Generic;
using Najlot.Map.Attributes;

namespace LogService.Mappings;

internal class LogArgumentMappings
{
	public void Map(IMap map, LogArgument from, LogArgument to)
	{
		to.Id = from.Id;
		to.Key = from.Key;
		to.Value = from.Value;
	}

	[MapIgnoreProperty(nameof(to.Id))]
	public void Map(IMap map, KeyValuePair<string, string> from, LogArgument to)
	{
		to.Key = from.Key;
		to.Value = from.Value;
	}
}