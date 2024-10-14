using System;
using System.Collections.Generic;

namespace LogService.Contracts
{
	public class LogMessage : ILogMessage<LogArgument>
	{
		public Guid Id { get; set; }
		public DateTime DateTime { get; set; }
		public LogLevel LogLevel { get; set; }
		public string Category { get; set; } = string.Empty;
		public string State { get; set; } = string.Empty;
		public string Source { get; set; } = string.Empty;
		public string RawMessage { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public string Exception { get; set; } = string.Empty;
		public bool ExceptionIsValid { get; set; }
		public List<LogArgument> Arguments { get; set; } = new();
	}
}
