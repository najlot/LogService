using Najlot.Map;
using LogService.Mappings;

namespace LogService;

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