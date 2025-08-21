using NUnit.Framework;
using System;
using LogService.Client.Data.Identity;

#nullable enable

namespace LogService.Client.Data.Test.Identity;

[TestFixture]
public class RegistrationResultTests
{
	[Test]
	public void Success_ShouldCreateSuccessfulResult()
	{
		// Act
		var result = RegistrationResult.Success();

		// Assert
		Assert.That(result.IsSuccess, Is.True);
		Assert.That(result.ErrorMessage, Is.EqualTo(""));
	}

	[Test]
	public void Failure_WithValidMessage_ShouldCreateFailureResult()
	{
		// Arrange
		var errorMessage = "Test error message";

		// Act
		var result = RegistrationResult.Failure(errorMessage);

		// Assert
		Assert.That(result.IsSuccess, Is.False);
		Assert.That(result.ErrorMessage, Is.EqualTo(errorMessage));
	}

	[Test]
	public void Failure_WithNullMessage_ShouldThrowArgumentException()
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => RegistrationResult.Failure(null!));
	}

	[Test]
	public void Failure_WithEmptyMessage_ShouldThrowArgumentException()
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => RegistrationResult.Failure(""));
	}

	[Test]
	public void Failure_WithWhitespaceMessage_ShouldThrowArgumentException()
	{
		// Act & Assert
		Assert.Throws<ArgumentException>(() => RegistrationResult.Failure("   "));
	}

	[Test]
	public void Equals_WithSameValues_ShouldReturnTrue()
	{
		// Arrange
		var result1 = RegistrationResult.Success();
		var result2 = RegistrationResult.Success();

		// Act & Assert
		Assert.That(result1.Equals(result2), Is.True);
		Assert.That(result1, Is.EqualTo(result2));
	}

	[Test]
	public void Equals_WithDifferentValues_ShouldReturnFalse()
	{
		// Arrange
		var result1 = RegistrationResult.Success();
		var result2 = RegistrationResult.Failure("Error");

		// Act & Assert
		Assert.That(result1.Equals(result2), Is.False);
		Assert.That(result1, Is.Not.EqualTo(result2));
	}

	[Test]
	public void Equals_WithNull_ShouldReturnFalse()
	{
		// Arrange
		var result = RegistrationResult.Success();

		// Act & Assert
		Assert.That(result.Equals(null), Is.False);
	}

	[Test]
	public void Equals_WithSameReference_ShouldReturnTrue()
	{
		// Arrange
		var result = RegistrationResult.Success();

		// Act & Assert
		Assert.That(result.Equals(result), Is.True);
	}

	[Test]
	public void GetHashCode_WithSameValues_ShouldReturnSameHashCode()
	{
		// Arrange
		var result1 = RegistrationResult.Success();
		var result2 = RegistrationResult.Success();

		// Act & Assert
		Assert.That(result1.GetHashCode(), Is.EqualTo(result2.GetHashCode()));
	}

	[Test]
	public void OperatorEquals_WithSameValues_ShouldReturnTrue()
	{
		// Arrange
		var result1 = RegistrationResult.Success();
		var result2 = RegistrationResult.Success();

		// Act & Assert
		Assert.That(result1 == result2, Is.True);
	}

	[Test]
	public void OperatorEquals_WithDifferentValues_ShouldReturnFalse()
	{
		// Arrange
		var result1 = RegistrationResult.Success();
		var result2 = RegistrationResult.Failure("Error");

		// Act & Assert
		Assert.That(result1 == result2, Is.False);
	}

	[Test]
	public void OperatorEquals_WithBothNull_ShouldReturnTrue()
	{
		// Arrange
		RegistrationResult? result1 = null;
		RegistrationResult? result2 = null;

		// Act & Assert
		Assert.That(result1 == result2, Is.True);
	}

	[Test]
	public void OperatorEquals_WithOneNull_ShouldReturnFalse()
	{
		// Arrange
		var result1 = RegistrationResult.Success();
		RegistrationResult? result2 = null;

		// Act & Assert
		Assert.That(result1 == result2, Is.False);
		Assert.That(result2 == result1, Is.False);
	}

	[Test]
	public void OperatorNotEquals_WithSameValues_ShouldReturnFalse()
	{
		// Arrange
		var result1 = RegistrationResult.Success();
		var result2 = RegistrationResult.Success();

		// Act & Assert
		Assert.That(result1 != result2, Is.False);
	}

	[Test]
	public void OperatorNotEquals_WithDifferentValues_ShouldReturnTrue()
	{
		// Arrange
		var result1 = RegistrationResult.Success();
		var result2 = RegistrationResult.Failure("Error");

		// Act & Assert
		Assert.That(result1 != result2, Is.True);
	}
}