using System;
using System.Collections.Generic;

namespace LogService.Contracts.Events;

public class LogMessageCreated(
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
	public Guid Id { get; } = id;
	public DateTime DateTime { get; } = dateTime;
	public LogLevel LogLevel { get; } = logLevel;
	public string Category { get; } = category;
	public string State { get; } = state;
	public string Source { get; } = source;
	public string RawMessage { get; } = rawMessage;
	public string Message { get; } = message;
	public string Exception { get; } = exception;
	public bool ExceptionIsValid { get; } = exceptionIsValid;
	public List<LogArgument> Arguments { get; } = arguments;
}