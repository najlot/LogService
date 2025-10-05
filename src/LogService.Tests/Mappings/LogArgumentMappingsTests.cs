using FluentAssertions;
using LogService.Contracts;
using LogService.Mappings;
using Najlot.Map;
using Xunit;

namespace LogService.Tests.Mappings;

public class LogArgumentMappingsTests
{
    private readonly IMap _map;
    private readonly LogArgumentMappings _mappings;

    public LogArgumentMappingsTests()
    {
        _map = new Map();
        _mappings = new LogArgumentMappings();
        _map.Register(_mappings);
    }

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
        var target = new LogArgument();

        // Act
        _mappings.Map(_map, source, target);

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
        var target = new LogArgument();

        // Act
        _mappings.Map(_map, source, target);

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
        var target = new LogArgument();

        // Act
        _mappings.Map(_map, source, target);

        // Assert
        target.Key.Should().Be("Key");
        target.Value.Should().BeEmpty();
    }
}
