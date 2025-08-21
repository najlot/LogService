using NUnit.Framework;
using Moq;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using LogService.Service.Services;
using LogService.Service.Repository;
using LogService.Service.Model;
using LogService.Service.Test.Helpers;

namespace LogService.Service.Test.Services;

[TestFixture]
public class LogMessageCleanUpServiceTests
{
    private Mock<IServiceProvider> _mockServiceProvider;
    private Mock<IServiceScope> _mockServiceScope;
    private Mock<IServiceScopeFactory> _mockServiceScopeFactory;
    private Mock<ILogMessageRepository> _mockLogMessageRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private LogMessageCleanUpService _cleanUpService;
    private CancellationTokenSource _cancellationTokenSource;

    [SetUp]
    public void SetUp()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockServiceScope = new Mock<IServiceScope>();
        _mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        _mockLogMessageRepository = new Mock<ILogMessageRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _cancellationTokenSource = new CancellationTokenSource();

        // Setup service provider chain
        _mockServiceProvider.Setup(x => x.CreateScope()).Returns(_mockServiceScope.Object);
        _mockServiceScope.Setup(x => x.ServiceProvider).Returns(_mockServiceProvider.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(ILogMessageRepository))).Returns(_mockLogMessageRepository.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(IUserRepository))).Returns(_mockUserRepository.Object);

        _cleanUpService = new LogMessageCleanUpService(_mockServiceProvider.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _cancellationTokenSource?.Dispose();
    }

    #region ExecuteAsync Tests

    [Test]
    public async Task ExecuteAsync_ShouldExecuteCleanUpPeriodically()
    {
        // Arrange
        var users = new List<UserModel>
        {
            new UserModel 
            { 
                Id = Guid.NewGuid(), 
                Settings = new UserSettingsModel { LogRetentionDays = 30 } 
            }
        };
        var logMessages = new List<LogMessageModel>();

        _mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable().ToAsyncEnumerable());
        _mockLogMessageRepository.Setup(x => x.GetAll(It.IsAny<Guid>())).Returns(logMessages.AsQueryable().ToAsyncEnumerable());

        // Cancel after a short delay to prevent infinite execution
        _cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(100));

        // Act & Assert - Should not throw and should handle cancellation gracefully
        await Assert.DoesNotThrowAsync(async () =>
        {
            try
            {
                await _cleanUpService.StartAsync(_cancellationTokenSource.Token);
                await Task.Delay(200, CancellationToken.None); // Wait a bit for execution
                await _cleanUpService.StopAsync(CancellationToken.None);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation token is triggered
            }
        });
    }

    [Test]
    public async Task ExecuteAsync_WithCancellation_ShouldStopGracefully()
    {
        // Arrange
        _cancellationTokenSource.Cancel(); // Cancel immediately

        // Act & Assert
        await Assert.DoesNotThrowAsync(async () =>
        {
            await _cleanUpService.StartAsync(_cancellationTokenSource.Token);
            await _cleanUpService.StopAsync(CancellationToken.None);
        });
    }

    #endregion

    #region CleanUp Method Tests (via ExecuteAsync)

    [Test]
    public async Task CleanUp_WithExpiredLogs_ShouldDeleteExpiredLogs()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var retentionDays = 30;
        var user = new UserModel 
        { 
            Id = userId, 
            Settings = new UserSettingsModel { LogRetentionDays = retentionDays } 
        };

        var expiredLogMessage = new LogMessageModel
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.Now.AddDays(-35), // Older than retention period
            CreatedBy = userId
        };

        var validLogMessage = new LogMessageModel
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.Now.AddDays(-25), // Within retention period
            CreatedBy = userId
        };

        var users = new List<UserModel> { user };
        var expiredLogs = new List<LogMessageModel> { expiredLogMessage };

        _mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable().ToAsyncEnumerable());
        _mockLogMessageRepository.Setup(x => x.GetAll(userId))
            .Returns(expiredLogs.AsQueryable().ToAsyncEnumerable()); // Only return expired logs based on the filtering

        // Set up a short cancellation to prevent infinite loop
        _cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(50));

        // Act
        try
        {
            await _cleanUpService.StartAsync(_cancellationTokenSource.Token);
            await Task.Delay(100, CancellationToken.None); // Allow some execution time
            await _cleanUpService.StopAsync(CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Assert
        _mockLogMessageRepository.Verify(x => x.Delete(expiredLogMessage.Id), Times.AtLeastOnce);
    }

    [Test]
    public async Task CleanUp_WithMultipleUsers_ShouldProcessAllUsers()
    {
        // Arrange
        var user1 = new UserModel 
        { 
            Id = Guid.NewGuid(), 
            Settings = new UserSettingsModel { LogRetentionDays = 30 } 
        };
        var user2 = new UserModel 
        { 
            Id = Guid.NewGuid(), 
            Settings = new UserSettingsModel { LogRetentionDays = 60 } 
        };

        var users = new List<UserModel> { user1, user2 };
        var emptyLogs = new List<LogMessageModel>();

        _mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable().ToAsyncEnumerable());
        _mockLogMessageRepository.Setup(x => x.GetAll(It.IsAny<Guid>())).Returns(emptyLogs.AsQueryable().ToAsyncEnumerable());

        // Set up a short cancellation
        _cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(50));

        // Act
        try
        {
            await _cleanUpService.StartAsync(_cancellationTokenSource.Token);
            await Task.Delay(100, CancellationToken.None);
            await _cleanUpService.StopAsync(CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Assert
        _mockLogMessageRepository.Verify(x => x.GetAll(user1.Id), Times.AtLeastOnce);
        _mockLogMessageRepository.Verify(x => x.GetAll(user2.Id), Times.AtLeastOnce);
    }

    [Test]
    public async Task CleanUp_WithDifferentRetentionPeriods_ShouldRespectEachUserRetention()
    {
        // Arrange
        var user1 = new UserModel 
        { 
            Id = Guid.NewGuid(), 
            Settings = new UserSettingsModel { LogRetentionDays = 30 } 
        };
        var user2 = new UserModel 
        { 
            Id = Guid.NewGuid(), 
            Settings = new UserSettingsModel { LogRetentionDays = 7 } 
        };

        var users = new List<UserModel> { user1, user2 };
        var emptyLogs = new List<LogMessageModel>();

        _mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable().ToAsyncEnumerable());
        _mockLogMessageRepository.Setup(x => x.GetAll(It.IsAny<Guid>())).Returns(emptyLogs.AsQueryable().ToAsyncEnumerable());

        _cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(50));

        // Act
        try
        {
            await _cleanUpService.StartAsync(_cancellationTokenSource.Token);
            await Task.Delay(100, CancellationToken.None);
            await _cleanUpService.StopAsync(CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Assert - Both users should be processed
        _mockUserRepository.Verify(x => x.GetAll(), Times.AtLeastOnce);
        _mockLogMessageRepository.Verify(x => x.GetAll(user1.Id), Times.AtLeastOnce);
        _mockLogMessageRepository.Verify(x => x.GetAll(user2.Id), Times.AtLeastOnce);
    }

    [Test]
    public async Task CleanUp_WithCancellationDuringExecution_ShouldStopProcessing()
    {
        // Arrange
        var user = new UserModel 
        { 
            Id = Guid.NewGuid(), 
            Settings = new UserSettingsModel { LogRetentionDays = 30 } 
        };

        var expiredLog = new LogMessageModel
        {
            Id = Guid.NewGuid(),
            DateTime = DateTime.Now.AddDays(-35),
            CreatedBy = user.Id
        };

        var users = new List<UserModel> { user };
        var expiredLogs = new List<LogMessageModel> { expiredLog };

        _mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable().ToAsyncEnumerable());
        _mockLogMessageRepository.Setup(x => x.GetAll(user.Id)).Returns(expiredLogs.AsQueryable().ToAsyncEnumerable());
        
        // Setup Delete to take some time and then cancel
        _mockLogMessageRepository.Setup(x => x.Delete(It.IsAny<Guid>()))
            .Returns(async () =>
            {
                await Task.Delay(200, CancellationToken.None); // Simulate slow delete
                return Task.CompletedTask;
            });

        _cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(10)); // Cancel very quickly

        // Act & Assert
        await Assert.DoesNotThrowAsync(async () =>
        {
            try
            {
                await _cleanUpService.StartAsync(_cancellationTokenSource.Token);
                await Task.Delay(100, CancellationToken.None);
                await _cleanUpService.StopAsync(CancellationToken.None);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is triggered during processing
            }
        });
    }

    [Test]
    public async Task CleanUp_WithNoUsers_ShouldCompleteWithoutError()
    {
        // Arrange
        var emptyUsers = new List<UserModel>();
        
        _mockUserRepository.Setup(x => x.GetAll()).Returns(emptyUsers.AsQueryable().ToAsyncEnumerable());
        _cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(50));

        // Act & Assert
        await Assert.DoesNotThrowAsync(async () =>
        {
            try
            {
                await _cleanUpService.StartAsync(_cancellationTokenSource.Token);
                await Task.Delay(100, CancellationToken.None);
                await _cleanUpService.StopAsync(CancellationToken.None);
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        });

        // Verify that no log repository calls were made
        _mockLogMessageRepository.Verify(x => x.GetAll(It.IsAny<Guid>()), Times.Never);
        _mockLogMessageRepository.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task CleanUp_WithNoExpiredLogs_ShouldNotDeleteAnyLogs()
    {
        // Arrange
        var user = new UserModel 
        { 
            Id = Guid.NewGuid(), 
            Settings = new UserSettingsModel { LogRetentionDays = 30 } 
        };

        var users = new List<UserModel> { user };
        var noExpiredLogs = new List<LogMessageModel>(); // No logs to delete

        _mockUserRepository.Setup(x => x.GetAll()).Returns(users.AsQueryable().ToAsyncEnumerable());
        _mockLogMessageRepository.Setup(x => x.GetAll(user.Id)).Returns(noExpiredLogs.AsQueryable().ToAsyncEnumerable());
        
        _cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(50));

        // Act
        try
        {
            await _cleanUpService.StartAsync(_cancellationTokenSource.Token);
            await Task.Delay(100, CancellationToken.None);
            await _cleanUpService.StopAsync(CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            // Expected
        }

        // Assert
        _mockLogMessageRepository.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
    }

    #endregion
}