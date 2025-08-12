using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Client.Data.Identity;

namespace LogService.Client.Data.Test.Identity;

[TestFixture]
public class TokenProviderExtensionsTests
{
	[Test]
	public async Task GetAuthorizationHeaders_ShouldReturnHeadersWithBearerToken()
	{
		// Arrange
		var mockTokenProvider = Substitute.For<ITokenProvider>();
		var expectedToken = "test-jwt-token";
		mockTokenProvider.GetToken().Returns(expectedToken);

		// Act
		var headers = await mockTokenProvider.GetAuthorizationHeaders();

		// Assert
		Assert.That(headers, Is.Not.Null);
		Assert.That(headers.ContainsKey("Authorization"), Is.True);
		Assert.That(headers["Authorization"], Is.EqualTo($"Bearer {expectedToken}"));
	}

	[Test]
	public async Task GetAuthorizationHeaders_WithEmptyToken_ShouldReturnHeadersWithEmptyBearer()
	{
		// Arrange
		var mockTokenProvider = Substitute.For<ITokenProvider>();
		var expectedToken = "";
		mockTokenProvider.GetToken().Returns(expectedToken);

		// Act
		var headers = await mockTokenProvider.GetAuthorizationHeaders();

		// Assert
		Assert.That(headers, Is.Not.Null);
		Assert.That(headers.ContainsKey("Authorization"), Is.True);
		Assert.That(headers["Authorization"], Is.EqualTo("Bearer "));
	}

	[Test]
	public async Task GetAuthorizationHeaders_ShouldCallGetTokenOnce()
	{
		// Arrange
		var mockTokenProvider = Substitute.For<ITokenProvider>();
		mockTokenProvider.GetToken().Returns("token");

		// Act
		await mockTokenProvider.GetAuthorizationHeaders();

		// Assert
		await mockTokenProvider.Received(1).GetToken();
	}

	[Test]
	public async Task GetAuthorizationHeaders_ShouldReturnNewDictionaryInstance()
	{
		// Arrange
		var mockTokenProvider = Substitute.For<ITokenProvider>();
		mockTokenProvider.GetToken().Returns("token");

		// Act
		var headers1 = await mockTokenProvider.GetAuthorizationHeaders();
		var headers2 = await mockTokenProvider.GetAuthorizationHeaders();

		// Assert
		Assert.That(headers1, Is.Not.SameAs(headers2));
	}
}