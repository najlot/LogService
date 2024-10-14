using MongoDB.Bson.Serialization.Attributes;
using LogService.Contracts;
using System;
using System.Collections.Generic;

namespace LogService.Service.Model
{
	[BsonIgnoreExtraElements]
	public class LogMessageModel
	{
		[BsonId]
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
	}
}
