using System;
using System.Collections.Generic;

namespace LogService.Contracts
{
	public interface ILogMessage<TLogArgument>
		where TLogArgument : ILogArgument
	{
		Guid Id { get; set; }
		DateTime DateTime { get; set; }
		LogLevel LogLevel { get; set; }
		string Category { get; set; }
		string State { get; set; }
		string Source { get; set; }
		string RawMessage { get; set; }
		string Message { get; set; }
		string Exception { get; set; }
		bool ExceptionIsValid { get; set; }
		List<string> RawArguments { get; set; }
		List<TLogArgument> Arguments { get; set; }
	}
}
