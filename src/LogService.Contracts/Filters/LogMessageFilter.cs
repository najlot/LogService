using System;
using System.Collections.Generic;

namespace LogService.Contracts.Filters;

public sealed class LogMessageFilter
{
	public DateTime? DateTimeFrom { get; set; }
	public DateTime? DateTimeTo { get; set; }
	public LogLevel? LogLevel { get; set; }
	public string? Category { get; set; }
	public string? State { get; set; }
	public string? Source { get; set; }
	public string? RawMessage { get; set; }
	public string? Message { get; set; }
	public string? Exception { get; set; }
	public bool? ExceptionIsValid { get; set; }
}