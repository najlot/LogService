using FluentAssertions;
using LogService.Contracts;
using Najlot.Map;

namespace LogService.Tests.Mappings;

public class LogArgumentMappingsTests
{
    private readonly IMap _map = new Map().RegisterLogServiceMappings();

    [Fact]
    public void Map_LogArgumentToLogArgument_ShouldCopyAllProperties()
    {
        // Arrange
        var source = new LogArgument
        {
            Id = 123,
            Key = "TestKey",
            Value = "TestValue"
        };

		// Act
		var target = _map.From(source).To<LogArgument>();

		// Assert
		target.Id.Should().Be(source.Id);
        target.Key.Should().Be(source.Key);
        target.Value.Should().Be(source.Value);
    }

    [Fact]
    public void Map_KeyValuePairToLogArgument_ShouldMapKeyAndValue()
    {
        // Arrange
        var source = new KeyValuePair<string, string>("MyKey", "MyValue");

		// Act
		var target = _map.From(source).To<LogArgument>();

		// Assert
		target.Key.Should().Be("MyKey");
        target.Value.Should().Be("MyValue");
        target.Id.Should().Be(0); // Id should not be set from KeyValuePair
    }

    [Fact]
    public void Map_KeyValuePairWithEmptyValue_ShouldMapCorrectly()
    {
        // Arrange
        var source = new KeyValuePair<string, string>("Key", "");

		// Act
		var target = _map.From(source).To<LogArgument>();

		// Assert
		target.Key.Should().Be("Key");
        target.Value.Should().BeEmpty();
    }
}
