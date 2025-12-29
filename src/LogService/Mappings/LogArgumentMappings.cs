using Najlot.Map;
using LogService.Contracts;
using Najlot.Map.Attributes;

namespace LogService.Mappings;

[Mapping]
internal partial class LogArgumentMappings
{
	public static partial void Map(IMap map, LogArgument from, LogArgument to);

	[MapIgnoreProperty(nameof(to.Id))]
	public static void Map(IMap map, KeyValuePair<string, string> from, LogArgument to)
	{
		to.Key = from.Key;
		to.Value = from.Value;
	}
}