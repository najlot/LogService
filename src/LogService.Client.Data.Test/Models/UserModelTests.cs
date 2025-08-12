using NUnit.Framework;
using System;
using LogService.Client.Data.Models;

namespace LogService.Client.Data.Test.Models;

[TestFixture]
public class UserModelTests
{
	[Test]
	public void Constructor_ShouldInitializeWithDefaultValues()
	{
		// Act
		var userModel = new UserModel();

		// Assert
		Assert.That(userModel.Id, Is.EqualTo(Guid.Empty));
		Assert.That(userModel.Username, Is.EqualTo(string.Empty));
		Assert.That(userModel.EMail, Is.EqualTo(string.Empty));
		Assert.That(userModel.Password, Is.EqualTo(string.Empty));
		Assert.That(userModel.Settings, Is.Not.Null);
		Assert.That(userModel.Settings, Is.TypeOf<UserSettingsModel>());
	}

	[Test]
	public void Properties_ShouldBeSettableAndGettable()
	{
		// Arrange
		var userModel = new UserModel();
		var expectedId = Guid.NewGuid();
		var expectedUsername = "testuser";
		var expectedEmail = "test@example.com";
		var expectedPassword = "password123";
		var expectedSettings = new UserSettingsModel { LogRetentionDays = 30 };

		// Act
		userModel.Id = expectedId;
		userModel.Username = expectedUsername;
		userModel.EMail = expectedEmail;
		userModel.Password = expectedPassword;
		userModel.Settings = expectedSettings;

		// Assert
		Assert.That(userModel.Id, Is.EqualTo(expectedId));
		Assert.That(userModel.Username, Is.EqualTo(expectedUsername));
		Assert.That(userModel.EMail, Is.EqualTo(expectedEmail));
		Assert.That(userModel.Password, Is.EqualTo(expectedPassword));
		Assert.That(userModel.Settings, Is.EqualTo(expectedSettings));
	}
}

[TestFixture]
public class UserSettingsModelTests
{
	[Test]
	public void Constructor_ShouldInitializeWithDefaultValues()
	{
		// Act
		var settingsModel = new UserSettingsModel();

		// Assert
		Assert.That(settingsModel.LogRetentionDays, Is.EqualTo(0));
	}

	[Test]
	public void LogRetentionDays_ShouldBeSettableAndGettable()
	{
		// Arrange
		var settingsModel = new UserSettingsModel();
		var expectedRetentionDays = 90;

		// Act
		settingsModel.LogRetentionDays = expectedRetentionDays;

		// Assert
		Assert.That(settingsModel.LogRetentionDays, Is.EqualTo(expectedRetentionDays));
	}
}