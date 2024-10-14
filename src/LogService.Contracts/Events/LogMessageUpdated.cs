using System;
using System.Collections.Generic;

namespace LogService.Contracts.Events
{
	public class LogMessageUpdated
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
		public List<LogArgument> Arguments { get; set; }

		private LogMessageUpdated() { }

		public LogMessageUpdated(
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
			Arguments = arguments;
		}
	}
}
