using System;
using System.Collections.Generic;
using LogService.Contracts;

namespace LogService.Client.Data.Models;

public class LogMessageListItemModel
{
	public Guid Id { get; set; }
	public DateTime DateTime { get; set; }
	public LogLevel LogLevel { get; set; }
	public string Message { get; set; } = string.Empty;
}