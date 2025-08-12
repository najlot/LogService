using NUnit.Framework;
using System;
using System.Collections.Generic;
using LogService.Client.Data.Models;
using LogService.Contracts;

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

[TestFixture]
public class LogArgumentModelTests
{
	[Test]
	public void Constructor_ShouldInitializeWithDefaultValues()
	{
		// Act
		var logArgumentModel = new LogArgumentModel();

		// Assert
		Assert.That(logArgumentModel.Id, Is.EqualTo(0));
		Assert.That(logArgumentModel.Key, Is.EqualTo(string.Empty));
		Assert.That(logArgumentModel.Value, Is.EqualTo(string.Empty));
	}

	[Test]
	public void Properties_ShouldBeSettableAndGettable()
	{
		// Arrange
		var logArgumentModel = new LogArgumentModel();
		var expectedId = 123;
		var expectedKey = "testKey";
		var expectedValue = "testValue";

		// Act
		logArgumentModel.Id = expectedId;
		logArgumentModel.Key = expectedKey;
		logArgumentModel.Value = expectedValue;

		// Assert
		Assert.That(logArgumentModel.Id, Is.EqualTo(expectedId));
		Assert.That(logArgumentModel.Key, Is.EqualTo(expectedKey));
		Assert.That(logArgumentModel.Value, Is.EqualTo(expectedValue));
	}
}

[TestFixture]
public class LogMessageModelTests
{
	[Test]
	public void Constructor_ShouldInitializeWithDefaultValues()
	{
		// Act
		var logMessageModel = new LogMessageModel();

		// Assert
		Assert.That(logMessageModel.Id, Is.EqualTo(Guid.Empty));
		Assert.That(logMessageModel.DateTime, Is.EqualTo(DateTime.MinValue));
		Assert.That(logMessageModel.LogLevel, Is.EqualTo(LogLevel.Trace));
		Assert.That(logMessageModel.Category, Is.EqualTo(string.Empty));
		Assert.That(logMessageModel.State, Is.EqualTo(string.Empty));
		Assert.That(logMessageModel.Source, Is.EqualTo(string.Empty));
		Assert.That(logMessageModel.RawMessage, Is.EqualTo(string.Empty));
		Assert.That(logMessageModel.Message, Is.EqualTo(string.Empty));
		Assert.That(logMessageModel.Exception, Is.EqualTo(string.Empty));
		Assert.That(logMessageModel.ExceptionIsValid, Is.EqualTo(false));
		Assert.That(logMessageModel.Arguments, Is.Not.Null);
		Assert.That(logMessageModel.Arguments, Is.Empty);
	}

	[Test]
	public void Properties_ShouldBeSettableAndGettable()
	{
		// Arrange
		var logMessageModel = new LogMessageModel();
		var expectedId = Guid.NewGuid();
		var expectedDateTime = DateTime.UtcNow;
		var expectedLogLevel = LogLevel.Error;
		var expectedCategory = "TestCategory";
		var expectedState = "TestState";
		var expectedSource = "TestSource";
		var expectedRawMessage = "Raw message";
		var expectedMessage = "Test message";
		var expectedException = "Test exception";
		var expectedExceptionIsValid = true;
		var expectedArguments = new List<LogArgumentModel>
		{
			new LogArgumentModel { Id = 1, Key = "key1", Value = "value1" }
		};

		// Act
		logMessageModel.Id = expectedId;
		logMessageModel.DateTime = expectedDateTime;
		logMessageModel.LogLevel = expectedLogLevel;
		logMessageModel.Category = expectedCategory;
		logMessageModel.State = expectedState;
		logMessageModel.Source = expectedSource;
		logMessageModel.RawMessage = expectedRawMessage;
		logMessageModel.Message = expectedMessage;
		logMessageModel.Exception = expectedException;
		logMessageModel.ExceptionIsValid = expectedExceptionIsValid;
		logMessageModel.Arguments = expectedArguments;

		// Assert
		Assert.That(logMessageModel.Id, Is.EqualTo(expectedId));
		Assert.That(logMessageModel.DateTime, Is.EqualTo(expectedDateTime));
		Assert.That(logMessageModel.LogLevel, Is.EqualTo(expectedLogLevel));
		Assert.That(logMessageModel.Category, Is.EqualTo(expectedCategory));
		Assert.That(logMessageModel.State, Is.EqualTo(expectedState));
		Assert.That(logMessageModel.Source, Is.EqualTo(expectedSource));
		Assert.That(logMessageModel.RawMessage, Is.EqualTo(expectedRawMessage));
		Assert.That(logMessageModel.Message, Is.EqualTo(expectedMessage));
		Assert.That(logMessageModel.Exception, Is.EqualTo(expectedException));
		Assert.That(logMessageModel.ExceptionIsValid, Is.EqualTo(expectedExceptionIsValid));
		Assert.That(logMessageModel.Arguments, Is.EqualTo(expectedArguments));
	}
}

[TestFixture]
public class LogMessageListItemModelTests
{
	[Test]
	public void Constructor_ShouldInitializeWithDefaultValues()
	{
		// Act
		var listItemModel = new LogMessageListItemModel();

		// Assert
		Assert.That(listItemModel.Id, Is.EqualTo(Guid.Empty));
		Assert.That(listItemModel.DateTime, Is.EqualTo(DateTime.MinValue));
		Assert.That(listItemModel.LogLevel, Is.EqualTo(LogLevel.Trace));
		Assert.That(listItemModel.Message, Is.EqualTo(string.Empty));
	}

	[Test]
	public void Properties_ShouldBeSettableAndGettable()
	{
		// Arrange
		var listItemModel = new LogMessageListItemModel();
		var expectedId = Guid.NewGuid();
		var expectedDateTime = DateTime.UtcNow;
		var expectedLogLevel = LogLevel.Warn;
		var expectedMessage = "Test message";

		// Act
		listItemModel.Id = expectedId;
		listItemModel.DateTime = expectedDateTime;
		listItemModel.LogLevel = expectedLogLevel;
		listItemModel.Message = expectedMessage;

		// Assert
		Assert.That(listItemModel.Id, Is.EqualTo(expectedId));
		Assert.That(listItemModel.DateTime, Is.EqualTo(expectedDateTime));
		Assert.That(listItemModel.LogLevel, Is.EqualTo(expectedLogLevel));
		Assert.That(listItemModel.Message, Is.EqualTo(expectedMessage));
	}
}

[TestFixture]
public class UserListItemModelTests
{
	[Test]
	public void Constructor_ShouldInitializeWithDefaultValues()
	{
		// Act
		var listItemModel = new UserListItemModel();

		// Assert
		Assert.That(listItemModel.Id, Is.EqualTo(Guid.Empty));
		Assert.That(listItemModel.Username, Is.EqualTo(string.Empty));
		Assert.That(listItemModel.EMail, Is.EqualTo(string.Empty));
	}

	[Test]
	public void Properties_ShouldBeSettableAndGettable()
	{
		// Arrange
		var listItemModel = new UserListItemModel();
		var expectedId = Guid.NewGuid();
		var expectedUsername = "testuser";
		var expectedEmail = "test@example.com";

		// Act
		listItemModel.Id = expectedId;
		listItemModel.Username = expectedUsername;
		listItemModel.EMail = expectedEmail;

		// Assert
		Assert.That(listItemModel.Id, Is.EqualTo(expectedId));
		Assert.That(listItemModel.Username, Is.EqualTo(expectedUsername));
		Assert.That(listItemModel.EMail, Is.EqualTo(expectedEmail));
	}
}