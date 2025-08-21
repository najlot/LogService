using NUnit.Framework;
using Najlot.Map;
using LogService.Client.Data;

namespace LogService.Client.Data.Test;

[TestFixture]
public class MapRegisterExtensionsTests
{
	[Test]
	public void RegisterDataMappings_ShouldRegisterAllMappings()
	{
		// Arrange
		var map = new Map();

		// Act
		var result = map.RegisterDataMappings();

		// Assert
		Assert.That(result, Is.SameAs(map));
		
		// Verify mappings are registered by validating the map
		Assert.DoesNotThrow(() => map.Validate());
	}

	[Test]
	public void RegisterDataMappings_OnAlreadyConfiguredMap_ShouldNotThrow()
	{
		// Arrange
		var map = new Map();
		map.RegisterDataMappings();

		// Act & Assert
		Assert.DoesNotThrow(() => map.RegisterDataMappings());
	}

	[Test]
	public void RegisterDataMappings_ShouldReturnSameMapInstance()
	{
		// Arrange
		var map = new Map();

		// Act
		var result = map.RegisterDataMappings();

		// Assert
		Assert.That(result, Is.SameAs(map));
	}
}