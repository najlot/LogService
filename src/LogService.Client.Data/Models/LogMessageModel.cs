using System;
using System.Collections.Generic;
using LogService.Contracts;

namespace LogService.Client.Data.Models
{
	public class LogMessageModel : ILogMessage<LogArgumentModel>
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
		public List<string> RawArguments { get; set; }
		public List<LogArgumentModel> Arguments { get; set; } = new();
	}
}
