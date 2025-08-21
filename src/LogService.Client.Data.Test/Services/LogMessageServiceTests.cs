using NUnit.Framework;
using NSubstitute;
using System;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Client.Data.Repositories;
using LogService.Client.Data.Services.Implementation;
using LogService.Contracts.Filters;

namespace LogService.Client.Data.Test.Services;

[TestFixture]
public class LogMessageServiceTests
{
	private ILogMessageRepository _mockRepository = null!;
	private LogMessageService _logMessageService = null!;

	[SetUp]
	public void SetUp()
	{
		_mockRepository = Substitute.For<ILogMessageRepository>();
		_logMessageService = new LogMessageService(_mockRepository);
	}

	[TearDown]
	public void TearDown()
	{
		_logMessageService?.Dispose();
	}

	[Test]
	public void CreateLogMessage_ShouldReturnNewLogMessageWithValidId()
	{
		// Act
		var logMessage = _logMessageService.CreateLogMessage();

		// Assert
		Assert.That(logMessage, Is.Not.Null);
		Assert.That(logMessage.Id, Is.Not.EqualTo(Guid.Empty));
		Assert.That(logMessage.Category, Is.EqualTo(""));
		Assert.That(logMessage.State, Is.EqualTo(""));
		Assert.That(logMessage.Source, Is.EqualTo(""));
		Assert.That(logMessage.RawMessage, Is.EqualTo(""));
		Assert.That(logMessage.Message, Is.EqualTo(""));
		Assert.That(logMessage.Exception, Is.EqualTo(""));
		Assert.That(logMessage.Arguments, Is.Not.Null);
		Assert.That(logMessage.Arguments, Is.Empty);
	}

	[Test]
	public void CreateLogMessage_MultipleCallsShouldReturnDifferentIds()
	{
		// Act
		var logMessage1 = _logMessageService.CreateLogMessage();
		var logMessage2 = _logMessageService.CreateLogMessage();

		// Assert
		Assert.That(logMessage1.Id, Is.Not.EqualTo(logMessage2.Id));
	}

	[Test]
	public async Task GetItemAsync_ShouldCallRepositoryGetItem()
	{
		// Arrange
		var logMessageId = Guid.NewGuid();
		var expectedLogMessage = new LogMessageModel { Id = logMessageId, Message = "test" };
		_mockRepository.GetItemAsync(logMessageId).Returns(expectedLogMessage);

		// Act
		var result = await _logMessageService.GetItemAsync(logMessageId);

		// Assert
		Assert.That(result, Is.EqualTo(expectedLogMessage));
		await _mockRepository.Received(1).GetItemAsync(logMessageId);
	}

	[Test]
	public async Task GetItemsAsync_WithoutFilter_ShouldCallRepositoryGetItems()
	{
		// Arrange
		var expectedLogMessages = new LogMessageListItemModel[]
		{
			new LogMessageListItemModel { Id = Guid.NewGuid(), Message = "log1" },
			new LogMessageListItemModel { Id = Guid.NewGuid(), Message = "log2" }
		};
		_mockRepository.GetItemsAsync().Returns(expectedLogMessages);

		// Act
		var result = await _logMessageService.GetItemsAsync();

		// Assert
		Assert.That(result, Is.EqualTo(expectedLogMessages));
		await _mockRepository.Received(1).GetItemsAsync();
	}

	[Test]
	public async Task GetItemsAsync_WithFilter_ShouldCallRepositoryGetItemsWithFilter()
	{
		// Arrange
		var filter = new LogMessageFilter();
		var expectedLogMessages = new LogMessageListItemModel[]
		{
			new LogMessageListItemModel { Id = Guid.NewGuid(), Message = "filtered log" }
		};
		_mockRepository.GetItemsAsync(filter).Returns(expectedLogMessages);

		// Act
		var result = await _logMessageService.GetItemsAsync(filter);

		// Assert
		Assert.That(result, Is.EqualTo(expectedLogMessages));
		await _mockRepository.Received(1).GetItemsAsync(filter);
	}

	[Test]
	public async Task DeleteItemAsync_ShouldCallRepositoryDeleteItem()
	{
		// Arrange
		var logMessageId = Guid.NewGuid();

		// Act
		await _logMessageService.DeleteItemAsync(logMessageId);

		// Assert
		await _mockRepository.Received(1).DeleteItemAsync(logMessageId);
	}

	[Test]
	public void Dispose_ShouldCallRepositoryDispose()
	{
		// Act
		_logMessageService.Dispose();

		// Assert
		_mockRepository.Received(1).Dispose();
	}
}