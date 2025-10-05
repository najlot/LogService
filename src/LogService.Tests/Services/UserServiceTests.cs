using FluentAssertions;
using LogService.Identity;
using LogService.Model;
using LogService.Repository;
using LogService.Services;
using Moq;
using Xunit;

namespace LogService.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<IAuthenticationService> _mockAuthService;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockAuthService = new Mock<IAuthenticationService>();
        _userService = new UserService(_mockRepository.Object, _mockAuthService.Object);
    }

    [Fact]
    public async Task CreateUser_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var id = Guid.NewGuid();
        var username = "TestUser";
        var email = "test@example.com";
        var password = "password123";

        _mockRepository
            .Setup(r => r.Get(It.IsAny<string>()))
            .ReturnsAsync((UserModel?)null);

        // Act
        await _userService.CreateUser(id, username, email, password);

        // Assert
        _mockRepository.Verify(r => r.Insert(It.Is<UserModel>(u =>
            u.Id == id &&
            u.Username == username.ToLower() &&
            u.EMail == email &&
            u.IsActive &&
            u.Settings != null &&
            u.Settings.LogRetentionDays == 7
        )), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WithExistingUsername_ShouldThrowException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var username = "ExistingUser";
        var email = "test@example.com";
        var password = "password123";

        _mockRepository
            .Setup(r => r.Get(It.IsAny<string>()))
            .ReturnsAsync(new UserModel { Username = username });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.CreateUser(id, username, email, password)
        );
    }

    [Fact]
    public async Task CreateUser_WithShortPassword_ShouldThrowException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var username = "TestUser";
        var email = "test@example.com";
        var password = "short";

        _mockRepository
            .Setup(r => r.Get(It.IsAny<string>()))
            .ReturnsAsync((UserModel?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.CreateUser(id, username, email, password)
        );
    }

    [Fact]
    public async Task CreateUser_ShouldNormalizeUsername()
    {
        // Arrange
        var id = Guid.NewGuid();
        var username = "TestUser";
        var email = "test@example.com";
        var password = "password123";

        _mockRepository
            .Setup(r => r.Get(It.IsAny<string>()))
            .ReturnsAsync((UserModel?)null);

        // Act
        await _userService.CreateUser(id, username, email, password);

        // Assert
        _mockRepository.Verify(r => r.Get("testuser"), Times.Once);
        _mockRepository.Verify(r => r.Insert(It.Is<UserModel>(u =>
            u.Username == "testuser"
        )), Times.Once);
    }

    [Fact]
    public async Task GetUserModelFromName_ShouldReturnUser()
    {
        // Arrange
        var username = "TestUser";
        var expectedUser = new UserModel
        {
            Id = Guid.NewGuid(),
            Username = username.ToLower(),
            EMail = "test@example.com"
        };

        _mockRepository
            .Setup(r => r.Get(username.ToLower()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserModelFromName(username);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedUser);
        _mockRepository.Verify(r => r.Get("testuser"), Times.Once);
    }

    [Fact]
    public async Task GetUserModelFromName_WithNonExistentUser_ShouldReturnNull()
    {
        // Arrange
        var username = "NonExistent";

        _mockRepository
            .Setup(r => r.Get(It.IsAny<string>()))
            .ReturnsAsync((UserModel?)null);

        // Act
        var result = await _userService.GetUserModelFromName(username);

        // Assert
        result.Should().BeNull();
    }
}
