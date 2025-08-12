using NUnit.Framework;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogService.Client.Data.Models;
using LogService.Client.Data.Repositories;
using LogService.Client.Data.Services.Implementation;

namespace LogService.Client.Data.Test.Services;

[TestFixture]
public class UserServiceTests
{
	private IUserRepository _mockRepository = null!;
	private UserService _userService = null!;

	[SetUp]
	public void SetUp()
	{
		_mockRepository = Substitute.For<IUserRepository>();
		_userService = new UserService(_mockRepository);
	}

	[TearDown]
	public void TearDown()
	{
		_userService?.Dispose();
	}

	[Test]
	public void CreateUser_ShouldReturnNewUserWithValidId()
	{
		// Act
		var user = _userService.CreateUser();

		// Assert
		Assert.That(user, Is.Not.Null);
		Assert.That(user.Id, Is.Not.EqualTo(Guid.Empty));
		Assert.That(user.Username, Is.EqualTo(""));
		Assert.That(user.EMail, Is.EqualTo(""));
		Assert.That(user.Password, Is.EqualTo(""));
		Assert.That(user.Settings, Is.Not.Null);
	}

	[Test]
	public void CreateUser_MultipleCallsShouldReturnDifferentIds()
	{
		// Act
		var user1 = _userService.CreateUser();
		var user2 = _userService.CreateUser();

		// Assert
		Assert.That(user1.Id, Is.Not.EqualTo(user2.Id));
	}

	[Test]
	public async Task AddItemAsync_ShouldCallRepositoryAddItem()
	{
		// Arrange
		var user = new UserModel { Id = Guid.NewGuid(), Username = "test" };

		// Act
		await _userService.AddItemAsync(user);

		// Assert
		await _mockRepository.Received(1).AddItemAsync(user);
	}

	[Test]
	public async Task DeleteItemAsync_ShouldCallRepositoryDeleteItem()
	{
		// Arrange
		var userId = Guid.NewGuid();

		// Act
		await _userService.DeleteItemAsync(userId);

		// Assert
		await _mockRepository.Received(1).DeleteItemAsync(userId);
	}

	[Test]
	public async Task GetCurrentUserAsync_ShouldCallRepositoryGetCurrentUser()
	{
		// Arrange
		var expectedUser = new UserModel { Id = Guid.NewGuid(), Username = "current" };
		_mockRepository.GetCurrentUserAsync().Returns(expectedUser);

		// Act
		var result = await _userService.GetCurrentUserAsync();

		// Assert
		Assert.That(result, Is.EqualTo(expectedUser));
		await _mockRepository.Received(1).GetCurrentUserAsync();
	}

	[Test]
	public async Task GetItemAsync_ShouldCallRepositoryGetItem()
	{
		// Arrange
		var userId = Guid.NewGuid();
		var expectedUser = new UserModel { Id = userId, Username = "test" };
		_mockRepository.GetItemAsync(userId).Returns(expectedUser);

		// Act
		var result = await _userService.GetItemAsync(userId);

		// Assert
		Assert.That(result, Is.EqualTo(expectedUser));
		await _mockRepository.Received(1).GetItemAsync(userId);
	}

	[Test]
	public async Task GetItemsAsync_ShouldCallRepositoryGetItems()
	{
		// Arrange
		var expectedUsers = new UserListItemModel[]
		{
			new UserListItemModel { Id = Guid.NewGuid(), Username = "user1" },
			new UserListItemModel { Id = Guid.NewGuid(), Username = "user2" }
		};
		_mockRepository.GetItemsAsync().Returns(expectedUsers);

		// Act
		var result = await _userService.GetItemsAsync();

		// Assert
		Assert.That(result, Is.EqualTo(expectedUsers));
		await _mockRepository.Received(1).GetItemsAsync();
	}

	[Test]
	public async Task UpdateSettingsAsync_ShouldCallRepositoryUpdateSettings()
	{
		// Arrange
		var settings = new UserSettingsModel();

		// Act
		await _userService.UpdateSettingsAsync(settings);

		// Assert
		await _mockRepository.Received(1).UpdateSettingsAsync(settings);
	}

	[Test]
	public async Task UpdateItemAsync_ShouldCallRepositoryUpdateItem()
	{
		// Arrange
		var user = new UserModel { Id = Guid.NewGuid(), Username = "updated" };

		// Act
		await _userService.UpdateItemAsync(user);

		// Assert
		await _mockRepository.Received(1).UpdateItemAsync(user);
	}

	[Test]
	public void Dispose_ShouldCallRepositoryDispose()
	{
		// Act
		_userService.Dispose();

		// Assert
		_mockRepository.Received(1).Dispose();
	}
}