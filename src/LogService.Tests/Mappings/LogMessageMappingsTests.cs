using FluentAssertions;
using LogService.Contracts;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Mappings;
using LogService.Model;
using Najlot.Map;
using Xunit;

namespace LogService.Tests.Mappings;

public class LogMessageMappingsTests
{
    private readonly IMap _map;
    private readonly LogMessageMappings _mappings;

    public LogMessageMappingsTests()
    {
        _map = new Map();
        _map.Register<LogArgumentMappings>();
        _mappings = new LogMessageMappings();
        _map.Register(_mappings);
    }

    [Fact]
    public void MapToCreated_LogMessageModel_ShouldCreateLogMessageCreatedEvent()
    {
        // Arrange
        var source = new LogMessageModel
        {
            Id = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            LogLevel = Contracts.LogLevel.Info,
            Category = "TestCategory",
            State = "TestState",
            Source = "TestSource",
            RawMessage = "RawMessage",
            Message = "Message",
            Exception = "Exception",
            ExceptionIsValid = true,
            Arguments = new List<LogArgument>
            {
                new LogArgument { Id = 1, Key = "key1", Value = "value1" }
            }
        };

        // Act
        var result = _mappings.MapToCreated(_map, source);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(source.Id);
        result.CreatedBy.Should().Be(source.CreatedBy);
        result.DateTime.Should().Be(source.DateTime);
        result.LogLevel.Should().Be(source.LogLevel);
        result.Category.Should().Be(source.Category);
        result.State.Should().Be(source.State);
        result.Source.Should().Be(source.Source);
        result.RawMessage.Should().Be(source.RawMessage);
        result.Message.Should().Be(source.Message);
        result.Exception.Should().Be(source.Exception);
        result.ExceptionIsValid.Should().Be(source.ExceptionIsValid);
        result.Arguments.Should().HaveCount(1);
    }

    [Fact]
    public void MapToModel_CreateLogMessage_ShouldMapToLogMessageModel()
    {
        // Arrange
        var source = new CreateLogMessage
        {
            DateTime = DateTime.UtcNow,
            LogLevel = (int)Contracts.LogLevel.Error,
            Category = "Category",
            State = "State",
            RawMessage = "Raw",
            Message = "Msg",
            Exception = "Ex",
            ExceptionIsValid = true,
            Arguments = [
                new KeyValuePair<string, string>("key1", "value1"),
                new KeyValuePair<string, string>("key2", "value2")
            ]
        };
        var target = new LogMessageModel();

        // Act
        _mappings.MapToModel(_map, source, target);

        // Assert
        target.DateTime.Should().Be(source.DateTime);
        target.LogLevel.Should().Be(Contracts.LogLevel.Error);
        target.Category.Should().Be(source.Category);
        target.State.Should().Be(source.State);
        target.RawMessage.Should().Be(source.RawMessage);
        target.Message.Should().Be(source.Message);
        target.Exception.Should().Be(source.Exception);
        target.ExceptionIsValid.Should().Be(source.ExceptionIsValid);
        target.Arguments.Should().HaveCount(2);
    }

    [Fact]
    public void MapToModel_CreateLogMessageWithNulls_ShouldUseEmptyStrings()
    {
        // Arrange
        var source = new CreateLogMessage
        {
            DateTime = DateTime.UtcNow,
            LogLevel = (int)Contracts.LogLevel.Warn,
            Category = null,
            State = null,
            RawMessage = null,
            Message = null,
            Exception = null,
            ExceptionIsValid = false,
            Arguments = null
        };
        var target = new LogMessageModel();

        // Act
        _mappings.MapToModel(_map, source, target);

        // Assert
        target.Category.Should().BeEmpty();
        target.State.Should().BeEmpty();
        target.RawMessage.Should().BeEmpty();
        target.Message.Should().BeEmpty();
        target.Exception.Should().BeEmpty();
        target.Arguments.Should().BeEmpty();
    }

    [Fact]
    public void MapToModel_LogMessageModelToListItem_ShouldMapCorrectly()
    {
        // Arrange
        var source = new LogMessageModel
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.UtcNow,
            LogLevel = Contracts.LogLevel.Debug,
            Category = "Cat",
            Source = "Src",
            Message = "Msg",
            ExceptionIsValid = true
        };
        var target = new LogMessageListItemModel();

        // Act
        _mappings.MapToModel(_map, source, target);

        // Assert
        target.Id.Should().Be(source.Id);
        target.DateTime.Should().Be(source.DateTime);
        target.LogLevel.Should().Be(source.LogLevel);
        target.Category.Should().Be(source.Category);
        target.Source.Should().Be(source.Source);
        target.Message.Should().Be(source.Message);
        target.HasException.Should().BeTrue();
    }

    [Fact]
    public void MapToModel_LogMessageCreatedToListItem_ShouldMapCorrectly()
    {
        // Arrange
        var source = new LogMessageCreated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            Contracts.LogLevel.Fatal,
            "Category",
            "State",
            "Source",
            "Raw",
            "Message",
            "Ex",
            true,
            new List<LogArgument>()
        );
        var target = new LogMessageListItemModel();

        // Act
        _mappings.MapToModel(_map, source, target);

        // Assert
        target.Id.Should().Be(source.Id);
        target.DateTime.Should().Be(source.DateTime);
        target.LogLevel.Should().Be(source.LogLevel);
        target.Category.Should().Be(source.Category);
        target.Source.Should().Be(source.Source);
        target.Message.Should().Be(source.Message);
        target.HasException.Should().BeTrue();
    }
}
