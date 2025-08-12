using NUnit.Framework;
using System.Threading.Tasks;
using LogService.Blazor.Identity;

namespace LogService.Client.Data.Test.Identity;

[TestFixture]
public class MemoryUserDataStoreTests
{
	private MemoryUserDataStore _dataStore = null!;

	[SetUp]
	public void SetUp()
	{
		_dataStore = new MemoryUserDataStore();
	}

	[Test]
	public async Task GetAccessToken_WhenNoTokenSet_ShouldReturnNull()
	{
		// Act
		var token = await _dataStore.GetAccessToken();

		// Assert
		Assert.That(token, Is.Null);
	}

	[Test]
	public async Task GetUsername_WhenNoUsernameSet_ShouldReturnNull()
	{
		// Act
		var username = await _dataStore.GetUsername();

		// Assert
		Assert.That(username, Is.Null);
	}

	[Test]
	public async Task SetAccessToken_ShouldStoreToken()
	{
		// Arrange
		var expectedToken = "test-token";

		// Act
		await _dataStore.SetAccessToken(expectedToken);
		var actualToken = await _dataStore.GetAccessToken();

		// Assert
		Assert.That(actualToken, Is.EqualTo(expectedToken));
	}

	[Test]
	public async Task SetUsername_ShouldStoreUsername()
	{
		// Arrange
		var expectedUsername = "test-user";

		// Act
		await _dataStore.SetUsername(expectedUsername);
		var actualUsername = await _dataStore.GetUsername();

		// Assert
		Assert.That(actualUsername, Is.EqualTo(expectedUsername));
	}

	[Test]
	public async Task SetUserData_ShouldStoreBothUsernameAndToken()
	{
		// Arrange
		var expectedUsername = "test-user";
		var expectedToken = "test-token";

		// Act
		await _dataStore.SetUserData(expectedUsername, expectedToken);
		var actualUsername = await _dataStore.GetUsername();
		var actualToken = await _dataStore.GetAccessToken();

		// Assert
		Assert.That(actualUsername, Is.EqualTo(expectedUsername));
		Assert.That(actualToken, Is.EqualTo(expectedToken));
	}

	[Test]
	public async Task SetAccessToken_Multiple_ShouldOverwritePreviousToken()
	{
		// Arrange
		var firstToken = "first-token";
		var secondToken = "second-token";

		// Act
		await _dataStore.SetAccessToken(firstToken);
		await _dataStore.SetAccessToken(secondToken);
		var actualToken = await _dataStore.GetAccessToken();

		// Assert
		Assert.That(actualToken, Is.EqualTo(secondToken));
	}

	[Test]
	public async Task SetUsername_Multiple_ShouldOverwritePreviousUsername()
	{
		// Arrange
		var firstUsername = "first-user";
		var secondUsername = "second-user";

		// Act
		await _dataStore.SetUsername(firstUsername);
		await _dataStore.SetUsername(secondUsername);
		var actualUsername = await _dataStore.GetUsername();

		// Assert
		Assert.That(actualUsername, Is.EqualTo(secondUsername));
	}

	[Test]
	public async Task SetUserData_AfterIndividualSets_ShouldOverwriteBoth()
	{
		// Arrange
		await _dataStore.SetUsername("old-user");
		await _dataStore.SetAccessToken("old-token");
		
		var newUsername = "new-user";
		var newToken = "new-token";

		// Act
		await _dataStore.SetUserData(newUsername, newToken);
		var actualUsername = await _dataStore.GetUsername();
		var actualToken = await _dataStore.GetAccessToken();

		// Assert
		Assert.That(actualUsername, Is.EqualTo(newUsername));
		Assert.That(actualToken, Is.EqualTo(newToken));
	}

	[Test]
	public async Task MultipleInstances_ShouldHaveIndependentData()
	{
		// Arrange
		var dataStore1 = new MemoryUserDataStore();
		var dataStore2 = new MemoryUserDataStore();
		
		var username1 = "user1";
		var token1 = "token1";
		var username2 = "user2";
		var token2 = "token2";

		// Act
		await dataStore1.SetUserData(username1, token1);
		await dataStore2.SetUserData(username2, token2);

		var actualUsername1 = await dataStore1.GetUsername();
		var actualToken1 = await dataStore1.GetAccessToken();
		var actualUsername2 = await dataStore2.GetUsername();
		var actualToken2 = await dataStore2.GetAccessToken();

		// Assert
		Assert.That(actualUsername1, Is.EqualTo(username1));
		Assert.That(actualToken1, Is.EqualTo(token1));
		Assert.That(actualUsername2, Is.EqualTo(username2));
		Assert.That(actualToken2, Is.EqualTo(token2));
	}
}