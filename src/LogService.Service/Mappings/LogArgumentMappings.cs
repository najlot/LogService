using Najlot.Map;
using LogService.Contracts;

namespace LogService.Service.Mappings;

internal class LogArgumentMappings
{
	public void Map(IMap map, LogArgument from, LogArgument to)
	{
		to.Id = from.Id;
		to.Key = from.Key;
		to.Value = from.Value;
	}
}