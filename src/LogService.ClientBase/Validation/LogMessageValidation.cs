using System;
using System.Collections.Generic;
using LogService.Client.MVVM.Validation;
using LogService.ClientBase.ViewModel;

namespace LogService.ClientBase.Validation
{
	public class LogMessageValidation : ValidationBase<LogMessageViewModel>
	{
		public override IEnumerable<ValidationResult> Validate(LogMessageViewModel o)
		{
			return Array.Empty<ValidationResult>();
		}
	}
}
