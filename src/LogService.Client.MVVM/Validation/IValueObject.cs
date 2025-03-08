using System.Collections.Generic;

namespace LogService.Client.MVVM.Validation;

public interface IValueObject
{
	IEnumerable<ValidationResult> Validate();
}