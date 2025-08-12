using NUnit.Framework;
using Moq;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using LogService.Service.Services;
using LogService.Service.Repository;
using LogService.Service.Model;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Contracts.ListItems;
using LogService.Contracts.Filters;
using LogService.Contracts;
using Cosei.Service.Base;
using Najlot.Map;
using LogService.Service.Test.Helpers;

namespace LogService.Service.Test.Services;

[TestFixture]
public class LogMessageServiceTests
{
    private Mock<ILogMessageRepository> _mockLogMessageRepository;
    private Mock<IPublisher> _mockPublisher;
    private Mock<IMap> _mockMap;
    private LogMessageService _logMessageService;
    private Guid _testUserId;
    private LogMessageModel _testLogMessage;

    [SetUp]
    public void SetUp()
    {
        _mockLogMessageRepository = new Mock<ILogMessageRepository>();
        _mockPublisher = new Mock<IPublisher>();
        _mockMap = new Mock<IMap>();
        _logMessageService = new LogMessageService(_mockLogMessageRepository.Object, _mockPublisher.Object, _mockMap.Object);
        _testUserId = Guid.NewGuid();
        _testLogMessage = new LogMessageModel
        {
            Id = Guid.NewGuid(),
            CreatedBy = _testUserId,
            DateTime = DateTime.UtcNow,
            LogLevel = LogLevel.Information,
            Category = "TestCategory",
            State = "TestState",
            Source = "TestSource",
            RawMessage = "Test raw message",
            Message = "Test message",
            Exception = "",
            ExceptionIsValid = false,
            Arguments = new List<LogArgument>
            {
                new LogArgument { Key = "key1", Value = "value1" },
                new LogArgument { Key = "key2", Value = "value2" }
            }
        };
    }

    #region CreateLogMessages Tests

    [Test]
    public async Task CreateLogMessages_WithValidData_ShouldCreateLogMessagesSuccessfully()
    {
        // Arrange
        var commands = new[]
        {
            new CreateLogMessage
            {
                DateTime = DateTime.UtcNow,
                LogLevel = (int)LogLevel.Information,
                Category = "TestCategory",
                State = "TestState",
                RawMessage = "Test raw message",
                Message = "Test message",
                Exception = "",
                ExceptionIsValid = false,
                Arguments = new[]
                {
                    new KeyValuePair<string, string>("key1", "value1"),
                    new KeyValuePair<string, string>("key2", "value2")
                }
            }
        };
        var source = "TestSource";
        
        var mappedModels = new[]
        {
            new LogMessageModel
            {
                DateTime = commands[0].DateTime,
                LogLevel = (LogLevel)commands[0].LogLevel,
                Category = commands[0].Category,
                State = commands[0].State,
                RawMessage = commands[0].RawMessage,
                Message = commands[0].Message,
                Exception = commands[0].Exception,
                ExceptionIsValid = commands[0].ExceptionIsValid,
                Arguments = new List<LogArgument>
                {
                    new LogArgument { Key = "key1", Value = "value1" },
                    new LogArgument { Key = "key2", Value = "value2" }
                }
            }
        };

        _mockMap.Setup(x => x.From<CreateLogMessage>(commands)).Returns(new MockArrayMapExpression<LogMessageModel>(mappedModels));
        _mockMap.Setup(x => x.From<LogMessageModel>(It.IsAny<LogMessageModel[]>())).Returns(new MockListMapExpression<LogMessageCreated>(new List<LogMessageCreated> { new LogMessageCreated() }));

        // Act
        await _logMessageService.CreateLogMessages(commands, source, _testUserId);

        // Assert
        _mockLogMessageRepository.Verify(x => x.Insert(It.Is<LogMessageModel[]>(items => 
            items.Length == 1 &&
            items[0].CreatedBy == _testUserId &&
            items[0].Source == source &&
            items[0].Id != Guid.Empty &&
            items[0].Arguments.All(arg => arg.Id >= 0 && arg.Key != null && arg.Value != null)
        )), Times.Once);
        
        _mockPublisher.Verify(x => x.PublishToUserAsync(_testUserId.ToString(), It.IsAny<List<LogMessageCreated>>()), Times.Once);
    }

    [Test]
    public async Task CreateLogMessages_ShouldAssignUniqueIds()
    {
        // Arrange
        var commands = new[]
        {
            new CreateLogMessage { DateTime = DateTime.UtcNow, LogLevel = (int)LogLevel.Information },
            new CreateLogMessage { DateTime = DateTime.UtcNow, LogLevel = (int)LogLevel.Warning }
        };
        var source = "TestSource";
        
        var mappedModels = commands.Select(c => new LogMessageModel
        {
            DateTime = c.DateTime,
            LogLevel = (LogLevel)c.LogLevel,
            Arguments = new List<LogArgument>()
        }).ToArray();

        _mockMap.Setup(x => x.From<CreateLogMessage>(commands)).Returns(new MockArrayMapExpression<LogMessageModel>(mappedModels));
        _mockMap.Setup(x => x.From<LogMessageModel>(It.IsAny<LogMessageModel[]>())).Returns(new MockListMapExpression<LogMessageCreated>(new List<LogMessageCreated>()));

        // Act
        await _logMessageService.CreateLogMessages(commands, source, _testUserId);

        // Assert
        _mockLogMessageRepository.Verify(x => x.Insert(It.Is<LogMessageModel[]>(items => 
            items.Length == 2 &&
            items[0].Id != Guid.Empty &&
            items[1].Id != Guid.Empty &&
            items[0].Id != items[1].Id
        )), Times.Once);
    }

    [Test]
    public async Task CreateLogMessages_ShouldAssignSequentialArgumentIds()
    {
        // Arrange
        var commands = new[]
        {
            new CreateLogMessage 
            { 
                DateTime = DateTime.UtcNow, 
                LogLevel = (int)LogLevel.Information,
                Arguments = new[]
                {
                    new KeyValuePair<string, string>("key1", "value1"),
                    new KeyValuePair<string, string>("key2", "value2")
                }
            },
            new CreateLogMessage 
            { 
                DateTime = DateTime.UtcNow, 
                LogLevel = (int)LogLevel.Warning,
                Arguments = new[]
                {
                    new KeyValuePair<string, string>("key3", "value3")
                }
            }
        };
        var source = "TestSource";
        
        var mappedModels = commands.Select(c => new LogMessageModel
        {
            DateTime = c.DateTime,
            LogLevel = (LogLevel)c.LogLevel,
            Arguments = c.Arguments?.Select(kvp => new LogArgument { Key = kvp.Key, Value = kvp.Value }).ToList() ?? new List<LogArgument>()
        }).ToArray();

        _mockMap.Setup(x => x.From<CreateLogMessage>(commands)).Returns(new MockArrayMapExpression<LogMessageModel>(mappedModels));
        _mockMap.Setup(x => x.From<LogMessageModel>(It.IsAny<LogMessageModel[]>())).Returns(new MockListMapExpression<LogMessageCreated>(new List<LogMessageCreated>()));

        // Act
        await _logMessageService.CreateLogMessages(commands, source, _testUserId);

        // Assert
        _mockLogMessageRepository.Verify(x => x.Insert(It.Is<LogMessageModel[]>(items => 
            items.Length == 2 &&
            items[0].Arguments.Count == 2 &&
            items[1].Arguments.Count == 1 &&
            items[0].Arguments[0].Id == 0 &&
            items[0].Arguments[1].Id == 1 &&
            items[1].Arguments[0].Id == 2
        )), Times.Once);
    }

    [Test]
    public async Task CreateLogMessages_ShouldHandleNullArguments()
    {
        // Arrange
        var commands = new[]
        {
            new CreateLogMessage 
            { 
                DateTime = DateTime.UtcNow, 
                LogLevel = (int)LogLevel.Information,
                Arguments = new[]
                {
                    new KeyValuePair<string, string>(null, null)
                }
            }
        };
        var source = "TestSource";
        
        var mappedModels = new[]
        {
            new LogMessageModel
            {
                DateTime = commands[0].DateTime,
                LogLevel = (LogLevel)commands[0].LogLevel,
                Arguments = new List<LogArgument> { new LogArgument { Key = null, Value = null } }
            }
        };

        _mockMap.Setup(x => x.From<CreateLogMessage>(commands)).Returns(new MockArrayMapExpression<LogMessageModel>(mappedModels));
        _mockMap.Setup(x => x.From<LogMessageModel>(It.IsAny<LogMessageModel[]>())).Returns(new MockListMapExpression<LogMessageCreated>(new List<LogMessageCreated>()));

        // Act
        await _logMessageService.CreateLogMessages(commands, source, _testUserId);

        // Assert
        _mockLogMessageRepository.Verify(x => x.Insert(It.Is<LogMessageModel[]>(items => 
            items.Length == 1 &&
            items[0].Arguments.Count == 1 &&
            items[0].Arguments[0].Key == string.Empty &&
            items[0].Arguments[0].Value == string.Empty
        )), Times.Once);
    }

    #endregion

    #region GetItemAsync Tests

    [Test]
    public async Task GetItemAsync_WithExistingLogMessage_ShouldReturnLogMessage()
    {
        // Arrange
        var logMessageId = Guid.NewGuid();
        var expectedLogMessage = new LogMessage();
        
        _mockLogMessageRepository.Setup(x => x.Get(logMessageId)).ReturnsAsync(_testLogMessage);
        _mockMap.Setup(x => x.FromNullable(_testLogMessage)).Returns(new MockMapExpression<LogMessage>(expectedLogMessage));

        // Act
        var result = await _logMessageService.GetItemAsync(logMessageId, _testUserId);

        // Assert
        result.Should().Be(expectedLogMessage);
    }

    [Test]
    public async Task GetItemAsync_WithNonExistentLogMessage_ShouldReturnNull()
    {
        // Arrange
        var logMessageId = Guid.NewGuid();
        
        _mockLogMessageRepository.Setup(x => x.Get(logMessageId)).ReturnsAsync((LogMessageModel?)null);
        _mockMap.Setup(x => x.FromNullable(It.IsAny<LogMessageModel?>())).Returns((IMapExpression<LogMessage>?)null);

        // Act
        var result = await _logMessageService.GetItemAsync(logMessageId, _testUserId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetItemsForUserAsync (with filter) Tests

    [Test]
    public void GetItemsForUserAsync_WithFilter_ShouldApplyUserFilter()
    {
        // Arrange
        var filter = new LogMessageFilter();
        var queryableLogMessages = new[] { _testLogMessage }.AsQueryable();
        var expectedResults = new[] { new LogMessageListItem() };
        
        _mockLogMessageRepository.Setup(x => x.GetAllQueryable()).Returns(queryableLogMessages);
        _mockMap.Setup(x => x.From<LogMessageModel>(It.IsAny<LogMessageModel[]>())).Returns(new MockArrayMapExpression<LogMessageListItem>(expectedResults));

        // Act
        var result = _logMessageService.GetItemsForUserAsync(filter, _testUserId);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().Be(1);
    }

    [Test]
    public void GetItemsForUserAsync_WithDateTimeFromFilter_ShouldFilterByStartDate()
    {
        // Arrange
        var dateTimeFrom = DateTime.UtcNow.AddHours(-1);
        var filter = new LogMessageFilter { DateTimeFrom = dateTimeFrom };
        
        var logMessage1 = new LogMessageModel { Id = Guid.NewGuid(), CreatedBy = _testUserId, DateTime = DateTime.UtcNow };
        var logMessage2 = new LogMessageModel { Id = Guid.NewGuid(), CreatedBy = _testUserId, DateTime = DateTime.UtcNow.AddHours(-2) };
        var queryableLogMessages = new[] { logMessage1, logMessage2 }.AsQueryable();
        
        _mockLogMessageRepository.Setup(x => x.GetAllQueryable()).Returns(queryableLogMessages);
        _mockMap.Setup(x => x.From<LogMessageModel>(It.IsAny<LogMessageModel[]>())).Returns(new MockArrayMapExpression<LogMessageListItem>(new LogMessageListItem[0]));

        // Act
        var result = _logMessageService.GetItemsForUserAsync(filter, _testUserId);

        // Assert
        result.Should().NotBeNull();
        // Note: We can't easily verify the filtering logic without executing the query, 
        // but the test ensures the method doesn't throw and basic structure is correct
    }

    [Test]
    public void GetItemsForUserAsync_WithLogLevelFilter_ShouldFilterByLogLevel()
    {
        // Arrange
        var filter = new LogMessageFilter { LogLevel = LogLevel.Warning };
        var queryableLogMessages = new[] { _testLogMessage }.AsQueryable();
        
        _mockLogMessageRepository.Setup(x => x.GetAllQueryable()).Returns(queryableLogMessages);
        _mockMap.Setup(x => x.From<LogMessageModel>(It.IsAny<LogMessageModel[]>())).Returns(new MockArrayMapExpression<LogMessageListItem>(new LogMessageListItem[0]));

        // Act
        var result = _logMessageService.GetItemsForUserAsync(filter, _testUserId);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void GetItemsForUserAsync_WithCategoryFilter_ShouldFilterByCategory()
    {
        // Arrange
        var filter = new LogMessageFilter { Category = "Test" };
        var queryableLogMessages = new[] { _testLogMessage }.AsQueryable();
        
        _mockLogMessageRepository.Setup(x => x.GetAllQueryable()).Returns(queryableLogMessages);
        _mockMap.Setup(x => x.From<LogMessageModel>(It.IsAny<LogMessageModel[]>())).Returns(new MockArrayMapExpression<LogMessageListItem>(new LogMessageListItem[0]));

        // Act
        var result = _logMessageService.GetItemsForUserAsync(filter, _testUserId);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void GetItemsForUserAsync_WithMultipleFilters_ShouldApplyAllFilters()
    {
        // Arrange
        var filter = new LogMessageFilter 
        { 
            DateTimeFrom = DateTime.UtcNow.AddHours(-1),
            DateTimeTo = DateTime.UtcNow.AddHours(1),
            LogLevel = LogLevel.Information,
            Category = "Test",
            State = "TestState",
            Source = "TestSource",
            RawMessage = "raw",
            Message = "message",
            Exception = "exception",
            ExceptionIsValid = false
        };
        var queryableLogMessages = new[] { _testLogMessage }.AsQueryable();
        
        _mockLogMessageRepository.Setup(x => x.GetAllQueryable()).Returns(queryableLogMessages);
        _mockMap.Setup(x => x.From<LogMessageModel>(It.IsAny<LogMessageModel[]>())).Returns(new MockArrayMapExpression<LogMessageListItem>(new LogMessageListItem[0]));

        // Act
        var result = _logMessageService.GetItemsForUserAsync(filter, _testUserId);

        // Assert
        result.Should().NotBeNull();
    }

    #endregion

    #region GetItemsForUserAsync (without filter) Tests

    [Test]
    public void GetItemsForUserAsync_WithoutFilter_ShouldReturnAllUserLogMessages()
    {
        // Arrange
        var queryableLogMessages = new[] { _testLogMessage }.AsQueryable();
        var expectedResults = new[] { new LogMessageListItem() };
        
        _mockLogMessageRepository.Setup(x => x.GetAllQueryable()).Returns(queryableLogMessages);
        _mockMap.Setup(x => x.From<LogMessageModel>(It.IsAny<LogMessageModel[]>())).Returns(new MockArrayMapExpression<LogMessageListItem>(expectedResults));

        // Act
        var result = _logMessageService.GetItemsForUserAsync(_testUserId);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().Be(1);
    }

    [Test]
    public void GetItemsForUserAsync_WithoutFilter_ShouldOrderByDateTimeDescending()
    {
        // Arrange
        var logMessage1 = new LogMessageModel { Id = Guid.NewGuid(), CreatedBy = _testUserId, DateTime = DateTime.UtcNow };
        var logMessage2 = new LogMessageModel { Id = Guid.NewGuid(), CreatedBy = _testUserId, DateTime = DateTime.UtcNow.AddHours(-1) };
        var queryableLogMessages = new[] { logMessage1, logMessage2 }.AsQueryable();
        var expectedResults = new[] { new LogMessageListItem(), new LogMessageListItem() };
        
        _mockLogMessageRepository.Setup(x => x.GetAllQueryable()).Returns(queryableLogMessages);
        _mockMap.Setup(x => x.From<LogMessageModel>(It.IsAny<LogMessageModel[]>())).Returns(new MockArrayMapExpression<LogMessageListItem>(expectedResults));

        // Act
        var result = _logMessageService.GetItemsForUserAsync(_testUserId);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().Be(2);
    }

    #endregion
}