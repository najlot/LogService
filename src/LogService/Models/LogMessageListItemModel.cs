using System;
using System.Collections.Generic;
using LogService;

namespace LogService.Models;

public class LogMessageListItemModel
{
	public Guid Id { get; set; }
	public DateTime DateTime { get; set; }
	public LogLevel LogLevel { get; set; }
	public string Message { get; set; } = string.Empty;
}