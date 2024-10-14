using System;
using System.Collections.Generic;

namespace LogService.Contracts.Commands
{
	public class UpdateLogMessage
	{
		public Guid Id { get; set; }
		public DateTime DateTime { get; set; }
		public LogLevel LogLevel { get; set; }
		public string Category { get; set; }
		public string State { get; set; }
		public string Source { get; set; }
		public string RawMessage { get; set; }
		public string Message { get; set; }
		public string Exception { get; set; }
		public bool ExceptionIsValid { get; set; }
		public List<string> RawArguments { get; set; }
		public List<LogArgument> Arguments { get; set; }

		private UpdateLogMessage() { }

		public UpdateLogMessage(
			Guid id,
			DateTime dateTime,
			LogLevel logLevel,
			string category,
			string state,
			string source,
			string rawMessage,
			string message,
			string exception,
			bool exceptionIsValid,
			List<string> rawArguments,
			List<LogArgument> arguments)
		{
			Id = id;
			DateTime = dateTime;
			LogLevel = logLevel;
			Category = category;
			State = state;
			Source = source;
			RawMessage = rawMessage;
			Message = message;
			Exception = exception;
			ExceptionIsValid = exceptionIsValid;
			RawArguments = rawArguments;
			Arguments = arguments;
		}
	}
}
