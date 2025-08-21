using NUnit.Framework;
using Moq;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using LogService.Service.Services;
using LogService.Service.Configuration;
using LogService.Service.Model;
using System.Text;
using System.Security.Cryptography;

namespace LogService.Service.Test.Services;

[TestFixture]
public class TokenServiceTests
{
    private Mock<IUserService> _mockUserService;
    private ServiceConfiguration _serviceConfiguration;
    private TokenService _tokenService;
    private const string TestSecret = "this-is-a-very-long-test-secret-key-for-jwt-token-validation";
    private Guid _testUserId;
    private UserModel _testUser;

    [SetUp]
    public void SetUp()
    {
        _mockUserService = new Mock<IUserService>();
        _serviceConfiguration = new ServiceConfiguration { Secret = TestSecret };
        _tokenService = new TokenService(_mockUserService.Object, _serviceConfiguration);
        _testUserId = Guid.NewGuid();
        _testUser = new UserModel
        {
            Id = _testUserId,
            Username = "testuser",
            PasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes("password123"))
        };
    }

    #region GetValidationParameters Tests

    [Test]
    public void GetValidationParameters_WithSecret_ShouldReturnValidParameters()
    {
        // Act
        var parameters = TokenService.GetValidationParameters(TestSecret);

        // Assert
        parameters.Should().NotBeNull();
        parameters.ValidateLifetime.Should().BeTrue();
        parameters.ValidateAudience.Should().BeFalse();
        parameters.ValidateIssuer.Should().BeFalse();
        parameters.ValidateActor.Should().BeFalse();
        parameters.ValidateIssuerSigningKey.Should().BeTrue();
        parameters.IssuerSigningKey.Should().NotBeNull();
        parameters.IssuerSigningKey.Should().BeOfType<SymmetricSecurityKey>();
    }

    [Test]
    public void GetValidationParameters_LifetimeValidator_ShouldValidateExpiration()
    {
        // Arrange
        var parameters = TokenService.GetValidationParameters(TestSecret);
        var futureExpiration = DateTime.UtcNow.AddMinutes(30);
        var pastExpiration = DateTime.UtcNow.AddMinutes(-30);

        // Act & Assert
        parameters.LifetimeValidator(null, futureExpiration, null, parameters).Should().BeTrue();
        parameters.LifetimeValidator(null, pastExpiration, null, parameters).Should().BeFalse();
    }

    [Test]
    public void GetValidationParameters_InstanceMethod_ShouldUseConfigurationSecret()
    {
        // Act
        var parameters = _tokenService.GetValidationParameters();

        // Assert
        parameters.Should().NotBeNull();
        parameters.ValidateIssuerSigningKey.Should().BeTrue();
        parameters.IssuerSigningKey.Should().NotBeNull();
    }

    #endregion

    #region GetRefreshToken Tests

    [Test]
    public void GetRefreshToken_WithValidParameters_ShouldGenerateValidToken()
    {
        // Arrange
        var username = "testuser";
        var userId = _testUserId;

        // Act
        var token = _tokenService.GetRefreshToken(username, userId);

        // Assert
        token.Should().NotBeNullOrEmpty();

        // Verify token structure
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == username);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        jwtToken.Issuer.Should().Be("Log.Service");
        jwtToken.Audiences.Should().Contain("Log.Service");
        jwtToken.ValidTo.Should().BeAfter(DateTime.UtcNow.AddDays(6));
    }

    [Test]
    public void GetRefreshToken_ShouldSetCorrectExpiration()
    {
        // Arrange
        var username = "testuser";
        var userId = _testUserId;
        var expectedExpiration = DateTime.UtcNow.AddDays(7);

        // Act
        var token = _tokenService.GetRefreshToken(username, userId);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        jwtToken.ValidTo.Should().BeCloseTo(expectedExpiration, TimeSpan.FromMinutes(1));
    }

    #endregion

    #region GetServiceToken Tests

    [Test]
    public void GetServiceToken_WithValidParameters_ShouldGenerateValidToken()
    {
        // Arrange
        var username = "testuser";
        var userId = _testUserId;
        var source = "TestApp";
        var validUntil = DateTime.UtcNow.AddHours(1);

        // Act
        var token = _tokenService.GetServiceToken(username, userId, source, validUntil);

        // Assert
        token.Should().NotBeNullOrEmpty();

        // Verify token structure
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == username);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == "Source" && c.Value == source);
        jwtToken.Issuer.Should().Be("Log.Service");
        jwtToken.Audiences.Should().Contain("Log.Service");
        jwtToken.ValidTo.Should().BeCloseTo(validUntil, TimeSpan.FromSeconds(10));
    }

    [Test]
    public void GetServiceToken_ShouldIncludeSourceClaim()
    {
        // Arrange
        var username = "testuser";
        var userId = _testUserId;
        var source = "MyApplication";
        var validUntil = DateTime.UtcNow.AddHours(1);

        // Act
        var token = _tokenService.GetServiceToken(username, userId, source, validUntil);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        jwtToken.Claims.Should().Contain(c => c.Type == "Source" && c.Value == source);
    }

    #endregion

    #region GetToken Tests

    [Test]
    public async Task GetToken_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";
        
        _mockUserService.Setup(x => x.GetUserModelFromName(username)).ReturnsAsync(_testUser);

        // Act
        var token = await _tokenService.GetToken(username, password);

        // Assert
        token.Should().NotBeNullOrEmpty();

        // Verify token structure
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == username);
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == _testUserId.ToString());
        jwtToken.Issuer.Should().Be("Log.Service");
        jwtToken.Audiences.Should().Contain("Log.Service");
        jwtToken.ValidTo.Should().BeAfter(DateTime.UtcNow.AddDays(6));
    }

    [Test]
    public async Task GetToken_WithNonExistentUser_ShouldReturnNull()
    {
        // Arrange
        var username = "nonexistentuser";
        var password = "password123";
        
        _mockUserService.Setup(x => x.GetUserModelFromName(username)).ReturnsAsync((UserModel?)null);

        // Act
        var token = await _tokenService.GetToken(username, password);

        // Assert
        token.Should().BeNull();
    }

    [Test]
    public async Task GetToken_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var username = "testuser";
        var invalidPassword = "wrongpassword";
        
        _mockUserService.Setup(x => x.GetUserModelFromName(username)).ReturnsAsync(_testUser);

        // Act
        var token = await _tokenService.GetToken(username, invalidPassword);

        // Assert
        token.Should().BeNull();
    }

    [Test]
    public async Task GetToken_WithCorrectPasswordHash_ShouldReturnToken()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";
        
        // Set up user with the exact password hash that should match
        var userWithHash = new UserModel
        {
            Id = _testUserId,
            Username = username,
            PasswordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password))
        };
        
        _mockUserService.Setup(x => x.GetUserModelFromName(username)).ReturnsAsync(userWithHash);

        // Act
        var token = await _tokenService.GetToken(username, password);

        // Assert
        token.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task GetToken_ShouldHashPasswordCorrectly()
    {
        // Arrange
        var username = "testuser";
        var password = "mypassword";
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        
        var userWithMatchingHash = new UserModel
        {
            Id = _testUserId,
            Username = username,
            PasswordHash = expectedHash
        };
        
        _mockUserService.Setup(x => x.GetUserModelFromName(username)).ReturnsAsync(userWithMatchingHash);

        // Act
        var token = await _tokenService.GetToken(username, password);

        // Assert
        token.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task GetToken_ShouldSetCorrectTokenExpiration()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";
        var expectedExpiration = DateTime.UtcNow.AddDays(7);
        
        _mockUserService.Setup(x => x.GetUserModelFromName(username)).ReturnsAsync(_testUser);

        // Act
        var token = await _tokenService.GetToken(username, password);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        jwtToken.ValidTo.Should().BeCloseTo(expectedExpiration, TimeSpan.FromMinutes(1));
    }

    #endregion
}