using NUnit.Framework;
using Moq;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using System.Linq;
using LogService.Service.Services;
using LogService.Service.Repository;
using LogService.Service.Model;
using LogService.Contracts.Commands;
using LogService.Contracts.Events;
using LogService.Contracts.ListItems;
using LogService.Contracts;
using Cosei.Service.Base;
using Najlot.Map;
using System.Text;
using System.Security.Cryptography;
using LogService.Service.Test.Helpers;

namespace LogService.Service.Test.Services;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IPublisher> _mockPublisher;
    private Mock<IMap> _mockMap;
    private UserService _userService;
    private Guid _testUserId;
    private UserModel _testUser;

    [SetUp]
    public void SetUp()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPublisher = new Mock<IPublisher>();
        _mockMap = new Mock<IMap>();
        _userService = new UserService(_mockUserRepository.Object, _mockPublisher.Object, _mockMap.Object);
        _testUserId = Guid.NewGuid();
        _testUser = new UserModel
        {
            Id = _testUserId,
            Username = "testuser",
            EMail = "test@example.com",
            PasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes("password123")),
            IsActive = true,
            Settings = new UserSettingsModel { LogRetentionDays = 30 }
        };
    }

    #region CreateUser Tests

    [Test]
    public async Task CreateUser_WithValidData_ShouldCreateUserSuccessfully()
    {
        // Arrange
        var command = new CreateUser(Guid.NewGuid(), "NewUser", "new@example.com", "password123");
        var normalizedUsername = command.Username.Normalize().ToLower();
        
        _mockUserRepository.Setup(x => x.Get(normalizedUsername)).ReturnsAsync((UserModel?)null);
        _mockMap.Setup(x => x.From(command)).Returns(new MockMapExpression<UserModel>(new UserModel()));
        _mockMap.Setup(x => x.From(It.IsAny<UserModel>())).Returns(new MockMapExpression<UserCreated>(new UserCreated()));

        // Act
        await _userService.CreateUser(command, _testUserId);

        // Assert
        _mockUserRepository.Verify(x => x.Get(normalizedUsername), Times.Once);
        _mockUserRepository.Verify(x => x.Insert(It.IsAny<UserModel>()), Times.Once);
        _mockPublisher.Verify(x => x.PublishToUserAsync(_testUserId.ToString(), It.IsAny<UserCreated>()), Times.Once);
    }

    [Test]
    public async Task CreateUser_WithExistingUsername_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateUser(Guid.NewGuid(), "ExistingUser", "existing@example.com", "password123");
        var normalizedUsername = command.Username.Normalize().ToLower();
        
        _mockUserRepository.Setup(x => x.Get(normalizedUsername)).ReturnsAsync(_testUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.CreateUser(command, _testUserId));
        exception.Message.Should().Be("User already exists!");
    }

    [Test]
    public async Task CreateUser_WithShortPassword_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateUser(Guid.NewGuid(), "NewUser", "new@example.com", "12345");
        var normalizedUsername = command.Username.Normalize().ToLower();
        
        _mockUserRepository.Setup(x => x.Get(normalizedUsername)).ReturnsAsync((UserModel?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.CreateUser(command, _testUserId));
        exception.Message.Should().Be("Password too short!");
    }

    [Test]
    public async Task CreateUser_WithPasswordWithWhitespace_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new CreateUser(Guid.NewGuid(), "NewUser", "new@example.com", "   123   ");
        var normalizedUsername = command.Username.Normalize().ToLower();
        
        _mockUserRepository.Setup(x => x.Get(normalizedUsername)).ReturnsAsync((UserModel?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.CreateUser(command, _testUserId));
        exception.Message.Should().Be("Password too short!");
    }

    #endregion

    #region UpdateUser Tests

    [Test]
    public async Task UpdateUser_WithValidData_ShouldUpdateUserSuccessfully()
    {
        // Arrange
        var command = new UpdateUser(_testUserId, "testuser", "updated@example.com", "newpassword123");
        
        _mockUserRepository.Setup(x => x.Get(_testUserId)).ReturnsAsync(_testUser);
        _mockMap.Setup(x => x.From(It.IsAny<UserModel>())).Returns(new MockMapExpression<UserUpdated>(new UserUpdated()));

        // Act
        await _userService.UpdateUser(command, _testUserId);

        // Assert
        _testUser.EMail.Should().Be("updated@example.com");
        _mockUserRepository.Verify(x => x.Update(_testUser), Times.Once);
        _mockPublisher.Verify(x => x.PublishToUserAsync(_testUserId.ToString(), It.IsAny<UserUpdated>()), Times.Once);
    }

    [Test]
    public async Task UpdateUser_WithNonExistentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new UpdateUser(_testUserId, "testuser", "updated@example.com", "newpassword123");
        
        _mockUserRepository.Setup(x => x.Get(_testUserId)).ReturnsAsync((UserModel?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.UpdateUser(command, _testUserId));
        exception.Message.Should().Be("User not found!");
    }

    [Test]
    public async Task UpdateUser_WithDifferentUserId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var otherUserId = Guid.NewGuid();
        var command = new UpdateUser(_testUserId, "testuser", "updated@example.com", "newpassword123");
        
        _mockUserRepository.Setup(x => x.Get(_testUserId)).ReturnsAsync(_testUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.UpdateUser(command, otherUserId));
        exception.Message.Should().Be("You must not modify other users!");
    }

    [Test]
    public async Task UpdateUser_WithDifferentUsername_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new UpdateUser(_testUserId, "differentuser", "updated@example.com", "newpassword123");
        
        _mockUserRepository.Setup(x => x.Get(_testUserId)).ReturnsAsync(_testUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.UpdateUser(command, _testUserId));
        exception.Message.Should().Be("Username can not be modified!");
    }

    [Test]
    public async Task UpdateUser_WithShortPassword_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new UpdateUser(_testUserId, "testuser", "updated@example.com", "12345");
        
        _mockUserRepository.Setup(x => x.Get(_testUserId)).ReturnsAsync(_testUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.UpdateUser(command, _testUserId));
        exception.Message.Should().Be("Password too short!");
    }

    [Test]
    public async Task UpdateUser_WithEmptyPassword_ShouldNotUpdatePassword()
    {
        // Arrange
        var originalPasswordHash = _testUser.PasswordHash;
        var command = new UpdateUser(_testUserId, "testuser", "updated@example.com", "");
        
        _mockUserRepository.Setup(x => x.Get(_testUserId)).ReturnsAsync(_testUser);
        _mockMap.Setup(x => x.From(It.IsAny<UserModel>())).Returns(new MockMapExpression<UserUpdated>(new UserUpdated()));

        // Act
        await _userService.UpdateUser(command, _testUserId);

        // Assert
        _testUser.PasswordHash.Should().BeEquivalentTo(originalPasswordHash);
        _testUser.EMail.Should().Be("updated@example.com");
    }

    #endregion

    #region UpdateUserSettings Tests

    [Test]
    public async Task UpdateUserSettings_WithValidData_ShouldUpdateSettingsSuccessfully()
    {
        // Arrange
        var command = new UpdateUserSettings(60);
        
        _mockUserRepository.Setup(x => x.Get(_testUserId)).ReturnsAsync(_testUser);

        // Act
        await _userService.UpdateUserSettings(command, _testUserId);

        // Assert
        _testUser.Settings.LogRetentionDays.Should().Be(60);
        _mockUserRepository.Verify(x => x.Update(_testUser), Times.Once);
    }

    [Test]
    public async Task UpdateUserSettings_WithNonExistentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new UpdateUserSettings(60);
        
        _mockUserRepository.Setup(x => x.Get(_testUserId)).ReturnsAsync((UserModel?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.UpdateUserSettings(command, _testUserId));
        exception.Message.Should().Be("User not found!");
    }

    #endregion

    #region DeleteUser Tests

    [Test]
    public async Task DeleteUser_WithValidUser_ShouldMarkUserAsInactive()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.Get(_testUserId)).ReturnsAsync(_testUser);

        // Act
        await _userService.DeleteUser(_testUserId, _testUserId);

        // Assert
        _testUser.IsActive.Should().BeFalse();
        _mockUserRepository.Verify(x => x.Update(_testUser), Times.Once);
        _mockPublisher.Verify(x => x.PublishToUserAsync(_testUserId.ToString(), It.IsAny<UserDeleted>()), Times.Once);
    }

    [Test]
    public async Task DeleteUser_WithNonExistentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.Get(_testUserId)).ReturnsAsync((UserModel?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.DeleteUser(_testUserId, _testUserId));
        exception.Message.Should().Be("User not found!");
    }

    [Test]
    public async Task DeleteUser_WithDifferentUserId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var otherUserId = Guid.NewGuid();
        _mockUserRepository.Setup(x => x.Get(_testUserId)).ReturnsAsync(_testUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.DeleteUser(_testUserId, otherUserId));
        exception.Message.Should().Be("You must not delete other user!");
    }

    #endregion

    #region GetItem Tests

    [Test]
    public async Task GetItem_WithExistingUser_ShouldReturnUser()
    {
        // Arrange
        var expectedUser = new User();
        _mockUserRepository.Setup(x => x.Get(_testUserId)).ReturnsAsync(_testUser);
        _mockMap.Setup(x => x.FromNullable(_testUser)).Returns(new MockMapExpression<User>(expectedUser));

        // Act
        var result = await _userService.GetItem(_testUserId);

        // Assert
        result.Should().Be(expectedUser);
    }

    [Test]
    public async Task GetItem_WithNonExistentUser_ShouldReturnNull()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.Get(_testUserId)).ReturnsAsync((UserModel?)null);
        _mockMap.Setup(x => x.FromNullable(It.IsAny<UserModel?>())).Returns((IMapExpression<User>?)null);

        // Act
        var result = await _userService.GetItem(_testUserId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetItemsForUser Tests

    [Test]
    public void GetItemsForUser_ShouldReturnActiveUsersForGivenUserId()
    {
        // Arrange
        var userList = new[] { _testUser }.AsQueryable();
        var expectedUserListItems = new[] { new UserListItem() };
        
        _mockUserRepository.Setup(x => x.GetAll()).Returns(userList);
        _mockMap.Setup(x => x.From(It.IsAny<IQueryable<UserModel>>())).Returns(expectedUserListItems.ToAsyncEnumerable());

        // Act
        var result = _userService.GetItemsForUser(_testUserId);

        // Assert
        result.Should().NotBeNull();
    }

    #endregion

    #region GetUserModelFromName Tests

    [Test]
    public async Task GetUserModelFromName_WithValidUsername_ShouldReturnUser()
    {
        // Arrange
        var username = "TestUser";
        var normalizedUsername = username.Normalize().ToLower();
        
        _mockUserRepository.Setup(x => x.Get(normalizedUsername)).ReturnsAsync(_testUser);

        // Act
        var result = await _userService.GetUserModelFromName(username);

        // Assert
        result.Should().Be(_testUser);
    }

    [Test]
    public async Task GetUserModelFromName_WithNonExistentUsername_ShouldReturnNull()
    {
        // Arrange
        var username = "NonExistentUser";
        var normalizedUsername = username.Normalize().ToLower();
        
        _mockUserRepository.Setup(x => x.Get(normalizedUsername)).ReturnsAsync((UserModel?)null);

        // Act
        var result = await _userService.GetUserModelFromName(username);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetUserModelFromName_ShouldNormalizeUsername()
    {
        // Arrange
        var username = "TESTUSER";
        var normalizedUsername = username.Normalize().ToLower();
        
        _mockUserRepository.Setup(x => x.Get(normalizedUsername)).ReturnsAsync(_testUser);

        // Act
        await _userService.GetUserModelFromName(username);

        // Assert
        _mockUserRepository.Verify(x => x.Get(normalizedUsername), Times.Once);
    }

    #endregion
}