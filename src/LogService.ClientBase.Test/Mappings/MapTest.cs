using Najlot.Map;
using NUnit.Framework;

namespace LogService.ClientBase.Test.Mappings;

public class MapTest
{
	[Test]
	public void Map_must_be_valid()
	{
		new Map().RegisterViewModelMappings().Validate();
	}
}