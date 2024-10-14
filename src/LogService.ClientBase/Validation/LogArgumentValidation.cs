using System;
using System.Collections.Generic;
using LogService.Client.MVVM.Validation;
using LogService.ClientBase.ViewModel;

namespace LogService.ClientBase.Validation
{
	public class LogArgumentValidation : ValidationBase<LogArgumentViewModel>
	{
		public override IEnumerable<ValidationResult> Validate(LogArgumentViewModel o)
		{
			return Array.Empty<ValidationResult>();
		}
	}
}
