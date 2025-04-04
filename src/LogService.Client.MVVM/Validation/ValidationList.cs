using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogService.Client.MVVM.Validation;

public abstract class ValidationList<T> : List<ValidationBase<T>>
{
	public async Task<IEnumerable<ValidationResult>> ValidateAsync(T o)
	{
		return await Task.Run(() => Validate(o));
	}

	public IEnumerable<ValidationResult> Validate(T o)
	{
		try
		{
			return this.SelectMany(x => x.Validate(o)).ToList();
		}
		catch (Exception ex)
		{
			return
			[
				new ValidationResult(
					ValidationSeverity.Error,
					"",
					"Error occured while validation: " + ex.Message)
			];
		}
	}
}