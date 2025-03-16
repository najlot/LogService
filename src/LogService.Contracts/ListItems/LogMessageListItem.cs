using System;
using System.Collections.Generic;

namespace LogService.Contracts.ListItems;

public sealed class LogMessageListItem
{
	public Guid Id { get; set; }
	public DateTime DateTime { get; set; }
	public LogLevel LogLevel { get; set; }
}