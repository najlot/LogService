using Najlot.Map;
using LogService.ClientBase.Mappings;

namespace LogService.ClientBase;

public static class MapRegisterExtensions
{
	public static IMap RegisterViewModelMappings(this IMap map)
	{
		map.Register<LogArgumentViewModelMappings>();
		map.Register<LogMessageViewModelMappings>();

		return map;
	}
}