using MongoDB.Bson.Serialization.Attributes;
using LogService.Contracts;
using System;
using System.Collections.Generic;

namespace LogService.Blazor.Model;

[BsonIgnoreExtraElements]
public class LogMessageModel
{
	[BsonId]
	public Guid Id { get; set; }
	public Guid CreatedBy { get; set; }
	public DateTime DateTime { get; set; }
	public Contracts.LogLevel LogLevel { get; set; }
	public string Category { get; set; } = string.Empty;
	public string State { get; set; } = string.Empty;
	public string Source { get; set; } = string.Empty;
	public string RawMessage { get; set; } = string.Empty;
	public string Message { get; set; } = string.Empty;
	public string Exception { get; set; } = string.Empty;
	public bool ExceptionIsValid { get; set; }
	public List<LogArgument> Arguments { get; set; } = [];
}