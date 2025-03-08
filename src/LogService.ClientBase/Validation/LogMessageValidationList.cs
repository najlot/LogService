using System.Collections.Generic;
using LogService.Client.MVVM.Validation;
using LogService.ClientBase.ViewModel;

namespace LogService.ClientBase.Validation;

public class LogMessageValidationList : ValidationList<LogMessageViewModel>
{
	public LogMessageValidationList()
	{
		Add(new LogMessageValidation());
	}
}