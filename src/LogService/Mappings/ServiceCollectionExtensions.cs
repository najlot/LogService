namespace LogService.Mappings;

public static class ServiceCollectionExtensions
{
	public static Najlot.Map.IMap RegisterDataMappings(this Najlot.Map.IMap map)
	{
		map.Register<LogArgumentMappings>();
		map.Register<LogMessageMappings>();

		return map;
	}
}