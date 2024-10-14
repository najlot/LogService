using System.Collections.Generic;
using LogService.Client.MVVM.Validation;
using LogService.ClientBase.ViewModel;

namespace LogService.ClientBase.Validation
{
	public class LogArgumentValidationList : ValidationList<LogArgumentViewModel>
	{
		public LogArgumentValidationList()
		{
			Add(new LogArgumentValidation());
		}
	}
}
