using NUnit.Framework;
using LogService.Service.Mappings;

namespace LogService.Service.Test.Mappings;

public class MapTest
{
	[Test]
	public void Map_must_be_valid()
	{
		new Najlot.Map.Map().RegisterDataMappings().Validate();
	}
}