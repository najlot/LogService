using Najlot.Map;
using LogService.Client.Data.Mappings;

namespace LogService.Client.Data;

public static class MapRegisterExtensions
{
	public static IMap RegisterDataMappings(this IMap map)
	{
		map.Register<LogArgumentMappings>();
		map.Register<LogMessageMappings>();
		map.Register<UserMappings>();

		return map;
	}
}