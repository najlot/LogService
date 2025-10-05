using FluentAssertions;
using LogService.Configuration;
using LogService.Services;
using Moq;
using Xunit;

namespace LogService.Tests.Services;

public class TokenServiceTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly ServiceConfiguration _serviceConfiguration;
    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        _mockUserService = new Mock<IUserService>();
        _serviceConfiguration = new ServiceConfiguration
        {
            Secret = "this-is-a-very-long-secret-key-for-jwt-token-generation-minimum-32-chars"
        };
        _tokenService = new TokenService(_mockUserService.Object, _serviceConfiguration);
    }

    [Fact]
    public void GetValidationParameters_ShouldReturnValidParameters()
    {
        // Act
        var result = _tokenService.GetValidationParameters();

        // Assert
        result.Should().NotBeNull();
        result.ValidateLifetime.Should().BeTrue();
        result.ValidateAudience.Should().BeFalse();
        result.ValidateIssuer.Should().BeFalse();
        result.ValidateIssuerSigningKey.Should().BeTrue();
        result.IssuerSigningKey.Should().NotBeNull();
    }

    [Fact]
    public void GetValidationParameters_Static_WithValidSecret_ShouldReturnValidParameters()
    {
        // Arrange
        var secret = "test-secret-key-minimum-32-characters-long";

        // Act
        var result = TokenService.GetValidationParameters(secret);

        // Assert
        result.Should().NotBeNull();
        result.ValidateLifetime.Should().BeTrue();
        result.ValidateIssuerSigningKey.Should().BeTrue();
        result.IssuerSigningKey.Should().NotBeNull();
    }

    [Fact]
    public void GetServiceToken_ShouldGenerateValidToken()
    {
        // Arrange
        var username = "testuser";
        var userId = Guid.NewGuid();
        var source = "TestApp";
        var validUntil = DateTime.UtcNow.AddHours(1);

        // Act
        var token = _tokenService.GetServiceToken(username, userId, source, validUntil);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT has 3 parts
    }

    [Fact]
    public async Task GetToken_ShouldGenerateValidToken()
    {
        // Arrange
        var username = "testuser";
        var password = "testpassword";

        // Mock a valid user
        _mockUserService
            .Setup(s => s.GetUserModelFromName(username))
            .ReturnsAsync(new Model.UserModel 
            { 
                Id = Guid.NewGuid(), 
                Username = username,
                PasswordHash = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password)),
                IsActive = true
            });

        // Act
        var token = await _tokenService.GetToken(username, password);

        // Assert
        token.Should().NotBeNullOrEmpty();
        if (token != null)
        {
            token.Split('.').Should().HaveCount(3); // JWT has 3 parts
        }
    }

    [Fact]
    public void GetServiceToken_WithPastDate_ShouldStillGenerateToken()
    {
        // Arrange
        var username = "testuser";
        var userId = Guid.NewGuid();
        var source = "TestApp";
        var validUntil = DateTime.UtcNow.AddHours(-1); // Past date

        // Act
        var token = _tokenService.GetServiceToken(username, userId, source, validUntil);

        // Assert
        token.Should().NotBeNullOrEmpty();
    }
}
