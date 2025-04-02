using NUnit.Framework;

namespace LogService.Client.Data.Test.Mappings;

public class MapTest
{
	[Test]
	public void Map_must_be_valid()
	{
		new Najlot.Map.Map().RegisterDataMappings().Validate();
	}
}