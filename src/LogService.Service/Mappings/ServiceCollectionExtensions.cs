using Najlot.Map;
using Microsoft.Extensions.DependencyInjection;

namespace LogService.Service.Mappings;

public static class ServiceCollectionExtensions
{
	public static IMap RegisterDataMappings(this IMap map)
	{
		map.Register<LogArgumentMappings>();
		map.Register<LogMessageMappings>();
		map.Register<UserMappings>();

		return map;
	}

	public static IServiceCollection RegisterDataMappings(this IServiceCollection services)
	{
		var map = new Map().RegisterDataMappings();
		return services.AddSingleton(map);
	}
}